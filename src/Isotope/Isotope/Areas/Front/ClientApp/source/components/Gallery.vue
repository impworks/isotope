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
            <div
                v-for="i in 100" 
                :key="i"
            >
                Gallery item
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
        flex: 1 0 auto;
        display: flex;
        flex-direction: column;

        &__header {
            background: $white;
            border-bottom: 1px solid $gray-300;

            @include media-breakpoint-down(md) {
                display: none;
            }
        }

        &__content {
            padding: 1rem;
            background: $gray-200;
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
                content: '>';
                position: absolute;
                display: inline-block;
                right: 0;
                top: 0;
                width: 2em;
                color: $gray-500;
                text-align: center;
            }

            &_active {
                font-weight: 500;
            }
        }
    }
</style>