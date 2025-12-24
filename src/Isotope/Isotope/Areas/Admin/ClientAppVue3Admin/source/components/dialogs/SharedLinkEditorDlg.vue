<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { SharedLinkDetails } from '@/vms/SharedLinkDetails';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Input } from '@ui/input';
import { Label } from '@ui/label';
import { Button } from '@ui/button';
import { Check, Copy } from 'lucide-vue-next';

interface Props {
  link?: SharedLinkDetails | null;
}

const props = defineProps<Props>();
const isOpen = defineModel<boolean>('open', { default: false });
const emit = defineEmits<{
  updated: [value: boolean];
}>();

const api = useApi();
const toast = useToast();
const { asyncState, showSaving } = useAsyncState();

const value = ref<SharedLinkDetails | null>(null);
const copied = ref(false);

watch(() => isOpen.value, (newVal) => {
  if (newVal && props.link) {
    value.value = { ...props.link };
  }
});

const url = computed(() => {
  if (!value.value) return '';
  return window.location.origin + '/?sh=' + value.value.key;
});

const canSave = computed(() => {
  return !asyncState.isSaving && !!value.value?.caption;
});

async function save() {
  if (!canSave.value || !value.value) return;

  const success = await showSaving(
    async () => {
      await api.sharedLinks.update(value.value!.key, value.value!);
      emit('updated', true);
      isOpen.value = false;
    },
    'Failed to update link'
  );
}

async function copy() {
  try {
    await navigator.clipboard.writeText(url.value);
    copied.value = true;
    toast.success('Link copied to clipboard');
    setTimeout(() => copied.value = false, 1500);
  } catch {
    toast.error('Failed to copy link');
  }
}

function cancel() {
  emit('updated', false);
  isOpen.value = false;
}
</script>

<template>
  <Dialog v-model:open="isOpen">
    <DialogContent v-if="value" class="sm:max-w-[500px]">
      <form @submit.prevent="save">
        <DialogHeader>
          <DialogTitle>Update shared link</DialogTitle>
        </DialogHeader>

        <div class="space-y-4 py-4">
          <div class="space-y-2">
            <Label for="caption">Caption</Label>
            <Input
              id="caption"
              v-model="value.caption"
              v-autofocus
              :disabled="asyncState.isSaving"
            />
          </div>

          <div class="space-y-2">
            <Label for="link">Link</Label>
            <div class="flex gap-2">
              <Input
                id="link"
                :model-value="url"
                readonly
                class="flex-1 bg-muted"
              />
              <Button type="button" variant="outline" @click="copy" title="Copy link to clipboard">
                <Check v-if="copied" class="h-4 w-4" />
                <Copy v-else class="h-4 w-4" />
              </Button>
            </div>
          </div>
        </div>

        <DialogFooter>
          <Button type="button" variant="secondary" @click="cancel">Cancel</Button>
          <Button type="submit" :disabled="!canSave">
            <span v-if="asyncState.isSaving">Saving...</span>
            <span v-else>Update</span>
          </Button>
        </DialogFooter>
      </form>
    </DialogContent>
  </Dialog>
</template>
