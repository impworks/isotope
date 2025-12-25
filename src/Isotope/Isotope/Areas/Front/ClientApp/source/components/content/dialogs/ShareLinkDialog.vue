<script setup lang="ts">
import { ref, computed, watch, inject } from 'vue';
import { ApiServiceKey, FilterStateServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';
import type { FilterStateService, IFilterState } from '@/services/FilterStateService';
import { useAsyncState } from '@/composables/useAsyncState';
import { SearchScope } from '../../../../../../Common/source/vms/SearchScope';
import { ArrayHelper } from '../../../../../../Common/source/utils/ArrayHelper';
import { DateHelper } from '../../../../../../Common/source/utils/DateHelper';
import ModalWindow from '@/components/utils/ModalWindow.vue';
import Loading from '@/components/utils/Loading.vue';

type LinkCreationState = 'new' | 'succeeded' | 'failed';

const model = defineModel<boolean>({ default: false });

const $api = inject(ApiServiceKey)!;
const $filter = inject(FilterStateServiceKey)!;
const { asyncState, showLoading, showSaving } = useAsyncState();

const caption = ref<string | null>(null);
const tags = ref<string[]>([]);
const scope = ref<string | null>(null);
const dates = ref<string | null>(null);
const state = ref<LinkCreationState>('new');
const link = ref<string | null>(null);

const canCreate = computed(() => {
  return state.value === 'new' && !asyncState.isLoading && !asyncState.isSaving;
});

async function getFolderName(filterState: IFilterState): Promise<string> {
  const empty = $filter.isEmpty(filterState);
  const root = filterState.folder === '/';
  const tree = await $api.getFolderTree();
  const folder = ArrayHelper.findRecursive(tree, (x) => x.path === filterState.folder, (x) => x.subfolders);
  if (!folder && !root) {
    throw Error('Folder not found');
  }

  if (empty) {
    return root ? 'Everywhere' : 'Folder "' + folder.caption + '" and subfolders';
  }

  if (filterState.scope === SearchScope.Everywhere || (root && filterState.scope === SearchScope.CurrentFolderAndSubfolders)) {
    return 'Everywhere';
  }

  if (root && filterState.scope === SearchScope.CurrentFolder) {
    return 'Root folder';
  }

  return 'Folder "' + folder.caption + '"' + (filterState.scope === SearchScope.CurrentFolder ? '' : ' and subfolders');
}

async function getTagNames(filterState: IFilterState): Promise<string[]> {
  if (!filterState.tags || !filterState.tags.length) {
    return [];
  }

  const allTags = await $api.getTags();
  return allTags.filter((x) => filterState.tags.indexOf(x.id) !== -1).map((x) => x.caption);
}

function getDateDescription(filterState: IFilterState): string | null {
  const parts: string[] = [];
  if (filterState.from) {
    parts.push('From ' + DateHelper.format(filterState.from));
  }
  if (filterState.to) {
    parts.push('to ' + DateHelper.format(filterState.to));
  }

  return parts.length ? parts.join(' ') : null;
}

async function load() {
  state.value = 'new';
  link.value = null;

  try {
    await showLoading(async () => {
      const filterState = $filter.state;
      tags.value = await getTagNames(filterState);
      scope.value = await getFolderName(filterState);
      dates.value = getDateDescription(filterState);
    });
  } catch (e) {
    console.log('Failed to load filter data', e);
    model.value = false;
  }
}

async function create() {
  if (!canCreate.value) {
    return;
  }

  try {
    await showSaving(async () => {
      const key = await $api.createSharedLink(caption.value, $filter.state);
      link.value = window.location.origin + '/?sh=' + key;
      state.value = 'succeeded';
    });
  } catch (e) {
    console.log('Link creation failed', e);
    state.value = 'failed';
  }
}

async function copyAndClose() {
  await navigator.clipboard.writeText(link.value!);
  model.value = false;
}

watch(model, (value) => {
  if (value) {
    load();
  }
});
</script>

<template>
  <modal-window v-model="model">
    <template v-slot:header>
      <div class="modal-window__header__caption">Link sharing</div>
      <div class="modal-window__header__actions">
        <button href="#" class="btn-header" @click.prevent="model = !model">
          <div class="btn-header__content">
            <i class="icon icon-cross"></i>
          </div>
        </button>
      </div>
    </template>
    <template v-slot:content>
      <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <div v-if="state === 'new'">
          <p>You are creating a public link with the following properties:</p>
          <div class="row mb-3">
            <label class="col-form-label col-sm-3">Caption</label>
            <div class="col-sm-9">
              <input type="text" class="form-control" v-model="caption" v-autofocus />
            </div>
          </div>
          <div class="row mb-4">
            <div class="col-sm-3">Scope</div>
            <div class="col-sm-9">{{ scope }}</div>
          </div>
          <div class="row mb-4" v-if="dates">
            <div class="col-sm-3">Date range</div>
            <div class="col-sm-9">{{ dates }}</div>
          </div>
          <div class="row" v-if="tags && tags.length">
            <div class="col-sm-3">Tags</div>
            <div class="col-sm-9">
              <ul class="mb-0 ps-3">
                <li v-for="(tag, i) in tags" :key="i">{{ tag }}</li>
              </ul>
            </div>
          </div>
        </div>
        <div v-if="state === 'succeeded'">
          <p>Please copy the link:</p>
          <div>
            <input type="text" class="form-control" readonly="readonly" :value="link" />
          </div>
        </div>
        <div v-if="state === 'failed'">
          <div class="alert alert-danger mb-0">
            <strong>Error</strong><br />
            Link could not be created.
          </div>
        </div>
      </loading>
    </template>
    <template v-slot:footer>
      <button
        v-if="state === 'new'"
        type="button"
        class="btn btn-primary w-100"
        :disabled="!canCreate"
        @click.prevent="create"
      >
        <span v-if="asyncState.isSaving">Creating...</span>
        <span v-else>Create</span>
      </button>
      <button v-if="state !== 'new'" type="button" class="btn btn-primary w-100" @click.prevent="copyAndClose">
        Copy and close
      </button>
    </template>
  </modal-window>
</template>
