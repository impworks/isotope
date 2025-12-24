<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import draggable from 'vuedraggable';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { Folder } from '@/vms/Folder';
import type { MediaThumbnail } from '@/vms/MediaThumbnail';
import { Button } from '@ui/button';
import { Alert, AlertDescription } from '@ui/alert';
import {
  ContextMenu,
  ContextMenuContent,
  ContextMenuItem,
  ContextMenuSeparator,
  ContextMenuTrigger
} from '@ui/context-menu';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger
} from '@ui/dropdown-menu';
import Loading from '@/components/utils/Loading.vue';
import FilePicker from '@/components/utils/FilePicker.vue';
import ConfirmationDlg from '@/components/dialogs/ConfirmationDlg.vue';
import MassMoveDlg from '@/components/dialogs/MassMoveDlg.vue';
import MassEditDlg from '@/components/dialogs/MassEditDlg.vue';
import MediaEditorDlg, { type EditorTab } from '@/components/dialogs/MediaEditorDlg.vue';
import {
  ArrowLeft, Layers, ArrowUpDown, Upload, X, Check, Pencil, Forward,
  CheckSquare, Square, RotateCcw, MoreVertical, Trash2, AlertCircle, Loader2,
  FileText, Tags, Image
} from 'lucide-vue-next';

type Mode = 'View' | 'Reorder' | 'MassActions' | 'Upload';

interface MediaWrapper {
  media: MediaThumbnail | null;
  isSelected: boolean;
  isUploading: boolean;
  progress: number;
  error: string | null;
}

const route = useRoute();
const router = useRouter();
const api = useApi();
const toast = useToast();
const { asyncState, showLoading, showSaving } = useAsyncState();

const folderKey = ref<string>('');
const folder = ref<Folder | null>(null);
const media = ref<MediaThumbnail[]>([]);
const mediaWraps = ref<MediaWrapper[]>([]);
const mode = ref<Mode>('View');

// Reorder mode
const mediaReorder = ref<MediaThumbnail[]>([]);

// Upload mode
const batchSize = ref<number | null>(null);
const batchIndex = ref<number | null>(null);

// Dialogs
const isConfirmOpen = ref(false);
const confirmText = ref('');
const confirmCallback = ref<(() => void) | null>(null);

const isMassMoveOpen = ref(false);
const isMassEditOpen = ref(false);
const isEditorOpen = ref(false);
const editorMediaKey = ref<string>('');
const editorInitialTab = ref<EditorTab>('details');

const selectedCount = computed(() => mediaWraps.value.filter(x => x.isSelected && x.media).length);
const selectedKeys = computed(() => mediaWraps.value.filter(x => x.isSelected && x.media).map(x => x.media!.key));
const selectedMedia = computed(() => mediaWraps.value.filter(x => x.isSelected && x.media).map(x => x.media!));

watch(() => route.params.key, async (newKey) => {
  if (newKey) {
    folderKey.value = newKey as string;
    await loadFolder();

    // Check for mode query parameter
    const queryMode = route.query.mode as Mode | undefined;
    if (queryMode === 'Upload') {
      mode.value = 'Upload';
    }
  }
}, { immediate: true });

async function loadFolder() {
  await showLoading(
    async () => {
      folder.value = await api.folders.get(folderKey.value);
      if (folder.value) {
        await loadMedia();
      }
    },
    'Folder was not found!'
  );
}

async function loadMedia() {
  media.value = await api.media.getList(folderKey.value);
  mediaWraps.value = media.value.map(m => wrap(m));
}

function wrap(m: MediaThumbnail | null): MediaWrapper {
  return {
    media: m,
    isSelected: false,
    isUploading: false,
    progress: 0,
    error: null
  };
}

// Confirmation dialog
function confirm(text: string, callback: () => void) {
  confirmText.value = text;
  confirmCallback.value = callback;
  isConfirmOpen.value = true;
}

function onConfirmed(result: boolean) {
  if (result && confirmCallback.value) {
    confirmCallback.value();
  }
  confirmCallback.value = null;
}

// Remove single media
async function remove(m: MediaWrapper) {
  confirm('Are you sure you want to remove this media?', async () => {
    await showSaving(
      async () => {
        await api.media.remove(m.media.key);
        const idx = mediaWraps.value.indexOf(m);
        if (idx !== -1) {
          mediaWraps.value.splice(idx, 1);
          media.value.splice(idx, 1);
        }
        toast.success('Media removed');
      },
      'Failed to remove media'
    );
  });
}

// Reorder mode
function startReorder() {
  mode.value = 'Reorder';
  mediaReorder.value = [...media.value];
}

