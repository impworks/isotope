<script lang="ts">
import { Component, Model, Vue, Watch } from "vue-property-decorator";
import Filters from './Filters.vue';

@Component({
    components: { Filters }
})
export default class MobileFilters extends Vue {
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
                            <a 
                                href="#" 
                                class="btn-header" 
                                @click.prevent="close"
                            >
                                <div class="btn-header__content">
                                    <i class="icon icon-cross"></i>
                                </div>
                            </a>
                        </div>
                    </div>
                    <simplebar class="mobile-filters__content">
                        <div v-for="i in 100">
                            scrollable filters
                        </div>
                    </simplebar>
                    <div class="mobile-filters__actions">
                        <button type="button" class="btn btn-block btn-primary">Reset?</button>
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
        }

        &__content {
            @include fixed();
            right: 0;
            width: 17rem;
            background: $white;
            z-index: $zindex-modal;
            box-shadow: $box-shadow;
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

    .mobile-filters-open {
        overflow: hidden;
        padding-right: 15px;

        .sidebar__header {
            padding-right: 15px;
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
            padding: 1rem;
        }

        &__actions {
            padding: 1rem;
            flex: 0 0 auto;
            border-top: 1px solid $gray-300;
        }
    }
</style>