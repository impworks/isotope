<script setup lang="ts">
import { ref, computed, onMounted, inject } from 'vue';
import { useAsyncState } from '@/composables/useAsyncState';
import { ApiServiceKey, AuthServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import type { AuthService } from '../../../../Common/source/services/AuthService';
import Loading from '@/components/utils/Loading.vue';

const $api = inject(ApiServiceKey)!;
const $auth = inject(AuthServiceKey)!;
const { asyncState, showLoading } = useAsyncState();

const username = ref<string | null>(null);
const password = ref<string | null>(null);
const error = ref<string | null>(null);
const caption = ref('isotope');

const canSignIn = computed(() => {
  return !!username.value && !!password.value;
});

async function signIn() {
  if (!canSignIn.value) {
    return;
  }

  error.value = null;
  try {
    await showLoading(async () => {
      const result = await $api.authorize({ username: username.value!, password: password.value! });
      if (result.success) {
        $auth.user = result.user;
      } else {
        error.value = result.errorMessage;
      }
    });
  } catch (e) {
    error.value = 'Server connection failed.';
  }
}

onMounted(async () => {
  caption.value = (await $api.getInfo()).caption;
});
</script>

<template>
  <form @submit.prevent="signIn()">
    <div class="card shadow-sm">
      <div class="card-header bg-white py-0">
        <div class="logotype">{{ caption }}</div>
      </div>
      <div class="card-body">
        <div class="form-group">
          <input
            type="text"
            class="form-control"
            placeholder="Username"
            autocapitalize="off"
            v-model="username"
            :disabled="asyncState.isLoading"
            v-autofocus
          />
        </div>
        <div class="form-group mb-0">
          <input
            type="password"
            v-model="password"
            placeholder="Password"
            class="form-control"
            :disabled="asyncState.isLoading"
          />
        </div>
        <div class="alert alert-danger m-0 mt-3" v-if="error">
          {{ error }}
        </div>
      </div>
      <div class="card-footer bg-white">
        <button type="submit" class="btn btn-primary w-100" :disabled="!canSignIn || asyncState.isLoading">
          <loading :is-loading="asyncState.isLoading" :text="'Working…'"> Log in </loading>
        </button>
      </div>
    </div>
  </form>
</template>
