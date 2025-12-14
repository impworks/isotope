<script setup lang="ts">
import { ref, computed, watch, onMounted, onBeforeUnmount, inject, reactive } from 'vue';
import { useLifetime } from '@/composables/useLifetime';
import { ApiServiceKey, FilterStateServiceKey, HostKey, EventBusServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import type { FilterStateService } from '@/services/FilterStateService';
import type { EventBusService } from '@/services/EventBusService';
import type { MediaThumbnail } from '@/vms/MediaThumbnail';
import type { Media } from '@/vms/Media';
import type { IObservable } from '../../../../../Common/source/utils/Interfaces';
import { BreakpointHelper, Breakpoints } from '@/utils/BreakpointHelper';

import MediaContent from './MediaContent.vue';
import MediaDetails from './MediaDetails.vue';

// Props
interface Props {
  indexFeed: IObservable<number>;
  source: MediaThumbnail[];
}

const props = defineProps<Props>();

// Inject dependencies
const $host = inject(HostKey)!;
const $api = inject(ApiServiceKey)!;
const $filter = inject(FilterStateServiceKey)!;
const $eventBus = inject(EventBusServiceKey)!;

// Use composables
const { observe } = useLifetime();

// Component state
const shown = ref(false);
const index = ref<number | null>(null);
const cache = reactive<ICachedMedia[]>([]);
const upcomingIndex = ref<number | null>(null);
const translateX = ref(0);
const translateY = ref(0);
const maxTranslateX = ref(0);
const maxTranslateY = ref(0);
const transformStyle = ref('translate(0, 0)');
const transitionClass = ref('transition-initial');
const isTransitioning = ref(false);
const isMobile = ref(false);
const isMobileOverlayVisible = ref(false);
const isClosing = ref(false);

// Computed properties
const prev = computed<ICachedMedia | null>(() => {
  return index.value !== null ? cache[index.value - 1] || null : null;
});

const curr = computed<ICachedMedia | null>(() => {
  return index.value !== null ? cache[index.value] || null : null;
});

const next = computed<ICachedMedia | null>(() => {
  return index.value !== null ? cache[index.value + 1] || null : null;
});

// Methods
function hide() {
  if (!shown.value) return;

  shown.value = false;
  index.value = null;
  clearCache();

  $filter.update('viewer', { mediaKey: null });
}

function show(i: number) {
  if (i < 0 || i >= props.source.length) return;

  shown.value = true;
  index.value = i;
  upcomingIndex.value = null;
  $filter.update('viewer', { mediaKey: props.source[i].key });

  updateCache(i);
}

function nav(pos: number) {
  const idx = (index.value ?? 0) + pos;
  if (idx < 0 || idx >= props.source.length) return;

  show(idx);
}

function updateCache(idx: number) {
  updateCacheItem(idx);

  const back = 1;
  const forward = 3;

  for (let i = idx - back; i < idx + forward; i++) {
    if (i >= 0 && i < props.source.length) updateCacheItem(i);
  }
}

async function updateCacheItem(idx: number) {
  if (cache[idx]) return;

  // Create reactive cache item
  cache[idx] = { isLoading: true };

  try {
    const key = props.source[idx].key;
    const { img, media } = await preloadMedia(key);
    // Update cache directly to ensure reactivity
    cache[idx].img = img;
    cache[idx].media = media;
  } catch (e) {
    console.log('Failed to load media!', e);
  } finally {
    cache[idx].isLoading = false;
  }
}

async function preloadMedia(key: string): Promise<IMedia> {
  const media = await $api.getMedia(key);
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.className = 'preload-image';
    img.onload = () => resolve({ media: media, img: img });
    img.onerror = (e) => reject(e);
    img.src = $host + media.fullPath;
    document.body.appendChild(img);
  });
}

async function clearCache() {
  for (let elem of cache) if (elem && elem.img) elem.img.parentElement?.removeChild(elem.img);

  cache.length = 0;
}

