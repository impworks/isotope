<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { User } from '@/vms/User';
import type { UserPassword } from '@/vms/UserPassword';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Input } from '@ui/input';
import { Label } from '@ui/label';
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

const value = ref<UserPassword>({
  password: '',
  passwordConfirmation: ''
});

watch(() => isOpen.value, (newVal) => {
  if (newVal) {
    value.value = {
      password: '',
      passwordConfirmation: ''
    };
  }
});

const canSave = computed(() => {
  return !asyncState.isSaving
    && !!value.value.password
    && value.value.password === value.value.passwordConfirmation;
});

const passwordsMatch = computed(() => {
  if (!value.value.passwordConfirmation) return true;
  return value.value.password === value.value.passwordConfirmation;
});

async function save() {
  if (!canSave.value || !props.user) return;

  const success = await showSaving(
    async () => {
      await api.users.updatePassword(props.user!.id, value.value);
      toast.success('User password updated');
      emit('updated', true);
      isOpen.value = false;
    },
    'Failed to update password!'
  );
}

function cancel() {
  emit('updated', false);
  isOpen.value = false;
}
</script>

<template>
  <Dialog v-model:open="isOpen">
    <DialogContent class="sm:max-w-[500px]">
      <form @submit.prevent="save">
        <DialogHeader>
          <DialogTitle>Update password for {{ user?.userName }}</DialogTitle>
        </DialogHeader>

        <div class="space-y-4 py-4">
          <div class="space-y-2">
            <Label for="password">Password</Label>
            <Input
              id="password"
              v-model="value.password"
              v-autofocus
              type="password"
              :disabled="asyncState.isSaving"
            />
          </div>

          <div class="space-y-2">
            <Label for="confirmation">Password Confirmation</Label>
            <Input
              id="confirmation"
              v-model="value.passwordConfirmation"
              type="password"
              :disabled="asyncState.isSaving"
              :class="{ 'border-destructive': !passwordsMatch }"
            />
            <p v-if="!passwordsMatch" class="text-sm text-destructive">Passwords do not match</p>
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
