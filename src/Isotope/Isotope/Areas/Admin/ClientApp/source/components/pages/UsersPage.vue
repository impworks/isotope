<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { User } from '@/vms/User';
import Loading from '@/components/utils/Loading.vue';
import ConfirmationDlg from '@/components/dialogs/ConfirmationDlg.vue';
import UserCreatorDlg from '@/components/dialogs/UserCreatorDlg.vue';
import UserEditorDlg from '@/components/dialogs/UserEditorDlg.vue';
import UserPasswordDlg from '@/components/dialogs/UserPasswordDlg.vue';
import { Button } from '@ui/button';
import { Badge } from '@ui/badge';
import { Alert, AlertDescription } from '@ui/alert';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@ui/table';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from '@ui/dropdown-menu';
import { ContextMenu, ContextMenuContent, ContextMenuItem, ContextMenuSeparator, ContextMenuTrigger } from '@ui/context-menu';
import { Plus, MoreVertical, Pencil, Lock, Trash2 } from 'lucide-vue-next';

const api = useApi();
const toast = useToast();
const { asyncState, showLoading, showProgress, showSaving } = useAsyncState();

const users = ref<User[]>([]);
const isCreatorOpen = ref(false);
const isEditorOpen = ref(false);
const isPasswordOpen = ref(false);
const isConfirmOpen = ref(false);
const editingUser = ref<User | null>(null);
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
      users.value = await api.users.getList();
    },
    'Failed to load users list!'
  );
}

async function remove(user: User) {
  const hint = `Are you sure you want to remove the user "<b>${user.userName}</b>"?<br/><br/>This operation cannot be undone.`;

  confirmText.value = hint;
  confirmCallback.value = async () => {
    await showSaving(
      async () => {
        await api.users.remove(user.id);
        toast.success('User removed.');
      },
      'Failed to remove the user'
    );
    await load();
  };
  isConfirmOpen.value = true;
}

function create() {
  isCreatorOpen.value = true;
}

function edit(user: User) {
  editingUser.value = user;
  isEditorOpen.value = true;
}

function setPassword(user: User) {
  editingUser.value = user;
  isPasswordOpen.value = true;
}

function onCreated(result: boolean) {
  if (result) {
    load();
  }
}

function onEdited(result: boolean) {
  if (result) {
    load();
  }
}

function onPasswordUpdated(result: boolean) {
  // Password update doesn't need reload
}

function onConfirmed(result: boolean) {
  if (result && confirmCallback.value) {
    confirmCallback.value();
  }
  confirmCallback.value = null;
}

</script>

<template>
  <div class="flex flex-col h-full">
    <div class="sticky top-0 z-10 bg-background border-b px-6 py-4">
      <div class="flex items-center justify-between">
        <h1 class="text-3xl font-bold">Users</h1>
        <Button @click="create" size="sm">
          <Plus class="h-4 w-4" />
          <span>Create user</span>
        </Button>
      </div>
    </div>

    <div class="flex-1 overflow-auto p-6">
    <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
      <div v-if="users.length === 0">
        <Alert>
          <AlertDescription>
            No users found.
          </AlertDescription>
        </Alert>
      </div>

      <div v-else class="border rounded-md bg-card">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead class="w-full">Username</TableHead>
              <TableHead class="w-px"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <ContextMenu v-for="user in users" :key="user.id">
              <ContextMenuTrigger as-child>
                <TableRow class="cursor-pointer">
                  <TableCell class="font-medium" :title="user.isAdmin ? 'Admin' : 'User'" @click="edit(user)">
                    <div class="flex items-center gap-2">
                      <div :class="['w-2 h-2 rounded-full', user.isAdmin ? 'bg-red-500' : 'bg-blue-500']"></div>
                      <span class="hover:underline">{{ user.userName }}</span>
                    </div>
                  </TableCell>
                  <TableCell>
                    <DropdownMenu v-model:open="dropdownMenuOpen[user.id]">
                      <DropdownMenuTrigger as-child>
                        <Button variant="ghost" size="icon-sm" @click.stop>
                          <MoreVertical class="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem @click="edit(user)">
                          <Pencil class="h-4 w-4 mr-2" />
                          Edit profile
                        </DropdownMenuItem>
                        <DropdownMenuItem @click="setPassword(user)">
                          <Lock class="h-4 w-4 mr-2" />
                          Change password
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem @click="remove(user)" class="text-destructive">
                          <Trash2 class="h-4 w-4 mr-2" />
                          Remove
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              </ContextMenuTrigger>
              <ContextMenuContent>
                <ContextMenuItem @click="edit(user)">
                  <Pencil class="h-4 w-4 mr-2" />
                  Edit profile
                </ContextMenuItem>
                <ContextMenuItem @click="setPassword(user)">
                  <Lock class="h-4 w-4 mr-2" />
                  Change password
                </ContextMenuItem>
                <ContextMenuSeparator />
                <ContextMenuItem @click="remove(user)" class="text-destructive">
                  <Trash2 class="h-4 w-4 mr-2" />
                  Remove
                </ContextMenuItem>
              </ContextMenuContent>
            </ContextMenu>
          </TableBody>
        </Table>
      </div>
    </Loading>

    <UserCreatorDlg v-model:open="isCreatorOpen" @created="onCreated" />
    <UserEditorDlg v-model:open="isEditorOpen" :user="editingUser" @updated="onEdited" />
    <UserPasswordDlg v-model:open="isPasswordOpen" :user="editingUser" @updated="onPasswordUpdated" />
    <ConfirmationDlg v-model:open="isConfirmOpen" :text="confirmText" @confirmed="onConfirmed" />
    </div>
  </div>
</template>
