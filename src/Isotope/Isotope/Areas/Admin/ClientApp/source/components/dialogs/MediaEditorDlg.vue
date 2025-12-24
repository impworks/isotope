<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { GlobalEvents } from 'vue-global-events';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { Media } from '@/vms/Media';
import type { MediaThumbnail } from '@/vms/MediaThumbnail';
import type { Tag } from '@/vms/Tag';
import { TagBindingType } from '@common/source/vms/TagBindingType';
import type { Rect } from '@common/source/vms/Rect';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Tabs, TabsList, TabsTrigger } from '@ui/tabs';
import { Label } from '@ui/label';
import { Button } from '@ui/button';
import { Checkbox } from '@ui/checkbox';
import Loading from '@/components/utils/Loading.vue';
import MediaDetailsEditor from '@/components/media/editors/MediaDetailsEditor.vue';
import MediaTagsEditor from '@/components/media/editors/MediaTagsEditor.vue';
import MediaThumbEditor from '@/components/media/editors/MediaThumbEditor.vue';
import TagPickerDlg from '@/components/dialogs/TagPickerDlg.vue';
import { ChevronLeft, ChevronRight, Tags, FileText, Image } from 'lucide-vue-next';

export type EditorTab = 'details' | 'tags' | 'preview';

interface Props {
  mediaKey: string;
  allMedia: MediaThumbnail[];
  initialTab?: EditorTab;
}

const props = withDefaults(defineProps<Props>(), {
  initialTab: 'details'
});

const isOpen = defineModel<boolean>('open', { default: false });
const emit = defineEmits<{
  saved: [value: boolean];
  navigate: [key: string, tab: EditorTab];
}>();

const api = useApi();
const toast = useToast();
const { asyncState, showLoading, showSaving } = useAsyncState();

const currentKey = ref<string>('');
const media = ref<Media | null>(null);
const tags = ref<Tag[]>([]);
const activeTab = ref<EditorTab>('details');
const hasChanges = ref(false);
const hasSavedChanges = ref(false);
const editNext = ref<boolean | 'indeterminate'>(false);
const pickedTagIds = ref<number[]>([]);
const tagPickerOpen = ref(false);

// Form fields
const dateValue = ref<string>('');
const descriptionValue = ref<string>('');
const selectedTagIds = ref<number[]>([]);
const cropRect = ref<Rect | null>(null);

// Child component refs
const tagsEditorRef = ref<InstanceType<typeof MediaTagsEditor> | null>(null);

watch(() => isOpen.value, async (newVal) => {
  if (newVal) {
    currentKey.value = props.mediaKey;
    activeTab.value = props.initialTab;
    hasSavedChanges.value = false;
    await loadData();
  } else {
    hasChanges.value = false;
    emit('saved', hasSavedChanges.value);
  }
});

watch(() => props.mediaKey, (newKey) => {
  if (isOpen.value && newKey !== currentKey.value) {
    currentKey.value = newKey;
    loadData();
  }
});

watch(() => props.initialTab, (newTab) => {
  if (isOpen.value && newTab) {
    activeTab.value = newTab;
  }
});

const currentIndex = computed(() => {
  return props.allMedia.findIndex(m => m.key === currentKey.value);
});

const canGoPrev = computed(() => currentIndex.value > 0);
const canGoNext = computed(() => currentIndex.value < props.allMedia.length - 1);

// Filter tags based on picked tags (for faster tagging workflow)
const filteredTags = computed(() => {
  if (pickedTagIds.value.length === 0) {
    return tags.value;
  }
  const pickedSet = new Set(pickedTagIds.value);
  return tags.value.filter(t => pickedSet.has(t.id));
});

const hasInvalidTags = computed(() => {
  return tagsEditorRef.value?.hasInvalidTags ?? false;
});

const canSave = computed(() => {
  return !asyncState.isSaving && !asyncState.isLoading && hasChanges.value && !hasInvalidTags.value;
});

async function loadData() {
  await showLoading(
    async () => {
      const [mediaData, tagsData, thumbData] = await Promise.all([
        api.media.get(currentKey.value),
        api.tags.getList(),
        api.media.getThumb(currentKey.value)
      ]);

      media.value = mediaData;
      tags.value = tagsData;
      cropRect.value = thumbData;

      // Populate form fields
      dateValue.value = mediaData.date ? mediaData.date.split('T')[0] : '';
      descriptionValue.value = mediaData.description || '';
      selectedTagIds.value = mediaData.extraTags.map(t => t.tagId);

      hasChanges.value = false;
    },
    'Failed to load media'
  );
}

function markChanged() {
  hasChanges.value = true;
}

async function save() {
  if (!canSave.value || !media.value) return;

  const success = await showSaving(
    async () => {
      const updatedMedia: Media = {
        ...media.value!,
        date: dateValue.value ? new Date(dateValue.value).toISOString() : '',
        description: descriptionValue.value,
        extraTags: selectedTagIds.value.map(id => ({
          tagId: id,
          type: TagBindingType.Custom
        }))
      };

      await api.media.update(currentKey.value, updatedMedia);

      if (cropRect.value) {
        await api.media.updateThumb(currentKey.value, cropRect.value);
      }

      hasChanges.value = false;
      hasSavedChanges.value = true;
      toast.success('Media updated');
    },
    'Failed to save media'
  );

  if (!success) return;

  // Auto navigate to next if enabled and there's a next item, otherwise close
  if (editNext.value === true && canGoNext.value) {
    navigate('next');
  } else {
    isOpen.value = false;
  }
}

