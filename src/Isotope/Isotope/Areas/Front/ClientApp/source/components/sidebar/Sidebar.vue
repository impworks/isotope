<script setup lang="ts">
import { computed, inject } from 'vue';
import { FilterStateServiceKey } from '@/config/Injector';
import type { FilterStateService } from '@/services/FilterStateService';
import DesktopFiltersWrapper from './DesktopFiltersWrapper.vue';
import Folders from './Folders.vue';

const $filter = inject(FilterStateServiceKey)!;

const isFilterShown = computed(() => {
  return !$filter.shareId;
});
</script>

<template>
  <div class="sidebar">
    <div class="sidebar__content">
      <desktop-filters-wrapper v-if="isFilterShown"></desktop-filters-wrapper>
      <folders></folders>
    </div>
  </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";
@import "bootstrap/scss/mixins";

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

    @supports (padding: max(0px)) {
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

    @supports (padding: max(0px)) {
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
    border-bottom: 1px solid $gray-300;
    transition: all 200ms linear;

    &:before {
      left: 0;
      bottom: 100%;
      content: '';
      height: 1px;
      width: 100%;
      display: block;
      position: absolute;
      background: $gray-300;
      transition: all 200ms linear;
    }

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

      &:after,
      &:before {
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
      content: '';
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
      transition:
        color 200ms linear,
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