function orientationHandler() {
  document.documentElement.style.height = `initial`;
  setTimeout(() => {
    document.documentElement.style.height = `100%`;
    setTimeout(() => {
      window.scrollTo(0, 1);
      $eventBus.uiUpdated.notify();
    }, 200);
  }, 200);
}

let resizeTimeout: number | undefined;
function resizeHandler() {
  if (resizeTimeout) clearTimeout(resizeTimeout);
  resizeTimeout = window.setTimeout(() => {
    isMobile.value = BreakpointHelper.down(Breakpoints.lg);
    $eventBus.uiUpdated.notify();
  }, 50);
}

// TODO: Implement touch gestures
// Original used v-hammer:swipe, v-hammer:pan, v-hammer:tap
// Need to implement:
// - Swipe left/right for navigation between images
// - Pan for dragging images with smooth animation
// - Pinch to zoom (if needed)
// - Tap to toggle mobile overlay visibility
// - Vertical pan to close viewer (swipe down)
// Consider using native Pointer Events or a Vue3-compatible gesture library

function handleHorizontalTouchEvents(e: any) {
  // TODO: Implement horizontal touch/swipe gestures
  // This should handle:
  // - Horizontal panning to navigate between images
  // - Edge cases when at first/last image
  // - Smooth transition animations
  console.warn('Touch gestures not yet implemented');
}

function handleVerticalTouchEvents(e: any) {
  // TODO: Implement vertical touch/swipe gestures
  // This should handle:
  // - Vertical panning to dismiss the viewer
  // - Background opacity change during drag
  // - Snap back if not dragged far enough
  console.warn('Touch gestures not yet implemented');
}

function handleHorizontalGestureMove(deltaX: number) {
  // TODO: Implement horizontal gesture movement
  // Calculate transform based on deltaX
  // Handle edge resistance when no prev/next image
}

function handleHorizontalGestureEnd(deltaX: number) {
  // TODO: Implement horizontal gesture end
  // Decide whether to navigate or snap back
}

function handleVerticalGestureMove(deltaY: number) {
  // TODO: Implement vertical gesture movement
  // Update transform and closing state
}

function handleVerticalGestureEnd(deltaY: number) {
  // TODO: Implement vertical gesture end
  // Close viewer if dragged far enough
}

function swipe(dir: number) {
  // TODO: Implement swipe animation
  // Animate to next/prev image
  if (isTransitioning.value || !cache[index.value! - dir]) {
    return;
  }

  transitionClass.value = 'transition-item';
  transformStyle.value = `translate(${dir * 100}vw, 0)`;
  upcomingIndex.value = Math.min(Math.max(index.value! - dir, 0), props.source.length - 1);
}

function onTap(e: MouseEvent) {
  // TODO: Replace with proper gesture detection
  // For now, simple click to toggle overlay
  const target = e.target as HTMLElement;
  if (target.classList[0] !== 'overlay-tag' && target.tagName !== 'A' && isMobile.value) {
    isMobileOverlayVisible.value = !isMobileOverlayVisible.value;
  }
}

function updateCurrentItem() {
  if (isClosing.value) {
    hide();
    isClosing.value = false;
  }

  isTransitioning.value = false;
  transitionClass.value = 'transition-initial';
  transformStyle.value = 'translate(0, 0)';
  translateX.value = 0;
  maxTranslateX.value = 0;
  translateY.value = 0;
  maxTranslateY.value = 0;

  if (upcomingIndex.value !== null && upcomingIndex.value !== index.value) {
    show(upcomingIndex.value);
  }
}

function handleTouchEvents(e: any) {
  // TODO: Implement unified touch event handler
  // Determine direction and delegate to horizontal/vertical handlers
}

// Keyboard navigation handlers
function handleKeyDown(e: KeyboardEvent) {
  if (!shown.value) return;

  switch (e.key) {
    case 'ArrowLeft':
      nav(-1);
      break;
    case 'ArrowRight':
      nav(1);
      break;
    case 'Escape':
      hide();
      break;
  }
}

// Watchers
watch(shown, (value) => {
  isMobile.value = BreakpointHelper.down(Breakpoints.lg);
  document.body.classList.toggle('media-viewer-open', value);
});

