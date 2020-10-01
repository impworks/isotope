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
    <portal to="modal" class="mobile-filters">
        <transition name="backgrop-fade">
            <div 
                v-if="model"
                class="mobile-filters__backdrop"
                @click.prevent="close"
            ></div>
        </transition>
        <transition name="modal-slide">
            <simplebar 
                v-if="model"
                class="mobile-filters__content p-3"
            >
                <div v-for="i in 100">
                    scrollable filters
                </div>
            </simplebar>
        </transition>
    </portal>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";

    .mobile-filters {

        @mixin fixed () {
            top: 0;
            height: 100%;
            position: fixed;
        }
        
        &__backdrop {
            @include fixed();
            left: 0;
            width: 100%;
            background: rgba($dark, 0.7);
            z-index: $zindex-modal-backdrop;
        }

        &__content {
            @include fixed();
            right: 0;
            width: 17rem;
            background: $white;
            z-index: $zindex-modal;
        }
    }

    .modal-slide {

        &-enter-active, 
        &-leave-active {
            transition: transform 300ms ease-out;
        }

        &-enter, 
        &-leave-to {
            transform: translateX(100%);
        }
    }

    .backgrop-fade {

        &-enter-active, 
        &-leave-active {
            transition: opacity 150ms;
        }

        &-enter, 
        &-leave-to {
            opacity: 0;
        }
    }

    .mobile-filters-open {
        overflow: hidden;
    }
</style>