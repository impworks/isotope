<script lang="ts">
import { Component, Mixins, Prop, Vue, Watch } from "vue-property-decorator";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { IObservable } from "../../utils/Interfaces";
import { Media } from "../../vms/Media";
import { HasLifetime } from "../mixins/HasLifetime";
import { HasAsyncState } from "../mixins/HasAsyncState";
import { ApiService } from "../../services/ApiService";
import { FilterStateService } from "../../services/FilterStateService";
import { Dep } from "../../utils/VueInjectDecorator";
import OverlayTag from "./OverlayTag.vue";
import debounce from 'lodash.debounce';

@Component({
    components: { OverlayTag }
})
export default class MediaViewer extends Mixins(HasLifetime) {
    @Dep('$host') $host: string;
    @Dep('$api') $api: ApiService;
    @Dep('$filter') $filter: FilterStateService;

    @Prop({ required: true }) indexFeed: IObservable<number>;
    @Prop({ required: true, type: Array }) source: MediaThumbnail[];

    shown: boolean = false;
    index: number = null;
    cache: ICachedMedia[] = [];
    showOverlay: boolean = false;
    upcomingIndex: number = null;
    translateX: number = 0;
    maxTranslateX: number = 0;
    transformStyle = "translateX(0)";
    transitionClass = "transition-initial";
    isTransitioning: boolean = false;
    leftEdgeScale: number = 0;
    rightEdgeScale: number = 0;

    get prev(): ICachedMedia {
        return this.cache[this.index - 1] || null;
    }

    get curr(): ICachedMedia {
        return this.cache[this.index] || null;
    }

    get next(): ICachedMedia {
        return this.cache[this.index + 1] || null;
    }

    mounted() {
        this.observe(this.indexFeed, x => this.show(x));
        this.observe(this.$filter.onStateChanged, x => x.source === 'viewer' || this.hide());
    }

    hide() {
        this.shown = false;
        this.index = null;
        this.clearCache();

        this.$filter.update('viewer', { mediaKey: null });
    }

    show(i: number) {
        if(i < 0 || i >= this.source.length)
            return;

        this.shown = true;
        this.index = i;
        this.$filter.update('viewer', { mediaKey: this.source[i].key });

        this.updateCache(i);
    }

    nav(pos: number) {
        const idx = this.index + pos;
        if(idx < 0 || idx >= this.source.length)
            return;

        this.show(idx);
    }

    getAbsolutePath(path: string) {
        return this.$host + path;
    }

    private updateCache(idx: number) {
        this.updateCacheItem(idx);

        const back = 1;
        const forward = 3;

        for(let i = idx - back; i < idx + forward; i++) {
            if(i >= 0 && i < this.source.length)
                this.updateCacheItem(i);
        }
    }

    private async updateCacheItem(idx: number) {
        if(this.cache[idx])
            return;

        const item: ICachedMedia = { isLoading: true };
        Vue.set(this.cache, idx, item);

        try {
            const key = this.source[idx].key;
            const { img, media } = await this.preloadMedia(key);
            item.img = img;
            item.media = media;
        } catch (e) {
            console.log('Failed to load media!', e);
        } finally {
            item.isLoading = false;
        }
    }

    private async preloadMedia(key: string): Promise<IMedia> {
        const media = await this.$api.getMedia(key);
        return new Promise((resolve, reject) => {
            const img = new Image();
            img.className = 'preload-image';
            img.onload = () => resolve({ media: media, img: img });
            img.onerror = () => reject();
            img.src = this.$host + media.fullPath;
            document.body.appendChild(img);
        });
    }

    private async clearCache() {
        for(let elem of this.cache)
            elem.img.parentElement.removeChild(elem.img);

        this.cache = [];
    }

    @Watch('shown')
    onShownChanged(value: boolean) {
        document.body.classList.toggle('media-viewer-open', value);
    }

    get isNextAvailable() {
      return this.index < this.source.length - 1;
    }

    get isPreviousAvailable() {
      return this.index > 0;
    }

    handleTouchEvents(e: any) {

        if (this.isTransitioning) {
            return;
        }

        if (
            (Math.abs(e.deltaX) < 8 || Math.abs(e.deltaY) - Math.abs(e.deltaX) > -1) &&
            !this.translateX &&
            !this.leftEdgeScale &&
            !this.rightEdgeScale
        ) {
            return;
        }

        if (
            (!this.isPreviousAvailable && e.deltaX > 0) ||
            (!this.isNextAvailable && e.deltaX < 0)
        ) {
            this.updateEdgeEffect(e.deltaX, e.isFinal);
        } else if (e.isFinal) {
            this.handleGestureEnd();
        } else {
            this.handleGestureMove(e.deltaX);
        }
    }

    handleGestureMove(deltaX: number) {
        if (Math.abs(deltaX) > Math.abs(this.maxTranslateX)) {
            this.maxTranslateX = deltaX;
        }

        this.translateX = deltaX;
        this.transitionClass = "transition-initial";
        this.transformStyle = `translateX(${deltaX}px)`;
    }

    handleGestureEnd() {
        if (Math.abs(this.translateX) - Math.abs(this.maxTranslateX) < -1) {
            this.transitionClass = 'transition-item';
            this.transformStyle = 'translateX(0)';
        } else if (this.translateX > 0) {
            this.swipeLeft();
        } else if (this.translateX < 0) {
            this.swipeRight();
        }
    }

