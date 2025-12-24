<script setup lang="ts">
import { ref, onMounted, inject, getCurrentInstance } from 'vue';
import { useAsyncState } from '@/composables/useAsyncState';
import { useLifetime } from '@/composables/useLifetime';
import { ApiServiceKey, FilterStateServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import type { FilterStateService } from '@/services/FilterStateService';
import { Folder } from '@/vms/Folder';
import FolderTreeItem from './FolderTreeItem.vue';
import Loading from '@/components/utils/Loading.vue';

const $api = inject(ApiServiceKey)!;
const $filter = inject(FilterStateServiceKey)!;
const { asyncState, showLoading } = useAsyncState();
const { observe } = useLifetime();
const instance = getCurrentInstance();

const currentPath = ref<string | null>(null);
const folders = ref<Folder[] | null>(null);
const scrollRef = ref<any>(null);

function scrollToSelected() {
  setTimeout(() => {
    const scroll = scrollRef.value?.scrollElement as HTMLElement;
    if (!scroll) {
      return;
    }

    if (scroll.scrollHeight <= scroll.clientHeight) {
      return;
    }

    const selected = scroll.querySelector('.folder-tree-link_active');
    if (!selected) {
      return;
    }

    const scrollRect = scroll.getBoundingClientRect();
    const selectedRect = selected.getBoundingClientRect();
    const top = selectedRect.top - scrollRect.top - selectedRect.height * 2 + 1;
    scroll.scrollTo({ behavior: 'smooth', top: top });
  }, 10);
}

onMounted(async () => {
  currentPath.value = $filter.state.folder;
  observe($filter.onStateChanged, (x) => {
    currentPath.value = x.folder;
    if (x.source !== 'tree') {
      scrollToSelected();
    }
  });
  try {
    await showLoading(async () => {
      folders.value = await $api.getFolderTree();
      scrollToSelected();
    });
  } catch (e) {
    console.log('Failed to get folders.', e);
  }
});
</script>

<template>
  <loading :is-loading="asyncState.isLoading" :is-full-page="true">
    <simplebar class="folder-tree" ref="scrollRef">
      <div class="alert alert-warning m-3" v-if="folders && folders.length === 0">
        <span>There are no folders yet.</span>
      </div>
      <FolderTreeItem v-for="f in folders" :folder="f" :key="f.path" :depth="0" :current-path="currentPath!" />
    </simplebar>
  </loading>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";

.folder-tree {
  height: 0;
  flex: 1 1 auto;
}
</style>
