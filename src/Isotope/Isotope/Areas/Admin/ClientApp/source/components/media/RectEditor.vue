<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted, nextTick } from 'vue';
import VueDraggableResizable from 'vue-draggable-resizable';
import 'vue-draggable-resizable/style.css';
import type { OverlayTagBinding } from '@/vms/OverlayTagBinding';
import type { Tag } from '@/vms/Tag';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@ui/select';
import { Button } from '@ui/button';
import { Trash2 } from 'lucide-vue-next';

interface Props {
  binding: OverlayTagBinding;
  tags: Tag[];
  containerRef: HTMLElement | null;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  change: [];
  remove: [];
}>();

// Pixel coordinates for the draggable
const originX = ref(0);
const originY = ref(0);
const sizeW = ref(100);
const sizeH = ref(100);

// Track if we have valid dimensions
const isInitialized = ref(false);

// Use v-model:active for two-way binding with vue-draggable-resizable
const isActive = ref(false);
const selectOpen = ref(false);

const isInvalid = computed(() => !props.binding.tagId);

const popoverStyle = computed(() => ({
  left: Math.round((sizeW.value - 280) / 2) + 'px'
}));

// When binding location changes externally, update pixel coords
watch(() => props.binding.location, () => {
  updateFromRelative();
}, { deep: true });

// Watch containerRef - it may not be ready at mount time or may resize when image loads
watch(() => props.containerRef, (newRef, oldRef) => {
  // Clean up observer on old ref
  if (resizeObserver && oldRef) {
    resizeObserver.unobserve(oldRef);
  }

  if (newRef) {
    updateFromRelative();
    // Observe for dimension changes (e.g., when image loads)
    if (resizeObserver) {
      resizeObserver.observe(newRef);
    }
  }
});

// ResizeObserver to handle container dimension changes (e.g., image load)
let resizeObserver: ResizeObserver | null = null;

// Handle Delete key to remove tag when active (but not when typing in dropdown)
function onKeyDown(e: KeyboardEvent) {
  if (e.key === 'Delete' && isActive.value && !selectOpen.value) {
    e.preventDefault();
    onRemove();
  }
}

onMounted(() => {
  // Initialize from relative coordinates
  updateFromRelative();

  // Set up ResizeObserver to recalculate when container dimensions change
  resizeObserver = new ResizeObserver(() => {
    updateFromRelative();
  });

  if (props.containerRef) {
    resizeObserver.observe(props.containerRef);
  }

  // Listen for Delete key
  document.addEventListener('keydown', onKeyDown);
});

onUnmounted(() => {
  if (resizeObserver) {
    resizeObserver.disconnect();
    resizeObserver = null;
  }
  document.removeEventListener('keydown', onKeyDown);
});

function updateFromRelative() {
  if (!props.containerRef) return;

  const w = props.containerRef.offsetWidth;
  const h = props.containerRef.offsetHeight;

  // Skip if container has no valid dimensions yet (image not loaded)
  if (w === 0 || h === 0) return;

  const r = props.binding.location;

  originX.value = Math.round(w * r.x);
  originY.value = Math.round(h * r.y);
  sizeW.value = Math.round(w * r.width);
  sizeH.value = Math.round(h * r.height);

  isInitialized.value = true;
}

function updateRelative() {
  if (!props.containerRef) return;

  const w = props.containerRef.offsetWidth;
  const h = props.containerRef.offsetHeight;

  props.binding.location.x = originX.value / w;
  props.binding.location.y = originY.value / h;
  props.binding.location.width = sizeW.value / w;
  props.binding.location.height = sizeH.value / h;

  emit('change');
}

function onDragging(left: number, top: number) {
  originX.value = left;
  originY.value = top;
  updateRelative();
}

function onResizing(left: number, top: number, width: number, height: number) {
  originX.value = left;
  originY.value = top;
  sizeW.value = width;
  sizeH.value = height;
  updateRelative();
}

// Watch active state to auto-open dropdown when no tag selected
watch(isActive, (value) => {
  if (!value) {
    selectOpen.value = false;
    return;
  }

  // Auto-open dropdown if no tag selected
  if (!props.binding.tagId) {
    nextTick(() => {
      selectOpen.value = true;
    });
  }
});

function onTagChange(value: string) {
  props.binding.tagId = value ? parseInt(value, 10) : null;
  selectOpen.value = false;
  emit('change');
}

function onRemove() {
  emit('remove');
}

// Expose activate method for external triggering
function activate() {
  isActive.value = true;
}

defineExpose({ activate });
</script>

<template>
  <VueDraggableResizable
    v-if="isInitialized"
    v-model:active="isActive"
    :x="originX"
    :y="originY"
    :w="sizeW"
    :h="sizeH"
    :min-width="32"
    :min-height="32"
    :parent="true"
    class="rect-editor"
    :class="{ 'rect-invalid': isInvalid }"
    :title="isInvalid ? 'Please select a tag' : undefined"
    @dragging="onDragging"
    @resizing="onResizing"
  >
    <div
      v-if="isActive"
      class="tag-popover"
      :style="popoverStyle"
      @mousedown.stop
      @touchstart.stop
    >
      <div class="tag-popover-content">
        <Select
          :model-value="binding.tagId?.toString() ?? ''"
          :open="selectOpen"
          @update:model-value="onTagChange"
          @update:open="selectOpen = $event"
        >
          <SelectTrigger class="w-[200px] h-8">
            <SelectValue placeholder="Select tag..." />
          </SelectTrigger>
          <SelectContent :portal-disabled="true">
            <SelectItem
              v-for="tag in tags"
              :key="tag.id"
              :value="tag.id.toString()"
            >
              {{ tag.caption }}
            </SelectItem>
          </SelectContent>
        </Select>
        <Button
          variant="outline"
          size="icon-sm"
          class="ml-2 text-destructive hover:text-destructive"
          @click="onRemove"
          title="Remove tag"
        >
          <Trash2 class="h-4 w-4" />
        </Button>
      </div>
    </div>
  </VueDraggableResizable>
</template>

<style>
.rect-editor.vdr {
  border: 2px solid hsl(var(--primary));
  background: hsl(var(--primary) / 0.2);
  z-index: 1;
}

.rect-editor.vdr.active {
  z-index: 10;
}

.rect-editor.vdr.rect-invalid {
  border-color: hsl(var(--destructive) / 0.5);
  background: hsl(var(--destructive) / 0.1);
}

.rect-editor.vdr.rect-invalid.active {
  border-color: hsl(var(--destructive));
}

.rect-editor .vdr-stick {
  background: hsl(var(--primary));
  border: 1px solid hsl(var(--background));
}

.rect-editor.rect-invalid .vdr-stick {
  background: hsl(var(--destructive));
}

.tag-popover {
  position: absolute;
  bottom: -68px;
  z-index: 1000;
}

.tag-popover-content {
  position: relative;
  z-index: 1000;
  display: flex;
  align-items: center;
  padding: 8px;
  background: hsl(var(--background));
  border: 1px solid hsl(var(--border));
  border-radius: 6px;
  box-shadow: 0 4px 6px -1px rgb(0 0 0 / 0.1);
}

/* Ensure dropdown content appears above dialog footer */
.tag-popover :deep([data-radix-popper-content-wrapper]) {
  z-index: 1001 !important;
}
</style>