// Lifecycle hooks
onMounted(() => {
  observe(props.indexFeed, (x) => show(x));
  observe($filter.onStateChanged, (x) => x.source === 'viewer' || hide());

  isMobile.value = BreakpointHelper.down(Breakpoints.lg);
  window.addEventListener('resize', resizeHandler);
  window.addEventListener('orientationchange', orientationHandler);
  window.addEventListener('keydown', handleKeyDown);
});

onBeforeUnmount(() => {
  window.removeEventListener('resize', resizeHandler);
  window.removeEventListener('orientationchange', orientationHandler);
  window.removeEventListener('keydown', handleKeyDown);
  if (resizeTimeout) clearTimeout(resizeTimeout);
});

// Interfaces
interface IMedia {
  media?: Media;
  img?: HTMLImageElement;
}

interface ICachedMedia extends IMedia {
  isLoading: boolean;
}
</script>

<template>
  <Teleport to="body">
    <transition name="media-viewer__fade">
      <div
        class="media-viewer"
        v-if="shown"
        :class="{ 'media-viewer_closing': isClosing }"
      >
        <button
          class="btn-header media-viewer__close-btn"
          v-if="isMobileOverlayVisible"
          @click.prevent="hide()"
        >
          <div class="btn-header__content">
            <i class="icon icon-cross"></i>
          </div>
        </button>
        <div
          class="media-viewer__content"
          @click="onTap"
          :class="transitionClass"
          :style="{ transform: transformStyle }"
          @transitionstart.self="isTransitioning = true"
          @transitionend.self="updateCurrentItem"
        >
          <media-content
            :elem="prev"
            :key="'p' + index"
            :isMobileOverlayVisible="isMobileOverlayVisible"
          >
          </media-content>
          <media-content
            :elem="curr"
            :hasOverlay="true"
            :isFirst="!prev"
            :isLast="!next"
            :isMobileOverlayVisible="isMobileOverlayVisible"
            @nav="nav($event)"
            @close="hide()"
          >
          </media-content>
          <media-content
            :elem="next"
            :key="'n' + index"
            :isMobileOverlayVisible="isMobileOverlayVisible"
          >
          </media-content>
        </div>
        <media-details
          v-if="isMobile && curr && !curr.isLoading"
          :isOpenOnMobile="isMobileOverlayVisible"
          :isMobile="true"
          :media="curr.media"
        >
        </media-details>
      </div>
    </transition>
  </Teleport>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import './node_modules/bootstrap/scss/functions';
@import './node_modules/bootstrap/scss/variables';
@import './node_modules/bootstrap/scss/mixins';

.media-viewer {
  z-index: $zindex-modal;
  background-color: rgba($dark, 0.5);
  top: 0;
  left: 0;
  height: 100%;
  width: 100%;
  position: fixed;
  overflow: hidden;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  transition: background-color 200ms linear;

  &_closing {
    background-color: rgba($dark, 0);
  }

  &__content {
    display: flex;
    justify-content: center;
    height: 100%;
    width: 100%;
    box-sizing: border-box;
    touch-action: pan-y;
    will-change: transform;
    transition: transform 0s linear;
    backface-visibility: hidden;
    -webkit-backface-visibility: hidden;

    &.transition-initial {
      transition: transform 0s linear;
    }

    &.transition-item {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }

    &.transition-edge {
      transition: transform 500ms ease-out;
    }
  }

  &__fade {
    &-enter-active,
    &-leave-active {
      transition: opacity 200ms linear;
    }

    &-enter-from,
    &-leave-to {
      opacity: 0;
    }
  }

  &__close-btn.btn-header {
    top: 0;
    right: 0;
    z-index: 999;
    position: absolute;
    padding-right: 1rem;

    @supports (padding: max(0px)) {
      padding-right: max(1rem, env(safe-area-inset-right));
    }
  }
}

.media-viewer-open {
  touch-action: none;
  overflow: hidden;
}
</style>
