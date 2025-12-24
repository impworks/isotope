<script setup lang="ts">
import { computed } from 'vue';
import type { Rect } from '@common/source/vms/Rect';

interface Props {
  rect: Rect;
}

const props = defineProps<Props>();

const style = computed(() => ({
  left: props.rect.x + 'px',
  top: props.rect.y + 'px',
  width: props.rect.width + 'px',
  height: props.rect.height + 'px'
}));
</script>

<template>
  <div class="rect-preview" :style="style"></div>
</template>

<style scoped>
.rect-preview {
  position: absolute;
  border: 2px solid rgba(255, 255, 255, 0.5);
  background: rgba(255, 255, 255, 0.1);
  pointer-events: none;
}
</style>
