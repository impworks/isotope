<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { HasLifetime } from "../mixins/HasLifetime";
import { HasAsyncState } from "../mixins/HasAsyncState";
import { FilterStateService, IFilterStateChangedEvent } from "../../services/FilterStateService";
import { FolderContents } from "../../vms/FolderContents";
import { Dep } from "../../utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { Folder } from "../../vms/Folder";
import { TagBinding } from "../../vms/TagBinding";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { SearchMode } from "../../vms/SearchMode";
import { IObservable } from "../../utils/Interfaces";
import { Observable } from "../../utils/Observable";
import Breadcrumbs from './Breadcrumbs.vue';
import MediaViewer from "./MediaViewer.vue";

@Component({
    components: { MediaViewer, Breadcrumbs }
})
export default class Gallery extends Mixins(HasAsyncState(), HasLifetime) {
    @Dep('$host') $host: string;
    @Dep('$api') $api: ApiService;
    @Dep('$filter') $filter: FilterStateService;
    
    error: string = null;
    empty: boolean = false;
    contents: FolderContents = null;
    isFilterActive: boolean = false;
    
    indexFeed: IObservable<number> = new Observable<number>();
    
    get isEmpty() {
        return !this.contents?.media?.length
            && !this.contents?.subfolders?.length;
    }
    
    async mounted() {
        this.observe(this.$filter.onStateChanged, x => this.refresh(x));
        await this.refresh({ ...this.$filter.state, source: null });
    }
    
    async refresh(state: IFilterStateChangedEvent) {
        if(state.source === 'viewer')
            return;
        
        this.error = null;
        this.isFilterActive = !this.$filter.isEmpty(state);
        
        try {
            await this.showLoading(async () => {
                this.contents = await this.$api.getFolderContents({
                    folder: state.folder,
                    searchMode: state.searchMode,
                    dateFrom: state.dateFrom,
                    dateTo: state.dateTo,
                    tags: state.tags ? state.tags.join(',') : null
                });
            });
            
            if(state.mediaKey) {
                const idx = this.contents.media.findIndex(x => x.key == state.mediaKey);
                if(idx !== -1)
                    this.showMedia(idx);
            }
        } catch (e) {
            this.error = 'Folder not found!';
        }
    }
    
    showFolder(f: Folder) {
        this.$filter.update('list', { folder: f.path })
    }
    
    filterByTag(t: TagBinding) {
        this.$filter.update('list', { folder: '/', searchMode: SearchMode.Everywhere, tags: [t.tag.id] });
    }
    
    showMedia(idx: number) {
        this.indexFeed.notify(idx);
    }
    
    getThumbnailPath(m: MediaThumbnail) {
        return 'url(' + this.$host + m.thumbnailPath + ')';
    }
} 
</script>

