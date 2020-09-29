<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import { Dep } from "../../utils/VueInjectDecorator";
import { FilterStateService } from "../../services/FilterStateService";
import Filters from './Filters.vue';
import Folders from './Folders.vue';

@Component({
    components: { Filters, Folders }
})
export default class Sidebar extends Vue {
    @Dep('$filter') $filter: FilterStateService;
    
    get isFilterShown() {
        return !this.$filter.shareId;
    }
    
    goToRoot() {
        this.$filter.update('logo', { folder: '/' });
    }
} 
</script>

<template>
    <div class="sidebar">
        <div class="sidebar__header">
            <a
                class="logotype clickable"
                @click.prevent="goToRoot()"
            >
                isotope
            </a>
        </div>
        <div class="sidebar__content">
            <filters v-if="isFilterShown"></filters>
            <folders></folders>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .sidebar {
        z-index: 1;
        width: 18.5rem;
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

        @include media-breakpoint-up(lg) {
            width: 21rem;
        }

        &__header {
            flex: 0 0 auto;
            display: flex;
            flex-direction: row;
            justify-content: space-between;
            align-items: center;
            padding: 1rem;
        }

        &-button {
            display: flex;
            flex-direction: row;
            position: relative;
            color: $gray-800;
            background: $gray-200;
            padding: 0.5em 1em;
            border-top: 1px solid $gray-300;
            border-bottom: 1px solid $gray-300;
            transition: all 200ms linear;

            &:hover {
                color: $gray-800;
                background: $gray-300;
                border-color: $gray-400;
                text-decoration: none;

                &:after {
                    background: $gray-400;
                }

                .sidebar-button__arrow {
                    color: $gray-700;
                }
            }

            &:active {
                background: $gray-400;
            }

            &:after {
                top: -1px;
                right: -1px;
                content: "";
                width: 1px;
                height: calc(100% + 2px);
                position: absolute;
                background: $gray-300;
            }

            &__icon {
                flex: 0 1 auto;

                > * {
                    $size: 1.4em;

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

            &_opened &__arrow {
                transform: rotate(180deg);
            }
        }

        &__content {
            position: relative;

            @include media-breakpoint-down(sm) {
                display: none;
            }

            @include media-breakpoint-up(md) {
                flex: 1 1 auto;
                display: flex;
                min-height: 0;
                flex-direction: column;
            }
        }
    }
</style>