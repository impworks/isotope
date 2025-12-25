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
const transformStyle = ref('translate(0, 0)');
const transitionClass = ref('transition-initial');
const isTransitioning = ref(false);
const isMobile = ref(false);
const isMobileOverlayVisible = ref(false);
const isClosing = ref(false);

// Touch/drag state
const isDragging = ref(false);
const dragDirection = ref<'horizontal' | 'vertical' | null>(null);
const dragStartX = ref(0);
const dragStartY = ref(0);
const dragCurrentX = ref(0);
const dragCurrentY = ref(0);
const dragStartTime = ref(0);
const viewportWidth = ref(window.innerWidth);

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

// Touch gesture handlers
const SWIPE_THRESHOLD = 0.2; // 20% of viewport width to trigger navigation
const VELOCITY_THRESHOLD = 0.3; // pixels per ms - fast flicks trigger navigation
const MIN_SWIPE_DISTANCE = 30; // minimum pixels for velocity-based swipe
const VERTICAL_THRESHOLD = 100; // pixels to trigger close
const DIRECTION_LOCK_THRESHOLD = 10; // pixels before locking direction

function onTouchStart(e: TouchEvent) {
  if (!isMobile.value || isTransitioning.value) return;

  const touch = e.touches[0];
  dragStartX.value = touch.clientX;
  dragStartY.value = touch.clientY;
  dragCurrentX.value = touch.clientX;
  dragCurrentY.value = touch.clientY;
  dragStartTime.value = Date.now();
  isDragging.value = true;
  dragDirection.value = null;
  transitionClass.value = 'transition-initial';
  viewportWidth.value = window.innerWidth;
}

function onTouchMove(e: TouchEvent) {
  if (!isDragging.value || !isMobile.value) return;

  const touch = e.touches[0];
  dragCurrentX.value = touch.clientX;
  dragCurrentY.value = touch.clientY;

  const deltaX = dragCurrentX.value - dragStartX.value;
  const deltaY = dragCurrentY.value - dragStartY.value;

  // Lock direction after initial movement
  if (dragDirection.value === null) {
    if (Math.abs(deltaX) > DIRECTION_LOCK_THRESHOLD || Math.abs(deltaY) > DIRECTION_LOCK_THRESHOLD) {
      dragDirection.value = Math.abs(deltaX) > Math.abs(deltaY) ? 'horizontal' : 'vertical';
    } else {
      return;
    }
  }

  if (dragDirection.value === 'horizontal') {
    // Apply resistance at edges (no prev/next image)
    let adjustedDeltaX = deltaX;
    if ((deltaX > 0 && !prev.value) || (deltaX < 0 && !next.value)) {
      adjustedDeltaX = deltaX * 0.3; // Resistance factor
    }
    transformStyle.value = `translate(${adjustedDeltaX}px, 0)`;
  } else if (dragDirection.value === 'vertical') {
    // Only allow downward drag for closing
    if (deltaY > 0) {
      transformStyle.value = `translate(0, ${deltaY}px)`;
      isClosing.value = deltaY > VERTICAL_THRESHOLD;
    }
  }
}

function onTouchEnd() {
  if (!isDragging.value || !isMobile.value) return;

  const deltaX = dragCurrentX.value - dragStartX.value;
  const deltaY = dragCurrentY.value - dragStartY.value;
  const elapsed = Date.now() - dragStartTime.value;
  const velocityX = Math.abs(deltaX) / elapsed;

  isDragging.value = false;

  if (dragDirection.value === 'horizontal') {
    const distanceThreshold = viewportWidth.value * SWIPE_THRESHOLD;
    const isFastSwipe = velocityX > VELOCITY_THRESHOLD && Math.abs(deltaX) > MIN_SWIPE_DISTANCE;

    if ((deltaX > distanceThreshold || (isFastSwipe && deltaX > 0)) && prev.value) {
      // Swipe right - go to previous
      animateToImage(1);
    } else if ((deltaX < -distanceThreshold || (isFastSwipe && deltaX < 0)) && next.value) {
      // Swipe left - go to next
      animateToImage(-1);
    } else {
      // Snap back
      snapBack();
    }
  } else if (dragDirection.value === 'vertical') {
    if (deltaY > VERTICAL_THRESHOLD) {
      // Close viewer with animation
      animateClose();
    } else {
      // Snap back
      snapBack();
    }
  } else {
    // No direction locked - treat as potential tap
    snapBack();
  }

  dragDirection.value = null;
}

function animateToImage(dir: number) {
  transitionClass.value = 'transition-item';
  transformStyle.value = `translate(${dir * viewportWidth.value}px, 0)`;
  upcomingIndex.value = Math.min(Math.max((index.value ?? 0) - dir, 0), props.source.length - 1);
}

function animateClose() {
  transitionClass.value = 'transition-item';
  transformStyle.value = `translate(0, ${window.innerHeight}px)`;
  isClosing.value = true;
}

function snapBack() {
  transitionClass.value = 'transition-edge';
  transformStyle.value = 'translate(0, 0)';
  isClosing.value = false;
}

function onTap(e: MouseEvent) {
  // Only handle tap if we weren't dragging
  if (dragDirection.value !== null) return;

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

  if (upcomingIndex.value !== null && upcomingIndex.value !== index.value) {
    show(upcomingIndex.value);
  }
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
          @touchstart.passive="onTouchStart"
          @touchmove="onTouchMove"
          @touchend="onTouchEnd"
          @touchcancel="onTouchEnd"
          :class="[transitionClass, { 'is-dragging': isDragging }]"
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
@import 'bootstrap/scss/functions';
@import 'bootstrap/scss/variables';
@import 'bootstrap/scss/mixins';

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
    touch-action: none;
    will-change: transform;
    transition: transform 0s linear;
    backface-visibility: hidden;
    -webkit-backface-visibility: hidden;

    &.transition-initial {
      transition: transform 0s linear;
    }

    &.transition-item {
      transition: transform 300ms cubic-bezier(0.4, 0.0, 0.2, 1);
    }

    &.transition-edge {
      transition: transform 250ms cubic-bezier(0.25, 0.1, 0.25, 1);
    }

    &.is-dragging {
      cursor: grabbing;
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
