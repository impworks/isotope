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

@Component({
    components: { ModalWindow, Filters, UserDropdown }
})
export default class MainHeader extends Mixins(HasLifetime) {
    @Dep('$filter') $filter: FilterStateService;
    
    isMobileFiltersVisible: boolean = false;
    isFilterActive : boolean = false;
    
    get isFilterShown() {
        return !this.$filter.shareId;
    }

    mounted() {
        window.addEventListener("resize", this.resizeHandler);
    }

    goToRoot() {
        this.$filter.update('logo', { folder: '/', tags: null, dateFrom: null, dateTo: null });
    }

    resetFilters() {
        this.$filter.clear('filters');
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
    <div class="main-header">
        <a
            class="logotype clickable"
            @click.prevent="goToRoot()"
        >
            isotope
        </a>
        <div class="main-header__actions">
            <user-dropdown></user-dropdown>
            <button
                v-if="isFilterShown"
                class="main-header__filter-button btn-header"
                @click.prevent="isMobileFiltersVisible = !isMobileFiltersVisible"
            >
                <div class="btn-header__content">
                    <i class="icon icon-filter"></i>
                    <div v-if="isFilterActive" class="btn-header__badge"></div>
                </div>
            </button>
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

    .main-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: center;
        padding-left: 1rem;
        background: $white;

        @supports(padding: max(0px)) {
            padding-left: max(1rem, env(safe-area-inset-left));
        }

        @include media-breakpoint-down(md) {
            width: 100%;
            top: 0;
            width: 100%;
            z-index: 10;
            position: fixed;
            border-bottom: 1px solid $gray-300;
        }
        
        @include media-breakpoint-up(lg) {
            width: 21rem;
            border-right: 1px solid $gray-300;
        }

        &__actions {
            display: flex;
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
    }
</style>