async function confirmReorder() {
  await showSaving(
    async () => {
      await api.media.reorder(folderKey.value, mediaReorder.value.map(x => x.key));
      media.value = [...mediaReorder.value];
      mediaWraps.value = media.value.map(m => wrap(m));
      toast.success('Media order updated');
    },
    'Failed to update media order'
  );
  mode.value = 'View';
}

function cancelReorder() {
  mediaReorder.value = [];
  mode.value = 'View';
}

// Mass actions mode
function startMassActions() {
  mode.value = 'MassActions';
  massSelect('None');
}

function cancelMassActions() {
  mode.value = 'View';
  massSelect('None');
}

function massSelect(selectMode: 'All' | 'None' | 'Invert') {
  for (const m of mediaWraps.value) {
    m.isSelected = selectMode === 'All' ? true : selectMode === 'None' ? false : !m.isSelected;
  }
}

function toggleSelection(m: MediaWrapper) {
  m.isSelected = !m.isSelected;
}

// Mass remove
function massRemove() {
  confirm(`Are you sure you want to remove the selected media (${selectedCount.value})?`, async () => {
    await showSaving(
      async () => {
        await api.media.massRemove({ keys: selectedKeys.value });
        removeSelection();
        toast.success('Media removed');
        cancelMassActions();
      },
      'Failed to remove media'
    );
  });
}

function removeSelection() {
  let idx = mediaWraps.value.length;
  while (idx--) {
    if (mediaWraps.value[idx].isSelected) {
      mediaWraps.value.splice(idx, 1);
      media.value.splice(idx, 1);
    }
  }
}

// Mass move
function massMove() {
  isMassMoveOpen.value = true;
}

function onMassMoved(result: boolean) {
  if (result) {
    removeSelection();
    toast.success('Media moved');
    cancelMassActions();
  }
}

// Mass edit
function massEdit() {
  isMassEditOpen.value = true;
}

function onMassEdited(result: boolean) {
  if (result) {
    toast.success('Media updated');
  }
}

// Upload mode
function startUpload() {
  mode.value = 'Upload';
}

function cancelUpload() {
  mode.value = 'View';
  batchSize.value = null;
  batchIndex.value = null;
}

async function handleUpload(files: File[]) {
  if (files.length === 0) return;

  try {
    batchSize.value = files.length;
    batchIndex.value = 1;

    for (const file of files) {
      await uploadOne(file);
      batchIndex.value!++;
    }
  } finally {
    cancelUpload();
  }
}

async function uploadOne(file: File) {
  const uploadWrap = wrap(null);
  uploadWrap.isUploading = true;
  mediaWraps.value.push(uploadWrap);

  try {
    const uploaded = await api.media.upload(
      folderKey.value,
      file,
      (progress) => {
        uploadWrap.progress = progress;
      }
    );

    uploadWrap.media = uploaded;
    uploadWrap.isUploading = false;
    media.value.push(uploaded);
  } catch (e: any) {
    uploadWrap.isUploading = false;
    if (e.response?.status === 400) {
      uploadWrap.error = e.response.data.error || 'Upload failed';
    } else {
      uploadWrap.error = e.message || 'Upload failed';
    }
  }
}

function goBack() {
  router.push({ name: 'folders' });
}

// Media editor
function openEditor(mediaKey: string, tab: EditorTab = 'details') {
  editorMediaKey.value = mediaKey;
  editorInitialTab.value = tab;
  isEditorOpen.value = true;
}

function onEditorNavigate(key: string, tab: EditorTab) {
  editorMediaKey.value = key;
  editorInitialTab.value = tab;
}

function onEditorSaved(result: boolean) {
  if (result) {
    // Reload media to get updated thumbnails
    loadMedia();
  }
}
</script>

