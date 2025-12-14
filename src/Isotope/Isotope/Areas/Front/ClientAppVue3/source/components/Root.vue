<script setup lang="ts">
import { ref, watch, onBeforeMount, inject } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAsyncState } from '@/composables/useAsyncState';
import { useLifetime } from '@/composables/useLifetime';
import { ApiServiceKey, AuthServiceKey, FilterStateServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import type { AuthService } from '../../../../Common/source/services/AuthService';
import type { FilterStateService } from '@/services/FilterStateService';
import { DeviceHelper } from '@/utils/DeviceHelper';
import LoginForm from './LoginForm.vue';
import Loading from './utils/Loading.vue';

const $api = inject(ApiServiceKey)!;
const $auth = inject(AuthServiceKey)!;
const $filter = inject(FilterStateServiceKey)!;
const { asyncState, showLoading } = useAsyncState();
const { observe } = useLifetime();
const route = useRoute();
const router = useRouter();

const error = ref<string | null>(null);
const authRequired = ref(true);
const loaded = ref(false);
const isTouchDevice = ref(false);

watch(route, () => {
  $filter.updateFromRoute(route);
});

onBeforeMount(async () => {
  $filter.updateFromRoute(route);
  observe($filter.onUrlChanged, (x) => router.push(x));

  isTouchDevice.value = DeviceHelper.isTouch();
  document.addEventListener('touchstart', function () {}, false); // enables :active pseudo for iOS Safari

  try {
    await showLoading(async () => {
      const info = await $api.getInfo();
      document.title = info.caption;
      if (info.allowGuests) {
        authRequired.value = false;
      } else {
        authRequired.value = !info.isAuthorized && info.isLinkValid === null;
        observe($auth.onUserChanged, (x) => (authRequired.value = !x));
      }
      if (info.isLinkValid === false) {
        error.value = 'The specified share link is invalid.';
      }
    });
  } catch (e) {
    error.value = 'Gallery is unavailable.';
  } finally {
    loaded.value = true;
  }
});
</script>

<template>
  <loading :is-loading="asyncState.isLoading" :is-full-page="true">
    <div
      class="root"
      :class="{
        touch: isTouchDevice,
        'no-touch': !isTouchDevice
      }"
      v-if="loaded"
    >
      <div class="root__centered-content" v-if="error || authRequired">
        <div v-if="error" class="gallery__error">
          <div class="gallery__error__content">
            <div class="gallery__error__image"></div>
            <h3>Error</h3>
            <p>{{ error }}</p>
          </div>
        </div>
        <LoginForm v-else></LoginForm>
      </div>
      <router-view v-else></router-view>
      <Teleport to="body">
        <div id="overlay"></div>
      </Teleport>
    </div>
  </loading>
</template>

<style lang="scss">
@import "../../../../Common/styles/variables";
@import "./node_modules/bootstrap/scss/functions";
@import "./node_modules/bootstrap/scss/variables";
@import "./node_modules/bootstrap/scss/mixins";

.root {
  height: 100%;
  display: flex;
  flex-direction: column;

  &__centered-content {
    margin: auto 0;
    padding: 1rem;
    min-width: 300px;

    @include media-breakpoint-up(sm) {
      width: 23rem;
      margin: auto;
    }
  }
}
</style>
