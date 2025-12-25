<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { Tag } from '@/vms/Tag';
import { TagType } from '@common/source/vms/TagType';
import Loading from '@/components/utils/Loading.vue';
import ConfirmationDlg from '@/components/dialogs/ConfirmationDlg.vue';
import TagEditorDlg from '@/components/dialogs/TagEditorDlg.vue';
import { Button } from '@ui/button';
import { Badge } from '@ui/badge';
import { Alert, AlertDescription } from '@ui/alert';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@ui/table';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from '@ui/dropdown-menu';
import { ContextMenu, ContextMenuContent, ContextMenuItem, ContextMenuSeparator, ContextMenuTrigger } from '@ui/context-menu';
import { Plus, MoreVertical, Pencil, ExternalLink, Trash2 } from 'lucide-vue-next';

const api = useApi();
const toast = useToast();
const { asyncState, showLoading, showProgress } = useAsyncState();

const tags = ref<Tag[]>([]);
const isEditorOpen = ref(false);
const isConfirmOpen = ref(false);
const editingTag = ref<Tag | null>(null);
const confirmText = ref('');
const confirmCallback = ref<(() => void) | null>(null);
const dropdownMenuOpen = ref<{ [key: number]: boolean }>({});

onMounted(async () => {
  await load(true);
});

async function load(showPreloader: boolean = false) {
  await showProgress(
    showPreloader ? 'isLoading' : 'isWorking',
    async () => {
      tags.value = await api.tags.getList();
    },
    'Failed to load tags list!'
  );
}

async function remove(tag: Tag) {
  const hint = `Are you sure you want to remove the tag "<b>${tag.caption}</b>"?<br/><br/>This operation cannot be undone.`;

  confirmText.value = hint;
  confirmCallback.value = async () => {
    await api.tags.remove(tag.id);
    await load();
    toast.success('Tag removed.');
  };
  isConfirmOpen.value = true;
}

function create() {
  editingTag.value = null;
  isEditorOpen.value = true;
}

function edit(tag: Tag) {
  editingTag.value = tag;
  isEditorOpen.value = true;
}

function onEditorSaved(result: boolean) {
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

function externalLink(tag: Tag) {
  window.open('/?tags=' + tag.id, '_blank');
}

function getTagTypeColor(type: TagType) {
  switch (type) {
    case TagType.Person:
      return { color: 'bg-blue-500', label: 'Person' };
    case TagType.Location:
      return { color: 'bg-green-500', label: 'Location' };
    case TagType.Pet:
      return { color: 'bg-orange-500', label: 'Pet' };
    case TagType.Custom:
      return { color: 'bg-red-500', label: 'Other' };
    default:
      return { color: 'bg-gray-500', label: 'Unknown' };
  }
}

</script>

<template>
  <div class="flex flex-col h-full">
    <div class="sticky top-0 z-10 bg-background border-b px-6 py-4">
      <div class="flex items-center justify-between">
        <h1 class="text-3xl font-bold">Tags</h1>
        <Button @click="create" size="sm">
          <Plus class="h-4 w-4" />
          <span>Create tag</span>
        </Button>
      </div>
    </div>

    <div class="flex-1 overflow-auto p-6">
    <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
      <div v-if="tags.length === 0">
        <Alert>
          <AlertDescription>
            There are no tags yet.
          </AlertDescription>
        </Alert>
      </div>

      <div v-else class="border rounded-md bg-card">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead class="w-full">Name</TableHead>
              <TableHead class="w-px whitespace-nowrap">Type</TableHead>
              <TableHead class="w-px whitespace-nowrap text-right">Usages</TableHead>
              <TableHead class="w-px"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <ContextMenu v-for="tag in tags" :key="tag.id">
              <ContextMenuTrigger as-child>
                <TableRow class="cursor-pointer">
                  <TableCell class="font-medium">{{ tag.caption }}</TableCell>
                  <TableCell class="whitespace-nowrap">
                    <div class="flex items-center gap-2">
                      <div :class="['w-2 h-2 rounded-full', getTagTypeColor(tag.type).color]"></div>
                      <span class="text-sm text-muted-foreground">{{ getTagTypeColor(tag.type).label }}</span>
                    </div>
                  </TableCell>
                  <TableCell class="whitespace-nowrap text-right text-sm text-muted-foreground">
                    <span v-if="tag.count > 0">{{ tag.count }}</span>
                    <span v-else>-</span>
                  </TableCell>
                  <TableCell>
                    <DropdownMenu v-model:open="dropdownMenuOpen[tag.id]">
                      <DropdownMenuTrigger as-child>
                        <Button variant="ghost" size="icon-sm" @click.stop>
                          <MoreVertical class="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem @click="edit(tag)">
                          <Pencil class="h-4 w-4 mr-2" />
                          Edit tag
                        </DropdownMenuItem>
                        <DropdownMenuItem @click="externalLink(tag)">
                          <ExternalLink class="h-4 w-4 mr-2" />
                          View
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem @click="remove(tag)" class="text-destructive">
                          <Trash2 class="h-4 w-4 mr-2" />
                          Remove
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              </ContextMenuTrigger>
              <ContextMenuContent>
                <ContextMenuItem @click="edit(tag)">
                  <Pencil class="h-4 w-4 mr-2" />
                  Edit tag
                </ContextMenuItem>
                <ContextMenuItem @click="externalLink(tag)">
                  <ExternalLink class="h-4 w-4 mr-2" />
                  View
                </ContextMenuItem>
                <ContextMenuSeparator />
                <ContextMenuItem @click="remove(tag)" class="text-destructive">
                  <Trash2 class="h-4 w-4 mr-2" />
                  Remove
                </ContextMenuItem>
              </ContextMenuContent>
            </ContextMenu>
          </TableBody>
        </Table>
      </div>
    </Loading>

    <TagEditorDlg v-model:open="isEditorOpen" :tag="editingTag" @saved="onEditorSaved" />
    <ConfirmationDlg v-model:open="isConfirmOpen" :text="confirmText" @confirmed="onConfirmed" />
    </div>
  </div>
</template>
