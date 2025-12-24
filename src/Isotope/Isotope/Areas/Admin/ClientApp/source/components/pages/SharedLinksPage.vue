<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { SharedLinkDetails } from '@/vms/SharedLinkDetails';
import { DateHelper } from '@common/source/utils/DateHelper';
import Loading from '@/components/utils/Loading.vue';
import ConfirmationDlg from '@/components/dialogs/ConfirmationDlg.vue';
import SharedLinkEditorDlg from '@/components/dialogs/SharedLinkEditorDlg.vue';
import { Button } from '@ui/button';
import { Badge } from '@ui/badge';
import { Alert, AlertDescription } from '@ui/alert';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@ui/table';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from '@ui/dropdown-menu';
import { ContextMenu, ContextMenuContent, ContextMenuItem, ContextMenuSeparator, ContextMenuTrigger } from '@ui/context-menu';
import { MoreVertical, Pencil, ExternalLink, Trash2 } from 'lucide-vue-next';

const api = useApi();
const toast = useToast();
const { asyncState, showLoading, showProgress } = useAsyncState();

const links = ref<SharedLinkDetails[]>([]);
const isEditorOpen = ref(false);
const isConfirmOpen = ref(false);
const editingLink = ref<SharedLinkDetails | null>(null);
const confirmText = ref('');
const confirmCallback = ref<(() => void) | null>(null);
const dropdownMenuOpen = ref<{ [key: string]: boolean }>({});

onMounted(async () => {
  await load(true);
});

async function load(showPreloader: boolean = false) {
  await showProgress(
    showPreloader ? 'isLoading' : 'isWorking',
    async () => {
      links.value = await api.sharedLinks.getList();
    },
    'Failed to load links list!'
  );
}

async function remove(link: SharedLinkDetails) {
  const hint = 'Are you sure you want to remove this shared link?<br/><br/>This operation cannot be undone.';

  confirmText.value = hint;
  confirmCallback.value = async () => {
    await api.sharedLinks.remove(link.key);
    await load();
    toast.success('Shared link removed.');
  };
  isConfirmOpen.value = true;
}

function edit(link: SharedLinkDetails) {
  editingLink.value = link;
  isEditorOpen.value = true;
}

function onEdited(result: boolean) {
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

function externalLink(link: SharedLinkDetails) {
  window.open('/?sh=' + link.key, '_blank');
}

function formatDate(d: string) {
  return DateHelper.formatFull(d);
}

</script>

<template>
  <div class="p-6">
    <div class="mb-6">
      <h1 class="text-3xl font-bold">Shared Links</h1>
    </div>

    <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
      <div v-if="links.length === 0">
        <Alert>
          <AlertDescription>
            There are no shared links yet. To create one, use the "share" icon in the gallery.
          </AlertDescription>
        </Alert>
      </div>

      <div v-else class="border rounded-md bg-card">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead class="w-full">Caption</TableHead>
              <TableHead class="w-px whitespace-nowrap">Folder</TableHead>
              <TableHead class="w-px whitespace-nowrap">Created</TableHead>
              <TableHead class="w-px whitespace-nowrap text-right">Tags</TableHead>
              <TableHead class="w-px"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <ContextMenu v-for="link in links" :key="link.key">
              <ContextMenuTrigger as-child>
                <TableRow class="cursor-pointer">
                  <TableCell class="font-medium">{{ link.caption || '-' }}</TableCell>
                  <TableCell class="whitespace-nowrap text-sm text-muted-foreground">{{ link.folderCaption }}</TableCell>
                  <TableCell class="whitespace-nowrap text-sm text-muted-foreground">{{ formatDate(link.creationDate) }}</TableCell>
                  <TableCell class="whitespace-nowrap text-right text-sm text-muted-foreground">
                    <span v-if="link.tagCount > 0">{{ link.tagCount }}</span>
                    <span v-else>-</span>
                  </TableCell>
                  <TableCell>
                    <DropdownMenu v-model:open="dropdownMenuOpen[link.key]">
                      <DropdownMenuTrigger as-child>
                        <Button variant="ghost" size="icon-sm" @click.stop>
                          <MoreVertical class="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem @click="edit(link)">
                          <Pencil class="h-4 w-4 mr-2" />
                          Edit / copy
                        </DropdownMenuItem>
                        <DropdownMenuItem @click="externalLink(link)">
                          <ExternalLink class="h-4 w-4 mr-2" />
                          View
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem @click="remove(link)" class="text-destructive">
                          <Trash2 class="h-4 w-4 mr-2" />
                          Remove
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              </ContextMenuTrigger>
              <ContextMenuContent>
                <ContextMenuItem @click="edit(link)">
                  <Pencil class="h-4 w-4 mr-2" />
                  Edit / copy
                </ContextMenuItem>
                <ContextMenuItem @click="externalLink(link)">
                  <ExternalLink class="h-4 w-4 mr-2" />
                  View
                </ContextMenuItem>
                <ContextMenuSeparator />
                <ContextMenuItem @click="remove(link)" class="text-destructive">
                  <Trash2 class="h-4 w-4 mr-2" />
                  Remove
                </ContextMenuItem>
              </ContextMenuContent>
            </ContextMenu>
          </TableBody>
        </Table>
      </div>
    </Loading>

    <SharedLinkEditorDlg v-model:open="isEditorOpen" :link="editingLink" @updated="onEdited" />
    <ConfirmationDlg v-model:open="isConfirmOpen" :text="confirmText" @confirmed="onConfirmed" />
  </div>
</template>
