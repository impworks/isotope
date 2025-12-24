<script setup lang="ts">
import { ref } from 'vue';
import { Upload } from 'lucide-vue-next';

interface Props {
  multiple?: boolean;
  disabled?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  multiple: false,
  disabled: false
});

const emit = defineEmits<{
  change: [files: File[]];
}>();

const inputRef = ref<HTMLInputElement | null>(null);

function onClick() {
  inputRef.value?.click();
}

function onDrop(e: DragEvent) {
  if (props.disabled) return;
  const files = e.dataTransfer?.files;
  if (files && files.length) {
    onChange(files);
  }
}

function onChange(files: FileList) {
  const payload = !props.multiple && files.length > 0
    ? [files[0]]
    : Array.from(files);
  emit('change', payload);

  // Reset input so the same file can be selected again
  setTimeout(() => {
    if (inputRef.value) {
      inputRef.value.value = '';
    }
  }, 0);
}

function onInputChange(e: Event) {
  const target = e.target as HTMLInputElement;
  if (target.files) {
    onChange(target.files);
  }
}
</script>

<template>
  <div
    class="border-2 border-dashed border-primary/50 rounded-lg p-8 text-center cursor-pointer hover:border-primary hover:bg-primary/5 transition-colors"
    :class="{ 'opacity-50 cursor-not-allowed': disabled }"
    @click="onClick"
    @drop.prevent="onDrop"
    @dragover.prevent
  >
    <div class="text-lg text-muted-foreground">
      <slot>
        <Upload class="h-5 w-5 mr-2 inline" /> Drag files here or click to select
      </slot>
    </div>
    <input
      ref="inputRef"
      type="file"
      :multiple="multiple"
      :disabled="disabled"
      class="hidden"
      @change="onInputChange"
    />
  </div>
</template>
