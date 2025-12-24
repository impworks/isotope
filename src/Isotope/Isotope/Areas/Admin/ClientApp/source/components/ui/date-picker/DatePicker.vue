<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { CalendarDate, DateFormatter, getLocalTimeZone, parseDate, today } from '@internationalized/date';
import { Calendar } from '@ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '@ui/popover';
import { Button } from '@ui/button';
import { cn } from '@/lib/utils';
import { Calendar as CalendarIcon } from 'lucide-vue-next';

interface Props {
  modelValue?: string;
  placeholder?: string;
  class?: string;
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: 'Pick a date'
});

const emit = defineEmits<{
  'update:modelValue': [value: string];
}>();

const df = new DateFormatter('en-US', {
  year: 'numeric',
  month: '2-digit',
  day: '2-digit',
});

const calendarValue = ref<CalendarDate>();

// Parse modelValue string (YYYY-MM-DD) to CalendarDate
watch(() => props.modelValue, (newVal) => {
  if (newVal) {
    try {
      calendarValue.value = parseDate(newVal);
    } catch {
      calendarValue.value = undefined;
    }
  } else {
    calendarValue.value = undefined;
  }
}, { immediate: true });

// Format CalendarDate to display string (yyyy.MM.dd)
const displayValue = computed(() => {
  if (!calendarValue.value) return '';
  const year = calendarValue.value.year;
  const month = String(calendarValue.value.month).padStart(2, '0');
  const day = String(calendarValue.value.day).padStart(2, '0');
  return `${year}.${month}.${day}`;
});

function onSelect(date: CalendarDate | undefined) {
  calendarValue.value = date;
  if (date) {
    const year = date.year;
    const month = String(date.month).padStart(2, '0');
    const day = String(date.day).padStart(2, '0');
    emit('update:modelValue', `${year}-${month}-${day}`);
  } else {
    emit('update:modelValue', '');
  }
}
</script>

<template>
  <Popover>
    <PopoverTrigger as-child>
      <Button
        variant="outline"
        :class="cn(
          'w-[200px] justify-start text-left font-normal',
          !calendarValue && 'text-muted-foreground',
          props.class
        )"
      >
        <CalendarIcon class="mr-2 h-4 w-4" />
        {{ displayValue || placeholder }}
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-auto p-0">
      <Calendar
        :model-value="calendarValue"
        @update:model-value="onSelect"
        initial-focus
      />
    </PopoverContent>
  </Popover>
</template>
