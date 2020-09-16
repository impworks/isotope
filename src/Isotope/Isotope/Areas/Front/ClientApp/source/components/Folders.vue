<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component
    export default class Folders extends Vue {
        opened: number = 0;
    } 
</script>

<template>
    <div class="folders">
        <div>
            <button class="btn btn-block btn-primary">
                Back button
            </button>
        </div>
        <div class="scrollable">
            <a 
                href="#"
                class="folder"
                v-for="i in 100" 
                :key="i"
                @click="opened = i"
            >
                Folder name
            </a>
        </div>
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
            display: block;
        }
    }
</style>