<script setup lang="ts">
interface Props {
  isLoading: boolean;
  isFullPage?: boolean;
  text?: string;
}

defineProps<Props>();
</script>

<template>
  <div v-if="isLoading && isFullPage" class="flex items-center justify-center flex-grow overflow-hidden h-full">
    <div class="spinner w-12 h-12 border-4 border-primary border-t-transparent rounded-full"></div>
  </div>
  <div v-else-if="isLoading && !isFullPage" class="inline-flex items-center gap-2">
    <div class="spinner w-4 h-4 border-2 border-primary border-t-transparent rounded-full"></div>
    <span v-if="text">{{ text }}</span>
  </div>
  <slot v-else></slot>
</template>
