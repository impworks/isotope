<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component
    export default class Gallery extends Vue {} 
</script>

<template>
    <div class="gallery">
        <div class="gallery__header">
            <ul class="breadcrumbs">
                <li class="breadcrumbs__item">
                    <a href="#">My gallery</a>
                </li>
                <li class="breadcrumbs__item">
                    <a href="#">Subfolder</a>
                </li>
                <li class="breadcrumbs__item breadcrumbs__item_active">
                    Active folder
                </li>
            </ul>
        </div>
        <perfect-scrollbar class="gallery__content">
            <div class="gallery__grid">
                <div class="gallery__grid__row">
                    <div
                        class="gallery__item gallery__item_folder"
                        v-for="i in 13"
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

        &__grid {
            padding: 0.5rem 0.5rem 1.5rem;

            &__row {
                display: flex;
                flex-wrap: wrap;
            }
        }

        &__item {
            flex: 0 0 33.3333333333%;
            max-width: 33.3333333333%;

            @include media-breakpoint-up(sm) {
                flex: 0 0 25%;
                max-width: 25%;
            }

            @include media-breakpoint-up(md) {
                flex: 0 0 20%;
                max-width: 20%;
            }
            
            @include media-breakpoint-up(lg) {
                flex: 0 0 16.6666666667%;
                max-width: 16.6666666667%;
            }

            @include media-breakpoint-up(xl) {
                flex: 0 0 14.2857142857%;
                max-width: 14.2857142857%;
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
                height: 5rem;
                background-color: $gray-200;
                background-image: url(../../images/image.svg);
                background-size: auto 4rem;
            }

            &_picture &__preview {
                width: 100%;
                height: 100%;
                background-size: cover;
            }
        }
    }
    
    .breadcrumbs {
        list-style-type: none;
        margin: 0;
        padding: 1rem;
        display: flex;
        flex-direction: row;
        line-height: 1.625;

        &__item {
            position: relative;
            padding: 0 2em 0 0;
            
            &:not(:last-child):after {
                top: 0;
                right: 0;
                width: 2em;
                content: '>';
                color: $gray-500;
                position: absolute;
                text-align: center;
                display: inline-block;
            }

            &_active {
                font-weight: 500;
            }
        }
    }
</style>