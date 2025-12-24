<script setup lang="ts">
import { ref, nextTick, watch } from 'vue';
import VueDraggableResizable from 'vue-draggable-resizable';
import 'vue-draggable-resizable/style.css';
import type { Media } from '@/vms/Media';
import type { Rect } from '@common/source/vms/Rect';

interface Props {
  media: Media;
}

const props = defineProps<Props>();

const rect = defineModel<Rect>('rect', { required: true });

const emit = defineEmits<{
  change: [];
}>();

const mediaWrapperRef = ref<HTMLDivElement | null>(null);
const imageLoaded = ref(false);

// Drag resize state (pixel values)
const dragX = ref(0);
const dragY = ref(0);
const dragW = ref(100);
const dragH = ref(100);

// Re-initialize when media changes
watch(() => props.media, () => {
  imageLoaded.value = false;
});

function onImageLoad() {
  imageLoaded.value = true;
  nextTick(() => {
    initDragResize();
  });
}

function initDragResize() {
  if (!mediaWrapperRef.value || !rect.value) return;

  const w = mediaWrapperRef.value.offsetWidth;
  const h = mediaWrapperRef.value.offsetHeight;
  const r = rect.value;

  dragX.value = Math.round(w * r.x);
  dragY.value = Math.round(h * r.y);
  dragW.value = Math.round(w * r.width);
  dragH.value = Math.round(h * r.height);
}

function onDragging(left: number, top: number) {
  if (!mediaWrapperRef.value || !rect.value) return;

  const w = mediaWrapperRef.value.offsetWidth;
  const h = mediaWrapperRef.value.offsetHeight;

  rect.value = {
    ...rect.value,
    x: left / w,
    y: top / h
  };

  emit('change');
}

function onResizing(left: number, top: number, width: number, height: number) {
  if (!mediaWrapperRef.value || !rect.value) return;

  const w = mediaWrapperRef.value.offsetWidth;
  const h = mediaWrapperRef.value.offsetHeight;

  rect.value = {
    x: left / w,
    y: top / h,
    width: width / w,
    height: height / h
  };

  emit('change');
}

defineExpose({
  resetImageLoaded: () => { imageLoaded.value = false; }
});
</script>

<template>
  <div class="flex justify-center">
    <div
      ref="mediaWrapperRef"
      class="media-wrapper relative"
    >
      <img
        :src="media.fullPath"
        class="block max-w-full max-h-[600px]"
        draggable="false"
        @load="onImageLoad"
      />
      <div v-if="rect && imageLoaded" class="absolute inset-0">
        <VueDraggableResizable
          :x="dragX"
          :y="dragY"
          :w="dragW"
          :h="dragH"
          :min-width="50"
          :min-height="50"
          :parent="true"
          :lock-aspect-ratio="true"
          @dragging="onDragging"
          @resizing="onResizing"
        />
      </div>
    </div>
  </div>
</template>

<style>
.media-wrapper {
  display: inline-block;
}

.media-wrapper img {
  display: block;
}

/* vue-draggable-resizable styling */
.vdr {
  border: 2px solid hsl(var(--primary));
  background: hsl(var(--primary) / 0.2);
}

.vdr-stick {
  background: hsl(var(--primary));
  border: 1px solid hsl(var(--background));
}
</style>
