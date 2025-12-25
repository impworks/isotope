<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import type { UserCreation } from '@/vms/UserCreation';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Input } from '@ui/input';
import { Label } from '@ui/label';
import { ToggleGroup, ToggleGroupItem } from '@ui/toggle-group';
import { Button } from '@ui/button';

const isOpen = defineModel<boolean>('open', { default: false });
const emit = defineEmits<{
  created: [value: boolean];
}>();

const api = useApi();
const { asyncState, showSaving } = useAsyncState();

const value = ref<UserCreation>({
  userInfo: {
    id: '',
    userName: '',
    isAdmin: false
  },
  passwordInfo: {
    password: '',
    passwordConfirmation: ''
  }
});

const accessLevels = [
  { caption: 'User', isAdmin: false, color: 'bg-blue-500' },
  { caption: 'Admin', isAdmin: true, color: 'bg-red-500' }
];

watch(() => isOpen.value, (newVal) => {
  if (newVal) {
    value.value = {
      userInfo: {
        id: '',
        userName: '',
        isAdmin: false
      },
      passwordInfo: {
        password: '',
        passwordConfirmation: ''
      }
    };
  }
});

const canSave = computed(() => {
  const u = value.value;
  return !asyncState.isSaving
    && !!u.userInfo.userName
    && !!u.passwordInfo.password
    && u.passwordInfo.password === u.passwordInfo.passwordConfirmation;
});

const passwordsMatch = computed(() => {
  if (!value.value.passwordInfo.passwordConfirmation) return true;
  return value.value.passwordInfo.password === value.value.passwordInfo.passwordConfirmation;
});

async function save() {
  if (!canSave.value) return;

  const success = await showSaving(
    async () => {
      await api.users.create(value.value);
      emit('created', true);
      isOpen.value = false;
    },
    'Failed to create user!'
  );
}

function cancel() {
  emit('created', false);
  isOpen.value = false;
}
</script>

<template>
  <Dialog v-model:open="isOpen">
    <DialogContent class="sm:max-w-[500px]">
      <form @submit.prevent="save">
        <DialogHeader>
          <DialogTitle>Create user</DialogTitle>
        </DialogHeader>

        <div class="space-y-4 py-4">
          <div class="space-y-2">
            <Label for="email">Email</Label>
            <Input
              id="email"
              v-model="value.userInfo.userName"
              v-autofocus
              type="email"
              :disabled="asyncState.isSaving"
            />
          </div>

          <div class="space-y-3">
            <Label>Access Level</Label>
            <ToggleGroup v-model="value.userInfo.isAdmin" type="single" :disabled="asyncState.isSaving" class="justify-start">
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

          <div class="space-y-2">
            <Label for="password">Password</Label>
            <Input
              id="password"
              v-model="value.passwordInfo.password"
              type="password"
              :disabled="asyncState.isSaving"
            />
          </div>

          <div class="space-y-2">
            <Label for="confirmation">Password Confirmation</Label>
            <Input
              id="confirmation"
              v-model="value.passwordInfo.passwordConfirmation"
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
            <span v-if="asyncState.isSaving">Creating...</span>
            <span v-else>Create</span>
          </Button>
        </DialogFooter>
      </form>
    </DialogContent>
  </Dialog>
</template>
