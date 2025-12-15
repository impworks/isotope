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
const captionInput = ref<HTMLInputElement | null>(null);
const thumbnailPath = ref<string>('');
const isThumbPickerOpen = ref(false);
const thumbs = ref<MediaThumbnail[]>([]);
const isLoadingThumbs = ref(false);

watch(() => isOpen.value, async (newVal) => {
  if (newVal) {
    await loadData();
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

      nextTick(() => {
        captionInput.value?.focus();
      });
    },
    'Failed to load data'
  );
}

async function save() {
  if (!canSave.value || !value.value) return;

  await showSaving(
    async () => {
      if (isNew.value) {
        await api.folders.create(props.parent?.key || null, value.value!);
        toast.success('Folder created');

        if (createMore.value) {
          value.value = { key: '', caption: '', slug: '', tags: [], thumbnailKey: '' };
          result.value = true;
          nextTick(() => captionInput.value?.focus());
          return;
        }
      } else {
        await api.folders.update(props.folder!.key, value.value!);
        toast.success('Folder updated');
      }

      emit('saved', true);
      isOpen.value = false;
    },
    isNew.value ? 'Failed to create folder' : 'Failed to update folder'
  );
}

function cancel() {
  emit('saved', result.value);
  isOpen.value = false;
}

async function openThumbPicker() {
  if (isThumbPickerOpen.value || !props.folder) return;

  isThumbPickerOpen.value = true;
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

function closeThumbPicker() {
  isThumbPickerOpen.value = false;
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
              <Input :value="props.parent.caption" readonly class="bg-muted" />
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
                <Popover v-model:open="isThumbPickerOpen">
                  <PopoverTrigger as-child>
                    <button
                      type="button"
                      @click="openThumbPicker"
                      class="w-24 h-24 border rounded bg-muted cursor-pointer hover:border-primary transition-colors bg-cover bg-center"
                      :style="thumbnailPath ? { backgroundImage: `url(${thumbnailPath})` } : {}"
                      title="Pick thumbnail..."
                    >
                      <span v-if="!thumbnailPath" class="fa fa-image text-muted-foreground text-2xl"></span>
                    </button>
                  </PopoverTrigger>
                  <PopoverContent class="w-[420px]" align="start" side="bottom">
                    <div class="space-y-2">
                      <div class="flex items-center justify-between">
                        <h4 class="font-medium">Folder thumbnail</h4>
                        <Button
                          v-if="value.thumbnailKey"
                          type="button"
                          variant="ghost"
                          size="icon-sm"
                          @click="pickThumb(null)"
                          title="Remove thumbnail"
                        >
                          <span class="fa fa-trash-o"></span>
                        </Button>
                      </div>
                      <Loading :is-loading="isLoadingThumbs">
                        <div v-if="thumbs.length > 0" class="max-h-[300px] overflow-y-auto pr-2">
                          <div class="grid grid-cols-4 gap-2">
                            <button
                              v-for="thumb in thumbs"
                              :key="thumb.key"
                              type="button"
                              @click="pickThumb(thumb)"
                              class="w-24 h-24 bg-cover bg-center border rounded hover:border-primary transition-colors cursor-pointer"
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
              <Checkbox v-model:checked="createMore" />
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
