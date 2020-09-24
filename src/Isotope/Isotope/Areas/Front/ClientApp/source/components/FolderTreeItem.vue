<script lang="ts">
import { Vue, Component, Prop } from "vue-property-decorator";
import { Folder } from "../vms/Folder";

@Component
export default class FolderTreeItem extends Vue {
    @Prop({ required: true }) folder: Folder;
    @Prop({ required: true }) depth: number;
    
    expanded: boolean = false;
    active: boolean = false;
    
    mounted() {
        const path = this.$route.path;
        this.expanded = path.startsWith(this.folder.path);
        this.active = path === this.folder.path;
    }
}
</script>

<template>
    <fragment>
        <a
            :href="folder.path"
            :class="{folder: true, active: active}"
            :key="folder.path"
        >
            <div class="folder-icon" :style="{marginLeft: depth + 'em'}"></div>
            <div class="folder-name">{{ folder.caption }}</div>
            <div v-if="folder.subfolders && folder.subfolders.length" class="float-right">
                <span class="clickable" @click.prevent="expanded = !expanded">{{expanded ? '[V]' : '[^]'}}</span>
            </div>
        </a>
        <div v-if="folder.subfolders && folder.subfolders.length && expanded">
            <FolderTreeItem v-for="s in folder.subfolders" :folder="s" :key="s.path" :depth="depth + 1" />
        </div>
    </fragment>
</template>

<style lang="scss">
@import "../../styles/variables";
@import "./node_modules/bootstrap/scss/functions";
@import "./node_modules/bootstrap/scss/variables";

.folder {
    display: flex;
    flex-direction: row;
    padding: 0.5em 1em;
    line-height: 1.5;
    color: $gray-800;
    border-top: 1px solid $gray-200;

    &:first-of-type {
        border-top-color: rgba(0,0,0,0);
    }

    &:hover, &.active {
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
</style>