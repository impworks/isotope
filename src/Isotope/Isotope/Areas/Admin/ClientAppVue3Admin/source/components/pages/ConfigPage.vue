<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useApi } from '@/composables/useApi';
import { useAsyncState } from '@/composables/useAsyncState';
import { useToast } from '@/composables/useToast';
import type { Config } from '@/vms/Config';
import Loading from '@/components/utils/Loading.vue';
import { Card, CardContent, CardFooter } from '@ui/card';
import { Input } from '@ui/input';
import { Label } from '@ui/label';
import { RadioGroup, RadioGroupItem } from '@ui/radio-group';
import { Button } from '@ui/button';

const api = useApi();
const toast = useToast();
const { asyncState, showLoading, showSaving } = useAsyncState();

const value = ref<Config | null>(null);

onMounted(async () => {
  await showLoading(
    async () => {
      value.value = await api.config.get();
    },
    'Failed to load config state!'
  );
});

const canSave = computed(() => {
  return value.value && !!value.value.title;
});

async function save() {
  if (!value.value) return;

  const success = await showSaving(
    async () => {
      await api.config.update({ ...value.value! });
      toast.success('Config updated');
    },
    'Failed to update config state!'
  );
}
</script>

<template>
  <div class="p-6">
    <div class="max-w-2xl">
      <h1 class="text-3xl font-bold mb-6">Configuration</h1>

      <Loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <Card v-if="value">
          <form @submit.prevent="save">
            <CardContent class="space-y-6 pt-6">
              <div class="space-y-2">
                <Label for="title">Gallery Title</Label>
                <Input
                  id="title"
                  v-model="value.title"
                  v-autofocus
                  :disabled="asyncState.isSaving"
                  placeholder="Enter gallery title"
                />
              </div>

              <div class="space-y-3">
                <Label>Access Control</Label>
                <RadioGroup v-model="value.allowGuests" :disabled="asyncState.isSaving">
                  <div class="flex items-center space-x-2">
                    <RadioGroupItem id="access-true" :value="true" />
                    <Label for="access-true" class="font-normal cursor-pointer">
                      Allow anyone to view the gallery
                    </Label>
                  </div>
                  <div class="flex items-center space-x-2">
                    <RadioGroupItem id="access-false" :value="false" />
                    <Label for="access-false" class="font-normal cursor-pointer">
                      Only allow authorized users and shared links
                    </Label>
                  </div>
                </RadioGroup>
              </div>
            </CardContent>

            <CardFooter>
              <Button type="submit" :disabled="!canSave || asyncState.isSaving">
                <span v-if="asyncState.isSaving">Saving...</span>
                <span v-else>Save Changes</span>
              </Button>
            </CardFooter>
          </form>
        </Card>
      </Loading>
    </div>
  </div>
</template>
