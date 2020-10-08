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
        <transition name="media-viewer-modal__fade">
            <div 
                class="media-viewer-modal"
                v-if="shown"
            >
                <div 
                    class="media-viewer-modal__backdrop clickable" 
                    @click="hide()"
                ></div>
                <div 
                    class="media-viewer-modal__content" 
                    @mouseenter="showOverlay = true" 
                    @mouseleave="showOverlay = false"
                >
                    <div class="media-viewer" v-if="curr">
                        <GlobalEvents @keydown.left="nav(-1)" @keydown.right="nav(1)" @keydown.esc="hide()"></GlobalEvents>
                        <loading :is-loading="curr.isLoading">
                            <div v-if="curr.media" class="media-wrapper">
                                <img :src="getAbsolutePath(curr.media.fullPath)"
                                        :alt="curr.media.description" />
                                <template v-if="showOverlay">
                                    <a class="media-viewer__arrow media-viewer__arrow_left clickable" @click.prevent="nav(-1)" v-if="index > 0">&lt;</a>
                                    <a class="media-viewer__arrow media-viewer__arrow_right clickable" @click.prevent="nav(1)" v-if="index < source.length - 1">&gt;</a>
                                    <OverlayTag v-for="t in curr.media.overlayTags" :value="t" :show="true"></OverlayTag>
                                </template>
                            </div>
                        </loading>
                    </div>
                </div>
            </div>
        </transition>
    </portal>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .media-viewer-modal {
        
        z-index: $zindex-modal;
        display: flex;
        overflow: auto;
        flex-direction: row;
        justify-content: center;
        align-items: center;

        @mixin fixed () {
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            position: fixed;
        }

        @include fixed();
        
        &__backdrop {
            @include fixed();
            background: rgba($dark, 0.5);
            z-index: $zindex-modal-backdrop;
        }

        &__content {
            margin: auto;
            position: relative;
            z-index: $zindex-modal;
        }

        &__fade {

            &-enter-active, 
            &-leave-active {
                transition: opacity 200ms cubic-bezier(.645,.045,.355,1);
            }

            &-enter, 
            &-leave-to {
                opacity: 0;
            }
        }
    }

    .media-viewer {
        max-width: 100%;
        max-height: 100%;
        background: $white;
        margin: 1rem;
        padding: 1rem;
        position: relative;
        border-radius: $border-radius;
        box-shadow: $box-shadow-lg;
        box-sizing: border-box;

        img {
            max-height: 100%;
            max-width: 100%;
        }

        &__arrow {
            color: $white;
            position: absolute;
            top: 0;
            bottom: 0;
            font-size: 4rem;
            opacity: 0.4;
            display: flex;
            padding: 0 2rem;
            justify-content: center;
            align-items: center;
            
            &:hover {
                opacity: 1;
                color: $white;
                text-decoration: none;
            }
            
            &_left {
                left: 0;
            }
            
            &_right {
                right: 0;
            }
        }
    }

    .media-viewer-open {
        overflow: hidden;
    }
</style>