<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
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
  HardDrive,
  Cpu,
  Code2,
  Monitor,
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

const diskUsedBytes = computed(() => {
  if (!stats.value) return 0;
  return stats.value.databaseSizeBytes + stats.value.originalPhotosSizeBytes + stats.value.imageCacheSizeBytes;
});

const diskFreeBytes = computed(() => {
  if (!stats.value) return 0;
  return Math.max(0, stats.value.totalDiskBytes - diskUsedBytes.value);
});

// Proportions of each type within the used space only
const diskTypePct = computed(() => {
  if (!stats.value || diskUsedBytes.value === 0) return { db: 0, original: 0, cache: 0 };
  const total = diskUsedBytes.value;
  return {
    db: (stats.value.databaseSizeBytes / total) * 100,
    original: (stats.value.originalPhotosSizeBytes / total) * 100,
    cache: (stats.value.imageCacheSizeBytes / total) * 100,
  };
});

// Used vs free relative to total disk
const diskUsagePct = computed(() => {
  if (!stats.value || stats.value.totalDiskBytes === 0) return 0;
  return (diskUsedBytes.value / stats.value.totalDiskBytes) * 100;
});

const memPct = computed(() => {
  if (!stats.value) return { used: 0 };
  const total = stats.value.usedMemoryBytes + stats.value.availableMemoryBytes;
  if (total === 0) return { used: 0 };
  return { used: (stats.value.usedMemoryBytes / total) * 100 };
});
</script>

