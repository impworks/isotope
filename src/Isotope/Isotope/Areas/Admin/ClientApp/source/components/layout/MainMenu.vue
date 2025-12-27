<script setup lang="ts">
import { RouterLink } from 'vue-router';
import { useRoute } from 'vue-router';
import { Folder, Tags, Share2, Users, Settings, BarChart3, type LucideIcon } from 'lucide-vue-next';

const route = useRoute();

const menuItems: { to: string; icon: LucideIcon; label: string }[] = [
  { to: '/folders', icon: Folder, label: 'Folders' },
  { to: '/tags', icon: Tags, label: 'Tags' },
  { to: '/shared-links', icon: Share2, label: 'Shared Links' },
  { to: '/users', icon: Users, label: 'Users' },
  { to: '/config', icon: Settings, label: 'Configuration' },
  { to: '/stats', icon: BarChart3, label: 'Statistics' }
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
      <component :is="item.icon" class="size-4" />
      <span>{{ item.label }}</span>
    </RouterLink>
  </div>
</template>
