<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import type { MediaThumbnail } from '@/vms/MediaThumbnail';
import type { MassMediaUpdate } from '@/vms/MassMediaAction';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Label } from '@ui/label';
import { Input } from '@ui/input';
import { Button } from '@ui/button';
import { Checkbox } from '@ui/checkbox';
import Loading from '@/components/utils/Loading.vue';
import { HelpCircle } from 'lucide-vue-next';

interface Props {
  media: MediaThumbnail[];
}

const props = defineProps<Props>();
const isOpen = defineModel<boolean>('open', { default: false });
const emit = defineEmits<{
  saved: [value: boolean];
}>();

const api = useApi();
const { asyncState, showSaving } = useAsyncState();

const dateEnabled = ref(true);
const dateValue = ref<string>('');
const descriptionEnabled = ref(true);
const descriptionValue = ref<string>('');

watch(() => isOpen.value, (newVal) => {
  if (newVal) {
    // Reset form
    dateEnabled.value = true;
    dateValue.value = '';
    descriptionEnabled.value = true;
    descriptionValue.value = '';
  }
});

const canSave = computed(() => {
  return !asyncState.isSaving && (dateEnabled.value || descriptionEnabled.value);
});

async function save() {
  if (!canSave.value) return;

  await showSaving(
    async () => {
      const request: MassMediaUpdate = {
        keys: props.media.map(x => x.key),
        date: {
          isSet: dateEnabled.value,
          value: dateEnabled.value && dateValue.value ? new Date(dateValue.value).toISOString() : null
        },
        description: {
          isSet: descriptionEnabled.value,
          value: descriptionEnabled.value ? (descriptionValue.value || null) : null
        }
      };

      await api.media.massUpdate(request);
      emit('saved', true);
      isOpen.value = false;
    },
    'Failed to update media'
  );
}

function cancel() {
  emit('saved', false);
  isOpen.value = false;
}
</script>

<template>
  <Dialog v-model:open="isOpen">
    <DialogContent class="sm:max-w-[500px]">
      <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <form @submit.prevent="save">
          <DialogHeader>
            <DialogTitle>Edit {{ media.length }} media file(s)</DialogTitle>
          </DialogHeader>

          <div class="space-y-4 py-4">
            <div class="space-y-2">
              <div class="flex items-center gap-2">
                <Checkbox id="date-enabled" v-model:checked="dateEnabled" />
                <Label for="date-enabled" class="cursor-pointer">Date</Label>
              </div>
              <Input
                type="date"
                v-model="dateValue"
                :disabled="!dateEnabled"
                class="w-48"
              />
            </div>

            <div class="space-y-2">
              <div class="flex items-center gap-2">
                <Checkbox id="description-enabled" v-model:checked="descriptionEnabled" />
                <Label for="description-enabled" class="cursor-pointer">Description</Label>
              </div>
              <textarea
                v-model="descriptionValue"
                :disabled="!descriptionEnabled"
                rows="5"
                class="flex w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              ></textarea>
            </div>

            <p class="text-sm text-muted-foreground flex items-center">
              <HelpCircle class="h-4 w-4 mr-1" />
              Uncheck the fields which you do not want to change.
            </p>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" @click="cancel">Cancel</Button>
            <Button type="submit" :disabled="!canSave">
              <span v-if="asyncState.isSaving">Saving...</span>
              <span v-else>Save</span>
            </Button>
          </DialogFooter>
        </form>
      </Loading>
    </DialogContent>
  </Dialog>
</template>
