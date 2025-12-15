<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { FolderTitle } from '@/vms/FolderTitle';
import Loading from '@/components/utils/Loading.vue';
import ConfirmationDlg from '@/components/dialogs/ConfirmationDlg.vue';
import FolderEditorDlg from '@/components/dialogs/FolderEditorDlg.vue';
import FolderMoveDlg from '@/components/dialogs/FolderMoveDlg.vue';
import { Button } from '@ui/button';
import { Alert, AlertDescription } from '@ui/alert';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@ui/table';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from '@ui/dropdown-menu';
import { ContextMenu, ContextMenuContent, ContextMenuItem, ContextMenuSeparator, ContextMenuTrigger } from '@ui/context-menu';

const router = useRouter();
const api = useApi();
const toast = useToast();
const { asyncState, showProgress, showSaving } = useAsyncState();

const folders = ref<FolderTitle[]>([]);
const isEditorOpen = ref(false);
const isMoveOpen = ref(false);
const isConfirmOpen = ref(false);
const editingFolder = ref<FolderTitle | null>(null);
const editingParent = ref<FolderTitle | null>(null);
const confirmText = ref('');
const confirmCallback = ref<(() => void) | null>(null);
const dropdownMenuOpen = ref<{ [key: string]: boolean }>({});

onMounted(async () => {
  await load(true);
});

const flatFolders = computed(() => {
  const result: FolderTitle[] = [];
  flatten(folders.value);
  return result;

  function flatten(list: FolderTitle[]) {
    for (const item of list) {
      result.push(item);
      if (item.subfolders && item.subfolders.length) {
        flatten(item.subfolders);
      }
    }
  }
});

async function load(showPreloader: boolean = false) {
  await showProgress(
    showPreloader ? 'isLoading' : 'isWorking',
    async () => {
      folders.value = loadCollapsedState(await api.folders.getTree());
    },
    'Failed to load folders tree!'
  );
}

function create(parent: FolderTitle | null) {
  editingFolder.value = null;
  editingParent.value = parent;
  isEditorOpen.value = true;
}

function edit(folder: FolderTitle) {
  editingFolder.value = folder;
  editingParent.value = null;
  isEditorOpen.value = true;
}

function move(folder: FolderTitle) {
  editingFolder.value = folder;
  isMoveOpen.value = true;
}

async function remove(folder: FolderTitle) {
  const hint = `Are you sure you want to remove folder "<b>${folder.caption}</b>", with all subfolders and media inside?<br/><br/>This operation cannot be undone!`;

  confirmText.value = hint;
  confirmCallback.value = async () => {
    await showSaving(
      async () => {
        await api.folders.remove(folder.key);
        await load();
        toast.success('Folder removed');
      },
      'Failed to remove folder'
    );
  };
  isConfirmOpen.value = true;
}

function onEditorSaved(result: boolean) {
  if (result) {
    load();
  }
}

function onMoveSaved(result: boolean) {
  if (result) {
    load();
  }
}

function onConfirmed(result: boolean) {
  if (result && confirmCallback.value) {
    confirmCallback.value();
  }
  confirmCallback.value = null;
}

function viewFolder(folder: FolderTitle) {
  router.push(`/folders/${folder.key}`);
}

function externalLink(folder: FolderTitle) {
  window.open(folder.path, '_blank');
}

function toggleCollapsedState(folder: FolderTitle) {
  if (!folder.subfolders?.length) return;

  folder.collapsed = !folder.collapsed;
  applyCollapsedState(folder, folder.collapsed);

  withLocalState(state => {
    if (folder.collapsed) {
      state[folder.key] = true;
    } else {
      delete state[folder.key];
    }
  });
}

function applyCollapsedState(folder: FolderTitle, hidden: boolean) {
  for (const sub of folder.subfolders || []) {
    sub.hidden = hidden;
    applyCollapsedState(sub, hidden || sub.collapsed);
  }
}

function loadCollapsedState(fs: FolderTitle[]): FolderTitle[] {
  withLocalState(state => {
    for (const f of fs) {
      applyLoadedState(f, state, false);
    }
  });

  return fs;
}

function applyLoadedState(folder: FolderTitle, state: Record<string, boolean>, collapsed: boolean) {
  folder.collapsed = folder.key in state;
  folder.hidden = collapsed;

  for (const sub of folder.subfolders || []) {
    applyLoadedState(sub, state, collapsed || folder.collapsed);
  }
}

function withLocalState(fx: (state: Record<string, boolean>) => void) {
  try {
    const ls = window.localStorage;
    const key = 'isotope-folder-collapse';
    const stateStr = ls.getItem(key);
    const state: Record<string, boolean> = stateStr ? JSON.parse(stateStr) : {};
    fx(state);
    ls.setItem(key, JSON.stringify(state));
  } catch (ex) {
    console.error('Local storage operation failed!', ex);
  }
}
</script>

