<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";
    import Folders from './Folders.vue';

    @Component({
        components: { 
            Folders
        }
    })
    export default class Sidebar extends Vue {} 
</script>

<template>
    <div class="sidebar">
        <div class="header">
            <a class="logo">
                logo
            </a>
            <div class="actions">
                actions
            </div>
        </div>
        <div class="sidebar-content">
            <folders></folders>
        </div>
    </div>
</template>

<style lang="scss">
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .sidebar {
        z-index: 1;
        position: relative;
        background: $white;
        width: 21rem;

        @include media-breakpoint-down(sm) {
            top: 0;
            width: 100%;
            position: fixed;
        }

        @include media-breakpoint-up(md) {
            flex: 0 0 auto;
            display: flex;
            flex-direction: column;
        }

        .header {
            flex: 0 0 auto;
            display: flex;
            flex-direction: row;
            justify-content: space-between;
            padding: 1rem;

            .logo {
                display: block;
            }
        }

        &-content {
            
            @include media-breakpoint-down(sm) {
                display: none;
            }

            @include media-breakpoint-up(md) {
                flex: 1 1 auto;
                display: flex;
                flex-direction: column;
                overflow: auto;
            }
        }
    }
</style>