<script setup lang="ts">
import { ref, computed, nextTick } from 'vue';
import type { Media } from '@/vms/Media';
import type { Tag } from '@/vms/Tag';
import type { OverlayTagBinding } from '@/vms/OverlayTagBinding';
import type { Rect } from '@common/source/vms/Rect';
import { TagBindingType } from '@common/source/vms/TagBindingType';
import { Button } from '@ui/button';
import TagMultiselect from '@/components/utils/TagMultiselect.vue';
import RectPreview from '@/components/media/RectPreview.vue';
import RectEditor from '@/components/media/RectEditor.vue';
import { Plus, Settings } from 'lucide-vue-next';

interface Props {
  media: Media;
  tags: Tag[];
}

const props = defineProps<Props>();

const selectedTagIds = defineModel<number[]>('selectedTagIds', { required: true });

const emit = defineEmits<{
  change: [];
  configureTags: [];
}>();

// Refs
const wrapperRef = ref<HTMLElement | null>(null);
const rectEditorRefs = ref<InstanceType<typeof RectEditor>[]>([]);

// Draw mode state
const isDrawMode = ref(false);
const drawOrigin = ref<{ x: number; y: number } | null>(null);
const newRect = ref<Rect | null>(null);

// Check if any overlay tag is invalid (missing tagId)
const hasInvalidTags = computed(() => {
  return props.media.overlayTags.some(t => !t.tagId);
});

function toggleDrawMode(state: boolean) {
  isDrawMode.value = state;
  newRect.value = null;
  drawOrigin.value = null;
}

function getMouseOffset(evt: MouseEvent): { x: number; y: number } {
  if (!wrapperRef.value) return { x: 0, y: 0 };
  const rect = wrapperRef.value.getBoundingClientRect();
  return {
    x: evt.clientX - rect.left,
    y: evt.clientY - rect.top
  };
}

function onDrawStart(evt: MouseEvent) {
  if (!isDrawMode.value || newRect.value) return;

  drawOrigin.value = getMouseOffset(evt);
  newRect.value = {
    x: drawOrigin.value.x,
    y: drawOrigin.value.y,
    width: 0,
    height: 0
  };
}

function onDrawMove(evt: MouseEvent) {
  if (!newRect.value || !drawOrigin.value) return;

  const { x, y } = getMouseOffset(evt);
  const o = drawOrigin.value;
  const r = newRect.value;

  // Handle drawing in any direction
  const w = x - o.x;
  if (w >= 0) {
    r.x = o.x;
    r.width = w;
  } else {
    r.x = o.x + w;
    r.width = Math.abs(w);
  }

  const h = y - o.y;
  if (h >= 0) {
    r.y = o.y;
    r.height = h;
  } else {
    r.y = o.y + h;
    r.height = Math.abs(h);
  }
}

function onDrawComplete() {
  if (!newRect.value || !wrapperRef.value) return;

  const elem = wrapperRef.value;
  const w = elem.offsetWidth;
  const h = elem.offsetHeight;
  const r = newRect.value;

  // Only create if the rectangle has some size
  if (r.width < 20 || r.height < 20) {
    newRect.value = null;
    drawOrigin.value = null;
    return;
  }

  // Convert to relative coordinates
  const relRect: Rect = {
    x: r.x / w,
    y: r.y / h,
    width: Math.min(r.width / w, 1 - (r.x / w)),
    height: Math.min(r.height / h, 1 - (r.y / h))
  };

  // Add new overlay tag
  const newBinding: OverlayTagBinding = {
    type: TagBindingType.Depicted,
    location: relRect,
    tagId: null
  };
  props.media.overlayTags.push(newBinding);

  newRect.value = null;
  drawOrigin.value = null;
  isDrawMode.value = false;

  emit('change');

  // Activate the newly created rect editor
  nextTick(() => {
    const lastIdx = props.media.overlayTags.length - 1;
    const lastEditor = rectEditorRefs.value[lastIdx];
    if (lastEditor) {
      lastEditor.activate();
    }
  });
}

function removeOverlayTag(binding: OverlayTagBinding) {
  const idx = props.media.overlayTags.indexOf(binding);
  if (idx !== -1) {
    props.media.overlayTags.splice(idx, 1);
    emit('change');
  }
}

function onRectChange() {
  emit('change');
}

function onExtraTagsChange() {
  emit('change');
}

// Exposed methods for keyboard shortcuts from parent
function startDrawMode() {
  toggleDrawMode(true);
}

function cancelDrawMode() {
  toggleDrawMode(false);
}

defineExpose({
  startDrawMode,
  cancelDrawMode,
  isDrawMode,
  hasInvalidTags
});
</script>

<template>
  <div class="media-tags-editor">
    <!-- Toolbar with tag selector and buttons -->
    <div class="flex items-center gap-2 mb-3">
      <TagMultiselect
        class="flex-1"
        :tags="tags"
        v-model="selectedTagIds"
        :disabled="isDrawMode"
        placeholder="Select custom tags..."
        @update:model-value="onExtraTagsChange"
      />
      <Button
        v-if="isDrawMode"
        variant="outline"
        size="sm"
        @click="toggleDrawMode(false)"
        title="Cancel (Esc)"
      >
        Cancel
      </Button>
      <Button
        v-else
        variant="outline"
        size="sm"
        @click="toggleDrawMode(true)"
        title="Add tag (Ctrl+Space)"
      >
        <Plus class="h-4 w-4 mr-1" />
        Add tag
      </Button>
      <Button
        variant="outline"
        size="icon-sm"
        @click="emit('configureTags')"
        title="Configure tags"
      >
        <Settings class="h-4 w-4" />
      </Button>
    </div>

    <!-- Image with overlay tags -->
    <div class="media-container">
      <div
        ref="wrapperRef"
        class="media-wrapper"
        :class="{ 'draw-mode': isDrawMode }"
        @mousedown.prevent="onDrawStart"
        @mousemove.prevent="onDrawMove"
        @mouseup.prevent="onDrawComplete"
        @mouseleave="onDrawComplete"
      >
        <img :src="media.fullPath" draggable="false" />

        <!-- Preview rect while drawing -->
        <RectPreview v-if="newRect" :rect="newRect" />

        <!-- Existing overlay tags (hidden during draw mode) -->
        <div v-if="!isDrawMode" class="tag-wrapper">
          <RectEditor
            v-for="(binding, idx) in media.overlayTags"
            :key="idx"
            :ref="el => { if (el) rectEditorRefs[idx] = el as InstanceType<typeof RectEditor> }"
            :binding="binding"
            :tags="tags"
            :container-ref="wrapperRef"
            @change="onRectChange"
            @remove="removeOverlayTag(binding)"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.media-tags-editor {
  overflow: visible;
}

.media-container {
  display: flex;
  justify-content: center;
  position: relative;
  z-index: 1;
  overflow: visible;
}

.media-wrapper {
  position: relative;
  display: inline-block;
  user-select: none;
}

.media-wrapper img {
  display: block;
  max-width: 100%;
  max-height: 400px;
}

.media-wrapper.draw-mode {
  cursor: crosshair;
}

.tag-wrapper {
  position: absolute;
  inset: 0;
}
</style>
