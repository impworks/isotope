<script setup lang="ts">
import { RouterLink } from 'vue-router';
import { computed } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();

const menuItems = [
  { to: '/folders', icon: 'fa-folder', label: 'Folders' },
  { to: '/tags', icon: 'fa-tags', label: 'Tags' },
  { to: '/shared-links', icon: 'fa-share-alt', label: 'Shared Links' },
  { to: '/users', icon: 'fa-users', label: 'Users' },
  { to: '/config', icon: 'fa-cog', label: 'Configuration' }
];

function isActive(path: string): boolean {
  return route.path === path || route.path.startsWith(path + '/');
}
</script>

<template>
  <div class="space-y-1 px-3">
    <RouterLink
      v-for="item in menuItems"
      :key="item.to"
      :to="item.to"
      class="flex items-center gap-3 px-3 py-2 rounded-md text-sm font-medium transition-colors"
      :class="[
        isActive(item.to)
          ? 'bg-primary text-primary-foreground'
          : 'text-muted-foreground hover:bg-accent hover:text-accent-foreground'
      ]"
    >
      <span class="fa" :class="item.icon"></span>
      <span>{{ item.label }}</span>
    </RouterLink>
  </div>
</template>
