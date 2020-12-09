<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { HasLifetime } from "../mixins/HasLifetime";
import { HasAsyncState } from "../mixins/HasAsyncState";
import { FilterStateService, IFilterStateChangedEvent } from "../../services/FilterStateService";
import { FolderContents } from "../../vms/FolderContents";
import { ApiService } from "../../services/ApiService";
import { Folder } from "../../vms/Folder";
import { TagBinding } from "../../vms/TagBinding";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { SearchScope } from "../../../../../Common/source/vms/SearchScope";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { IObservable } from "../../../../../Common/source/utils/Interfaces";
import { Observable } from "../../../../../Common/source/utils/Observable";
import GalleryHeader from './GalleryHeader.vue';
import MediaViewer from "./MediaViewer.vue";

@Component({
    components: { MediaViewer, GalleryHeader }
})
export default class Gallery extends Mixins(HasAsyncState(), HasLifetime) {
    @Dep('$host') $host: string;
    @Dep('$api') $api: ApiService;
    @Dep('$filter') $filter: FilterStateService;
    
    error: string = null;
    empty: boolean = false;
    contents: FolderContents = null;
    isFilterActive: boolean = false;
    hasSafePaddings: boolean = false;

    indexFeed: IObservable<number> = new Observable<number>();

    $refs: {
        gallery: HTMLElement
    }
    
    get isEmpty() {
        return !this.contents?.media?.length
            && !this.contents?.subfolders?.length;
    }
    
    async mounted() {
        this.observe(this.$filter.onStateChanged, x => this.refresh(x));
        await this.refresh({ ...this.$filter.state, source: null });

        this.checkSafePaddings();
        window.addEventListener('orientationchange', this.orientationHandler);
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
                    scope: state.scope,
                    from: state.from?.toISOString(),
                    to: state.to?.toISOString(),
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

    checkSafePaddings() {
        this.hasSafePaddings = parseFloat(getComputedStyle(this.$refs.gallery).getPropertyValue("--sar")) > 0;
    }
    
    showFolder(f: Folder) {
        this.$filter.update('list', { folder: f.path })
    }
    
    filterByTag(t: TagBinding) {
        this.$filter.update('list', { folder: '/', scope: SearchScope.Everywhere, tags: [t.tag.id] });
    }
    
    showMedia(idx: number) {
        this.indexFeed.notify(idx);
    }
    
    getThumbnailPath(m: MediaThumbnail) {
        return 'url(' + this.$host + m.thumbnailPath + ')';
    }

    orientationHandler () {
        setTimeout(this.checkSafePaddings, 200);
    }

    beforeDestroy() {
        window.removeEventListener('orientationchange', this.orientationHandler);
    }
} 
</script>

<template>
    <div 
        ref="gallery"
        class="gallery" 
        :class="{'gallery_safe-paddings': hasSafePaddings}"
    >
        <gallery-header></gallery-header>
        <loading 
            :is-full-page="true"
            :is-loading="asyncState.isLoading"
        >
            <simplebar v-if="contents && !error && !isEmpty" class="gallery__content">
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
                                <div 
                                    class="folder-large"
                                    :class="{'folder-large_no-thumbnail': !f.thumbnailPath }"
                                >
                                    <div 
                                        v-if="f.thumbnailPath"
                                        class="folder-large__thumbnail" 
                                    >
                                        <div class="folder-large__thumbnail__icon">
                                            <div 
                                                class="folder-large__thumbnail__picture" 
                                                :style="{backgroundImage: getThumbnailPath(f)}"
                                            ></div>
                                        </div>
                                    </div>
                                </div>
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
            </simplebar>
        </loading>
        <div v-if="!asyncState.isLoading && isEmpty && !error" class="gallery__error">
            <div class="gallery__error__content">
                <div class="gallery__error__image gallery__error__image_cat"></div>
                <h3>Oops…</h3>
                <p v-if="isFilterActive">No matching media found.</p>
                <p v-else>This folder is empty.</p>
            </div>
        </div>
        <div v-if="!asyncState.isLoading && error" class="gallery__error">
            <div class="gallery__error__content">
                <div class="gallery__error__image"></div>
                <h3>Error</h3>
                <p>{{error}}</p>
            </div>
        </div>
        <MediaViewer :index-feed="indexFeed" :source="contents.media" v-if="contents && contents.media && contents.media.length"></MediaViewer>
    </div>
</template>

<style lang="scss">
    @import "../../../../../Common/styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .gallery {
        flex: 1 1 auto;
        display: flex;
        flex-direction: column;
        --sar: env(safe-area-inset-right);