<template>
  <div class="p-6">
    <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
      <template v-if="folder">
        <!-- Header -->
        <div class="flex items-center justify-between mb-6">
          <div class="flex items-center gap-4">
            <Button variant="ghost" size="icon" @click="goBack">
              <ArrowLeft class="h-4 w-4" />
            </Button>
            <h1 class="text-3xl font-bold">{{ folder.caption }}</h1>
          </div>

          <!-- View mode actions -->
          <div v-if="mode === 'View'" class="flex gap-2">
            <Button variant="outline" size="sm" @click="startMassActions" :disabled="media.length < 2">
              <Layers class="h-4 w-4 mr-2" /> Mass actions
            </Button>
            <Button variant="outline" size="sm" @click="startReorder" :disabled="media.length < 2">
              <ArrowUpDown class="h-4 w-4 mr-2" /> Reorder
            </Button>
            <Button variant="outline" size="sm" @click="startUpload">
              <Upload class="h-4 w-4 mr-2" /> Upload
            </Button>
          </div>

          <!-- Upload mode actions -->
          <div v-if="mode === 'Upload'" class="flex gap-2">
            <Button variant="outline" size="sm" @click="cancelUpload" :disabled="!!batchSize">
              <X class="h-4 w-4 mr-2" /> Cancel
            </Button>
          </div>

          <!-- Reorder mode actions -->
          <div v-if="mode === 'Reorder'" class="flex gap-2">
            <Button variant="default" size="sm" @click="confirmReorder" :disabled="asyncState.isSaving">
              <Check class="h-4 w-4 mr-2" /> Save
            </Button>
            <Button variant="outline" size="sm" @click="cancelReorder">
              <X class="h-4 w-4 mr-2" /> Cancel
            </Button>
          </div>

          <!-- Mass actions mode actions -->
          <div v-if="mode === 'MassActions'" class="flex gap-2">
            <div class="flex gap-1 mr-2">
              <Button variant="outline" size="sm" @click="massEdit" :disabled="selectedCount === 0">
                <Pencil class="h-4 w-4 mr-1" /> Edit {{ selectedCount > 0 ? selectedCount : '' }}
              </Button>
              <Button variant="outline" size="sm" @click="massMove" :disabled="selectedCount === 0">
                <Forward class="h-4 w-4 mr-1" /> Move {{ selectedCount > 0 ? selectedCount : '' }}
              </Button>
              <Button variant="destructive" size="sm" @click="massRemove" :disabled="selectedCount === 0">
                <Trash2 class="h-4 w-4 mr-1" /> Remove {{ selectedCount > 0 ? selectedCount : '' }}
              </Button>
            </div>
            <div class="flex gap-1 mr-2">
              <Button variant="outline" size="sm" @click="massSelect('All')">
                <CheckSquare class="h-4 w-4 mr-1" /> All
              </Button>
              <Button variant="outline" size="sm" @click="massSelect('None')">
                <Square class="h-4 w-4 mr-1" /> None
              </Button>
              <Button variant="outline" size="sm" @click="massSelect('Invert')">
                <RotateCcw class="h-4 w-4 mr-1" /> Invert
              </Button>
            </div>
            <Button variant="outline" size="sm" @click="cancelMassActions">
              <X class="h-4 w-4 mr-2" /> Cancel
            </Button>
          </div>
        </div>

        <!-- Upload picker -->
        <FilePicker
          v-if="mode === 'Upload'"
          :multiple="true"
          :disabled="!!batchSize"
          @change="handleUpload"
          class="mb-4"
        >
          <template v-if="!batchSize">
            <Upload class="h-5 w-5 mr-2 inline" /> Drag files here or click to select
          </template>
          <template v-else>
            <Loader2 class="h-5 w-5 mr-2 inline animate-spin" />
            Uploading {{ batchIndex }} of {{ batchSize }}...
          </template>
        </FilePicker>

        <!-- Media grid -->
        <div v-if="media.length > 0 || mediaWraps.some(w => w.isUploading || w.error)">
          <!-- Reorder mode with drag and drop -->
          <template v-if="mode === 'Reorder'">
            <draggable
              v-model="mediaReorder"
              item-key="key"
              class="flex flex-wrap gap-2"
              ghost-class="opacity-50"
            >
              <template #item="{ element }">
                <div
                  class="w-40 h-40 bg-cover bg-center border rounded cursor-move"
                  :style="{ backgroundImage: `url(${element.thumbnailPath})` }"
                ></div>
              </template>
            </draggable>
          </template>

          <!-- View mode and Upload mode with context menu -->
          <template v-else-if="mode === 'View' || mode === 'Upload'">
            <div class="flex flex-wrap gap-2">
              <template v-for="(wrap, idx) in mediaWraps" :key="idx">
                <!-- Uploading placeholder -->
                <div
                  v-if="wrap.isUploading"
                  class="w-40 h-40 border rounded bg-muted flex items-center justify-center relative"
                >
                  <div class="absolute inset-x-2 top-1/2 -translate-y-1/2">
                    <div class="h-2 bg-gray-200 rounded overflow-hidden">
                      <div
                        class="h-full bg-primary transition-all duration-150"
                        :style="{ width: `${wrap.progress}%` }"
                      ></div>
                    </div>
                    <div class="text-xs text-center mt-1 text-muted-foreground">
                      {{ Math.round(wrap.progress) }}%
                    </div>
                  </div>
                </div>

                <!-- Error state -->
                <div
                  v-else-if="wrap.error"
                  class="w-40 h-40 border border-destructive rounded bg-destructive/10 flex items-center justify-center p-2"
                >
                  <div class="text-center">
                    <AlertCircle class="h-8 w-8 text-destructive mx-auto" />
                    <p class="text-xs text-destructive mt-1 break-words">{{ wrap.error }}</p>
                  </div>
                </div>

                <!-- Normal media item -->
                <ContextMenu v-else-if="wrap.media">
                  <ContextMenuTrigger>
                    <div
                      class="w-40 h-40 bg-cover bg-center border rounded cursor-pointer relative group"
                      :style="{ backgroundImage: `url(${wrap.media.thumbnailPath})` }"
                    >
                      <div class="absolute top-1 right-1 opacity-0 group-hover:opacity-100 transition-opacity">
                        <DropdownMenu>
                          <DropdownMenuTrigger as-child>
                            <button
                              type="button"
                              class="w-7 h-7 bg-white/70 rounded-full flex items-center justify-center shadow cursor-pointer"
                              @click.stop
                            >
                              <MoreVertical class="h-4 w-4 text-gray-700" />
                            </button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent>
                            <DropdownMenuItem class="cursor-pointer" @click="openEditor(wrap.media!.key, 'details')">
                              <FileText class="h-4 w-4 mr-2" /> Edit properties
                            </DropdownMenuItem>
                            <DropdownMenuItem class="cursor-pointer" @click="openEditor(wrap.media!.key, 'tags')">
                              <Tags class="h-4 w-4 mr-2" /> Edit tags
                            </DropdownMenuItem>
                            <DropdownMenuItem class="cursor-pointer" @click="openEditor(wrap.media!.key, 'preview')">
                              <Image class="h-4 w-4 mr-2" /> Update thumbnail
                            </DropdownMenuItem>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem class="text-destructive cursor-pointer" @click="remove(wrap)">
                              <Trash2 class="h-4 w-4 mr-2" /> Remove
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </div>
                    </div>
                  </ContextMenuTrigger>
                  <ContextMenuContent>
                    <ContextMenuItem class="cursor-pointer" @click="openEditor(wrap.media!.key, 'details')">
                      <FileText class="h-4 w-4 mr-2" /> Edit properties
                    </ContextMenuItem>
                    <ContextMenuItem class="cursor-pointer" @click="openEditor(wrap.media!.key, 'tags')">
                      <Tags class="h-4 w-4 mr-2" /> Edit tags
                    </ContextMenuItem>
                    <ContextMenuItem class="cursor-pointer" @click="openEditor(wrap.media!.key, 'preview')">
                      <Image class="h-4 w-4 mr-2" /> Update thumbnail
                    </ContextMenuItem>
                    <ContextMenuSeparator />
                    <ContextMenuItem class="text-destructive cursor-pointer" @click="remove(wrap)">
                      <Trash2 class="h-4 w-4 mr-2" /> Remove
                    </ContextMenuItem>
                  </ContextMenuContent>
                </ContextMenu>
              </template>
            </div>
          </template>

          <!-- Mass actions mode with selection -->
          <template v-else-if="mode === 'MassActions'">
            <div class="flex flex-wrap gap-2">
              <template v-for="wrap in mediaWraps" :key="wrap.media?.key">
                <div
                  v-if="wrap.media"
                  class="w-40 h-40 bg-cover bg-center border rounded cursor-pointer relative"
                  :class="{ 'opacity-50': !wrap.isSelected }"
                  :style="{ backgroundImage: `url(${wrap.media.thumbnailPath})` }"
                  @click="toggleSelection(wrap)"
                >
                  <div class="absolute top-1 left-1">
                    <div
                      class="w-7 h-7 bg-white/70 rounded-full flex items-center justify-center shadow"
                    >
                      <Check v-if="wrap.isSelected" class="h-5 w-5 text-primary" />
                    </div>
                  </div>
                </div>
              </template>
            </div>
          </template>
        </div>

        <!-- Empty state -->
        <Alert v-else>
          <AlertDescription>
            No media in this folder.
          </AlertDescription>
        </Alert>
      </template>

      <!-- Folder not found -->
      <template v-else>
        <Alert variant="destructive">
          <AlertDescription>
            Folder does not exist.
          </AlertDescription>
        </Alert>
      </template>
    </Loading>

    <!-- Confirmation dialog -->
    <ConfirmationDlg
      v-model:open="isConfirmOpen"
      :text="confirmText"
      @confirmed="onConfirmed"
    />

    <!-- Mass move dialog -->
    <MassMoveDlg
      v-model:open="isMassMoveOpen"
      :folder-key="folderKey"
      :media="selectedMedia"
      @saved="onMassMoved"
    />

    <!-- Mass edit dialog -->
    <MassEditDlg
      v-model:open="isMassEditOpen"
      :media="selectedMedia"
      @saved="onMassEdited"
    />

    <!-- Media editor dialog -->
    <MediaEditorDlg
      v-model:open="isEditorOpen"
      :media-key="editorMediaKey"
      :all-media="media"
      :initial-tab="editorInitialTab"
      @saved="onEditorSaved"
      @navigate="onEditorNavigate"
    />
  </div>
</template>
