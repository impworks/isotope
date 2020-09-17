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
    export default class Sidebar extends Vue {
        isFiltersActive: boolean = false;
    } 
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
            <div class="actions">
                actions
            </div>
        </div>
         <div class="sidebar-navigation">
            <div class="switch">
                <button 
                    class="left"
                    :class="{ 'active': !isFiltersActive }"
                    @click="isFiltersActive = false"
                >
                    <i class="icon icon-folder"></i> 
                    Folders
                </button>
                <button 
                    class="right"
                    :class="{ 'active': isFiltersActive }"
                    @click="isFiltersActive = true"
                >
                    <i class="icon icon-filter"></i> 
                    Filters
                </button>
                <div class="indicator"></div>
            </div>
        </div>
        <div class="sidebar-content">
            <transition name="slide-fade-left" mode="out-in">
                <filters v-if="isFiltersActive"></filters>
                <folders v-else></folders>
            </transition>
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
        position: relative;
        background: $white;
        width: 21rem;
        box-shadow: $box-shadow-sm;

        @include media-breakpoint-down(sm) {
            top: 0;
            width: 100%;
            position: fixed;
        }

        @include media-breakpoint-up(md) {
            flex: 0 0 auto;
            display: flex;
            flex-direction: column;
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

        &-navigation {
            background: $gray-100;
            border-top: 1px solid $gray-300;
            border-bottom: 1px solid $gray-300;
            padding: 0.75rem 1rem;

            @include media-breakpoint-down(sm) {
                display: none;
            }

            .switch {
                display: flex;
                position: relative;

                button {
                    z-index: 2;
                    flex: 1 1 50%;
                    display: block;
                    color: $gray-600;
                    font-size: 0.9rem;
                    font-weight: 500;
                    line-height: 1;
                    padding: 0.5em 1em;
                    transition: color 200ms ease;
                    border: 2px solid $gray-600;
                    background: none;

                    &:focus {
                        outline: none;
                    }

                    &.active {
                        color: $white;
                    }

                    &.left {
                        border-radius: $border-radius 0 0 $border-radius;
                        border-right: 0;

                        &.active ~ .indicator {
                            left: 0;
                        }
                    }

                    &.right {
                        border-radius: 0 $border-radius $border-radius 0;
                        border-left: 0;
                        
                        &.active ~ .indicator {
                            left: 50%;
                        }
                    }

                    .icon {
                        margin-right: 0.1em;
                    }
                }

                .indicator {
                    width: 50%;
                    height: 100%;
                    position: absolute;
                    z-index: 1;
                    background: $gray-600;
                    border-radius: $border-radius;
                    transition: left 200ms cubic-bezier(0.77, 0, 0.175, 1);
                }
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
                overflow: hidden;
            }
        }
    }
</style>