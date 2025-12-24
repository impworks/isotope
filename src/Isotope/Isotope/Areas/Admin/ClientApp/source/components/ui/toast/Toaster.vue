<script setup lang="ts">
import { isVNode } from "vue"
import { Toast, ToastClose, ToastDescription, ToastProvider, ToastTitle, ToastViewport } from "."
import { useToast } from "./use-toast"

const { toasts } = useToast()
</script>

<template>
  <ToastProvider>
    <Toast v-for="toast in toasts" :key="toast.id" v-bind="toast">
      <div class="flex items-center gap-2">
        <template v-if="toast.description">
          <div v-if="isVNode(toast.description)" class="text-base shrink-0">
            <component :is="toast.description" />
          </div>
          <div v-else class="text-base shrink-0">
            {{ toast.description }}
          </div>
        </template>
        <div class="flex-1 min-w-0">
          <ToastTitle v-if="toast.title" class="text-sm">
            {{ toast.title }}
          </ToastTitle>
        </div>
        <ToastClose />
      </div>
      <component :is="toast.action" />
    </Toast>
    <ToastViewport />
  </ToastProvider>
</template>
