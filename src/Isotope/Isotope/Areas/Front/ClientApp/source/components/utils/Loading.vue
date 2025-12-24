<script setup lang="ts">
interface Props {
    isLoading?: boolean;
    isFullPage?: boolean;
    text?: string;
}

const props = defineProps<Props>();
</script>

<template>
    <div v-if="isLoading && isFullPage" class="d-flex align-items-center justify-content-center flex-grow-1 overflow-hidden h-100">
        <div class="spinner-border spinner-large"></div>
    </div>
    <div v-if="isLoading && !isFullPage">
        <span class="spinner-border spinner-border-sm spinner-small"></span>
        <span>{{text}}</span>
    </div>
    <slot v-if="!isLoading"></slot>
</template>

<style lang="scss">
.spinner-border.spinner-small {
    vertical-align: middle;
}

.spinner-border.spinner-large {
    display: inline-block;
    width: 3rem;
    height: 3rem;
    margin: 0.5em;
}
</style>
