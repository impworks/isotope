<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import type { Stats } from '@/vms/Stats';
import Loading from '@/components/utils/Loading.vue';
import { Card, CardContent, CardHeader, CardTitle } from '@ui/card';
import {
  FolderOpen,
  Image,
  Tags,
  Link,
  Database,
  HardDrive,
  Cpu,
  Server,
  GitCommit
} from 'lucide-vue-next';

const api = useApi();
const { asyncState, showLoading } = useAsyncState();

const stats = ref<Stats | null>(null);

onMounted(async () => {
  await showLoading(
    async () => {
      stats.value = await api.stats.get();
    },
    'Failed to load statistics!'
  );
});

function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B';
  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function formatNumber(num: number): string {
  return num.toLocaleString();
}

function shortCommit(commit: string): string {
  return commit.substring(0, 7);
}
</script>

<template>
  <div class="p-6">
    <h1 class="text-3xl font-bold mb-6">Statistics</h1>

    <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
      <div v-if="stats" class="space-y-6">
        <!-- Entries -->
        <section>
          <h2 class="text-lg font-semibold mb-3 text-muted-foreground">Entries</h2>
          <div class="grid gap-3 grid-cols-2 md:grid-cols-4 xl:grid-cols-5">
            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Folders</CardTitle>
                <FolderOpen class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-2xl font-bold">{{ formatNumber(stats.folderCount) }}</div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Photos</CardTitle>
                <Image class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-2xl font-bold">{{ formatNumber(stats.photoCount) }}</div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Tags</CardTitle>
                <Tags class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-2xl font-bold">{{ formatNumber(stats.tagCount) }}</div>
                <p class="text-xs text-muted-foreground">
                  {{ formatNumber(stats.tagBindingCount) }} applied
                </p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Shared Links</CardTitle>
                <Link class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-2xl font-bold">{{ formatNumber(stats.sharedLinkCount) }}</div>
              </CardContent>
            </Card>
          </div>
        </section>

        <!-- Disk Usage -->
        <section>
          <h2 class="text-lg font-semibold mb-3 text-muted-foreground">Disk Usage</h2>
          <div class="grid gap-3 grid-cols-2 md:grid-cols-4 xl:grid-cols-5">
            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Database</CardTitle>
                <Database class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-2xl font-bold">{{ formatBytes(stats.databaseSizeBytes) }}</div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Original Photos</CardTitle>
                <HardDrive class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-2xl font-bold">{{ formatBytes(stats.originalPhotosSizeBytes) }}</div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Preview Cache</CardTitle>
                <HardDrive class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-2xl font-bold">{{ formatBytes(stats.imageCacheSizeBytes) }}</div>
              </CardContent>
            </Card>
          </div>
        </section>

        <!-- System -->
        <section>
          <h2 class="text-lg font-semibold mb-3 text-muted-foreground">System</h2>
          <div class="grid gap-3 grid-cols-2 md:grid-cols-4 xl:grid-cols-5">
            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Memory</CardTitle>
                <Cpu class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-2xl font-bold">{{ formatBytes(stats.usedMemoryBytes) }}</div>
                <p class="text-xs text-muted-foreground">
                  {{ formatBytes(stats.availableMemoryBytes) }} available
                </p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">.NET Runtime</CardTitle>
                <Server class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-lg font-bold truncate" :title="stats.dotNetVersion">
                  {{ stats.dotNetVersion }}
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Operating System</CardTitle>
                <Server class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-lg font-bold break-words">
                  {{ stats.operatingSystem }}
                </div>
              </CardContent>
            </Card>

            <Card v-if="stats.buildCommit !== 'unknown'">
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Build Commit</CardTitle>
                <GitCommit class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-lg font-bold font-mono" :title="stats.buildCommit">
                  {{ shortCommit(stats.buildCommit) }}
                </div>
              </CardContent>
            </Card>
          </div>
        </section>
      </div>
    </Loading>
  </div>
</template>
