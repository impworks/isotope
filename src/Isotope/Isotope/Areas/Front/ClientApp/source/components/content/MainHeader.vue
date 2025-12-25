<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, inject } from 'vue';
import { FilterStateServiceKey, ApiServiceKey } from '@/config/Injector';
import type { FilterStateService } from '@/services/FilterStateService';
import type { ApiService } from '@/services/ApiService';
import { BreakpointHelper, Breakpoints } from '@/utils/BreakpointHelper';
import { useDebounceFn } from '@vueuse/core';
import UserDropdown from './UserDropdown.vue';
import ModalWindow from '../utils/ModalWindow.vue';
import Filters from './Filters.vue';

const $filter = inject(FilterStateServiceKey)!;
const $api = inject(ApiServiceKey)!;

const isMobileFiltersVisible = ref(false);
const isFilterActive = ref(false);
const caption = ref('isotope');

const isFilterShown = computed(() => {
  return !$filter.shareId;
});

function goToRoot() {
  $filter.update('logo', { folder: '/', tags: null, dateFrom: null, dateTo: null });
}

function resetFilters() {
  $filter.clear('filters');
}

const resizeHandler = useDebounceFn(() => {
  if (BreakpointHelper.up(Breakpoints.md)) {
    isMobileFiltersVisible.value = false;
  }
}, 50);

onMounted(async () => {
  window.addEventListener('resize', resizeHandler);

  try {
    const info = await $api.getInfo();
    caption.value = info.caption;
  } catch {
    // Ignore errors
  }
});

onUnmounted(() => {
  window.removeEventListener('resize', resizeHandler);
});
</script>

<template>
  <div class="main-header">
    <a class="logotype clickable" @click.prevent="goToRoot()">
      {{ caption }}
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
    <modal-window v-model="isMobileFiltersVisible" :isMobileOnly="true">
      <template v-slot:header>
        <div class="modal-window__header__caption">Filters</div>
        <div class="modal-window__header__actions">
          <button href="#" class="btn-header btn-header_danger" @click.prevent="resetFilters">
            <div class="btn-header__content">Clear</div>
          </button>
        </div>
      </template>
      <template v-slot:content>
        <filters></filters>
      </template>
      <template v-slot:footer>
        <button type="button" class="btn btn-block btn-primary" @click.prevent="isMobileFiltersVisible = !isMobileFiltersVisible">
          Ok
        </button>
      </template>
    </modal-window>
  </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";
@import "bootstrap/scss/mixins";

.main-header {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  padding-left: 1rem;
  background: $white;

  @supports (padding: max(0px)) {
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

    @supports (padding: max(0px)) {
      padding-right: max(1rem, env(safe-area-inset-right));
    }

    @include media-breakpoint-up(lg) {
      display: none;
    }
  }
}
</style>
