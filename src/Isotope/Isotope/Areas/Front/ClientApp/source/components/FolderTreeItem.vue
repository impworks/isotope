<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";
import { Folder } from "../vms/Folder";
import { Dep } from "../utils/VueInjectDecorator";
import FilterStateService from "../services/FilterStateService";

@Component
export default class FolderTreeItem extends Vue {
    @Dep('$filter') $filter: FilterStateService;
    
    @Prop({ required: true }) folder: Folder;
    @Prop({ required: true }) depth: number;
    @Prop({ required: true }) currentPath: string;
    
    expanded: boolean = false;
    active: boolean = false;
    
    mounted() {
        this.refreshState();
    }
    
    selectFolder() {
        this.$filter.update({ folder: this.folder.path });
    }
    
    @Watch('currentPath')
    refreshState() {
        this.expanded = this.currentPath.startsWith(this.folder.path);
        this.active = this.currentPath === this.folder.path;
    }
}
</script>

<template>
    <fragment>
        <a
            class="folder-tree-item clickable"
            :class="{active: active}"
            :key="folder.path"
            @click.prevent="selectFolder()"
        >
            <div class="folder-tree-item__icon" :style="{marginLeft: depth + 'em'}"></div>
            <div class="folder-tree-item__name">{{ folder.caption }}</div>
            <div v-if="folder.subfolders && folder.subfolders.length" class="float-right">
                <span class="clickable" @click.prevent="expanded = !expanded">{{expanded ? '[V]' : '[^]'}}</span>
            </div>
        </a>
        <div v-if="folder.subfolders && folder.subfolders.length && expanded">
            <FolderTreeItem v-for="s in folder.subfolders" :folder="s" :key="s.path" :depth="depth + 1" :current-path="currentPath" />
        </div>
    </fragment>
</template>

<style lang="scss">
@import "../../styles/variables";
@import "./node_modules/bootstrap/scss/functions";
@import "./node_modules/bootstrap/scss/variables";

.folder-tree-item {
    display: flex;
    flex-direction: row;
    padding: 0.5em 1em;
    line-height: 1.5;
    color: $gray-800;
    border-top: 1px solid $gray-200;

    &:first-of-type {
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

    &__icon {
        $size: 1.5em;

        width: $size;
        height: $size;
        background-image: url(../../images/folder.svg);
        background-position: 0 0;
        background-size: auto 200%;
        background-repeat: no-repeat;
    }

    &__name {
        flex-grow: 1;
        padding: 0 1em;
        flex: 0 1 auto;
    }
    
    &.active {
        color: $white;
        background-color: $primary;
        border-color: $primary;

        & + .folder-tree-item {
            border-top-color: rgba(0,0,0,0);
        }

        .folder-tree-item__icon {
            background-position: 0 100%;
        }
    }
}
</style>