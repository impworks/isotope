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
            <router-link
                to="/"
                class="logo"
            >
                isotope
            </router-link>
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
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .sidebar {
        z-index: 1;
        position: relative;
        background: $white;
        width: 21rem;
        box-shadow: $box-shadow-sm;

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
            align-items: center;
            padding: 1rem;

            .logo {
                $font-size: 1.8rem;

                display: block;
                color: $gray-900;
                line-height: 1;
                font-weight: bold;
                font-size: $font-size;
                background-image: url(../../images/logo.svg);
                background-size: auto 100%;
                background-repeat: no-repeat;
                padding-left: $font-size * 1.85;
                padding-right: 1rem;

                &:hover {
                    text-decoration: none;
                }
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