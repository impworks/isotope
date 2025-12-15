<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { FolderTitle } from '@/vms/FolderTitle';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Input } from '@ui/input';
import { Label } from '@ui/label';
import { RadioGroup, RadioGroupItem } from '@ui/radio-group';
import { Button } from '@ui/button';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@ui/command';
import { Popover, PopoverContent, PopoverTrigger } from '@ui/popover';
import Loading from '@/components/utils/Loading.vue';

interface Props {
  folder?: FolderTitle | null;
}

interface FlatFolder {
  key: string;
  caption: string;
  prefix: string;
  selectable: boolean;
}

const props = defineProps<Props>();
const isOpen = defineModel<boolean>('open', { default: false });
const emit = defineEmits<{
  saved: [value: boolean];
}>();

const api = useApi();
const toast = useToast();
const { asyncState, showLoading, showSaving } = useAsyncState();

const targets = ref<FlatFolder[]>([]);
const canMoveToRoot = ref(false);
const targetMode = ref<'root' | 'folder'>('folder');
const targetKey = ref<string | null>(null);
const comboboxOpen = ref(false);
const searchQuery = ref('');

watch(() => isOpen.value, async (newVal) => {
  if (newVal && props.folder) {
    await loadData();
  }
});

const canSave = computed(() => {
  return !asyncState.isSaving
    && !asyncState.isLoading
    && ((canMoveToRoot.value && targetMode.value === 'root') || !!targetKey.value);
});

const selectedTargetLabel = computed(() => {
  const target = targets.value.find(t => t.key === targetKey.value);
  if (!target) return 'Select folder...';
  return target.prefix ? `${target.prefix} / ${target.caption}` : target.caption;
});

const filteredTargets = computed(() => {
  const query = searchQuery.value.toLowerCase();
  return targets.value.filter(t => {
    if (!t.selectable) return false;
    if (!query) return true;
    const fullPath = t.prefix ? `${t.prefix} / ${t.caption}` : t.caption;
    return fullPath.toLowerCase().includes(query);
  });
});

async function loadData() {
  if (!props.folder) return;

  await showLoading(
    async () => {
      const allFolders = await api.folders.getTree();
      targets.value = flattenFolders(allFolders, props.folder!.key);

      // Check if folder can be moved to root (i.e., it's not already at root level)
      const currentFolder = findFolder(targets.value, props.folder!.key);
      canMoveToRoot.value = currentFolder ? currentFolder.prefix !== '' : false;

      // Reset selection
      targetMode.value = 'folder';
      targetKey.value = targets.value.find(x => x.selectable)?.key || null;
    },
    'Failed to load data'
  );
}

function flattenFolders(folders: FolderTitle[], excludeKey: string, prefix: string = ''): FlatFolder[] {
  const result: FlatFolder[] = [];

  for (const folder of folders) {
    const isExcluded = folder.key === excludeKey || isDescendantOf(folder, excludeKey);
    const currentPrefix = prefix;
    const newPrefix = prefix + (prefix ? ' / ' : '');

    result.push({
      key: folder.key,
      caption: folder.caption,
      prefix: currentPrefix,
      selectable: !isExcluded
    });

    if (folder.subfolders && folder.subfolders.length > 0) {
      result.push(...flattenFolders(folder.subfolders, excludeKey, newPrefix + folder.caption));
    }
  }

  return result;
}

function isDescendantOf(folder: FolderTitle, ancestorKey: string): boolean {
  if (!folder.subfolders) return false;

  for (const sub of folder.subfolders) {
    if (sub.key === ancestorKey || isDescendantOf(sub, ancestorKey)) {
      return true;
    }
  }

  return false;
}

function findFolder(folders: FlatFolder[], key: string): FlatFolder | null {
  return folders.find(f => f.key === key) || null;
}

async function save() {
  if (!canSave.value || !props.folder) return;

  await showSaving(
    async () => {
      const target = targetMode.value === 'root' ? null : targetKey.value;
      await api.folders.move(props.folder!.key, target);
      toast.success('Folder moved');
      emit('saved', true);
      isOpen.value = false;
    },
    'Failed to move folder'
  );
}

function cancel() {
  emit('saved', false);
  isOpen.value = false;
}
</script>

<template>
  <Dialog v-model:open="isOpen">
    <DialogContent class="sm:max-w-[500px]">
      <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <form @submit.prevent="save" v-if="folder">
          <DialogHeader>
            <DialogTitle>Move folder</DialogTitle>
          </DialogHeader>

          <div class="space-y-4 py-4">
            <div class="space-y-2">
              <Label>Folder</Label>
              <Input :value="props.folder?.caption" readonly class="bg-muted" />
            </div>

            <div class="space-y-3">
              <Label>Move to</Label>

              <RadioGroup v-if="canMoveToRoot" v-model="targetMode" class="space-y-3">
                <div class="flex items-center gap-2">
                  <RadioGroupItem value="root" id="target-root" />
                  <label for="target-root" class="text-sm cursor-pointer">Gallery root</label>
                </div>
                <div class="flex items-center gap-2">
                  <RadioGroupItem value="folder" id="target-folder" />
                  <label for="target-folder" class="text-sm cursor-pointer">Other folder</label>
                </div>
              </RadioGroup>

              <div class="space-y-2" v-if="targetMode === 'folder'">
                <Label for="target-select">Target Folder</Label>
                <Popover v-model:open="comboboxOpen">
                  <PopoverTrigger as-child>
                    <Button
                      variant="outline"
                      role="combobox"
                      :aria-expanded="comboboxOpen"
                      class="w-full justify-between"
                      :disabled="asyncState.isSaving || targetMode === 'root'"
                    >
                      <span class="truncate">{{ selectedTargetLabel }}</span>
                      <span class="fa fa-chevron-down ml-2 h-4 w-4 shrink-0 opacity-50"></span>
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent class="w-[400px] p-0" align="start">
                    <Command v-model:search-term="searchQuery">
                      <CommandInput placeholder="Search folders..." />
                      <CommandList>
                        <CommandEmpty>No folders found.</CommandEmpty>
                        <CommandGroup>
                          <CommandItem
                            v-for="target in filteredTargets"
                            :key="target.key"
                            :value="target.key"
                            @select="() => { targetKey = target.key; comboboxOpen = false; }"
                            class="cursor-pointer"
                          >
                            <span
                              class="fa fa-check mr-2 h-4 w-4"
                              :class="targetKey === target.key ? 'opacity-100' : 'opacity-0'"
                            ></span>
                            <span v-if="target.prefix" class="text-muted-foreground">{{ target.prefix }} / </span>
                            <span>{{ target.caption }}</span>
                          </CommandItem>
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
                <p v-if="targets.filter(x => x.selectable).length === 0" class="text-sm text-muted-foreground">
                  There are no other folders.
                </p>
              </div>
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="secondary" @click="cancel">Cancel</Button>
            <Button type="submit" :disabled="!canSave">
              <span v-if="asyncState.isSaving">Moving...</span>
              <span v-else>Move</span>
            </Button>
          </DialogFooter>
        </form>
      </Loading>
    </DialogContent>
  </Dialog>
</template>
