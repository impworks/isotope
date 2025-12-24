<script setup lang="ts">
import { Label } from '@ui/label';
import { DatePicker } from '@ui/date-picker';

const dateValue = defineModel<string>('date', { required: true });
const descriptionValue = defineModel<string>('description', { required: true });

const emit = defineEmits<{
  change: [];
}>();

function onDateChange(value: string) {
  dateValue.value = value;
  emit('change');
}

function onDescriptionInput() {
  emit('change');
}
</script>

<template>
  <div class="space-y-4">
    <div class="space-y-2">
      <Label for="date">Date</Label>
      <DatePicker
        :model-value="dateValue"
        @update:model-value="onDateChange"
        placeholder="Select date"
      />
    </div>

    <div class="space-y-2">
      <Label for="description">Description</Label>
      <textarea
        id="description"
        v-model="descriptionValue"
        @input="onDescriptionInput"
        rows="5"
        class="flex w-full rounded-md border border-input bg-background px-3 py-2 text-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:border-muted-foreground disabled:cursor-not-allowed disabled:opacity-50"
      ></textarea>
    </div>
  </div>
</template>
