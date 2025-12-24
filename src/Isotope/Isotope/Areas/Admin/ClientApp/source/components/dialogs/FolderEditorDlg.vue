<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { Folder } from '@/vms/Folder';
import type { FolderTitle } from '@/vms/FolderTitle';
import type { Tag } from '@/vms/Tag';
import type { MediaThumbnail } from '@/vms/MediaThumbnail';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Input } from '@ui/input';
import { Label } from '@ui/label';
import { Button } from '@ui/button';
import { Checkbox } from '@ui/checkbox';
import { Popover, PopoverContent, PopoverTrigger } from '@ui/popover';
import Loading from '@/components/utils/Loading.vue';
import TagMultiselect from '@/components/utils/TagMultiselect.vue';
import { Image, Trash2 } from 'lucide-vue-next';

interface Props {
  folder?: FolderTitle | null;
  parent?: FolderTitle | null;
}

const props = defineProps<Props>();
const isOpen = defineModel<boolean>('open', { default: false });
const emit = defineEmits<{
  saved: [value: boolean];
}>();

const api = useApi();
const toast = useToast();
const { asyncState, showLoading, showSaving } = useAsyncState();

const value = ref<Folder | null>(null);
const tags = ref<Tag[]>([]);
const createMore = ref(false);
const result = ref(false);
const captionInput = ref<InstanceType<typeof Input> | null>(null);
const thumbnailPath = ref<string>('');
const isThumbPickerOpen = ref(false);
const thumbs = ref<MediaThumbnail[]>([]);
const isLoadingThumbs = ref(false);

watch(() => isOpen.value, async (newVal) => {
  if (newVal) {
    result.value = false;
    await loadData();
  } else {
    emit('saved', result.value);
  }
});

const isNew = computed(() => !props.folder);

const canSave = computed(() => {
  return !asyncState.isSaving
    && !asyncState.isLoading
    && !!value.value?.caption
    && !!value.value?.slug;
});

async function loadData() {
  await showLoading(
    async () => {
      value.value = props.folder
        ? await api.folders.get(props.folder.key)
        : { key: '', caption: '', slug: '', tags: [], thumbnailKey: '' };

      tags.value = await api.tags.getList();

      // Load thumbnail preview if editing and has thumbnail
      if (props.folder && value.value.thumbnailKey) {
        const media = await api.media.get(value.value.thumbnailKey);
        thumbnailPath.value = media.thumbnailPath;
      } else {
        thumbnailPath.value = '';
      }
    },
    'Failed to load data'
  );
}

async function save() {
  if (!canSave.value || !value.value) return;

  const success = await showSaving(
    async () => {
      if (isNew.value) {
        await api.folders.create(props.parent?.key || null, value.value!);
        toast.success('Folder created');
      } else {
        await api.folders.update(props.folder!.key, value.value!);
        toast.success('Folder updated');
      }
    },
    isNew.value ? 'Failed to create folder' : 'Failed to update folder'
  );

  if (!success) return;

  if (isNew.value && createMore.value) {
    value.value = { key: '', caption: '', slug: '', tags: [], thumbnailKey: '' };
    result.value = true;
    nextTick(() => (captionInput.value?.$el as HTMLInputElement)?.focus());
    return;
  }

  emit('saved', true);
  isOpen.value = false;
}

function cancel() {
  isOpen.value = false;
}

async function onThumbPickerOpenChange(open: boolean) {
  isThumbPickerOpen.value = open;
  if (open && props.folder) {
    await loadThumbnails();
  }
}

async function loadThumbnails() {
  if (!props.folder) return;

  isLoadingThumbs.value = true;

  try {
    const mediaList = await api.media.getList(props.folder.key);
    const subfolders = await api.folders.getTree(props.folder.key);

    const allThumbs: MediaThumbnail[] = [...mediaList];

    // Add subfolder thumbnails
    for (const f of subfolders) {
      if (f.thumbnailPath) {
        allThumbs.push({
          key: f.thumbnailKey,
          thumbnailPath: f.thumbnailPath,
          tags: 0,
          type: 0, // Photo type
          uploadDate: ''
        });
      }
    }

    thumbs.value = allThumbs;
  } catch (error) {
    toast.error('Failed to load thumbnails');
  } finally {
    isLoadingThumbs.value = false;
  }
}

function pickThumb(thumb: MediaThumbnail | null) {
  if (thumb) {
    value.value!.thumbnailKey = thumb.key;
    thumbnailPath.value = thumb.thumbnailPath;
  } else {
    value.value!.thumbnailKey = '';
    thumbnailPath.value = '';
  }
  isThumbPickerOpen.value = false;
}

