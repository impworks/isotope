<script lang="ts">
import { Component, Model, Prop, Vue, Watch } from "vue-property-decorator";

@Component
export default class ModalWindow extends Vue {
    @Model() model: boolean;
    @Prop({ required: false }) isMobileOnly: boolean;

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
        const bodyClass = "modal-window-open";

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
    <transition name="modal-overlay__transition">
        <div 
            class="modal-overlay" 
            v-if="model"
            :class="{'modal-overlay_mobile-only' : isMobileOnly}"
        >
            <div 
                class="modal-overlay__backdrop clickable"
                @click.prevent="close"
            ></div>
            <div class="modal-window">
                <div class="modal-window__header">
                    <slot name="header"></slot>
                </div>
                <div class="modal-window__content">
                    <simplebar class="modal-window__scrollable">
                        <slot name="content"></slot>
                    </simplebar>
                </div>
                <div class="modal-window__footer" v-if="$slots.footer">
                    <slot name="footer"></slot>
                </div>
            </div>
        </div>
    </transition>
</template>

<style lang="scss">
    @import "../../../../../Common/styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .modal-overlay {
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        position: fixed;
        display: flex;
        z-index: $zindex-modal-backdrop;

        @include media-breakpoint-down(sm) {
            justify-content: flex-end;
        }

        @include media-breakpoint-up(md) {
            padding: 1rem;
            align-items: center;
            justify-content: center;
        }

        &_mobile-only {
            @include media-breakpoint-up(md) {
                display: none;
            }
        }

        &__backdrop {
            top: 0;
            left: 0;
            z-index: 1;
            width: 100%;
            height: 100%;
            position: absolute;
            background: rgba($dark, 0.5);
        }

        &__transition {

            &-enter-active, 
            &-leave-active {
                transition: all 400ms linear;

                .modal-overlay__backdrop {
                    transition: opacity 200ms cubic-bezier(.645,.045,.355,1);
                }
            
                .modal-window {
                    
                    @include media-breakpoint-down(sm) {
                        transition: transform 400ms cubic-bezier(.645,.045,.355,1);
                    }

                    @include media-breakpoint-up(md) {
                        transition: opacity 200ms cubic-bezier(.645,.045,.355,1);
                    }
                }
            }

            &-enter, 
            &-leave-to {
                .modal-overlay__backdrop {
                    opacity: 0;
                }

                .modal-window {

                    @include media-breakpoint-down(sm) {
                        transform: translateX(100%);
                    }

                    @include media-breakpoint-up(md) {
                        opacity: 0;
                    }
                }
            }
        }
    }

    .modal-window {
        z-index: 2;
        position: relative;
        display:flex; 
        flex-direction:column;
        background-color: $white;

        @media only screen and (max-width: 300px) {
            width: 100%;
        }

        @media only screen and (max-width: 320px) {
            width: 90%;
        }

        @include media-breakpoint-down(sm) {
            height: 100%;
            width: 18.5rem;
            box-shadow: $box-shadow;
        }

        @include media-breakpoint-up(md) {
            width: 33rem;
            max-height: 100%;
            box-shadow: $box-shadow-lg;
            border-radius: $border-radius;
        }

        &__header,
        &__footer   {
            flex: 0 0 auto;
        }

        &__header {
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

        &__content {
            display: flex;
            flex: 1 1 auto;
            overflow: hidden;
        }

        &__scrollable {
            padding: 1rem;
            flex: 1 1 auto;
        }

        &__footer {
            flex: 0 0 auto;
            padding: 1rem;
            border-top: 1px solid $gray-300;
        }
    }

    .modal-window-open {
        overflow: hidden;
    }
</style>