        $content-sizes:
            3 375px  5.7291666667rem,
            3 390px  6.0666666667rem,
            3 414px  6.1666666667rem,
            3 438px  6.6666666667rem,
            3 504px  8.0416666667rem,
            4 507px  5.546875rem,
            4 551px  6.234375rem,
            4 568px  6.5rem,
            4 639px  7.609375rem,
            5 667px  6.0125rem,
            5 678px  6.15rem,
            5 694px  6.35rem,
            5 736px  6.875rem,
            6 768px  7.275rem,
            6 800px  6.0416666667rem,
            6 812px  6.1666666667rem,
            6 834px  6.3958333333rem,
            6 844px  6.5rem,
            6 896px  7.0416666667rem,
            6 926px  7.3541666667rem,
            6 981px  7.9270833333rem,
            5 992px  5.875rem,
            5 1024px 6.275rem,
            5 1112px 7.375rem,
            6 1280px 7.5416666667rem,
            6 1366px 8.4375rem,
            6 1440px 9.2083333333rem;

        &_safe-paddings #{&}__item #{&}__item__content {
            @each $amount, $screen-size, $content-size in $content-sizes {
                @media only screen and (min-width: $screen-size) {
                    width: calc(#{$content-size} - (((env(safe-area-inset-left)* 2) - 2rem)  / #{$amount}));
                }
            }
        }

        &:not(#{&}_safe-paddings) #{&}__item #{&}__item__content{
            @each $amount, $screen-size, $content-size in $content-sizes {
                @media only screen and (min-width: $screen-size) {
                    width: $content-size;
                }
            }
        }

        &_safe-paddings #{&}__item_picture #{&}__item__icon {
            @each $amount, $screen-size, $content-size in $content-sizes {
                @media only screen and (min-width: $screen-size) {
                    height: calc(#{$content-size} - (((env(safe-area-inset-left)* 2) - 2rem)  / #{$amount}));
                }
            }
        }

        &:not(#{&}_safe-paddings) #{&}__item_picture #{&}__item__icon {
            @each $amount, $screen-size, $content-size in $content-sizes {
                @media only screen and (min-width: $screen-size) {
                    height: $content-size;
                }
            }
        }

        &__content {
            width: 100%;
            flex: 1 1 auto;
            display: block;

            @include media-breakpoint-up(lg) {
                height: 0;
            }
        }

        &__tags {
            font-size: 0;
            line-height: 1;
            padding: 0.5rem 1rem;

            @supports(padding: max(0px)) {
                padding-left: max(1rem, env(safe-area-inset-left));
                padding-right: max(1rem, env(safe-area-inset-right));
            }

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

            @supports(padding: max(0px)) {
                padding-left: max(0.5rem, calc(env(safe-area-inset-left) - 0.5rem));
                padding-right: max(0.5rem, calc(env(safe-area-inset-right) - 0.5rem));
            }

            &__row {
                display: flex;
                flex-wrap: wrap;
            }
        }

        &__item {
            flex: 0 0 auto;
            display: flex;

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

                &:hover {
                    border-color: $gray-400;
                    box-shadow: $box-shadow-sm;
                }
            }

            &__caption {
                text-align: center;

                @include media-breakpoint-down(md) {
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

            &_picture &__icon {
                height: 4.5458333333rem;
                background-color: $gray-200;
                background-image: url(../../../images/image.svg);
                background-size: auto 3rem;

                @include media-breakpoint-up(lg) {
                    background-size: auto 4rem;
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
                height: 7rem;
                background-repeat: no-repeat;
                background-position: center;
                background-size: auto 100%;
                margin-bottom: 1rem;
                background-image: url(../../../images/error.svg);

                &_cat {
                    height: 10rem;
                    background-image: url(../../../images/cat.svg);
                }
            }
        }
    }

    .folder-large {
        width: 5.6rem;
        height: 5.6rem;
        margin: 0.5rem auto;
        position: relative;

        @include media-breakpoint-down(md) {
            width: 4.6rem;
            height: 4.6rem;
        }

        &:before,
        &:after {
            width: 100%;
            height: 100%;
            display: block;
            position: absolute;
            content: "";
            background-image: url(../../../images/folder-large.svg);
            background-size: auto 200%;
        }

        &:before {
            z-index: 1;
            background-position: center top;
        }

        &:after {
            z-index: 3;
            background-position: center bottom;
        }

        &_no-thumbnail {
            &:before,
            &:after {
                background-image: url(../../../images/folder-large_empty.svg);
            }
        }

        &__thumbnail {
            top: 9%;
            left: 10%;
            z-index: 2;
            width: 80%;
            height: 80%;
            padding: 0.2rem;
            position: absolute;
            background: $white;
            border-radius: 0.2rem;
            box-sizing: border-box;
            transform: skew(0, 8deg);
            border: 1px solid $gray-300;

            &__icon {
                width: 100%;
                height: 100%;
                background-image: url(../../../images/image.svg);
                background-size: auto 50%;
                background-repeat: no-repeat;
                background-position: center;
                background-color: $gray-200;
            }

            &__picture {
                width: 100%;
                height: 100%;
                background-size: 100% 100%;
            }
        }
    }
</style>