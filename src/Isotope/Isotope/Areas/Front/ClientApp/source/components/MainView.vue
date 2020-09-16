<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";
    import Gallery from './Gallery.vue';
    import Sidebar from './Sidebar.vue';

    @Component({
        components: { 
            Gallery,
            Sidebar
        }
    })
    export default class MainView extends Vue {
        get folder(): string {
            return this.$route.path;
        }
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
        
        @include media-breakpoint-down(sm) {
            padding-top: 3.8rem;
        }

        @include media-breakpoint-up(md) {
            height: 100%;
            display: flex;
            flex-direction: row;
        }
    }
</style>
