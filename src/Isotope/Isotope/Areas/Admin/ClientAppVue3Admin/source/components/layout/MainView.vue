<script setup lang="ts">
import { RouterView } from 'vue-router';
import MainMenu from './MainMenu.vue';
</script>

<template>
  <div class="flex h-screen overflow-hidden">
    <!-- Sidebar -->
    <aside class="w-64 border-r bg-card flex-shrink-0">
      <div class="flex flex-col h-full">
        <!-- Logo/Brand -->
        <div class="h-16 flex items-center px-6 border-b">
          <h1 class="text-xl font-bold">Isotope Admin</h1>
        </div>

        <!-- Navigation -->
        <nav class="flex-1 overflow-y-auto py-4">
          <MainMenu />
        </nav>
      </div>
    </aside>

    <!-- Main Content -->
    <main class="flex-1 overflow-auto">
      <RouterView />
    </main>
  </div>
</template>
