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

@Component
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

    mounted() {
        this.observe(this.indexFeed, x => this.show(x));
        this.observe(this.$filter.onStateChanged, () => this.hide());
    }

    hide() {
        this.shown = false;
        this.index = null;
        this.media = null;
        this.clearCache();
    }

    async show(i: number) {
        if(i < 0 || i >= this.source.length)
            return;
        
        this.shown = true;
        this.index = i;
        try {
            const getter = this.cache[i] || (this.cache[i] = this.preloadMedia(i));
            await this.showLoading(async () => this.media = (await getter).media);
        } catch (e) {
            console.log('failed to load media', e);
        }

        this.updateCache(i); // sic! no awaiting
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
    <portal to="overlay">
        <div v-if="shown" class="viewer">
            <GlobalEvents @keydown.left="nav(-1)" @keydown.right="nav(1)" @keydown.esc="hide()"></GlobalEvents>
            <div class="overlay" @click="hide()"></div>
            <div class="viewer-wrapper">
                <div class="viewer-body card">
                    <div class="card-body">
                        <loading :is-loading="asyncState.isLoading">
                            <div v-if="media">
                                <img :src="getAbsolutePath(media.fullPath)" :alt="media.description"/>
                            </div>
                        </loading>
                    </div>
                </div>
            </div>
        </div>
    </portal>
</template>

<style lang="scss" scoped>
.viewer {
    position: absolute;
    left: 0;
    right: 0;
    top: 0;
    bottom: 0;
    z-index: 999;
    
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
        z-index: 1000;
        
        .viewer-body {
            margin: auto 0;
            padding: 1rem;
            min-width: 300px;

            img {
                width: 100%;
            }
        }
    }
}
</style>