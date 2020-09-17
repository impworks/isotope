<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component
    export default class Folders extends Vue {
        opened: number = 0;
    } 
</script>

<template>
    <div class="folders">
        <perfect-scrollbar class="scrollable">
            <a 
                href="#"
                class="folder"
                :class="{ 'opened': opened == i }"
                v-for="i in 100" 
                :key="i"
                @click="opened = i"
            >
                <div class="folder-icon"></div>
                <div class="folder-name">Folder name </div>
            </a>
        </perfect-scrollbar >
    </div>
</template>

<style lang="scss">
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";

    .folders {
        display: flex;
        flex-direction: column;
        flex: 1 1 auto;
        min-height: 0;

        > * {
            flex: 0 0 auto;
        }

        .scrollable {
            flex: 1 1 auto;
            overflow: auto;
            min-height: 0;
        }

        .folder {
            display: flex;
            flex-direction: row;
            padding: 0.5em 1em;
            line-height: 1.5;
            color: $gray-800;
            border-top: 1px solid $gray-200;

            &:first-child {
                border-top-color: rgba(0,0,0,0);
            }

            &:hover {
                text-decoration: none;
                background-color: $gray-200;
            }

            &-icon,
            &-name {
                flex: 0 0 auto;
            }

            &-icon {
                $size: 1.5em;

                width: $size;
                height: $size;
                background-image: url(../../images/folder.svg);
                background-position: 0 0;
                background-size: auto 200%;
                background-repeat: no-repeat;
            }

            &-name {
                flex-grow: 1;
                padding: 0 1em;
                flex: 0 1 auto;
            }

            &.opened {
                color: $white;
                background-color: $primary;
                border-color: $primary;

                & + .folder {
                    border-top-color: rgba(0,0,0,0);
                }

                .folder-icon {
                    background-position: 0 100%;
                }
            }
        }
    }
</style>