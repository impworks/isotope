<script setup lang="ts">
import { ref, watch, onMounted, inject } from 'vue';
import { FilterStateServiceKey } from '@/config/Injector';
import type { FilterStateService } from '@/services/FilterStateService';
import { useLifetime } from '@/composables/useLifetime';
import Filters from '../content/Filters.vue';

const $filter = inject(FilterStateServiceKey)!;
const { observe } = useLifetime();

const isOpen = ref(false);
const height = ref('0px');
const overflow = ref<string | null>('hidden');
const isTransitioning = ref(false);

const wrapperRef = ref<HTMLElement>();
const contentRef = ref<HTMLElement>();

function toggleOpen() {
  if (isOpen.value) {
    $filter.clear('filters');
  }
  isOpen.value = !isOpen.value;
}

watch(isOpen, (value) => {
  const wrapper = wrapperRef.value;
  const content = contentRef.value;

  if (!wrapper || !content) {
    return;
  }

  overflow.value = 'hidden';

  if (value) {
    if (content.clientHeight === 0) {
      height.value = 'auto';
      overflow.value = null;
    } else {
      height.value = content.clientHeight + 'px';
    }
    return;
  }

  if (wrapper.style.height === 'auto') {
    height.value = content.clientHeight + 'px';
    setTimeout(() => (height.value = '0px'), 1);
  } else {
    height.value = '0px';
  }
});

watch(isTransitioning, (value) => {
  if (!value && isOpen.value) {
    height.value = 'auto';
    overflow.value = null;
  }
});

onMounted(() => {
  isOpen.value = !$filter.isEmpty($filter.state);
  observe($filter.onStateChanged, (x) => {
    if (!$filter.isEmpty(x) && !isOpen.value) {
      isOpen.value = true;
    }
  });
});
</script>

<template>
  <div
    class="desktop-filters"
    :class="{
      'desktop-filters_opened': isOpen
    }"
  >
    <a class="sidebar-button clickable" :class="{ 'sidebar-button_opened': isOpen }" @click.prevent="toggleOpen()">
      <div class="sidebar-button__icon">
        <div class="filter-icon"></div>
      </div>
      <div class="sidebar-button__text">Filters</div>
      <div class="sidebar-button__arrow">
        <i class="icon icon-arrow"></i>
      </div>
    </a>
    <div
      ref="wrapperRef"
      class="desktop-filters__content"
      :style="{ height: height, overflow: overflow || undefined }"
      @transitionstart.self="isTransitioning = true"
      @transitionend.self="isTransitioning = false"
    >
      <div ref="contentRef">
        <filters></filters>
      </div>
    </div>
  </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";

.desktop-filters {
  flex: 0 0 auto;
  position: relative;
  z-index: 3;

  &_opened {
    border-bottom: 1px solid $gray-300;
  }

  .filter-icon {
    background-image: url(../../../images/filter.svg);
  }

  &__content {
    transition: height 400ms cubic-bezier(0.645, 0.045, 0.355, 1);

    .filter {
      padding: 0 1rem 1rem;

      &:first-child {
        padding-top: 1rem;
      }
    }
  }
}
</style>
