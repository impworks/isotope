<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { Alert, AlertDescription } from '@ui/alert';

const route = useRoute();
const folderKey = ref<string>('');

onMounted(() => {
  folderKey.value = route.params.key as string;
});
</script>

<template>
  <div class="p-6">
    <h1 class="text-3xl font-bold mb-6">Folder Media</h1>

    <Alert>
      <AlertDescription>
        Media management page for folder "{{ folderKey }}" (coming soon)
      </AlertDescription>
    </Alert>
  </div>
</template>
