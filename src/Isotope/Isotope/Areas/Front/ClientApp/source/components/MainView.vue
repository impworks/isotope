<script setup lang="ts">
import { ref, computed, onMounted, inject } from 'vue';
import { useAsyncState } from '@/composables/useAsyncState';
import { ApiServiceKey, FilterStateServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import type { FilterStateService } from '@/services/FilterStateService';
import { Folder } from '@/vms/Folder';
import Gallery from './content/Gallery.vue';
import MainHeader from './content/MainHeader.vue';
import Sidebar from './sidebar/Sidebar.vue';
import GalleryHeader from './content/GalleryHeader.vue';

const $api = inject(ApiServiceKey)!;
const $filter = inject(FilterStateServiceKey)!;
const { asyncState, showLoading } = useAsyncState();

const folderTree = ref<Folder[]>([]);

const isSidebarHidden = computed(() => {
  return $filter.shareId !== null && folderTree.value.length == 0 && !asyncState.isLoading;
});

onMounted(async () => {
  try {
    showLoading(async () => {
      folderTree.value = await $api.getFolderTree();
    });
  } catch (e) {
    // Ignore errors
  }
});
</script>

<template>
  <div class="main-view" :class="{ 'main-view_no-sidebar': isSidebarHidden }">
    <div class="main-view__header">
      <main-header></main-header>
      <gallery-header></gallery-header>
    </div>
    <div class="main-view__content">
      <sidebar></sidebar>
      <gallery></gallery>
    </div>
  </div>
</template>

<style lang="scss">
@import "../../../../Common/styles/variables";
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";
@import "bootstrap/scss/mixins";

.main-view {
  display: flex;
  min-height: 0;
  flex-direction: column;

  @include media-breakpoint-down(md) {
    padding-top: 3.6875rem;
    min-height: 100%;
  }

  @include media-breakpoint-up(lg) {
    height: 100%;
    overflow: hidden;
  }

  &_no-sidebar {
    .main-header {
      flex: 1 0 auto;
      border-right: none;
    }

    .gallery-header {
      display: none;
    }

    .sidebar {
      display: none;
    }
  }

  &__header {
    flex: 0 0 auto;
    display: flex;

    @include media-breakpoint-down(md) {
      flex-direction: column;
    }

    @include media-breakpoint-up(lg) {
      flex-direction: row;
      border-bottom: 1px solid $gray-300;
    }

    .gallery-header {
      flex: 1 0 auto;
    }
  }

  &__content {
    flex: 1 1 auto;
    display: flex;

    @include media-breakpoint-down(md) {
      flex-direction: column;
    }

    @include media-breakpoint-up(lg) {
      flex-direction: row;
      min-height: 0;
    }
  }
}
</style>
