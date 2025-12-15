<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuth } from '@/composables/useAuth';
import MainView from './MainView.vue';
import { Toaster } from '@ui/toast';

const auth = useAuth();
const isAuthorized = ref(false);

onMounted(() => {
  // Check if user is stored in localStorage
  const user = auth.user;
  isAuthorized.value = user?.isAdmin ?? false;

  // Watch for auth changes
  auth.onUserChanged.subscribe((user) => {
    isAuthorized.value = user?.isAdmin ?? false;
  });
});
</script>

<template>
  <div id="app" class="min-h-screen bg-background">
    <MainView v-if="isAuthorized" />
    <div v-else class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <h1 class="text-2xl font-bold mb-4">Authentication Required</h1>
        <p class="text-muted-foreground">Please log in to access the admin panel.</p>
      </div>
    </div>
    <Toaster />
  </div>
</template>
