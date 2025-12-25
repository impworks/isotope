<script setup lang="ts">
import { watch, onMounted, onUnmounted } from 'vue';

interface Props {
  isMobileOnly?: boolean;
}

defineProps<Props>();

const model = defineModel<boolean>({ default: false });

function close() {
  model.value = false;
}

function orientationHandler() {
  document.documentElement.style.height = `initial`;
  setTimeout(() => {
    document.documentElement.style.height = `100%`;
    setTimeout(() => {
      window.scrollTo(0, 1);
    }, 200);
  }, 200);
}

watch(model, (value) => {
  const bodyClass = 'modal-window-open';

  if (value) {
    document.body.classList.add(bodyClass);
  } else {
    document.body.classList.remove(bodyClass);
  }
});

onMounted(() => {
  window.addEventListener('orientationchange', orientationHandler);
});

onUnmounted(() => {
  window.removeEventListener('orientationchange', orientationHandler);
});
</script>

<template>
  <Transition name="modal-overlay__transition">
    <div v-if="model" class="modal-overlay" :class="{ 'modal-overlay_mobile-only': isMobileOnly }">
      <div class="modal-overlay__backdrop clickable" @click.prevent="close"></div>
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
  </Transition>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";
@import "bootstrap/scss/mixins";

.modal-overlay {
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  position: fixed;
  display: flex;
  z-index: $zindex-modal-backdrop;

  @include media-breakpoint-down(md) {
    justify-content: flex-end;
  }

  @include media-breakpoint-up(lg) {
    padding: 1rem;
    align-items: center;
    justify-content: center;
  }

  &_mobile-only {
    @include media-breakpoint-up(lg) {
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
        transition: opacity 200ms cubic-bezier(0.645, 0.045, 0.355, 1);
      }

      .modal-window {
        @include media-breakpoint-down(md) {
          transition: transform 400ms cubic-bezier(0.645, 0.045, 0.355, 1);
        }

        @include media-breakpoint-up(lg) {
          transition: opacity 200ms cubic-bezier(0.645, 0.045, 0.355, 1);
        }
      }
    }

    &-enter-from,
    &-leave-to {
      .modal-overlay__backdrop {
        opacity: 0;
      }

      .modal-window {
        @include media-breakpoint-down(md) {
          transform: translateX(100%);
        }

        @include media-breakpoint-up(lg) {
          opacity: 0;
        }
      }
    }
  }
}

.modal-window {
  z-index: 2;
  position: relative;
  display: flex;
  flex-direction: column;
  background-color: $white;

  @media only screen and (max-width: 300px) {
    width: 100%;
  }

  @media only screen and (max-width: 320px) {
    width: 90%;
  }

  @include media-breakpoint-down(md) {
    height: 100%;
    width: 18.5rem;
    box-shadow: $box-shadow;
  }

  @include media-breakpoint-up(lg) {
    width: 33rem;
    max-height: 100%;
    box-shadow: $box-shadow-lg;
    border-radius: $border-radius;
  }

  &__header,
  &__footer {
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

        @supports (padding: max(0px)) {
          padding-right: max(1rem, env(safe-area-inset-right));
        }
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

    @include media-breakpoint-down(md) {
      @supports (margin: max(0px)) {
        margin-right: max(1px, calc(env(safe-area-inset-right) - 1rem));
      }
    }
  }

  &__footer {
    flex: 0 0 auto;
    padding: 1rem;
    border-top: 1px solid $gray-300;

    @supports (padding: max(0px)) {
      padding-right: max(1rem, calc(env(safe-area-inset-right)));
    }
  }

  .simplebar-content-wrapper {
    height: 100% !important;
  }
}

.modal-window-open {
  touch-action: none;
  overflow: hidden;
}
</style>