function handleKeydown(e: KeyboardEvent) {
  if (e.ctrlKey && e.key === 's') {
    e.preventDefault();
    save();
  }
}
</script>

<template>
  <Dialog v-model:open="isOpen" @keydown="handleKeydown">
    <DialogContent class="sm:max-w-[600px]">
      <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <form @submit.prevent="save" v-if="value">
          <DialogHeader>
            <DialogTitle>
              <span v-if="isNew">Create folder</span>
              <span v-else>Update folder '{{ folder?.caption }}'</span>
            </DialogTitle>
          </DialogHeader>

          <div class="space-y-4 py-4">
            <div class="space-y-2" v-if="props.parent">
              <Label>Parent</Label>
              <Input :model-value="props.parent.caption" readonly class="bg-muted" />
            </div>

            <div class="flex gap-4">
              <div class="flex-1 space-y-4">
                <div class="space-y-2">
                  <Label for="caption">Caption</Label>
                  <Input
                    id="caption"
                    ref="captionInput"
                    v-model="value.caption"
                    :disabled="asyncState.isSaving"
                    v-autofocus
                  />
                </div>

                <div class="space-y-2">
                  <Label for="slug">URL Slug</Label>
                  <Input
                    id="slug"
                    v-model="value.slug"
                    :disabled="asyncState.isSaving"
                  />
                </div>
              </div>

              <div v-if="!isNew" class="shrink-0">
                <Popover :open="isThumbPickerOpen" @update:open="onThumbPickerOpenChange">
                  <PopoverTrigger as-child>
                    <button
                      type="button"
                      class="w-24 h-24 border rounded bg-muted cursor-pointer hover:border-primary transition-colors bg-cover bg-center flex items-center justify-center"
                      :style="thumbnailPath ? { backgroundImage: `url(${thumbnailPath})` } : {}"
                      title="Pick thumbnail..."
                    >
                      <Image v-if="!thumbnailPath" class="h-8 w-8 text-muted-foreground" />
                    </button>
                  </PopoverTrigger>
                  <PopoverContent class="w-auto p-2" align="center" side="bottom">
                    <div class="space-y-2">
                      <div class="flex items-center justify-between gap-4">
                        <h4 class="font-medium text-sm">Folder thumbnail</h4>
                        <Button
                          v-if="value.thumbnailKey"
                          type="button"
                          variant="ghost"
                          size="icon-sm"
                          @click="pickThumb(null)"
                          title="Remove thumbnail"
                        >
                          <Trash2 class="h-4 w-4" />
                        </Button>
                      </div>
                      <Loading :is-loading="isLoadingThumbs">
                        <div v-if="thumbs.length > 0" class="max-h-[200px] overflow-y-auto">
                          <div class="grid grid-cols-5 gap-1">
                            <button
                              v-for="thumb in thumbs"
                              :key="thumb.key"
                              type="button"
                              @click="pickThumb(thumb)"
                              class="w-12 h-12 bg-cover bg-center rounded cursor-pointer"
                              :style="{ backgroundImage: `url(${thumb.thumbnailPath})` }"
                            ></button>
                          </div>
                        </div>
                        <div v-else class="text-sm text-muted-foreground py-4 text-center">
                          This folder is empty.
                        </div>
                      </Loading>
                    </div>
                  </PopoverContent>
                </Popover>
              </div>
            </div>

            <div class="space-y-2" v-if="tags.length > 0">
              <Label>Tags</Label>
              <TagMultiselect v-model="value.tags" :tags="tags" :disabled="asyncState.isSaving" />
            </div>
          </div>

          <DialogFooter class="flex items-center sm:justify-between">
            <label v-if="isNew" class="flex items-center gap-2 text-sm cursor-pointer" title="Keep the dialog open to create another folder after saving">
              <Checkbox v-model="createMore" />
              <span>Create one more</span>
            </label>
            <div class="flex gap-2 sm:ml-auto">
              <Button type="button" variant="secondary" @click="cancel">Cancel</Button>
              <Button type="submit" :disabled="!canSave" title="Ctrl + S">
                <span v-if="asyncState.isSaving">Saving...</span>
                <span v-else-if="isNew">Create</span>
                <span v-else>Update</span>
              </Button>
            </div>
          </DialogFooter>
        </form>
      </Loading>
    </DialogContent>
  </Dialog>
</template>
