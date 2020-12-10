<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { FilterStateService, IFilterStateChangedEvent } from "../../services/FilterStateService";
import { HasLifetime } from "../mixins/HasLifetime";
import { Bind } from 'lodash-decorators';
import { Debounce } from 'lodash-decorators';
import { BreakpointHelper, Breakpoints } from "../../utils/BreakpointHelper";
import UserDropdown from "./UserDropdown.vue";
import ModalWindow from "../utils/ModalWindow.vue";
import Filters from "./Filters.vue";
import DesktopFiltersWrapper from "./DesktopFiltersWrapper.vue";
import Folders from './Folders.vue';

@Component({
    components: { DesktopFiltersWrapper, Folders, ModalWindow, Filters, UserDropdown }
})
export default class Sidebar extends Mixins(HasLifetime) {
    @Dep('$filter') $filter: FilterStateService;
    
    isMobileFiltersVisible: boolean = false;
    isFilterActive : boolean = false;
    
    get isFilterShown() {
        return !this.$filter.shareId;
    }

    mounted() {
        this.observe(this.$filter.onStateChanged, x => this.refresh(x));
        this.refresh({ ...this.$filter.state, source: null });
        window.addEventListener("resize", this.resizeHandler);
    }

    refresh(state: IFilterStateChangedEvent) {
        if(state.source === 'viewer')
            return;

        this.isFilterActive = !this.$filter.isEmpty(state);
    }

    resetFilters() {
        this.$filter.clear('filters');
    }

    goToRoot() {
        this.$filter.update('logo', { folder: '/', tags: null, dateFrom: null, dateTo: null });
    }

    beforeDestroy() {
        window.removeEventListener('resize', this.resizeHandler);
    }

    @Debounce(50)
    @Bind()
    resizeHandler() {
        if (BreakpointHelper.up(Breakpoints.md)) {
            this.isMobileFiltersVisible = false;
        }
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
                <user-dropdown></user-dropdown>
                <button
                    v-if="isFilterShown"
                    class="sidebar__filter-button btn-header"
                    @click.prevent="isMobileFiltersVisible = !isMobileFiltersVisible"
                >
                    <div class="btn-header__content">
                        <i class="icon icon-filter"></i>
                        <div v-if="isFilterActive" class="btn-header__badge"></div>
                    </div>
                </button>
            </div>
        </div>
        <div class="sidebar__content">
            <desktop-filters-wrapper v-if="isFilterShown"></desktop-filters-wrapper>
            <folders></folders>
        </div>
        <modal-window 
            v-model="isMobileFiltersVisible"
            :isMobileOnly="true"
        >
            <template v-slot:header>
                <div class="modal-window__header__caption">Filters</div>
                <div class="modal-window__header__actions">
                    <button
                        href="#" 
                        class="btn-header btn-header_danger" 
                        @click.prevent="resetFilters"
                    >
                        <div class="btn-header__content">
                            Clear
                        </div>
                    </button>
                </div>
            </template>
            <template v-slot:content>
                <filters></filters>
            </template>
            <template v-slot:footer>
                <button 
                    type="button" 
                    class="btn btn-block btn-primary"
                    @click.prevent="isMobileFiltersVisible =! isMobileFiltersVisible"
                >
                    Ok
                </button>
            </template>
        </modal-window>
    </div>
</template>

<style lang="scss">
    @import "../../../../../Common/styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .sidebar {
        z-index: 10;
        width: 18.5rem;
        position: relative;
        background: $white;

        @include media-breakpoint-down(md) {
            top: 0;
            width: 100%;
            position: fixed;
            border-bottom: 1px solid $gray-300;
        }

        @include media-breakpoint-up(lg) {
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

            @supports(padding: max(0px)) {
                padding-left: max(1rem, env(safe-area-inset-left));
            }

            &__actions {
                display: flex;
            }
        }

        .btn-header {
            @include media-breakpoint-up(lg) {
                padding-right: 1rem;
            }
        }

        &__filter-button.btn-header {
            padding-right: 1rem;

            @supports(padding: max(0px)) {
                padding-right: max(1rem, env(safe-area-inset-right));
            }

            @include media-breakpoint-up(lg) {
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
                background: $gray-200;
                text-decoration: none;
            }

            &:active,
            .no-touch &:hover {
                color: $gray-800;
                background: $gray-300;
                border-color: $gray-400;

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

            @include media-breakpoint-down(md) {
                display: none;
            }

            @include media-breakpoint-up(lg) {
                flex: 1 1 auto;
                display: flex;
                min-height: 0;
                flex-direction: column;
            }
        }
    }
</style>