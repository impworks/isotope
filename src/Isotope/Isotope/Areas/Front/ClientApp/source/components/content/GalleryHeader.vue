<script setup lang="ts">
import { ref, onMounted, inject } from 'vue';
import { ApiServiceKey, FilterStateServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import type { FilterStateService } from '@/services/FilterStateService';
import { useLifetime } from '@/composables/useLifetime';
import { Folder } from '@/vms/Folder';
import { GalleryInfo } from '@/vms/GalleryInfo';
import ModalWindow from '../utils/ModalWindow.vue';
import ShareLinkDialog from './dialogs/ShareLinkDialog.vue';

interface Crumb {
  caption: string;
  path: string;
}

const $api = inject(ApiServiceKey)!;
const $filter = inject(FilterStateServiceKey)!;
const { observe } = useLifetime();

const info = ref<GalleryInfo | null>(null);
const folderTree = ref<Folder[]>([]);
const crumbs = ref<Crumb[]>([]);
const currentFolder = ref<string | null>(null);
const isShareLinkVisible = ref(false);

function update(path: string) {
  if (path === '/' || !path) {
    crumbs.value = [];
    currentFolder.value = info.value?.subcaption || '';
    return;
  }

  path = path.replace(/\/$/, '');
  const parts = path.split('/');
  const newCrumbs: Crumb[] = [{ caption: info.value?.subcaption || '', path: '/' }];
  let scope = folderTree.value;
  let tmpPath = '';
  for (let i = 1; i < parts.length - 1; i++) {
    tmpPath += '/' + parts[i];
    const sub = scope.find((x) => x.path === tmpPath);
    if (!sub) return;
    newCrumbs.push({ caption: sub.caption, path: sub.path });
    scope = sub.subfolders || [];
  }

  crumbs.value = newCrumbs;
  currentFolder.value = scope.find((x) => x.path == path)?.caption || 'Not found';
}

function selectFolder(crumb: Crumb) {
  $filter.update('breadcrumb', { folder: crumb.path });
}

onMounted(async () => {
  try {
    info.value = await $api.getInfo();
    folderTree.value = await $api.getFolderTree();
  } catch (e) {
    // Ignore errors
  }

  observe($filter.onStateChanged, (x) => update(x.folder));
  update($filter.state.folder);
});
</script>

<template>
  <div class="gallery-header">
    <div class="desktop-navigation">
      <div class="desktop-navigation__content">
        <ul class="breadcrumbs">
          <li class="breadcrumbs__item" v-for="(c, i) in crumbs" :key="i">
            <a href="#" @click.prevent="selectFolder(c)">
              {{ c.caption }}
            </a>
          </li>
          <li class="breadcrumbs__item breadcrumbs__item_active">
            {{ currentFolder }}
          </li>
        </ul>
      </div>
      <div class="desktop-navigation__actions">
        <button class="btn-header" v-if="info && info.isAdmin && info.isLinkValid === null" @click.prevent="isShareLinkVisible = !isShareLinkVisible">
          <div class="btn-header__content">
            <i class="icon icon-share"></i>
          </div>
        </button>
      </div>
    </div>
    <div class="mobile-navigation">
      <div class="mobile-navigation__actions">
        <button class="btn-header" v-if="crumbs.length" @click.prevent="selectFolder(crumbs[crumbs.length - 1])">
          <div class="btn-header__content">
            <i class="icon icon-back"></i>
          </div>
        </button>
      </div>
      <div class="mobile-navigation__caption">
        {{ currentFolder }}
      </div>
      <div class="mobile-navigation__actions">
        <button class="btn-header" v-if="info && info.isAdmin && info.isLinkValid === null" @click.prevent="isShareLinkVisible = !isShareLinkVisible">
          <div class="btn-header__content">
            <i class="icon icon-share"></i>
          </div>
        </button>
      </div>
    </div>
    <share-link-dialog v-model="isShareLinkVisible"></share-link-dialog>
  </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";
@import "bootstrap/scss/mixins";

.gallery-header {
  background: $white;

  @include media-breakpoint-down(md) {
    margin-top: -1px;
    border-top: 1px solid $gray-300;
    border-bottom: 1px solid $gray-300;
  }
}

.desktop-navigation {
  flex: 0 0 auto;
  display: flex;
  flex-direction: row;
  justify-content: space-between;

  @include media-breakpoint-down(md) {
    display: none;
  }

  &__actions {
    &:first-child .btn-header {
      padding-left: 1rem;
    }

    &:last-child .btn-header {
      padding-right: 1rem;
    }
  }
}

.mobile-navigation {
  flex: 0 0 auto;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  background: $gray-200;
  min-height: 3.125rem;

  @include media-breakpoint-up(lg) {
    display: none;
  }

  &__caption {
    font-weight: bold;
    flex: 1 1 auto;
    padding: 0 0.5rem;
    white-space: nowrap;
    overflow: hidden;
    text-align: center;
    text-overflow: ellipsis;

    &:first-child {
      padding-left: 1rem;
    }

    &:last-child {
      padding-right: 1rem;
    }
  }

  &__actions {
    min-width: 3.625rem;

    .btn-header {
      padding: 0.5rem 0.5rem;

      &__content {
        background: none !important;
      }
    }

    &:first-child .btn-header {
      padding-left: 1rem;

      @supports (padding: max(0px)) {
        padding-left: max(1rem, env(safe-area-inset-left));
      }
    }

    &:last-child .btn-header {
      padding-right: 1rem;

      @supports (padding: max(0px)) {
        padding-right: max(1rem, env(safe-area-inset-right));
      }
    }
  }
}

.breadcrumbs {
  list-style-type: none;
  margin: 0;
  padding: 1rem;
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  line-height: 1.625;

  &__item {
    position: relative;
    padding: 0 2em 0 0;
    white-space: nowrap;

    a {
      color: $gray-700;
      text-decoration: none;

      &:hover {
        color: $gray-700;
        text-decoration: none;
      }

      &:active,
      .no-touch &:hover {
        color: $gray-800;
        text-decoration: underline;
      }
    }

    &:not(:last-child):after {
      top: 0;
      right: 0;
      width: 2em;
      content: '>';
      color: $gray-400;
      position: absolute;
      text-align: center;
      display: inline-block;
    }

    &_active {
      font-weight: 500;
    }
  }
}
</style>
