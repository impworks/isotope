<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, inject } from 'vue';
import { Media } from '@/vms/Media';
import { HostKey } from '@/config/Injector';
import { BreakpointHelper, Breakpoints } from '@/utils/BreakpointHelper';
import { TagBindingWithLocation } from '@/vms/TagBinding';
import { useDebounceFn } from '@vueuse/core';
import OverlayTag from './OverlayTag.vue';
import MediaDetails from './MediaDetails.vue';

interface IMedia {
  media?: Media;
  img?: HTMLImageElement;
}

interface ICachedMedia extends IMedia {
  isLoading: boolean;
}

interface Props {
  elem: ICachedMedia;
  isFirst?: boolean;
  isLast?: boolean;
  hasOverlay?: boolean;
  isMobileOverlayVisible?: boolean;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  nav: [pos: number];
  close: [];
}>();

const $host = inject(HostKey)!;

const maxHeight = ref(0);
const hover = ref(false);
const isMobile = ref(false);
const tappedTag = ref<TagBindingWithLocation | null>(null);

const cardRef = ref<HTMLElement>();
const wrapperRef = ref<HTMLElement>();

function getAbsolutePath(path: string) {
  return $host + path;
}

function selectTag(tag: TagBindingWithLocation) {
  tappedTag.value = tag;
}

function nav(pos: number) {
  emit('nav', pos);
}

function updateMaxHeight() {
  if (cardRef.value && wrapperRef.value && props.elem && props.elem.media) {
    const imageHeight = props.elem.media.height;
    const windowHeight = window.innerHeight;
    const cardStyles = getComputedStyle(cardRef.value, null);
    const wrapperStyles = getComputedStyle(wrapperRef.value, null);
    const paddingSize = parseFloat(cardStyles.paddingTop) + parseFloat(cardStyles.paddingBottom);
    const marginSize = parseFloat(wrapperStyles.paddingTop) + parseFloat(wrapperStyles.paddingBottom);

    if (windowHeight >= imageHeight + marginSize + paddingSize) {
      maxHeight.value = imageHeight;
    } else {
      maxHeight.value = windowHeight - paddingSize - marginSize;
    }
  }
}

const resizeHandler = useDebounceFn(() => {
  updateMaxHeight();
  isMobile.value = BreakpointHelper.down(Breakpoints.lg);
}, 50);

watch(
  [() => props.elem, () => props.elem?.isLoading],
  () => {
    updateMaxHeight();
  }
);

watch(
  () => props.isMobileOverlayVisible,
  (value) => {
    tappedTag.value = null;
  }
);

onMounted(() => {
  updateMaxHeight();
  isMobile.value = BreakpointHelper.down(Breakpoints.lg);
  window.addEventListener('resize', resizeHandler);
});

onUnmounted(() => {
  window.removeEventListener('resize', resizeHandler);
});
</script>

<template>
  <div class="media-viewer__item">
    <div ref="wrapperRef" class="media-content" v-if="elem">
      <div
        ref="cardRef"
        class="media-content__card"
        @mouseover="hover = true"
        @mouseleave="hover = false"
        :class="{ 'media-content__card_visible': hover }"
      >
        <div class="media-content__wrapper">
          <template v-if="!elem.isLoading">
            <div class="media-content__overlay" v-if="hasOverlay && elem.media">
              <button class="media-content__nav media-content__nav_left clickable" v-if="!isFirst" @click.prevent="nav(-1)">
                <i class="icon icon-slider-arrow"></i>
              </button>
              <button class="media-content__nav media-content__nav_right clickable" v-if="!isLast" @click.prevent="nav(1)">
                <i class="icon icon-slider-arrow"></i>
              </button>
              <overlay-tag
                v-for="t in elem.media.overlayTags"
                :key="t.id"
                :value="t"
                :tappedTag="tappedTag"
                :isMobile="isMobile"
                :isMobileOverlayVisible="isMobileOverlayVisible"
                @tagTapped="selectTag($event)"
              ></overlay-tag>
              <media-details v-if="!isMobile" :media="elem.media"></media-details>
            </div>
            <img
              v-if="elem.media"
              :src="getAbsolutePath(elem.media.fullPath)"
              :alt="elem.media.description"
              :style="{ maxHeight: maxHeight + 'px' }"
            />
          </template>
        </div>
      </div>
      <div class="media-content__close clickable" @click.prevent="$emit('close')"></div>
    </div>
  </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "./node_modules/bootstrap/scss/functions";
