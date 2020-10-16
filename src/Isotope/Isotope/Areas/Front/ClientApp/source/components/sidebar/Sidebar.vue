<script lang="ts">
import { Vue, Component, Mixins } from "vue-property-decorator";
import { Dep } from "../../utils/VueInjectDecorator";
import { FilterStateService, IFilterStateChangedEvent } from "../../services/FilterStateService";
import DesktopFiltersWrapper from "./DesktopFiltersWrapper.vue";
import MobileFiltersWrapper from "./MobileFiltersWrapper.vue";
import Folders from './Folders.vue';
import { IObservable } from "../../utils/Interfaces";
import { Observable } from "../../utils/Observable";
import { HasLifetime } from "../mixins/HasLifetime";

@Component({
    components: { DesktopFiltersWrapper, Folders, MobileFiltersWrapper }
})
export default class Sidebar extends Mixins(HasLifetime) {
    @Dep('$filter') $filter: FilterStateService;

    avatar: string = null;
    isMobileFiltersVisible: boolean = false;
    isFilterActive : boolean = false;
    
    get isFilterShown() {
        return !this.$filter.shareId;
    }

    async mounted() {
        this.observe(this.$filter.onStateChanged, x => this.refresh(x));
        await this.refresh({ ...this.$filter.state, source: null });
    }

    refresh(state: IFilterStateChangedEvent) {
        if(state.source === 'viewer')
            return;

        this.isFilterActive = !this.$filter.isEmpty(state);
    }

    goToRoot() {
        this.$filter.update('logo', { folder: '/', tags: null, dateFrom: null, dateTo: null });
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
            <div class="sidebar__header__actions">
                <button class="btn-header">
                    <div class="btn-header__content">
                        <i 
                            class="icon icon-user"
                            v-if="!avatar" 
                        ></i>
                        <div 
                            class="avatar"
                            v-else 
                            style="background-image: url('../../../images/avatar.jpg')"
                        ></div>
                    </div>
                </button>
                <button
                    class="sidebar__filter-button btn-header"
                    @click.prevent="isMobileFiltersVisible = !isMobileFiltersVisible"
                >
                    <div class="btn-header__content">
                        <i class="icon icon-filter"></i>
                        <div v-if="isFilterActive" class="btn-header__badge"></div>
                        <MobileFiltersWrapper v-model="isMobileFiltersVisible"></MobileFiltersWrapper>
                    </div>
                </button>
            </div>
        </div>
        <div class="sidebar__content">
            <DesktopFiltersWrapper v-if="isFilterShown"></DesktopFiltersWrapper>
            <Folders></Folders>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .sidebar {
        z-index: 10;
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
            padding-left: 1rem;

            &__actions {
                display: flex;
            }
        }

        .btn-header {
            @include media-breakpoint-up(md) {
                padding-right: 1rem;
            }
        }

        &__filter-button.btn-header {
            padding-right: 1rem;

            @include media-breakpoint-up(md) {
                display: none;
            }
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