<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { FolderTitle } from '@/vms/FolderTitle';
import type { MediaThumbnail } from '@/vms/MediaThumbnail';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Label } from '@ui/label';
import { RadioGroup, RadioGroupItem } from '@ui/radio-group';
import { Button } from '@ui/button';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@ui/command';
import { Popover, PopoverContent, PopoverTrigger } from '@ui/popover';
import Loading from '@/components/utils/Loading.vue';
import { ChevronDown, Check } from 'lucide-vue-next';

interface Props {
  folderKey: string;
  media: MediaThumbnail[];
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
  if (newVal) {
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
  return target.prefix ? `${target.prefix}${target.caption}` : target.caption;
});

const filteredTargets = computed(() => {
  const query = searchQuery.value.toLowerCase();
  return targets.value.filter(t => {
    if (!t.selectable) return false;
    if (!query) return true;
    const fullPath = t.prefix ? `${t.prefix}${t.caption}` : t.caption;
    return fullPath.toLowerCase().includes(query);
  });
});

async function loadData() {
  targetKey.value = null;
  targetMode.value = 'folder';
  searchQuery.value = '';

  await showLoading(
    async () => {
      const tree = await api.folders.getTree();
      targets.value = flattenFolders(tree, props.folderKey);

      // Check if current folder is not at root level
      const currentFolder = targets.value.find(x => x.key === props.folderKey);
      canMoveToRoot.value = currentFolder ? currentFolder.prefix !== '' : false;
    },
    'Failed to load folders'
  );
}

function flattenFolders(folders: FolderTitle[], currentFolderKey: string, prefix: string = ''): FlatFolder[] {
  const result: FlatFolder[] = [];
  for (const folder of folders) {
    const isCurrentFolder = folder.key === currentFolderKey;
    result.push({
      key: folder.key,
      caption: folder.caption,
      prefix: prefix,
      selectable: !isCurrentFolder
    });
    if (folder.subfolders && folder.subfolders.length > 0) {
      result.push(...flattenFolders(folder.subfolders, currentFolderKey, prefix + folder.caption + ' / '));
    }
  }
  return result;
}

async function save() {
  if (!canSave.value) return;

  await showSaving(
    async () => {
      await api.media.massMove({
        keys: props.media.map(x => x.key),
        folderKey: targetMode.value === 'root' ? null : targetKey.value!
      });
      emit('saved', true);
      isOpen.value = false;
    },
    'Failed to move media'
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
        <form @submit.prevent="save">
          <DialogHeader>
            <DialogTitle>Move {{ media.length }} media file(s)</DialogTitle>
          </DialogHeader>

          <div class="space-y-4 py-4">
            <div class="space-y-2">
              <Label>Move to</Label>

              <RadioGroup v-if="canMoveToRoot" v-model="targetMode" class="space-y-1">
                <div class="flex items-center space-x-2">
                  <RadioGroupItem id="target-root" value="root" />
                  <label for="target-root" class="text-sm cursor-pointer">Gallery root</label>
                </div>
                <div class="flex items-center space-x-2">
                  <RadioGroupItem id="target-folder" value="folder" />
                  <label for="target-folder" class="text-sm cursor-pointer">Other folder</label>
                </div>
              </RadioGroup>

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
                    <ChevronDown class="ml-2 h-4 w-4 shrink-0 opacity-50" />
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
                          <Check
                            class="mr-2 h-4 w-4"
                            :class="targetKey === target.key ? 'opacity-100' : 'opacity-0'"
                          />
                          <span v-if="target.prefix" class="text-muted-foreground">{{ target.prefix }}</span>
                          <span>{{ target.caption }}</span>
                        </CommandItem>
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" @click="cancel">Cancel</Button>
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
