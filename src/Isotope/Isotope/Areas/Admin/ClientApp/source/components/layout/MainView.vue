<script setup lang="ts">
import { RouterView } from 'vue-router';
import { useAuth } from '@/composables/useAuth';
import MainMenu from './MainMenu.vue';
import { Button } from '@ui/button';
import { LogOut, Github } from 'lucide-vue-next';

const auth = useAuth();

function logout() {
  auth.user = null;
  window.location.href = '/';
}
</script>

<template>
  <div class="flex h-screen overflow-hidden">
    <!-- Sidebar -->
    <aside class="w-64 border-r bg-card flex-shrink-0">
      <div class="flex flex-col h-full">
        <!-- Logo/Brand -->
        <div class="h-16 flex items-center px-6 border-b">
          <a href="/" class="text-xl font-bold hover:text-primary transition-colors">Isotope Admin</a>
        </div>

        <!-- Navigation -->
        <nav class="flex-1 overflow-y-auto py-4">
          <MainMenu />
        </nav>

        <!-- Footer -->
        <div class="p-4 border-t space-y-1">
          <Button variant="ghost" class="w-full justify-start" as-child>
            <a href="https://github.com/impworks/isotope" target="_blank" rel="noopener noreferrer">
              <Github class="mr-2 h-4 w-4" />
              GitHub
            </a>
          </Button>
          <Button variant="ghost" class="w-full justify-start" @click="logout">
            <LogOut class="mr-2 h-4 w-4" />
            Logout
          </Button>
        </div>
      </div>
    </aside>

    <!-- Main Content -->
    <main class="flex-1 overflow-auto">
      <RouterView />
    </main>
  </div>
</template>
