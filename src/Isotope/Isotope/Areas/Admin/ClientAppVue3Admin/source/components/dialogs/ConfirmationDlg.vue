<script setup lang="ts">
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@ui/dialog';
import { Button } from '@ui/button';

interface Props {
  text: string;
}

const props = defineProps<Props>();
const isOpen = defineModel<boolean>('open', { default: false });
const emit = defineEmits<{
  confirmed: [value: boolean];
}>();

function confirm() {
  emit('confirmed', true);
  isOpen.value = false;
}

function cancel() {
  emit('confirmed', false);
  isOpen.value = false;
}
</script>

<template>
  <Dialog v-model:open="isOpen">
    <DialogContent>
      <DialogHeader>
        <DialogTitle>Confirm Action</DialogTitle>
      </DialogHeader>
      <DialogDescription>
        <div v-html="text"></div>
      </DialogDescription>
      <DialogFooter>
        <Button variant="secondary" @click="cancel">Cancel</Button>
        <Button variant="destructive" @click="confirm">Confirm</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
