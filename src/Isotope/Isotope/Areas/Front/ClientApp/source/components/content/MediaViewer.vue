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
import MediaContent from "./MediaContent.vue";
import MediaDetails from "./MediaDetails.vue";
import { Bind } from 'lodash-decorators';
import { Debounce } from 'lodash-decorators';
import { BreakpointHelper, Breakpoints } from "../../utils/BreakpointHelper";

@Component({
    components: { MediaContent, MediaDetails }
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
    upcomingIndex: number = null;
    translateX: number = 0;
    translateY: number = 0;
    maxTranslateX: number = 0;
    maxTranslateY: number = 0;
    transformStyle = "translate(0, 0)";
    transitionClass = "transition-initial";
    isTransitioning: boolean = false;
    isMobile: boolean = false;
    isMobileOverlayVisible: boolean = false;
    isClosing: boolean = false;

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

        this.isMobile = BreakpointHelper.down(Breakpoints.md);
        window.addEventListener("resize", this.resizeHandler);
        window.addEventListener('orientationchange', this.oriantationHandler);
    }

    beforeDestroy() {
        window.removeEventListener('resize', this.resizeHandler);
        window.removeEventListener('orientationchange', this.oriantationHandler);
    }

    oriantationHandler() {
        document.documentElement.style.height = `initial`;
        setTimeout(() => {
            document.documentElement.style.height = `100%`;
            setTimeout(() => {
                window.scrollTo(0, 1);
            }, 200);
        }, 200);
    }

    @Debounce(50)
    @Bind()
    resizeHandler() {
        this.isMobile = BreakpointHelper.down(Breakpoints.md);
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
        this.upcomingIndex = null;
        this.$filter.update('viewer', { mediaKey: this.source[i].key });

        this.updateCache(i);
    }

    nav(pos: number) {
        const idx = this.index + pos;
        if(idx < 0 || idx >= this.source.length)
            return;

        this.show(idx);
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
            img.onerror = e => reject(e);
            img.src = this.$host + media.fullPath;
            document.body.appendChild(img);
        });
    }

    private async clearCache() {
        for(let elem of this.cache)
            if(elem && elem.img)
                elem.img.parentElement.removeChild(elem.img);

        this.cache = [];
    }

    @Watch('shown')
    onShownChanged(value: boolean) {
        this.isMobile = BreakpointHelper.down(Breakpoints.md);
        document.body.classList.toggle('media-viewer-open', value);
    }

    handleHorizontalTouchEvents(e: HammerInput) {
        if (
            this.isTransitioning || 
            !this.isMobile
        ) {
            return;
        }
        
        if (e.isFinal) {
            this.handleHorizontalGestureEnd(e.deltaX);
        } else {
            this.handleHorizontalGestureMove(e.deltaX);
        }
    }

    handleHorizontalGestureMove(deltaX: number) {
        if (Math.abs(deltaX) > Math.abs(this.maxTranslateX)) {
            this.maxTranslateX = deltaX;
        }

        const sign = Math.sign(this.translateX);
        const px = ((!this.prev && deltaX > 0) || (!this.next && deltaX < 0)) 
            ? sign * Math.sqrt(Math.abs(deltaX)) 
            : deltaX;

        this.translateX = deltaX;
        this.transitionClass = "transition-initial";
        this.transformStyle = `translate(${px}px, 0)`;
    }

    handleHorizontalGestureEnd(deltaX: number) {
        if (
            (!this.prev && deltaX > 0) ||
            (!this.next && deltaX < 0) ||
            (Math.abs(this.translateX) - Math.abs(this.maxTranslateX) < -1)
        ) {
            this.transitionClass = 'transition-item';
            this.transformStyle = 'translate(0, 0)';
        } else if (this.translateX !== 0) {
            this.swipe(Math.sign(this.translateX));
        }
    }

    handleVerticalTouchEvents(e: any) {
        if (
            this.isTransitioning || 
            !this.isMobile
        ) {
            return;
        }
        
        if (e.isFinal) {
            this.handleVerticalGestureEnd(e.deltaY);
        } else {
            this.handleVerticalGestureMove(e.deltaY);
        }
    }

    handleVerticalGestureMove(deltaY: number) {
        if (Math.abs(deltaY) > Math.abs(this.maxTranslateY)) {
            this.maxTranslateY = deltaY;
        }

        this.translateY = deltaY;
        this.transitionClass = "transition-initial";
        this.transformStyle = `translate(0, ${deltaY < 0 ? 0 : deltaY}px)`;

        this.isClosing = deltaY > 50;
    }

    handleVerticalGestureEnd(deltaY: number) {
        this.transitionClass = "transition-item";

        if (this.isClosing) {
            this.transformStyle = `translate(0, 100%)`;
        } else {
            this.transformStyle = `translate(0, 0)`;
        }

        this.translateY = 0;
    }
    
    @Debounce(20)
    @Bind()
    swipe(dir: number) {
        if (this.isTransitioning || !this.cache[this.index - dir]) {
            return;
        }

        this.transitionClass = "transition-item";
        this.transformStyle = `translate(${dir * 100}vw, 0)`;
        this.upcomingIndex = Math.min(Math.max(this.index - dir, 0), this.source.length - 1);
    }

    onTap(e: HammerInput) {
        // TO DO: Find a solution to prevent parent event from OverlayTag component.
        if ((e.target.classList[0] != 'overlay-tag') && (e.target.tagName != 'A') && this.isMobile) {
            this.isMobileOverlayVisible = !this.isMobileOverlayVisible;
        }
    }
    
    updateCurrentItem() {
        if (this.isClosing) {
            this.hide();
            this.isClosing = false;
        }

        this.isTransitioning = false;
        this.transitionClass = "transition-initial";
        this.transformStyle = "translate(0, 0)";
        this.translateX = 0;
        this.maxTranslateX = 0;
        this.translateY = 0;
        this.maxTranslateY = 0;

        if (this.upcomingIndex !== null && this.upcomingIndex !== this.index) {
            this.show(this.upcomingIndex);
        }
    }

    handleTouchEvents(e: HammerInput) {
        if ((e.direction == 4 || 2 && Math.abs(e.deltaX) > 8) && this.translateY == 0) {
            this.handleHorizontalTouchEvents(e);
        } else if (e.direction == 8 || 16 && Math.abs(e.deltaY) > 8 && this.translateX == 0) {
            this.handleVerticalTouchEvents(e);
        }
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
        <transition name="media-viewer__fade">
            <div class="media-viewer"
                v-if="shown"
                v-hammer:pan="handleTouchEvents"
                v-hammer:swipe.left.right="handleHorizontalTouchEvents"
                :class="{'media-viewer_closing' : isClosing}" 
            > 
                <div class="media-viewer__content"
                    v-hammer:tap="onTap"
                    :class="transitionClass" 
                    :style="{transform: transformStyle}" 
                    @transitionstart.self="isTransitioning = true" 
                    @transitionend.self="updateCurrentItem"
                >  
                    <media-content 
                        :elem="prev" 
                        :key="'p' + index"
                        :isMobileOverlayVisible="isMobileOverlayVisible"
                    ></media-content>
                    <media-content 
                        :elem="curr" 
                        :hasOverlay="true"
                        :isFirst="!prev"
                        :isLast="!next"
                        :isMobileOverlayVisible="isMobileOverlayVisible"
                        v-on:nav="nav($event)"
                        v-on:close="hide()"
                    ></media-content>
                    <media-content 
                        :elem="next" 
                        :key="'n' + index"
                        :isMobileOverlayVisible="isMobileOverlayVisible"
                    ></media-content>
                </div>
                <media-details
                    v-if="isMobile && curr && !curr.isLoading"
                    :isOpenOnMobile="isMobileOverlayVisible"
                    :isMobile="true"
                    :media="curr.media"
                ></media-details>
            </div>
        </transition>
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
        height: 100%;
        width: 100%;
        position: absolute;
        overflow: hidden;
        -webkit-user-select: none;  
        -moz-user-select: none;    
        -ms-user-select: none;      
        user-select: none;
        transition: background-color 200ms linear;

        &_closing {
            background-color: rgba($dark, 0.0);
        }
        
        &__content {
            display: flex;
            justify-content: center;
            height: 100%;
            width: 100%;
            box-sizing: border-box;
            touch-action: pan-y;
            will-change: transform;
            transition: transform 0s linear;
            backface-visibility: hidden;
            -webkit-backface-visibility: hidden;

            &.transition-initial {
                transition: transform 0s linear;
            }

            &.transition-item {
                transition: transform 250ms cubic-bezier(0.0, 0.0, 0.2, 1);
            }

            &.transition-edge {
                transition: transform 500ms ease-out;
            }
        }

        &__fade {

            &-enter-active, 
            &-leave-active {
                transition: opacity 200ms linear;
            }

            &-enter, 
            &-leave-to {
                opacity: 0;
            }
        }
    }

    .media-viewer-open {
        overflow: hidden;
    }
</style>