function cancel() {
  isOpen.value = false;
}

function navigate(direction: 'prev' | 'next') {
  const newIndex = direction === 'prev' ? currentIndex.value - 1 : currentIndex.value + 1;
  if (newIndex >= 0 && newIndex < props.allMedia.length) {
    const newKey = props.allMedia[newIndex].key;
    currentKey.value = newKey;
    emit('navigate', newKey, activeTab.value);
    loadData();
  }
}

function openTagsConfig() {
  tagPickerOpen.value = true;
}

function onTagsPicked(tagIds: number[]) {
  pickedTagIds.value = tagIds;
}

// Global keyboard handler (vue-global-events)
function handleKeydown(e: KeyboardEvent) {
  if (!isOpen.value) return;

  // Ctrl+S: Save
  if (e.ctrlKey && e.key === 's') {
    e.preventDefault();
    save();
    return;
  }

  // Ctrl+Left/Right: Navigate
  if (e.ctrlKey && e.key === 'ArrowLeft' && canGoPrev.value) {
    e.preventDefault();
    navigate('prev');
    return;
  }

  if (e.ctrlKey && e.key === 'ArrowRight' && canGoNext.value) {
    e.preventDefault();
    navigate('next');
    return;
  }

  // Tags tab specific shortcuts
  if (activeTab.value === 'tags' && tagsEditorRef.value) {
    // Ctrl+Space: Start draw mode
    if (e.ctrlKey && e.key === ' ') {
      e.preventDefault();
      tagsEditorRef.value.startDrawMode();
      return;
    }

    // Escape: Cancel draw mode
    if (e.key === 'Escape' && tagsEditorRef.value.isDrawMode.value) {
      e.preventDefault();
      tagsEditorRef.value.cancelDrawMode();
      return;
    }
  }
}
</script>

<template>
  <GlobalEvents @keydown="handleKeydown" />

  <Dialog v-model:open="isOpen">
    <DialogContent class="sm:max-w-[700px] flex flex-col !top-[5%] !translate-y-0 max-h-[90vh]">
      <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <DialogHeader class="flex-shrink-0">
          <DialogTitle>Edit media</DialogTitle>
        </DialogHeader>

        <Tabs v-model="activeTab">
          <TabsList class="w-full grid grid-cols-3">
            <TabsTrigger value="details">
              <FileText class="h-4 w-4" />
              <span>Details</span>
            </TabsTrigger>
            <TabsTrigger value="tags">
              <Tags class="h-4 w-4" />
              <span>Tags</span>
            </TabsTrigger>
            <TabsTrigger value="preview">
              <Image class="h-4 w-4" />
              <span>Preview</span>
            </TabsTrigger>
          </TabsList>
        </Tabs>

        <div class="min-h-[350px] mt-4">
          <!-- Details Tab -->
          <MediaDetailsEditor
            v-if="activeTab === 'details'"
            v-model:date="dateValue"
            v-model:description="descriptionValue"
            @change="markChanged"
          />

          <!-- Tags Tab -->
          <MediaTagsEditor
            v-if="activeTab === 'tags' && media"
            ref="tagsEditorRef"
            :media="media"
            :tags="filteredTags"
            v-model:selected-tag-ids="selectedTagIds"
            @change="markChanged"
            @configure-tags="openTagsConfig"
          />

          <!-- Preview Tab -->
          <MediaThumbEditor
            v-if="activeTab === 'preview' && media && cropRect"
            :media="media"
            v-model:rect="cropRect"
            @change="markChanged"
          />
        </div>

        <DialogFooter class="flex-shrink-0 mt-4">
          <div class="flex items-center justify-between w-full">
            <div class="flex items-center gap-4">
              <div class="flex items-center gap-2">
                <Checkbox
                  id="edit-next"
                  v-model="editNext"
                />
                <Label for="edit-next" class="text-sm cursor-pointer">Edit next after save</Label>
              </div>
              <div class="flex items-center gap-1">
                <Button
                  variant="outline"
                  size="icon-sm"
                  :disabled="!canGoPrev"
                  @click="navigate('prev')"
                  title="Previous (Ctrl+Left)"
                >
                  <ChevronLeft class="h-4 w-4" />
                </Button>
                <span class="text-sm text-muted-foreground min-w-[60px] text-center">
                  {{ currentIndex + 1 }} / {{ allMedia.length }}
                </span>
                <Button
                  variant="outline"
                  size="icon-sm"
                  :disabled="!canGoNext"
                  @click="navigate('next')"
                  title="Next (Ctrl+Right)"
                >
                  <ChevronRight class="h-4 w-4" />
                </Button>
              </div>
            </div>
            <div class="flex items-center gap-2">
              <Button type="button" variant="outline" @click="cancel">Cancel</Button>
              <Button @click="save" :disabled="!canSave" title="Ctrl+S">
                <span v-if="asyncState.isSaving">Saving...</span>
                <span v-else>Save</span>
              </Button>
            </div>
          </div>
        </DialogFooter>
      </Loading>
    </DialogContent>
  </Dialog>

  <!-- Tag Picker Dialog -->
  <TagPickerDlg
    v-model:open="tagPickerOpen"
    :all-tags="tags"
    :picked-tag-ids="pickedTagIds"
    @save="onTagsPicked"
  />
</template>
