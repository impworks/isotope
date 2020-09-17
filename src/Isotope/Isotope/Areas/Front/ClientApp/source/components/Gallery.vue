<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component
    export default class Gallery extends Vue {} 
</script>

<template>
    <div class="gallery">
        <div
            class="folder"
            v-for="i in 100" 
            :key="i"
        >
            Gallery item
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .gallery {
        padding: 1rem;
        background: $gray-200;

        @include media-breakpoint-up(md) {
            flex: 1 0 auto;
            min-height: 0;
            overflow: auto;
        }
    }
</style>