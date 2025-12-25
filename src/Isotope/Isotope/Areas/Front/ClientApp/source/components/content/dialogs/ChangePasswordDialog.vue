<script setup lang="ts">
import { ref, computed, inject } from 'vue';
import type { AxiosResponse } from 'axios';
import { ApiServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import { isAxiosError } from '@/utils/AxiosHelpers';
import { useAsyncState } from '@/composables/useAsyncState';
import ModalWindow from '@/components/utils/ModalWindow.vue';
import Loading from '@/components/utils/Loading.vue';

const model = defineModel<boolean>({ default: false });

const $api = inject(ApiServiceKey)!;
const { asyncState, showSaving } = useAsyncState();

const oldPassword = ref<string | null>(null);
const newPassword = ref<string | null>(null);
const newPassword2 = ref<string | null>(null);
const errorMessage = ref<string | null>(null);

const canReset = computed(() => {
  return oldPassword.value && newPassword.value && newPassword2.value && !asyncState.isSaving;
});

function validate(): string | null {
  if (!oldPassword.value || !newPassword.value || !newPassword2.value) {
    return 'Not all fields are filled!';
  }

  if (newPassword.value !== newPassword2.value) {
    return 'Passwords do not match!';
  }

  if (newPassword.value.length < 6) {
    return 'Password must be at least 6 characters long.';
  }

  return null;
}

async function reset() {
  if (!canReset.value) {
    return;
  }

  errorMessage.value = validate();
  if (errorMessage.value) {
    return;
  }

  try {
    await showSaving(async () => {
      await $api.changePassword({
        oldPassword: oldPassword.value!,
        newPassword: newPassword.value!,
        newPasswordRepeat: newPassword2.value!
      });
    });

    model.value = false;
  } catch (e) {
    if (isAxiosError(e)) {
      const resp = e.response as AxiosResponse;
      errorMessage.value = resp.data.error;
    } else {
      console.log('Failed to change password!', e);
    }
  }
}
</script>

<template>
  <modal-window v-model="model">
    <template v-slot:header>
      <div class="modal-window__header__caption">Change password</div>
      <div class="modal-window__header__actions">
        <button href="#" class="btn-header" @click.prevent="model = !model">
          <div class="btn-header__content">
            <i class="icon icon-cross"></i>
          </div>
        </button>
      </div>
    </template>
    <template v-slot:content>
      <div>
        <div class="row">
          <label class="col-form-label col-sm-4">Old password</label>
          <div class="col-sm-8">
            <input type="password" class="form-control" v-model="oldPassword" v-autofocus />
          </div>
        </div>
        <div class="row mt-3">
          <label class="col-form-label col-sm-4">New password</label>
          <div class="col-sm-8">
            <input type="password" class="form-control" v-model="newPassword" />
            <small class="text-muted">At least 6 characters</small>
          </div>
        </div>
        <div class="row mt-3">
          <label class="col-form-label col-sm-4">Confirmation</label>
          <div class="col-sm-8">
            <input type="password" class="form-control" v-model="newPassword2" />
          </div>
        </div>
        <div class="row mt-3" v-if="errorMessage">
          <div class="col-sm-12">
            <div class="alert alert-danger mb-0">
              {{ errorMessage }}
            </div>
          </div>
        </div>
      </div>
    </template>
    <template v-slot:footer>
      <button type="button" class="btn btn-primary w-100" :disabled="!canReset" @click.prevent="reset">
        <span v-if="asyncState.isSaving">Resetting...</span>
        <span v-else>Reset</span>
      </button>
    </template>
  </modal-window>
</template>