<template>
    <div class="gallery">
        <div class="gallery__header">
            <breadcrumbs></breadcrumbs>
        </div>
            <simplebar v-if="!isEmpty" class="gallery__content">
                <loading 
                    :is-full-page="true"
                    :is-loading="asyncState.isLoading"
                >
                <fragment v-if="contents">
                    <div 
                        class="gallery__tags"
                        v-if="contents.tags && contents.tags.length"
                    >
                        <a 
                            class="gallery__tags__item clickable" 
                            v-for="t in contents.tags" 
                            :key="t.tag.id"
                            @click.prevent="filterByTag(t)"
                        >
                            {{t.tag.caption}}
                        </a>
                    </div>
                    <div class="gallery__grid">
                        <div v-if="contents.subfolders && contents.subfolders.length" class="gallery__grid__row">
                            <div
                                class="gallery__item gallery__item_folder"
                                v-for="f in contents.subfolders"
                                :key="f.path"
                            >
                                <a
                                    class="gallery__item__content clickable"
                                    @click.prevent="showFolder(f)"
                                >
                                    <div class="gallery__item__icon"></div>
                                    <div class="gallery__item__caption">
                                        {{f.caption}}
                                    </div>
                                </a>
                            </div>
                        </div>
                        <div v-if="contents.media && contents.media.length" class="gallery__grid__row">
                            <div 
                                class="gallery__item gallery__item_picture"
                                v-for="(m, i) in contents.media"
                                :key="m.key"
                            >
                                <a
                                    class="gallery__item__content clickable"
                                    @click.prevent="showMedia(i)"
                                >
                                    <div class="gallery__item__icon">
                                        <div 
                                            class="gallery__item__preview" 
                                            :style="{backgroundImage: getThumbnailPath(m)}"
                                        ></div>
                                    </div>
                                </a>
                            </div>
                        </div>
                    </div>
                </fragment>
                
            </loading>
            <div v-if="error" class="alert alert-danger ml-5 mr-5 mt-5">
                <strong>Error</strong><br/>
                {{error}}
            </div>
        </simplebar>
        <div v-if="isEmpty" class="gallery__error">
            <div class="gallery__error__content">
                <div class="gallery__error__image"></div>
                <h3>Oops…</h3>
                <p v-if="isFilterActive">No matching media found.</p>
                <p v-else>This folder is empty.</p>
            </div>
        </div>
        <MediaViewer :index-feed="indexFeed" :source="contents.media" v-if="contents && contents.media && contents.media.length"></MediaViewer>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .gallery {
        flex: 1 1 auto;
        display: flex;
        flex-direction: column;

        &__header {
            flex: 0 1 auto;
            background: $white;
            border-bottom: 1px solid $gray-300;

            @include media-breakpoint-down(sm) {
                margin-top: -1px;
                border-top: 1px solid $gray-300;
            }
        }

        &__content {
            width: 100%;
            flex: 1 1 auto;
            display: block;

            @include media-breakpoint-up(md) {
                height: 0;
            }
        }

        &__tags {
            font-size: 0;
            line-height: 1;
            padding: 0.5rem 1rem;

            &__item {
                color: $white;
                font-size: 1rem;
                padding: 0.3rem 0.5rem;
                margin: 0.5rem 0.5rem 0 0;
                display: inline-block;
                border-radius: $border-radius;
                background-color: $primary;

                &:hover {
                    color: $white;
                    text-decoration: none;
                    background-color: darken($primary, 3%);
                }
            }

            & + .gallery__grid {
                padding-top: 0;
            }
        }

        &__grid {
            padding: 0.5rem 0.5rem 1.5rem;

            &__row {
                display: flex;
                flex-wrap: wrap;
            }
        }

        &__item {
            flex: 0 0 auto;
            display: flex;

            $content-sizes:
                375px  5.7291666667rem,
                414px  6.1666666667rem,
                438px  6.6666666667rem,
                504px  8.0416666667rem,
                507px  5.546875rem,
                551px  6.234375rem,
                568px  6.5rem,
                639px  7.609375rem,
                667px  6.0125rem,
                678px  6.15rem,
                694px  6.35rem,
                736px  5.375rem,
                768px  7.375rem,
                800px  8.0416666667rem,
                834px  8.75rem,
                981px  8.328125rem,
                992px  8.5rem,
                1024px 8.375rem,
                1112px 7.375rem,
                1280px 7.5416666667rem,
                1366px 8.4375rem,
                1440px 9.2083333333rem;

            &__content {
                display: block;
                margin: 0.5rem;
                padding: 0.3rem;
                color: $gray-800;
                border-radius: $border-radius;
                border: 1px solid $gray-300;
                background: $white;
                transition: border 150ms ease;
                width:  4.5833333333rem;
                box-sizing: content-box;

                @media only screen and (min-width: 414px) {
                    padding: 0.5rem;
                }

                @each $screen-size, $content-size in $content-sizes {
                    @media only screen and (min-width: $screen-size) {
                        width: $content-size;
                    }
                }

                &:hover {
                    border-color: $gray-400;
                    box-shadow: $box-shadow-sm;
                }
            }

            &__caption {
                padding-top: 0.5rem;
                text-align: center;

                @include media-breakpoint-down(sm) {
                    font-size: 0.8rem;
                }
            }

            &__icon {
                width: 100%;
                position: relative;
                background-repeat: no-repeat;
                background-size: auto 100%;
                background-position: center;
            }

            &_folder &__icon {
                height: 4rem;
                background-image: url(../../../images/folder.svg);
                background-size: auto 200%;
                background-position: center 0;

                @include media-breakpoint-down(sm) {
                    height: 3rem;
                }
            }

            &_picture &__icon {
                height: 4.5458333333rem;
                background-color: $gray-200;
                background-image: url(../../../images/image.svg);
                background-size: auto 3rem;

                @include media-breakpoint-up(md) {
                    background-size: auto 4rem;
                }

                @each $screen-size, $content-size in $content-sizes {
                    @media only screen and (min-width: $screen-size) {
                        height: $content-size;
                    }
                }
            }

            &_picture &__preview {
                width: 100%;
                height: 100%;
                background-size: cover;
            }
        }

        &__error {
            display: flex;
            flex: 1 1 auto;
            text-align: center;
            padding: 1rem 1rem 2rem;
            align-items: center;
            justify-content: center;

            &__content {
                flex: 1 1 auto;
                margin: auto;

                p {
                    color: $gray-600;
                }
            }

            &__image {
                display: block;
                height: 10rem;
                background-image: url(../../../images/cat.svg);
                background-repeat: no-repeat;
                background-position: center;
                background-size: auto 100%;
                margin-bottom: 1rem;
            }
        }
    }
</style>