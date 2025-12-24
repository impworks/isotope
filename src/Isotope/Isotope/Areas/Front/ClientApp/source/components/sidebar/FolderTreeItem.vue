<script setup lang="ts">
import { ref, watch, onMounted, inject } from 'vue';
import { Folder } from '@/vms/Folder';
import { FilterStateServiceKey } from '@/config/Injector';
import type { FilterStateService } from '@/services/FilterStateService';

interface Props {
  folder: Folder;
  depth: number;
  currentPath: string;
}

const props = defineProps<Props>();

const $filter = inject(FilterStateServiceKey)!;

const expanded = ref(false);
const active = ref(false);

function selectFolder() {
  $filter.update('tree', { folder: props.folder.path });
}

function refreshState() {
  active.value = props.currentPath === props.folder.path;
  expanded.value = active.value || props.currentPath.startsWith(props.folder.path + '/');
}

watch(() => props.currentPath, refreshState);

onMounted(() => {
  refreshState();
});
</script>

<template>
  <div class="folder-tree-item">
    <a
      class="folder-tree-link clickable"
      :class="{
        'folder-tree-link_active': active,
        'folder-tree-link_styled': depth > 1,
        'folder-tree-link_expanded': expanded
      }"
      :key="folder.path"
      @click.prevent="selectFolder()"
    >
      <div class="folder-tree-link__icon" :style="{ marginLeft: depth * 0.7 + 'em' }"></div>
      <div class="folder-tree-link__name">{{ folder.caption }}</div>
    </a>
    <template v-if="folder.subfolders && folder.subfolders.length && expanded">
      <FolderTreeItem
        v-for="s in folder.subfolders"
        :folder="s"
        :key="s.path"
        :depth="depth + 1"
        :current-path="currentPath"
      />
    </template>
  </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "./node_modules/bootstrap/scss/functions";
@import "./node_modules/bootstrap/scss/variables";

.folder-tree-link {
  display: flex;
  flex-direction: row;
  padding: 0.5em 1em;
  color: $gray-800;

  &:hover {
    color: $gray-800;
    text-decoration: none;
  }

  &:active,
  .no-touch &:hover:not(#{&}_active) {
    color: $gray-800;
    background-color: $gray-200;
  }

  &-icon,
  &-name {
    flex: 0 0 auto;
  }

  &__icon {
    $size: 1.4em;

    width: $size;
    height: $size;
    background-image: url(../../../images/folder.svg);
    background-position: 0 0;
    background-size: auto 200%;
    background-repeat: no-repeat;
  }

  &__name {
    flex-grow: 1;
    padding: 0 1em;
    flex: 0 1 auto;
  }

  &_active #{&}__icon,
  &_expanded #{&}__icon {
    background-position: 0 100%;
  }

  &_active,
  &_active:hover {
    color: $white;
    background-color: $primary;
    border-color: $primary;
    text-decoration: none;
  }
}
</style>
