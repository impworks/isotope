<script setup lang="ts">
import { ref, watch, onMounted, inject } from 'vue';
import { useLifetime } from '@/composables/useLifetime';
import { useAsyncState } from '@/composables/useAsyncState';
import { ApiServiceKey, FilterStateServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import type { FilterStateService, IFilterState } from '@/services/FilterStateService';
import { Tag } from '@/vms/Tag';
import { SearchScope } from '../../../../../Common/source/vms/SearchScope';

const $api = inject(ApiServiceKey)!;
const $filter = inject(FilterStateServiceKey)!;
const { observe } = useLifetime();
const { asyncState, showLoading } = useAsyncState();

const filter = ref<Partial<IFilterState>>({ scope: SearchScope.CurrentFolderAndSubfolders });
const tags = ref<Tag[]>([]);

function updateFromState(state: IFilterState) {
  filter.value = {
    tags: state.tags,
    scope: state.scope || SearchScope.CurrentFolderAndSubfolders,
    from: state.from,
    to: state.to
  };
}

watch(
  filter,
  () => {
    $filter.update('filters', { ...filter.value });
  },
  { deep: true }
);

onMounted(async () => {
  observe($filter.onStateChanged, (x) => updateFromState(x));
  updateFromState($filter.state);

  try {
    await showLoading(async () => {
      tags.value = await $api.getTags();
    });
  } catch (e) {
    console.log('Failed to load tags', e);
  }
});
</script>

<template>
  <div class="filter">
    <h6>Tags</h6>
    <v-select
      multiple
      v-model="filter.tags"
      :disabled="asyncState.isLoading"
      :options="tags"
      :reduce="(x) => x.id"
      label="caption"
    >
      <template #open-indicator="{ attributes }">
        <i class="icon icon-arrow" v-bind="attributes"></i>
      </template>
    </v-select>
  </div>
  <div class="filter hide-mobile">
    <h6>Date range</h6>
    <div class="d-flex align-items-center justify-content-between">
      <div>
        <datepicker
          v-model="filter.from"
          :auto-apply="true"
          :time-config="{ enableTimePicker: false }"
          :formats="{ input: 'yyyy/MM/dd' }"
          class="filter-datepicker"
        >
          <template #input-icon>
          </template>
        </datepicker>
      </div>
      <div class="px-1">—</div>
      <div>
        <datepicker
          v-model="filter.to"
          :auto-apply="true"
          :time-config="{ enableTimePicker: false }"
          :formats="{ input: 'yyyy/MM/dd' }"
          class="filter-datepicker"
        >
          <template #input-icon>
          </template>
        </datepicker>
      </div>
    </div>
  </div>
  <div class="filter hide-desktop">
    <h6>Date from</h6>
    <input type="date" class="form-control mb-3" v-model="filter.dateFrom" />
    <h6>Date to</h6>
    <input type="date" class="form-control" v-model="filter.dateTo" />
  </div>
  <div class="filter">
    <h6>Search in</h6>
    <div class="form-check">
      <input
        type="radio"
        id="sm-current-folder"
        class="form-check-input"
        v-model="filter.scope"
        :value="1"
      />
      <label class="form-check-label" for="sm-current-folder"> Current folder </label>
    </div>
    <div class="form-check">
      <input
        type="radio"
        id="sm-current-folder-nested"
        class="form-check-input"
        v-model="filter.scope"
        :value="2"
      />
      <label class="form-check-label" for="sm-current-folder-nested"> Current and subfolders </label>
    </div>
    <div class="form-check">
      <input id="sm-everywhere" type="radio" class="form-check-input" v-model="filter.scope" :value="3" />
      <label class="form-check-label" for="sm-everywhere"> Everywhere </label>
    </div>
  </div>
</template>

<style scoped lang="scss">
@import "../../../../../Common/styles/variables";
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";

.filter {
  padding-bottom: 1rem;

  h6 {
    color: $gray-900;
    margin-bottom: 0.5rem;
  }
}

// Datepicker customization
.filter-datepicker {
  // CSS variables for vue3datepicker
  --dp-input-icon-padding: 10px;

  // Override specific element styles
  :deep(.dp__active_date) {
    background: $primary !important;
  }
  
  :deep(.dp__today) {
    border: 1px solid $primary !important;
  }
}
</style>