<template>
  <div class="p-6">
    <h1 class="text-3xl font-bold mb-6">Statistics</h1>

    <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
      <div v-if="stats" class="space-y-8">

        <!-- Entities -->
        <section>
          <h2 class="text-lg font-semibold mb-3 text-muted-foreground">Entities</h2>
          <div class="flex flex-wrap gap-3">
            <Card class="w-40">
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Folders</CardTitle>
                <FolderOpen class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-4xl font-bold">{{ formatNumber(stats.folderCount) }}</div>
              </CardContent>
            </Card>

            <Card class="w-40">
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Photos</CardTitle>
                <Image class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-4xl font-bold">{{ formatNumber(stats.photoCount) }}</div>
              </CardContent>
            </Card>

            <Card class="w-40">
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Tags</CardTitle>
                <Tags class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-4xl font-bold">{{ formatNumber(stats.tagCount) }}</div>
                <p class="text-xs text-muted-foreground">
                  {{ formatNumber(stats.tagBindingCount) }} applied
                </p>
              </CardContent>
            </Card>

            <Card class="w-40">
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle class="text-sm font-medium">Shared Links</CardTitle>
                <Link class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="text-4xl font-bold">{{ formatNumber(stats.sharedLinkCount) }}</div>
              </CardContent>
            </Card>
          </div>
        </section>

        <!-- Resource Usage -->
        <section>
          <h2 class="text-lg font-semibold mb-3 text-muted-foreground">Resource Usage</h2>
          <div class="w-1/2 space-y-3">

            <!-- Disk -->
            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-3">
                <CardTitle class="text-sm font-medium">Disk</CardTitle>
                <HardDrive class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent class="space-y-4">

                <!-- By type -->
                <div>
                  <p class="text-xs text-muted-foreground mb-1.5">By type</p>
                  <div class="h-4 w-full rounded-full overflow-hidden flex">
                    <div
                      class="bg-blue-500"
                      :style="{ width: `${diskTypePct.db}%` }"
                      :title="`Database: ${formatBytes(stats.databaseSizeBytes)}`"
                    />
                    <div
                      class="bg-red-500"
                      :style="{ width: `${diskTypePct.original}%` }"
                      :title="`Original Photos: ${formatBytes(stats.originalPhotosSizeBytes)}`"
                    />
                    <div
                      class="flex-1 bg-yellow-500"
                      :title="`Preview Cache: ${formatBytes(stats.imageCacheSizeBytes)}`"
                    />
                  </div>
                  <div class="flex flex-wrap gap-x-4 gap-y-1.5 mt-2 text-sm">
                    <div class="flex items-center gap-1.5">
                      <div class="w-2.5 h-2.5 rounded-full bg-blue-500 shrink-0" />
                      <span class="text-muted-foreground">Database:</span>
                      <span class="font-medium">{{ formatBytes(stats.databaseSizeBytes) }}</span>
                    </div>
                    <div class="flex items-center gap-1.5">
                      <div class="w-2.5 h-2.5 rounded-full bg-red-500 shrink-0" />
                      <span class="text-muted-foreground">Original Photos:</span>
                      <span class="font-medium">{{ formatBytes(stats.originalPhotosSizeBytes) }}</span>
                    </div>
                    <div class="flex items-center gap-1.5">
                      <div class="w-2.5 h-2.5 rounded-full bg-yellow-500 shrink-0" />
                      <span class="text-muted-foreground">Preview Cache:</span>
                      <span class="font-medium">{{ formatBytes(stats.imageCacheSizeBytes) }}</span>
                    </div>
                  </div>
                </div>

                <!-- Used / free -->
                <div>
                  <p class="text-xs text-muted-foreground mb-1.5">Total</p>
                  <div class="h-4 w-full rounded-full overflow-hidden flex">
                    <div
                      class="bg-blue-500"
                      :style="{ width: `${diskUsagePct}%` }"
                      :title="`Used: ${formatBytes(diskUsedBytes)}`"
                    />
                    <div
                      class="flex-1 bg-muted-foreground/20"
                      :title="`Free: ${formatBytes(diskFreeBytes)}`"
                    />
                  </div>
                  <div class="flex flex-wrap gap-x-4 gap-y-1.5 mt-2 text-sm">
                    <div class="flex items-center gap-1.5">
                      <div class="w-2.5 h-2.5 rounded-full bg-blue-500 shrink-0" />
                      <span class="text-muted-foreground">Used:</span>
                      <span class="font-medium">{{ formatBytes(diskUsedBytes) }}</span>
                    </div>
                    <div class="flex items-center gap-1.5">
                      <div class="w-2.5 h-2.5 rounded-full bg-muted-foreground/25 shrink-0" />
                      <span class="text-muted-foreground">Free:</span>
                      <span class="font-medium">{{ formatBytes(diskFreeBytes) }}</span>
                    </div>
                  </div>
                </div>

              </CardContent>
            </Card>

            <!-- Memory -->
            <Card>
              <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-3">
                <CardTitle class="text-sm font-medium">Memory</CardTitle>
                <Cpu class="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div class="h-4 w-full rounded-full overflow-hidden flex">
                  <div
                    class="bg-blue-500"
                    :style="{ width: `${memPct.used}%` }"
                    :title="`Used: ${formatBytes(stats.usedMemoryBytes)}`"
                  />
                  <div
                    class="flex-1 bg-muted-foreground/20"
                    :title="`Free: ${formatBytes(stats.availableMemoryBytes)}`"
                  />
                </div>
                <div class="flex flex-wrap gap-x-5 gap-y-1.5 mt-3 text-sm">
                  <div class="flex items-center gap-1.5">
                    <div class="w-2.5 h-2.5 rounded-full bg-blue-500 shrink-0" />
                    <span class="text-muted-foreground">Used:</span>
                    <span class="font-medium">{{ formatBytes(stats.usedMemoryBytes) }}</span>
                  </div>
                  <div class="flex items-center gap-1.5">
                    <div class="w-2.5 h-2.5 rounded-full bg-muted-foreground/25 shrink-0" />
                    <span class="text-muted-foreground">Free:</span>
                    <span class="font-medium">{{ formatBytes(stats.availableMemoryBytes) }}</span>
                  </div>
                </div>
              </CardContent>
            </Card>

          </div>
        </section>

        <!-- System -->
        <section>
          <h2 class="text-lg font-semibold mb-3 text-muted-foreground">System</h2>
          <div class="w-1/2">
          <Card>
            <CardContent class="pt-6">
              <dl class="space-y-2.5">
                <div class="flex items-start gap-4">
                  <dt class="text-sm text-muted-foreground w-28 shrink-0 flex items-center gap-1.5">
                    <Code2 class="h-3.5 w-3.5" />
                    Runtime
                  </dt>
                  <dd class="text-sm font-medium">{{ stats.dotNetVersion }}</dd>
                </div>
                <div class="flex items-start gap-4">
                  <dt class="text-sm text-muted-foreground w-28 shrink-0 flex items-center gap-1.5">
                    <Monitor class="h-3.5 w-3.5" />
                    OS
                  </dt>
                  <dd class="text-sm font-medium">{{ stats.operatingSystem }}</dd>
                </div>
                <div v-if="stats.buildCommit !== 'unknown'" class="flex items-start gap-4">
                  <dt class="text-sm text-muted-foreground w-28 shrink-0 flex items-center gap-1.5">
                    <GitCommit class="h-3.5 w-3.5" />
                    Build Commit
                  </dt>
                  <dd class="text-sm font-medium font-mono" :title="stats.buildCommit">
                    {{ shortCommit(stats.buildCommit) }}
                  </dd>
                </div>
              </dl>
            </CardContent>
          </Card>
          </div>
        </section>

      </div>
    </Loading>
  </div>
</template>