    swipeLeft = debounce(
    function(this: MediaViewer) {
        if (this.isTransitioning) {
            return;
        }

        if (!this.isPreviousAvailable) {
            return;
        }

        this.transitionClass = "transition-item";
        this.transformStyle = "translateX(100vw)";

        const prevIndex = this.index === 0 ? this.source.length - 1 : this.index - 1;
        this.upcomingIndex = prevIndex;
    },

    100);

    swipeRight = debounce(
        function(this: MediaViewer) {
            if (this.isTransitioning || !this.isNextAvailable) {
                return;
            }

            this.transitionClass = "transition-item";
            this.transformStyle = "translateX(-100vw)";

            const nextIndex =  this.index === this.source.length - 1 ? 0 : this.index + 1;
            this.upcomingIndex = nextIndex;
        }, 
    100);

    updateEdgeEffect(deltaX: number, isFinal: boolean) {
        if (isFinal) {
            this.transitionClass = "transition-edge";
            this.leftEdgeScale = 0;
            this.rightEdgeScale = 0;
        } else {
            this.transitionClass = "transition-initial";
            const scaleVal = Math.min(0.2 + Math.abs(deltaX) / 50, 1);
            if (deltaX > 0) {
                this.leftEdgeScale = scaleVal;
            }
            if (deltaX < 0) {
                this.rightEdgeScale = scaleVal;
            }
        }
    }

    updateCurrentItem() {
      this.show(this.upcomingIndex);
      this.resetTranslate();
    }

    resetTranslate() {
      this.isTransitioning = false;
      this.transitionClass = "transition-initial";
      this.transformStyle = "translateX(0)";
      this.translateX = 0;
      this.maxTranslateX = 0;
    }
}

interface IMedia {
    media?: Media;
    img?: HTMLImageElement;
}

interface ICachedMedia extends IMedia {
    isLoading: boolean;
}
</script>

<template>
    <portal to="overlay">
        <GlobalEvents 
            @keydown.left="nav(-1)" 
            @keydown.right="nav(1)" 
            @keydown.esc="hide()"
        ></GlobalEvents>
        <div class="media-viewer"
            v-if="shown"
            v-hammer:swipe.left="handleTouchEvents"
            v-hammer:pan="handleTouchEvents"
        >
            <div class="media-viewer__content"
                :class="transitionClass" 
                :style="{transform: transformStyle}" 
                @transitionstart="isTransitioning = true" 
                @transitionend="updateCurrentItem"
            >
                <div class="media-viewer__content__item">
                    <div v-if="prev">
                        <loading :is-loading="prev.isLoading">
                            <img :src="getAbsolutePath(prev.media.fullPath)"
                                :alt="prev.media.description" />
                        </loading>
                    </div>
                </div>
                <div class="media-viewer__content__item">
                    <div v-if="curr">
                        <loading :is-loading="curr.isLoading">
                            <img :src="getAbsolutePath(curr.media.fullPath)"
                                :alt="curr.media.description" />
                        </loading>
                    </div>
                </div>
                <div class="media-viewer__content__item">
                    <div v-if="next">
                        <loading :is-loading="next.isLoading">
                            <img :src="getAbsolutePath(next.media.fullPath)"
                                :alt="next.media.description" />
                        </loading>
                    </div>
                </div>
            </div>
            <div class="touch-tap-left" role="button" aria-label="Previous" tabindex="0">
                <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" viewBox="0 0 10 100" height="100%" width="40px" preserveAspectRatio="none" class="left-edge-shape" :class="transitionClass" :style="{transform: 'scaleX(' + leftEdgeScale + ')'}">
                    <path d="M0,0v100h5.2c3-14.1,4.8-31.4,4.8-50S8.2,14.1,5.2,0H0z" />
                </svg>
            </div>
            <div class="touch-tap-right" role="button" aria-label="Next" tabindex="0">
                <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" viewBox="0 0 10 100" height="100%" width="40px" preserveAspectRatio="none" class="right-edge-shape" :class="transitionClass" :style="{transform: 'scaleX(' + rightEdgeScale + ')'}">
                    <path d="M10,100V0L4.8,0C1.8,14.1,0,31.4,0,50c0,18.6,1.8,35.9,4.8,50H10z" />
                </svg>
            </div>
        </div>
    </portal>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .media-viewer {
        z-index: $zindex-modal;
        background-color: rgba($dark, 0.5);
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        position: fixed;

        &__content {
            width: 100vw;
            height: 100%;
            position: relative;
            justify-content: center;
            box-sizing: border-box;
            will-change: transform;
            touch-action: pan-y;
            display: flex;

            &__item {
                height: 100%;
                min-width: 100vw;
                position: relative;

                img {
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    margin: auto;
                    max-width: 100%;
                    max-height: 100%;
                    position: absolute;
                    vertical-align: bottom;
                }
            }
        }
        
    }

    .transition-initial {
        transition: transform 0s ease,
    }

    .transition-item {
        transition: transform 250ms cubic-bezier(0.0, 0.0, 0.2, 1); // ease-out timing function
    }

    .transition-edge {
        transition: transform 500ms ease-out;
    }

    .touch-tap-left,
    .touch-tap-right {
        position: absolute;
        top: 0;
        width: 20%;
        height: 100%;
    }

    .touch-tap-left {
        left: 0;
    }

    .touch-tap-right {
        right: 0;
    }

    .touch-tap-left:focus, .touch-tap-right:focus {
        outline: none;
    }

    .left-edge-shape, .right-edge-shape {
        position: absolute;
        fill: white;
        opacity: 0.3;
    }

    .left-edge-shape {
        left: 0;
        transform-origin: left;
    }

    .right-edge-shape {
        right: 0;
        transform-origin: right;
    }
</style>