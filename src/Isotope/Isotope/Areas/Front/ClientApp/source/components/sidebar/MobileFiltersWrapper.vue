<script lang="ts">
import { Component, Model, Vue, Watch } from "vue-property-decorator";
import { FilterStateService } from "../../services/FilterStateService";
import { Dep } from "../../utils/VueInjectDecorator";
import Filters from './Filters.vue';

@Component({
    components: { Filters }
})
export default class MobileFiltersWrapper extends Vue {
    @Dep('$filter') $filter: FilterStateService;
    @Model() model: boolean;

    isVisible: boolean = false;

    mounted() {
        this.isVisible = this.model;
    }

    close() {
        this.isVisible = false;
    }

    @Watch('model')
    onModelChanged() {
        this.isVisible = this.model;
    }

    @Watch('isVisible')
    onVisibilityChanged(value: boolean) {
        const bodyClass = "mobile-filters-open";

        this.$emit('model', value);

        if (value) {
            document.body.classList.add(bodyClass);
        } else {
            document.body.classList.remove(bodyClass);
        }
    }
    
    reset() {
        this.$filter.clear('filters');
        this.close();
    }
}
</script>

<template>
    <portal to="modal">
        <div class="mobile-filters-modal">
            <transition name="mobile-filters-modal__fade">
                <div 
                    class="mobile-filters-modal__backdrop clickable"
                    v-if="model"
                    @click.prevent="close"
                ></div>
            </transition>
            <transition name="mobile-filters-modal__slide">
                <div 
                    class="mobile-filters-modal__content mobile-filters"
                    v-if="model"
                >
                    <div class="mobile-header">
                        <div class="mobile-header__caption">Filters</div>
                        <div class="mobile-header__actions">
                            <button
                                href="#" 
                                class="btn-header btn-header_danger" 
                                @click.prevent="reset"
                            >
                                <div class="btn-header__content">
                                    Clear
                                </div>
                            </button>
                        </div>
                    </div>
                    <simplebar class="mobile-filters__content">
                        <Filters></Filters>
                    </simplebar>
                    <div class="mobile-filters__actions">
                        <button 
                            type="button" 
                            class="btn btn-block btn-primary"
                            @click.prevent="close"
                        >
                            Ok
                        </button>
                    </div>
                </div>
            </transition>
        </div>
    </portal>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .mobile-filters-modal {

        @include media-breakpoint-up(md) {
            display: none;
        }

        @mixin fixed () {
            top: 0;
            height: 100%;
            position: fixed;
        }
        
        &__backdrop {
            @include fixed();
            left: 0;
            width: 100%;
            background: rgba($dark, 0.5);
            z-index: $zindex-modal-backdrop;
            will-change: opacity;
            backface-visibility: hidden;
        }

        &__content {
            @include fixed();
            right: 0;
            width: 18.5rem;
            background: $white;
            z-index: $zindex-modal;
            box-shadow: $box-shadow;
            will-change: transform;
            backface-visibility: hidden;

            @include media-breakpoint-up(lg) {
                width: 21rem;
            }

            @media only screen and (max-width: 320px) {
                width: 90%;
            }

            @media only screen and (max-width: 300px) {
                width: 100%;
            }
        }

        &__slide {

            &-enter-active, 
            &-leave-active {
                transition: transform 300ms ease-out;
            }

            &-enter, 
            &-leave-to {
                transform: translateX(100%);
            }
        }

        &__fade {

            &-enter-active, 
            &-leave-active {
                transition: opacity 150ms;
            }

            &-enter, 
            &-leave-to {
                opacity: 0;
            }
        }
    }

    .mobile-header {
        display: flex;
        justify-content: space-between;
        border-bottom: 1px solid $gray-300;

        &__caption {
            flex: 1 1 auto;
            padding: 0.9125rem 1rem;
            font-weight: bold;
            font-size: 1.2rem;
        }

        &__actions {
            
            .btn-header:last-child {
                padding-right: 1rem;
            }
        }
    }

    .mobile-filters {
        display: flex;
        flex-direction: column;

        .mobile-header {
            flex: 0 0 auto;
        }

        &__content {
            height: 0;
            flex: 1 1 auto;
        }

        &__actions {
            padding: 1rem;
            flex: 0 0 auto;
            border-top: 1px solid $gray-300;
        }
    }

    .mobile-filters-open {
        overflow: hidden;
    }
</style>