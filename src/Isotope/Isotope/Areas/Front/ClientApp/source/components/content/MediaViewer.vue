<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
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
export default class MediaViewer extends Mixins(HasLifetime, HasAsyncState()) {
    @Dep('$host') $host: string;
    @Dep('$api') $api: ApiService;
    @Dep('$filter') $filter: FilterStateService;

    @Prop({ required: true }) indexFeed: IObservable<number>;
    @Prop({ required: true, type: Array }) source: MediaThumbnail[];

    shown: boolean = false;
    index: number = null;
    media: Media = null;
    cache: Promise<ICachedMedia>[] = [];
    showOverlay: boolean = false;

    mounted() {
        this.observe(this.indexFeed, x => this.show(x));
        this.observe(this.$filter.onStateChanged, x => x.source === 'viewer' || this.hide());
    }

    hide() {
        this.shown = false;
        this.index = null;
        this.media = null;
        this.clearCache();
        this.$filter.update('viewer', { mediaKey: null });
    }

    async show(i: number) {
        if(i < 0 || i >= this.source.length)
            return;
        
        this.shown = true;
        this.index = i;
        try {
            const getter = this.cache[i] || (this.cache[i] = this.preloadMedia(i));
            await this.showLoading(async () => this.media = (await getter).media);
            
            this.$filter.update('viewer', { mediaKey: this.media.key });
            this.updateCache(i); // sic! no awaiting
        } catch (e) {
            console.log('failed to load media', e);
        }
    }
    
    async nav(pos: number) {
        const idx = this.index + pos;
        if(idx < 0 || idx >= this.source.length)
            return;
        
        await this.show(idx);
        this.updateCache(idx); // sic! no awaiting
    }
    
    getAbsolutePath(path: string) {
        return this.$host + path;
    }
    
    private async updateCache(idx: number) {
        const back = 1;
        const forward = 3;
        
        for(let i = idx - back; i < idx + forward; i++) {
            if(i < 0 || i >= this.source.length || this.cache[i] != null)
                continue;
            
            this.cache[i] = this.preloadMedia(i);
        }
    }
    
    private async preloadMedia(idx: number): Promise<ICachedMedia> {
        const key = this.source[idx].key;
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
        for(let elem of this.cache) {
            const img = (await elem).img;
            img.parentElement.removeChild(img);
        }
        this.cache = [];
    }
}

interface ICachedMedia {
    media: Media;
    img: HTMLImageElement;
}
</script>

<template>
    <div>
        <portal to="overlay">
            <div v-if="shown" class="viewer">
                <GlobalEvents @keydown.left="nav(-1)" @keydown.right="nav(1)" @keydown.esc="hide()"></GlobalEvents>
                <div class="overlay" @click="hide()" @mouseenter="showOverlay = false" @mouseleave="showOverlay = true"></div>
                <div class="viewer-wrapper">
                    <div class="viewer-body card">
                        <div class="card-body">
                            <loading :is-loading="asyncState.isLoading">
                                <div v-if="media" class="media-wrapper">
                                    <img :src="getAbsolutePath(media.fullPath)"
                                         :alt="media.description" />
                                    <template v-if="showOverlay">
                                        <a class="navigation-arrow left clickable" @click.prevent="nav(-1)" v-if="index > 0">&lt;</a>
                                        <a class="navigation-arrow right clickable" @click.prevent="nav(1)" v-if="index < source.length - 1">&gt;</a>
                                        <OverlayTag v-for="t in media.overlayTags" :value="t" :show="true"></OverlayTag>
                                    </template>
                                </div>
                            </loading>
                        </div>
                    </div>
                </div>
            </div>
        </portal>
    </div>
</template>

<style lang="scss" scoped>
.viewer {
    position: absolute;
    left: 0;
    right: 0;
    top: 0;
    bottom: 0;
    z-index: 100;
    
    display: flex;
    justify-content: center;
    align-items: center;
    
    .overlay {
        position: absolute;
        width: 100%;
        height: 100%;
        background: #000;
        opacity: 0.4;
    }
    
    .viewer-wrapper {
        position: absolute;
        z-index: 101;
        
        .viewer-body {
            margin: auto 0;
            padding: 1rem;
            min-width: 300px;
            
            .media-wrapper {
                position: relative;
                width: 100%;
                
                .navigation-arrow {
                    color: #FFFFFF;
                    text-decoration: none;
                    position: absolute;
                    top: 0;
                    bottom: 0;
                    width: 64px;
                    font-size: 72px;
                    opacity: 0.4;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    
                    &:hover {
                        opacity: 1;
                    }
                    
                    &.left {
                        left: 0;
                    }
                    
                    &.right {
                        right: 0;
                    }
                }
                
                img {
                    width: 100%;
                }
            }
        }
    }
}
</style>