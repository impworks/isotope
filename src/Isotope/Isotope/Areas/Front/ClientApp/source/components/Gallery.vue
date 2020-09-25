<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";
    import Breadcrumbs from './Breadcrumbs.vue';

    @Component({
        components: { 
            Breadcrumbs
        }
    })
    export default class Gallery extends Vue {} 
</script>

<template>
    <div class="gallery">
        <div class="gallery__header">
            <breadcrumbs/>
        </div>
        <perfect-scrollbar class="gallery__content">
            <div class="gallery__tags">
                <a href="#" class="badge badge-primary">Primary</a>
                <a href="#" class="badge badge-success">Success</a>
                <a href="#" class="badge badge-danger">Danger</a>
                <a href="#" class="badge badge-warning">Warning</a>
                <a href="#" class="badge badge-info">Info</a>
                <a href="#" class="badge badge-dark">Dark</a>
            </div>
            <div class="gallery__grid">
                <div class="gallery__grid__row">
                    <div
                        class="gallery__item gallery__item_folder"
                        v-for="i in 12"
                        :key="i"
                    >
                        <a 
                            href="#"
                            class="gallery__item__content"
                        >
                            <div class="gallery__item__icon"></div>
                            <div class="gallery__item__caption">
                                Folder name
                            </div>
                        </a>
                    </div>
                </div>
                <div class="gallery__grid__row">
                    <div 
                        href="#"
                        class="gallery__item gallery__item_picture"
                        v-for="i in 45"
                        :key="i"
                    >
                        <a 
                            href="#"
                            class="gallery__item__content"
                        >
                            <div class="gallery__item__icon">
                                <div class="gallery__item__preview" v-if="i % 5 == 0" style="background-image:url(../../images/test_image1.jpg)"></div>
                                <div class="gallery__item__preview" v-else-if="i % 6 == 0" style="background-image:url(../../images/test_image2.jpg)"></div>
                                <div class="gallery__item__preview" v-else-if="i % 4 == 0" style="background-image:url(../../images/test_image3.jpg)"></div>
                            </div>
                        </a>
                    </div>
                </div>
            </div>
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