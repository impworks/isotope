<script setup lang="ts">
import { ref, computed } from 'vue';
import type { Tag } from '@/vms/Tag';
import { Button } from '@ui/button';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@ui/command';
import { Popover, PopoverContent, PopoverTrigger } from '@ui/popover';
import { X, ChevronDown, Check } from 'lucide-vue-next';

interface Props {
  tags: Tag[];
  disabled?: boolean;
  placeholder?: string;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  placeholder: 'Select tags...'
});

const selectedIds = defineModel<number[]>({ required: true });

const open = ref(false);
const searchQuery = ref('');

const selectedTags = computed(() => {
  return props.tags.filter(t => selectedIds.value.includes(t.id));
});

const availableTags = computed(() => {
  const query = searchQuery.value.toLowerCase();
  return props.tags.filter(t =>
    !query || t.caption.toLowerCase().includes(query)
  );
});

function toggleTag(tagId: number) {
  const index = selectedIds.value.indexOf(tagId);
  if (index === -1) {
    selectedIds.value = [...selectedIds.value, tagId];
  } else {
    selectedIds.value = selectedIds.value.filter(id => id !== tagId);
  }
}

function removeTag(tagId: number) {
  selectedIds.value = selectedIds.value.filter(id => id !== tagId);
}

function isSelected(tagId: number) {
  return selectedIds.value.includes(tagId);
}
</script>

<template>
  <div>
    <Popover v-model:open="open">
      <PopoverTrigger as-child>
        <Button
          variant="outline"
          role="combobox"
          :aria-expanded="open"
          class="w-full justify-between h-auto min-h-8"
          :disabled="disabled"
        >
          <div class="flex flex-wrap gap-1.5 flex-1 items-center min-h-[1.625rem]">
            <template v-if="selectedTags.length > 0">
              <span
                v-for="tag in selectedTags"
                :key="tag.id"
                class="inline-flex items-center gap-1 px-2 py-0.5 rounded-md text-sm bg-primary/15 border border-primary/30"
              >
                {{ tag.caption }}
                <button
                  type="button"
                  @click.stop="removeTag(tag.id)"
                  class="text-muted-foreground hover:text-foreground focus:outline-none"
                  :disabled="disabled"
                >
                  <X class="h-3 w-3" :stroke-width="1.5" />
                </button>
              </span>
            </template>
            <span v-else class="text-muted-foreground text-sm">{{ placeholder }}</span>
          </div>
          <ChevronDown class="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent class="w-[400px] p-0" align="start">
        <Command v-model:search-term="searchQuery">
          <CommandInput placeholder="Search tags..." />
          <CommandList>
            <CommandEmpty>No tags found.</CommandEmpty>
            <CommandGroup>
              <CommandItem
                v-for="tag in availableTags"
                :key="tag.id"
                :value="tag.id.toString()"
                @select="toggleTag(tag.id)"
                class="cursor-pointer"
              >
                <Check
                  class="mr-2 h-4 w-4"
                  :class="isSelected(tag.id) ? 'opacity-100' : 'opacity-0'"
                />
                {{ tag.caption }}
              </CommandItem>
            </CommandGroup>
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  </div>
</template>
