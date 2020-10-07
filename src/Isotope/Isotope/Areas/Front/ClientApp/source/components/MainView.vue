<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import Gallery from './content/Gallery.vue';
import Sidebar from './sidebar/Sidebar.vue';

@Component({
    components: { Gallery, Sidebar }
})
export default class MainView extends Vue {
}
</script>

<template>
    <div class="main-view">
        <sidebar></sidebar>
        <gallery></gallery>
    </div>
</template>

<style lang="scss">
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .main-view {
        display: flex;
        min-height: 0;
        flex: 1 0 auto;

        @include media-breakpoint-down(sm) {
            padding-top: 3.6875rem;
            flex-direction: column;
            min-height: 100%;
        }

        @include media-breakpoint-up(md) {
            height: 100%;
            overflow: hidden;
            flex-direction: row;
        }
    }
</style>
