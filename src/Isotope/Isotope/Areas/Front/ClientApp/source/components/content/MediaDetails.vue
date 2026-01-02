<script setup lang="ts">
import { ref, computed, watch, inject } from 'vue';
import { Media } from '@/vms/Media';
import { FilterStateServiceKey } from '@/config/Injector';
import type { FilterStateService } from '@/services/FilterStateService';
import { TagBinding } from '@/vms/TagBinding';
import { GalleryInfo } from '@/vms/GalleryInfo';
import { SearchScope } from '../../../../../Common/source/vms/SearchScope';
import { Folder } from '@/vms/Folder';

interface Props {
  media: Media;
  isMobile?: boolean;
  isOpenOnMobile?: boolean;
}

const props = defineProps<Props>();

const $filter = inject(FilterStateServiceKey)!;

const height = ref('0px');
const isAnimated = ref(false);
const isOpen = ref(false);
const isTransitioning = ref(false);
const info = ref<GalleryInfo | null>(null);

const wrapperRef = ref<HTMLElement>();
const contentRef = ref<HTMLElement>();

const canFilter = computed(() => {
  return !$filter.shareId;
});

const hasExifData = computed(() => {
  const m = props.media;
  return m.cameraMake || m.cameraModel || m.lensModel ||
         m.exposureTime || m.fNumber || m.isoSpeed || m.focalLength ||
         (m.latitude && m.longitude);
});

const cameraInfo = computed(() => {
  const m = props.media;
  if (!m.cameraModel) return m.cameraMake || null;
  // If model already contains make, just use model
  if (m.cameraMake && m.cameraModel.startsWith(m.cameraMake)) {
    return m.cameraModel;
  }
  return [m.cameraMake, m.cameraModel].filter(Boolean).join(' ');
});

const exposureInfo = computed(() => {
  const m = props.media;
  const parts = [m.exposureTime, m.fNumber];
  if (m.isoSpeed) parts.push(`ISO ${m.isoSpeed}`);
  if (m.focalLength) parts.push(m.focalLength);
  const result = parts.filter(Boolean).join(' · ');
  return result || null;
});

const googleMapsUrl = computed(() => {
  const m = props.media;
  if (!m.latitude || !m.longitude) return null;
  return `https://www.google.com/maps?q=${m.latitude},${m.longitude}`;
});

function filterByTag(binding: TagBinding) {
  if (!canFilter.value) {
    return;
  }

  $filter.update(
    'tag',
    {
      folder: '/',
      tags: [binding.tag.id],
      from: null,
      to: null,
      mediaKey: null,
      scope: SearchScope.CurrentFolderAndSubfolders
    }
  );
}

function openFolder(f: Folder) {
  $filter.update('details', {
    folder: f.path,
    mediaKey: '',
    tags: [],
    from: null,
    to: null,
    scope: SearchScope.CurrentFolder
  });
}

watch(isOpen, (value) => {
  if (props.isMobile) {
    return;
  }

  const content = contentRef.value;
  if (!content) {
    return;
  }

  if (value) {
    isAnimated.value = true;
    height.value = content.clientHeight + 'px';
  } else {
    height.value = content.clientHeight + 'px';

    setTimeout(() => {
      isAnimated.value = true;
      height.value = '0px';
    }, 1);
  }
});

watch(isTransitioning, (value) => {
  if (!value && !props.isMobile) {
    isAnimated.value = false;

    if (isOpen.value) {
      height.value = 'auto';
    }
  }
});
</script>

<template>
  <div
    v-if="media"
    class="media-details"
    :class="{
      'media-details_open': isOpen || isOpenOnMobile,
      'media-details_mobile': isMobile,
      'media-details_transitioning': isTransitioning
    }"
    @transitionstart.self="isTransitioning = true"
    @transitionend.self="isTransitioning = false"
  >
    <div class="media-details__header">
      <button
        class="media-details__button"
        @click.prevent="isOpen = !isOpen"
      >
        <div class="media-details__button__caption">
          Details
        </div>
        <div class="media-details__button__arrow">
          <i class="icon icon-arrow"></i>
        </div>
      </button>
    </div>
    <div
      ref="wrapperRef"
      :style="{ height: isMobile ? null : height }"
      @transitionstart.self="isTransitioning = true"
      @transitionend.self="isTransitioning = false"
      :class="{ 'media-details__wrapper_animated': isAnimated }"
    >
      <div ref="contentRef" class="media-details__content">
        <div v-if="media.date">{{ media.date }}</div>
        <div v-if="media.description">{{ media.description }}</div>

        <div class="media-details__exif" v-if="hasExifData">
          <div v-if="cameraInfo" class="media-details__exif__row">
            <span class="fa fa-fw fa-camera"></span>
            <span>{{ cameraInfo }}</span>
          </div>
          <div v-if="media.lensModel" class="media-details__exif__row">
            <span class="fa fa-fw fa-dot-circle-o"></span>
            <span>{{ media.lensModel }}</span>
          </div>
          <div v-if="exposureInfo" class="media-details__exif__row">
            <span class="fa fa-fw fa-sliders"></span>
            <span>{{ exposureInfo }}</span>
          </div>
          <div v-if="googleMapsUrl" class="media-details__exif__row">
            <span class="fa fa-fw fa-map-marker"></span>
            <a :href="googleMapsUrl" target="_blank" rel="noopener" class="media-details__exif__link">
              View on map
            </a>
          </div>
        </div>

        <div class="media-details__tags" v-if="media.extraTags.length">
          <template v-for="t in media.extraTags">
            <a
              class="media-details__tags__item clickable"
              v-if="canFilter"
              :key="t.tag.id"
              @click.prevent="filterByTag(t)"
            >
              {{ t.tag.caption }}
            </a>
            <span v-else class="media-details__tags__item" :key="t.tag.id">
              {{ t.tag.caption }}
            </span>
          </template>
        </div>
        <div class="media-details__folder" v-if="media.folder">
          <span class="fa fa-fw fa-folder"></span>
          <a class="media-details__folder__button" href="#" @click.prevent="openFolder(media.folder)">{{
            media.folder.caption
          }}</a>
        </div>
        <div class="media-details__original">
          <span class="fa fa-fw fa-download"></span>
          <a class="media-details__original__button" :href="media.originalPath" target="_blank">View original</a>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";
