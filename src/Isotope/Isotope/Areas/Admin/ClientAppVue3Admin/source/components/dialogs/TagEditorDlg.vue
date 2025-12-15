<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { Tag } from '@/vms/Tag';
import { TagType } from '@common/source/vms/TagType';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Input } from '@ui/input';
import { Label } from '@ui/label';
import { ToggleGroup, ToggleGroupItem } from '@ui/toggle-group';
import { Checkbox } from '@ui/checkbox';
import { Button } from '@ui/button';

interface Props {
  tag?: Tag | null;
}

const props = defineProps<Props>();
const isOpen = defineModel<boolean>('open', { default: false });
const emit = defineEmits<{
  saved: [value: boolean];
}>();

const api = useApi();
const toast = useToast();
const { asyncState, showSaving } = useAsyncState();

const value = ref<Tag | null>(null);
const createMore = ref(false);
const result = ref(false);
const captionInput = ref<HTMLInputElement | null>(null);

const tagTypes = [
  { type: TagType.Person, caption: 'Person' },
  { type: TagType.Location, caption: 'Location' },
  { type: TagType.Pet, caption: 'Pet' },
  { type: TagType.Custom, caption: 'Other' }
];

// Watch for dialog open state and tag changes
watch(() => isOpen.value, (newVal) => {
  if (newVal) {
    value.value = props.tag
      ? { ...props.tag }
      : { id: 0, caption: '', type: TagType.Person, count: 0 };

    nextTick(() => {
      captionInput.value?.focus();
    });
  }
});

const isNew = computed(() => !value.value?.id);

const canSave = computed(() => {
  return !asyncState.isSaving && !!value.value?.caption;
});

async function save() {
  if (!canSave.value || !value.value) return;

  const success = await showSaving(
    async () => {
      if (isNew.value) {
        await api.tags.create(value.value!);
        toast.success('Tag created');

        if (createMore.value) {
          const currentType = value.value!.type;
          value.value = { id: 0, caption: '', type: currentType, count: 0 };
          result.value = true;
          nextTick(() => captionInput.value?.focus());
          return;
        }
      } else {
        await api.tags.update(value.value!.id, value.value!);
        toast.success('Tag updated');
      }

      emit('saved', true);
      isOpen.value = false;
    },
    isNew.value ? 'Failed to create tag' : 'Failed to update tag'
  );
}

function cancel() {
  emit('saved', result.value);
  isOpen.value = false;
}

function handleKeydown(e: KeyboardEvent) {
  if (e.ctrlKey && e.key === 's') {
    e.preventDefault();
    save();
  }
}
</script>

<template>
  <Dialog v-model:open="isOpen" @keydown="handleKeydown">
    <DialogContent v-if="value" class="sm:max-w-[500px]">
      <form @submit.prevent="save">
        <DialogHeader>
          <DialogTitle>
            <span v-if="isNew">Create a new tag</span>
            <span v-else>Update tag '{{ tag?.caption }}'</span>
          </DialogTitle>
        </DialogHeader>

        <div class="space-y-4 py-4">
          <div class="space-y-2">
            <Label for="caption">Caption</Label>
            <Input
              id="caption"
              ref="captionInput"
              v-model="value.caption"
              :disabled="asyncState.isSaving"
            />
          </div>

          <div class="space-y-3">
            <Label>Type</Label>
            <ToggleGroup v-model="value.type" type="single" :disabled="asyncState.isSaving" class="justify-start">
              <ToggleGroupItem
                v-for="t in tagTypes"
                :key="t.type"
                :value="t.type"
                class="data-[state=on]:bg-primary data-[state=on]:text-primary-foreground"
              >
                {{ t.caption }}
              </ToggleGroupItem>
            </ToggleGroup>
          </div>
        </div>

        <DialogFooter class="flex items-center sm:justify-between">
          <label v-if="isNew" class="flex items-center gap-2 text-sm cursor-pointer" title="Keep the dialog open to create another tag after saving">
            <Checkbox v-model:checked="createMore" />
            <span>Create one more</span>
          </label>
          <div class="flex gap-2 sm:ml-auto">
            <Button type="button" variant="secondary" @click="cancel">Cancel</Button>
            <Button type="submit" :disabled="!canSave" title="Ctrl + S">
              <span v-if="asyncState.isSaving">Saving...</span>
              <span v-else-if="isNew">Create</span>
              <span v-else>Update</span>
            </Button>
          </div>
        </DialogFooter>
      </form>
    </DialogContent>
  </Dialog>
</template>
