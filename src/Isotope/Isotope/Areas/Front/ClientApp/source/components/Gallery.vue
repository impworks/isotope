<script lang="ts">
import { Vue, Component, Mixins } from "vue-property-decorator";
    import Breadcrumbs from './Breadcrumbs.vue';
import { HasLifetime } from "./mixins/HasLifetime";
import { HasAsyncState } from "./mixins/HasAsyncState";
import FilterStateService, { IFilterState } from "../services/FilterStateService";
import { FolderContents } from "../vms/FolderContents";
import { Dep } from "../utils/VueInjectDecorator";
import { ApiService } from "../services/ApiService";
import { Folder } from "../vms/Folder";
import { TagBinding } from "../vms/TagBinding";
import { MediaThumbnail } from "../vms/MediaThumbnail";

    @Component({
        components: { 
            Breadcrumbs
        }
    })
    export default class Gallery extends Mixins(HasAsyncState(), HasLifetime) {
        @Dep('$api') $api: ApiService;
        @Dep('$filter') $filter: FilterStateService;
        
        error: string = null;
        empty: boolean = false;
        contents: FolderContents = null;
        
        get isEmpty() {
            return !this.contents?.media?.length
                && !this.contents?.subfolders?.length;
        }
        
        async mounted() {
            this.observe(this.$filter.onStateChanged, x => this.refresh(x));
            await this.refresh(this.$filter.state);
        }
        
        async refresh(state: IFilterState) {
            this.error = null;
            
            try {
                await this.showLoading(async () => {
                    this.contents = await this.$api.getFolderContents({
                        folder: state.folder,
                        searchMode: state.searchMode,
                        dateFrom: state.dateFrom,
                        dateTo: state.dateTo,
                        tags: state.tags ? state.tags.join(',') : null
                    });
                })
            } catch (e) {
                this.error = 'Folder not found!';
            }
        }
        
        showFolder(f: Folder) {
            this.$filter.update('view', { folder: f.path })
        }
        
        filterByTag(t: TagBinding) {
            this.$filter.update('view', { folder: '/', tags: [t.tag.id] });
        }
        
        showMedia(m: MediaThumbnail) {
            console.log('showing media: ', m);
        }
    } 
</script>

<template>
    <div class="gallery">
        <div class="gallery__header">
            <breadcrumbs/>
        </div>
            <perfect-scrollbar class="gallery__content">
                <loading :is-loading="asyncState.isLoading" :is-full-page="true">
                <div v-if="contents">
                    <div v-if="contents.tags" class="gallery__tags">
                        <a v-for="t in contents.tags" class="badge badge-primary clickable" @click.prevent="filterByTag(t)">{{t.tag.caption}}</a>
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
                                class="gallery__item gallery__item_picture clickable"
                                v-for="m in contents.media"
                                :key="m.key"
                            >
                                <a
                                    class="gallery__item__content clickable"
                                    @click.prevent="showMedia(m)"
                                >
                                    <div class="gallery__item__icon">
                                        <div class="gallery__item__preview" :style="{backgroundImage: 'url(' + m.thumbnailPath + ')'}"></div>
                                    </div>
                                </a>
                            </div>
                        </div>
                    </div>
                    <div v-if="isEmpty" class="alert alert-info ml-5 mr-5">
                        This folder is empty.
                    </div>
                </div>
                <div v-if="error" class="alert alert-danger ml-5 mr-5 mt-5">
                    <strong>Error</strong><br/>
                    {{error}}
                </div>
            </loading>
        </perfect-scrollbar>
    </div>
</template>

<style lang="scss">
    @import "../../styles/variables";
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

            @include media-breakpoint-down(md) {
                display: none;
            }
        }

        &__content {
            width: 100%;
            flex: 1 1 auto;
            display: block;
            background: $gray-200;
        }

        &__tags {
            padding: 1rem 1rem 0;
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
            flex-basis: 9rem;

            @include media-breakpoint-up(md) {
                flex-basis: 11.3rem;
            }

            &__content {
                display: block;
                margin: 0.5rem;
                padding: 0.5rem;
                color: $gray-800;
                border-radius: $border-radius;
                border: 1px solid $gray-300;
                background: $white;
                transition: all 150ms ease;

                &:hover {
                    border-color: $gray-500;
                    box-shadow: $box-shadow;
                }
            }

            &__caption {
                text-align: center;
            }

            &__icon {
                position: relative;
                background-repeat: no-repeat;
                background-size: auto 100%;
                background-position: center;
            }

            &_folder &__icon {
                height: 4rem;
                background-image: url(../../images/folder.svg);
                background-size: auto 200%;
                background-position: center 0;
            }

            &_picture &__icon {
                height: 7rem;
                background-color: $gray-200;
                background-image: url(../../images/image.svg);
                background-size: auto 4rem;

                @include media-breakpoint-up(md) {
                    flex-basis: 9.3rem;
                }
            }

            &_picture &__preview {
                width: 100%;
                height: 100%;
                background-size: cover;
            }
        }
    }
</style>