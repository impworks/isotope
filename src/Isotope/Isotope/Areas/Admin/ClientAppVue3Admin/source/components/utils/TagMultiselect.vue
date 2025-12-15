<script setup lang="ts">
import { ref, computed } from 'vue';
import type { Tag } from '@/vms/Tag';
import { Badge } from '@ui/badge';
import { Button } from '@ui/button';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@ui/command';
import { Popover, PopoverContent, PopoverTrigger } from '@ui/popover';

interface Props {
  tags: Tag[];
  disabled?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false
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
          <div class="flex flex-wrap gap-1.5 flex-1 items-center">
            <template v-if="selectedTags.length > 0">
              <Badge
                v-for="tag in selectedTags"
                :key="tag.id"
                variant="secondary"
                class="gap-1"
              >
                {{ tag.caption }}
                <button
                  type="button"
                  @click.stop="removeTag(tag.id)"
                  class="ml-1 ring-offset-background rounded-full outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2"
                  :disabled="disabled"
                >
                  <span class="fa fa-times text-xs"></span>
                </button>
              </Badge>
            </template>
            <span v-else class="text-muted-foreground text-sm">Select tags...</span>
          </div>
          <span class="fa fa-chevron-down ml-2 h-4 w-4 shrink-0 opacity-50"></span>
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
                <span
                  class="fa mr-2 h-4 w-4"
                  :class="isSelected(tag.id) ? 'fa-check' : ''"
                ></span>
                {{ tag.caption }}
              </CommandItem>
            </CommandGroup>
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  </div>
</template>
