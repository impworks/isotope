<script setup lang="ts">
import { ref, watch } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { User } from '@/vms/User';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Label } from '@ui/label';
import { ToggleGroup, ToggleGroupItem } from '@ui/toggle-group';
import { Button } from '@ui/button';

interface Props {
  user?: User | null;
}

const props = defineProps<Props>();
const isOpen = defineModel<boolean>('open', { default: false });
const emit = defineEmits<{
  updated: [value: boolean];
}>();

const api = useApi();
const toast = useToast();
const { asyncState, showSaving } = useAsyncState();

const value = ref<User | null>(null);

const accessLevels = [
  { caption: 'User', isAdmin: false, color: 'bg-blue-500' },
  { caption: 'Admin', isAdmin: true, color: 'bg-red-500' }
];

watch(() => isOpen.value, (newVal) => {
  if (newVal && props.user) {
    value.value = { ...props.user };
  }
});

async function save() {
  if (!value.value) return;

  const success = await showSaving(
    async () => {
      await api.users.update(props.user!.id, value.value!);
      toast.success('User updated');
      emit('updated', true);
      isOpen.value = false;
    },
    'Failed to update user!'
  );
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
          <DialogTitle>Update user {{ user?.userName }}</DialogTitle>
        </DialogHeader>

        <div class="space-y-4 py-4">
          <div class="space-y-3">
            <Label>Access Level</Label>
            <ToggleGroup v-model="value.isAdmin" type="single" :disabled="asyncState.isSaving" class="justify-start">
              <ToggleGroupItem
                v-for="al in accessLevels"
                :key="al.caption"
                :value="al.isAdmin"
                class="data-[state=on]:bg-gray-200 dark:data-[state=on]:bg-gray-700 data-[state=on]:text-foreground data-[state=on]:hover:bg-gray-300 dark:data-[state=on]:hover:bg-gray-600"
              >
                <div class="flex items-center gap-2">
                  <div :class="['w-2 h-2 rounded-full', al.color]"></div>
                  <span>{{ al.caption }}</span>
                </div>
              </ToggleGroupItem>
            </ToggleGroup>
          </div>
        </div>

        <DialogFooter>
          <Button type="button" variant="secondary" @click="cancel">Cancel</Button>
          <Button type="submit" :disabled="asyncState.isSaving">
            <span v-if="asyncState.isSaving">Saving...</span>
            <span v-else>Update</span>
          </Button>
        </DialogFooter>
      </form>
    </DialogContent>
  </Dialog>
</template>
