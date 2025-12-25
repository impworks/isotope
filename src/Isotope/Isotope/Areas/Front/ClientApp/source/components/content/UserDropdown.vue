<script setup lang="ts">
import { ref, onMounted, onUnmounted, inject, getCurrentInstance } from 'vue';
import { GalleryInfo } from '@/vms/GalleryInfo';
import { ApiServiceKey, AuthServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import type { AuthService } from '../../../../../Common/source/services/AuthService';
import ChangePasswordDialog from './dialogs/ChangePasswordDialog.vue';

const $api = inject(ApiServiceKey)!;
const $auth = inject(AuthServiceKey)!;
const instance = getCurrentInstance();

const avatar = ref<string | null>(null);
const isOpen = ref(false);
const info = ref<GalleryInfo | null>(null);
const isChangePasswordVisible = ref(false);

function clickHandler(e: any) {
  if (isOpen.value && instance?.proxy?.$el && !instance.proxy.$el.contains(e.target)) {
    isOpen.value = false;
  }
}

function logout() {
  $auth.user = null;
}

function changePassword() {
  isChangePasswordVisible.value = true;
}

onMounted(async () => {
  try {
    info.value = await $api.getInfo();
  } catch {
    // Ignore errors
  }

  window.addEventListener('click', clickHandler);
});

onUnmounted(() => {
  window.removeEventListener('click', clickHandler);
});
</script>

<template>
  <div class="user-dropdown" v-if="info && info.isAuthorized && info.isLinkValid === null">
    <button class="btn-header" @click="isOpen = !isOpen">
      <div class="btn-header__content">
        <i class="icon icon-user" v-if="!avatar"></i>
        <div class="avatar" v-else style="background-image: url('../../../images/avatar.jpg')"></div>
      </div>
    </button>
    <Transition name="user-dropdown__transition">
      <div class="user-dropdown__content" v-if="isOpen">
        <ul>
          <li v-if="info.isAdmin"><a href="/@admin">Admin panel</a></li>
          <li><a class="clickable" @click.prevent="changePassword">Change password</a></li>
          <li><a class="clickable" @click.prevent="logout">Log out</a></li>
        </ul>
      </div>
    </Transition>
    <change-password-dialog v-model="isChangePasswordVisible"></change-password-dialog>
  </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";
@import "bootstrap/scss/mixins";

.user-dropdown {
  position: relative;

  &__transition {
    &-enter-active,
    &-leave-active {
      transition:
        opacity 200ms ease,
        transform 200ms ease;
    }

    &-enter-from,
    &-leave-to {
      opacity: 0;
      transform: translateY(-0.5rem);
    }
  }

  &__content {
    width: 11rem;
    top: calc(100% - 0.5rem);
    right: 0.5rem;
    position: absolute;
    z-index: $zindex-dropdown;

    @include media-breakpoint-down(md) {
      right: 0;
    }

    ul {
      box-shadow: $box-shadow;
      background-color: $white;
      border: 1px solid $gray-400;
      border-radius: $border-radius;
      list-style: none;
      padding: 0;
      color: $gray-800;
      text-align: left;
      position: relative;
      margin-top: -1px;

      li {
        &:first-child {
          a {
            border-radius: 0.25rem 0.25rem 0 0;

            &:after,
            &:before {
              bottom: 100%;
              right: 1.5rem;
              border: solid transparent;
              content: '';
              height: 0;
              width: 0;
              position: absolute;
              pointer-events: none;
            }

            &:after {
              border-color: rgba(0, 0, 0, 0);
              border-bottom-color: $white;
              border-width: 0.5rem;
              margin-right: -0.5rem;
            }

            &:before {
              border-color: rgba(0, 0, 0, 0);
              border-bottom-color: $gray-400;
              border-width: calc(0.5rem + 1px);
              margin-right: calc(-0.5rem - 1px);
            }

            &:active,
            .no-touch &:hover:after {
              border-bottom-color: $gray-200;
            }
          }
        }

        &:last-child a {
          border-radius: 0 0 0.25rem 0.25rem;
        }

        &:not(:last-child) a {
          border-bottom: 1px solid $gray-300;
        }

        a {
          color: $gray-800;
          display: block;
          padding: 0.6rem 1rem;

          &:hover {
            color: $gray-800;
            text-decoration: none;
          }

          &:active,
          .no-touch &:hover {
            color: $gray-800;
            background-color: $gray-200;
          }
        }
      }
    }
  }
}
</style>