@import "bootstrap/scss/mixins";

.media-details {
  left: 0;
  bottom: 0;
  color: $light;

  @include media-breakpoint-down(md) {
    opacity: 0;
    width: 100%;
    position: fixed;
    z-index: $zindex-tooltip;
    visibility: hidden;
    transition: opacity 200ms linear;
  }

  @include media-breakpoint-up(lg) {
    z-index: 2;
    min-width: 30%;
    min-height: 2.5rem;
    max-width: 100%;
    overflow: hidden;
    position: absolute;
  }

  $background: rgba(0, 0, 0, 0.7);

  &_open {
    z-index: $zindex-tooltip;

    @include media-breakpoint-down(md) {
      opacity: 1;
      visibility: visible;
    }

    .media-details__button {
      opacity: 1;
    }

    .media-details__button__arrow {
      transform: rotate(0);
    }
  }

  &_transitioning {
    visibility: visible;
  }

  &_mobile {
    @include media-breakpoint-up(lg) {
      display: none;
    }

    .media-details__header {
      display: none;
    }
  }

  &__button {
    margin: 0;
    border: 0;
    opacity: 0.7;
    color: $light;
    display: flex;
    flex-direction: row;
    padding: 0.5rem 1rem;
    border-radius: $border-radius $border-radius 0 0;
    background-color: $background;
    transition: opacity 200ms linear;

    @include media-breakpoint-down(md) {
      display: none;
    }

    &:hover {
      opacity: 1;
    }

    &:focus {
      outline: none;
    }

    &__caption {
      padding-right: 1rem;
    }

    &__arrow {
      transform: rotate(180deg);
      transition: transform 150ms ease;
    }
  }

  &__wrapper {
    &_animated {
      transition: height 300ms cubic-bezier(0.645, 0.045, 0.355, 1);
    }
  }

  &__content {
    padding: 1rem;
    background-color: $background;

    @supports (padding: max(0px)) {
      padding-left: max(1rem, env(safe-area-inset-left));
      padding-right: max(1rem, env(safe-area-inset-right));
    }

    @include media-breakpoint-up(lg) {
      border-radius: 0 $border-radius 0 0;
    }
  }

  &__tags {
    font-size: 0;

    &__item {
      color: $white;
      font-size: 1rem;
      padding: 0 0.5rem;
      margin: 0.5rem 0.5rem 0 0;
      display: inline-block;
      border-radius: $border-radius;
      background-color: $primary;
      text-decoration: none;

      &:hover {
        color: $white;
        text-decoration: none;
      }

      &:active,
      .no-touch &:hover {
        background-color: darken($primary, 4%);
      }
    }
  }

  &__exif {
    margin-top: 0.5rem;
    padding-top: 0.5rem;
    border-top: 1px solid rgba(255, 255, 255, 0.2);

    &__row {
      display: flex;
      align-items: center;
      margin-top: 0.25rem;
      font-size: 0.9rem;
      opacity: 0.9;

      &:first-child {
        margin-top: 0;
      }

      .fa {
        width: 1.5rem;
        flex-shrink: 0;
      }
    }

    &__link {
      color: $light;
      text-decoration: underline;

      &:hover {
        color: $white;
      }
    }
  }

  &__original,
  &__folder {
    font-size: 0;

    &:not(:first-child) {
      margin-top: 0.5rem;
    }

    .fa {
      font-size: 1rem;
      padding-right: 1rem;
      display: inline-block;
      position: relative;
      bottom: -1px;
    }

    &__button {
      font-size: 1rem;
      color: $light;
      text-decoration: underline;

      &:hover {
        color: $light;
      }

      &:active,
      .no-touch &:hover {
        color: $white;
      }
    }
  }
}
</style>