@import "./node_modules/bootstrap/scss/variables";
@import "./node_modules/bootstrap/scss/mixins";

.media-viewer__item {
  width: 100%;
  height: 100%;
  min-width: 100%;
  box-sizing: border-box;
  position: relative;
  -webkit-transform: translateZ(0);
  -webkit-backface-visibility: hidden;
}

.media-content {
  display: flex;
  width: 100%;
  height: 100%;
  position: relative;
  align-items: center;
  justify-content: center;
  will-change: contents;

  @include media-breakpoint-down(md) {
    padding: 0.5rem;

    @supports (padding: max(0px)) {
      padding-left: max(0.5rem, env(safe-area-inset-left));
      padding-right: max(0.5rem, env(safe-area-inset-right));
    }
  }

  @include media-breakpoint-up(lg) {
    padding: 1rem;
  }

  @mixin position-absolute() {
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    position: absolute;
  }

  &__card {
    margin: 0 auto;
    max-width: 100%;
    max-height: 100%;
    background: $white;
    position: relative;
    z-index: $zindex-modal;
    border-radius: $border-radius;

    @include media-breakpoint-down(md) {
      padding: 0.5rem;
      box-shadow: $box-shadow;
    }

    @include media-breakpoint-up(lg) {
      padding: 1rem;
      box-shadow: $box-shadow-lg;
    }

    &_visible .media-content__overlay {
      opacity: 1;
    }
  }

  &__wrapper {
    position: relative;
    background-color: $gray-200;
    background-position: center;
    background-repeat: no-repeat;
    background-image: url(../../../images/image.svg);

    @include media-breakpoint-down(md) {
      $min-size: 6rem;

      min-height: $min-size;
      min-width: $min-size;
      background-size: auto 3rem;
    }

    @include media-breakpoint-up(lg) {
      $min-size: 8rem;

      min-height: $min-size;
      min-width: $min-size;
      background-size: auto 4rem;
    }

    img {
      min-height: 1px;
      max-height: 100%;
      max-width: 100%;
      -webkit-user-drag: none;
      -khtml-user-drag: none;
      -moz-user-drag: none;
      -o-user-drag: none;
      user-drag: none;
    }
  }

  &__nav {
    top: 0;
    z-index: 1;
    height: 100%;
    width: 5rem;
    border: 0;
    margin: 0;
    color: $white;
    font-size: 2.5rem;
    line-height: 1;
    padding-top: 0.375rem;
    background: none;
    position: absolute;

    @include media-breakpoint-down(md) {
      display: none;
    }

    &:focus {
      outline: none;
    }

    &_left {
      left: 0;
    }

    &_right {
      right: 0;
      transform: rotate(180deg);
    }

    .icon {
      margin-right: 1rem;
    }

    &:hover:before {
      opacity: 1;
    }

    &:before {
      opacity: 0;
      content: '';
      z-index: -1;
      transition: opacity 200ms ease;
      background: linear-gradient(to left, rgba(0, 0, 0, 0), rgba(0, 0, 0, 0.2));

      @include position-absolute();
    }
  }

  &__overlay {
    @include position-absolute();

    @include media-breakpoint-up(lg) {
      opacity: 0;
      transition: opacity 300ms linear;
    }
  }

  &__close {
    z-index: $zindex-modal-backdrop;

    @include position-absolute();

    @include media-breakpoint-down(md) {
      display: none;
    }
  }
}
</style>
