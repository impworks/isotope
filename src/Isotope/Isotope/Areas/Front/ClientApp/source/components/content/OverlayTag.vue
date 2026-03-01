<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted, nextTick, inject } from 'vue';
import { createPopper, Instance as PopperInstance } from '@popperjs/core';
import { TagBindingWithLocation } from '@/vms/TagBinding';
import { FilterStateServiceKey, EventBusServiceKey } from '@/config/Injector';
import type { FilterStateService } from '@/services/FilterStateService';
import type { EventBusService } from '@/services/EventBusService';
import { SearchScope } from '../../../../../Common/source/vms/SearchScope';
import { useLifetime } from '@/composables/useLifetime';

interface Props {
  value: TagBindingWithLocation;
  isMobile?: boolean;
  isMobileOverlayVisible?: boolean;
  tappedTag?: TagBindingWithLocation;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  tagTapped: [tag: TagBindingWithLocation];
}>();

const $filter = inject(FilterStateServiceKey)!;
const $eventBus = inject(EventBusServiceKey)!;
const { observe } = useLifetime();

const isTransitioning = ref(false);
const isTapped = ref(false);

const frameRef = ref<HTMLElement>();
const popoverRef = ref<HTMLElement>();

let popperInstance: PopperInstance | null = null;

const canFilter = computed(() => {
  return !$filter.shareId;
});

const style = computed(() => {
  const pc = (v: number) => (v * 100) + '%';
  const loc = props.value.location;

  return {
    left: pc(loc.x),
    top: pc(loc.y),
    width: pc(loc.width),
    height: pc(loc.height)
  };
});

function onTap() {
  emit('tagTapped', props.value);
}

function filterByTag() {
  if (!canFilter.value) {
    return;
  }

  $filter.update(
    'tag',
    {
      folder: '/',
      tags: [props.value.tag.id],
      from: null,
      to: null,
      mediaKey: null,
      scope: SearchScope.CurrentFolderAndSubfolders
    }
  );
}

// Watch for overlay visibility changes
watch([() => props.isMobile, () => props.isMobileOverlayVisible], ([mobile, visible]) => {
  if (!visible) {
    isTapped.value = false;
  }
});

// Watch for tapped tag changes
watch(() => props.tappedTag, (value) => {
  isTapped.value = value === props.value;
});

onMounted(() => {
  nextTick(() => {
    if (frameRef.value && popoverRef.value) {
      popperInstance = createPopper(frameRef.value, popoverRef.value, {
        placement: 'bottom',
        modifiers: [
          {
            name: 'arrow',
            options: {
              element: '.arrow'
            }
          },
          {
            name: 'offset',
            options: {
              offset: [0, 5]
            }
          }
        ]
      });

      observe($eventBus.uiUpdated, () => {
        popperInstance?.update();
      });
    }
  });
});

onUnmounted(() => {
  popperInstance?.destroy();
});
</script>

<template>
  <div>
    <div
      ref="frameRef"
      class="overlay-tag tooltip-target"
      :class="{
        'overlay-tag_mobile': isMobile,
        'overlay-tag_mobile-visible': isMobileOverlayVisible,
        'overlay-tag_transitioning': isTransitioning,
        'overlay-tag_tapped': isTapped
      }"
      :style="style"
      @click="onTap"
      @transitionstart.self="isTransitioning = true"
      @transitionend.self="isTransitioning = false"
    >
      <div class="popover bs-popover-bottom tag-popover" ref="popoverRef">
        <div class="arrow"></div>
        <div class="popover-body">
          <a
            v-if="canFilter"
            class="clickable"
            @click.prevent="filterByTag()"
          >
            {{ value.tag.caption }}
          </a>
          <span v-else>
            {{ value.tag.caption }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "bootstrap/scss/functions";
@import "bootstrap/scss/variables";
@import "bootstrap/scss/mixins";

.overlay-tag {
  position: absolute;
  border: 2px solid rgba($white, 0.2);
  transition:
    border-color 200ms linear,
    opacity 200ms linear;

  @include media-breakpoint-down(md) {
    visibility: hidden;
  }

  &_mobile {
    opacity: 0;
  }

  &_mobile-visible {
    opacity: 1;
    visibility: visible;
  }

  &_transitioning {
    visibility: visible;
  }

  &_tapped .popover {
    @include media-breakpoint-down(md) {
      opacity: 1 !important;
      border-color: rgba($white, 0.5);
    }
  }

  .popover {
    opacity: 0;
    transition: opacity 200ms linear;

    &.tag-popover {
      min-width: 130px;

      .popover-body {
        padding: 0.3rem 0.6rem;
        text-align: center;
      }
    }

    &:focus {
      outline: none;
    }

    .popover-body a {
      text-decoration: none;

      &:active,
      .no-touch &:hover {
        text-decoration: underline;
      }
    }
  }

  &:hover {
    @include media-breakpoint-up(lg) {
      border-color: rgba($white, 0.5);
    }

    .popover {
      @include media-breakpoint-up(lg) {
        opacity: 1;
      }
    }
  }
}
</style>
