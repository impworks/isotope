<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";
    import Filters from './Filters.vue';
    import Folders from './Folders.vue';

    @Component({
        components: { 
            Filters,
            Folders
        }
    })
    export default class Sidebar extends Vue {} 
</script>

<template>
    <div class="sidebar">
        <div class="header">
            <router-link
                to="/"
                class="logo"
            >
                isotope
            </router-link>
        </div>
        <div class="sidebar-content">
            <filters></filters>
            <folders></folders>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .sidebar {
        z-index: 1;
        width: 21rem;
        position: relative;
        background: $white;

        @include media-breakpoint-down(sm) {
            top: 0;
            width: 100%;
            position: fixed;
            border-bottom: 1px solid $gray-300;
        }

        @include media-breakpoint-up(md) {
            flex: 0 0 auto;
            display: flex;
            flex-direction: column;
            border-right: 1px solid $gray-300;
        }

        .header {
            flex: 0 0 auto;
            display: flex;
            flex-direction: row;
            justify-content: space-between;
            align-items: center;
            padding: 1rem;

            .logo {
                $font-size: 1.8rem;

                display: block;
                color: $gray-900;
                line-height: 1;
                font-weight: bold;
                font-size: $font-size;
                background-image: url(../../images/logo.svg);
                background-size: auto 100%;
                background-repeat: no-repeat;
                padding-left: $font-size * 1.85;
                padding-right: 1rem;

                &:hover {
                    text-decoration: none;
                }
            }
        }

        &-button {
            display: flex;
            flex-direction: row;
            color: $gray-800;
            background: $gray-200;
            padding: 0.6rem 1rem;
            border-top: 1px solid $gray-300;
            border-bottom: 1px solid $gray-300;
            transition: all 200ms linear;

            &:hover {
                color: $gray-800;
                background: $gray-300;
                border-color: $gray-400;
                text-decoration: none;

                .sidebar-button__arrow {
                    color: $gray-700;
                }
            }

            &__icon {
                flex: 0 1 auto;

                > * {
                    $size: 1.5em;

                    width: $size;
                    height: $size;
                    background-position: 0 0;
                    background-size: auto 100%;
                    background-repeat: no-repeat;
                }
            }

            &__text {
                flex: 1 1 auto;
                padding: 0 1em;
            }

            &__arrow {
                color: $gray-600;
                transition: color 200ms linear,
                            transform 150ms ease;
            }

            &.opened &__arrow {
                transform: rotate(180deg);
            }
        }

        &-content {
            position: relative;

            @include media-breakpoint-down(sm) {
                display: none;
            }

            @include media-breakpoint-up(md) {
                flex: 1 1 auto;
                display: flex;
                flex-direction: column;
            }
        }
    }
</style>