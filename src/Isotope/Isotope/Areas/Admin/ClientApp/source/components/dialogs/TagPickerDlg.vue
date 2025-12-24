<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import type { Tag } from '@/vms/Tag';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogDescription
} from '@ui/dialog';
import { Button } from '@ui/button';
import TagMultiselect from '@/components/utils/TagMultiselect.vue';

interface Props {
  allTags: Tag[];
  pickedTagIds: number[];
}

const props = defineProps<Props>();

const emit = defineEmits<{
  save: [tagIds: number[]];
  cancel: [];
}>();

const isOpen = defineModel<boolean>('open', { required: true });

// Local state for selected tag IDs
const selectedIds = ref<number[]>([]);

// Reset selection when dialog opens
watch(isOpen, (open) => {
  if (open) {
    selectedIds.value = props.pickedTagIds?.slice() ?? [];
  }
});

function onSave() {
  emit('save', selectedIds.value);
  isOpen.value = false;
}

function onCancel() {
  emit('cancel');
  isOpen.value = false;
}

function onClear() {
  selectedIds.value = [];
}
</script>

<template>
  <Dialog v-model:open="isOpen">
    <DialogContent class="sm:max-w-[500px]">
      <DialogHeader>
        <DialogTitle>Tag picker</DialogTitle>
        <DialogDescription>
          You can temporarily select a subset of relevant tags to speed up tagging multiple photos.
          Other tags will be hidden until you reopen the media editor.
        </DialogDescription>
      </DialogHeader>

      <div class="py-4">
        <TagMultiselect
          :tags="allTags"
          v-model="selectedIds"
          placeholder="Select tags to show..."
        />
      </div>

      <DialogFooter class="gap-2">
        <Button variant="outline" @click="onClear" class="mr-auto">
          Clear
        </Button>
        <Button variant="outline" @click="onCancel">
          Cancel
        </Button>
        <Button @click="onSave">
          Apply
        </Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