<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-3xl font-bold">Folders</h1>
      <Button @click="create(null)" size="sm">
        <span class="fa fa-plus"></span>
        <span>Create top-level folder</span>
      </Button>
    </div>

    <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
      <div v-if="folders.length === 0">
        <Alert>
          <AlertDescription>
            There are no folders yet. Please create a folder to upload media.
          </AlertDescription>
        </Alert>
      </div>

      <div v-else class="border rounded-md bg-card">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead class="w-full">Caption</TableHead>
              <TableHead class="w-px whitespace-nowrap">Slug</TableHead>
              <TableHead class="w-px whitespace-nowrap text-right">Media</TableHead>
              <TableHead class="w-px"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <template v-for="folder in flatFolders" :key="folder.key">
              <ContextMenu v-if="!folder.hidden">
                <ContextMenuTrigger as-child>
                <TableRow class="cursor-pointer">
                  <TableCell :style="{ paddingLeft: `${(folder.depth + 1) * 1.5}rem` }">
                    <div class="flex items-center gap-2">
                      <img
                        v-if="folder.thumbnailPath"
                        :src="folder.thumbnailPath"
                        class="w-8 h-8 object-cover rounded"
                        :alt="folder.caption"
                      />
                      <span v-else class="fa fa-folder-o w-8 text-center text-muted-foreground"></span>

                      <a
                        href="#"
                        @click.prevent="viewFolder(folder)"
                        class="font-medium hover:underline"
                      >
                        {{ folder.caption }}
                      </a>

                      <button
                        v-if="folder.subfolders && folder.subfolders.length"
                        @click.stop="toggleCollapsedState(folder)"
                        class="text-muted-foreground hover:text-foreground"
                        :title="folder.collapsed ? `Show ${folder.subfolders.length} subfolder(s)` : 'Collapse subfolders'"
                      >
                        <span v-if="folder.collapsed" class="fa fa-chevron-right"></span>
                        <span v-else class="fa fa-chevron-down"></span>
                      </button>
                    </div>
                  </TableCell>
                  <TableCell class="whitespace-nowrap">
                    <code class="text-xs text-muted-foreground bg-muted px-1.5 py-0.5 rounded">{{ folder.slug }}</code>
                  </TableCell>
                  <TableCell class="whitespace-nowrap text-right text-sm text-muted-foreground">
                    <span v-if="folder.mediaCount > 0">{{ folder.mediaCount }}</span>
                    <span v-else>-</span>
                  </TableCell>
                  <TableCell>
                    <DropdownMenu v-model:open="dropdownMenuOpen[folder.key]">
                      <DropdownMenuTrigger as-child>
                        <Button variant="ghost" size="icon-sm" @click.stop>
                          <span class="fa fa-ellipsis-v"></span>
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem @click="create(folder)">
                          <span class="fa fa-fw fa-plus mr-2"></span>
                          Create subfolder
                        </DropdownMenuItem>
                        <DropdownMenuItem @click="edit(folder)">
                          <span class="fa fa-fw fa-edit mr-2"></span>
                          Edit
                        </DropdownMenuItem>
                        <DropdownMenuItem @click="move(folder)">
                          <span class="fa fa-fw fa-arrows mr-2"></span>
                          Move
                        </DropdownMenuItem>
                        <DropdownMenuItem @click="externalLink(folder)">
                          <span class="fa fa-fw fa-share mr-2"></span>
                          View
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem @click="remove(folder)" class="text-destructive">
                          <span class="fa fa-fw fa-remove mr-2"></span>
                          Remove
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              </ContextMenuTrigger>
              <ContextMenuContent>
                <ContextMenuItem @click="create(folder)">
                  <span class="fa fa-fw fa-plus mr-2"></span>
                  Create subfolder
                </ContextMenuItem>
                <ContextMenuItem @click="edit(folder)">
                  <span class="fa fa-fw fa-edit mr-2"></span>
                  Edit
                </ContextMenuItem>
                <ContextMenuItem @click="move(folder)">
                  <span class="fa fa-fw fa-arrows mr-2"></span>
                  Move
                </ContextMenuItem>
                <ContextMenuItem @click="externalLink(folder)">
                  <span class="fa fa-fw fa-share mr-2"></span>
                  View
                </ContextMenuItem>
                <ContextMenuSeparator />
                <ContextMenuItem @click="remove(folder)" class="text-destructive">
                  <span class="fa fa-fw fa-remove mr-2"></span>
                  Remove
                </ContextMenuItem>
              </ContextMenuContent>
              </ContextMenu>
            </template>
          </TableBody>
        </Table>
      </div>
    </Loading>

    <FolderEditorDlg
      v-model:open="isEditorOpen"
      :folder="editingFolder"
      :parent="editingParent"
      @saved="onEditorSaved"
    />
    <FolderMoveDlg
      v-model:open="isMoveOpen"
      :folder="editingFolder"
      @saved="onMoveSaved"
    />
    <ConfirmationDlg v-model:open="isConfirmOpen" :text="confirmText" @confirmed="onConfirmed" />
  </div>
</template>
