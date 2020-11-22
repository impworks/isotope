<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";
import { Folder } from "../../vms/Folder";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { FilterStateService } from "../../services/FilterStateService";

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
        this.$filter.update('tree', { folder: this.folder.path });
    }
    
    @Watch('currentPath')
    refreshState() {
        this.active = this.currentPath === this.folder.path;
        this.expanded = this.active || this.currentPath.startsWith(this.folder.path + '/');
    }
}
</script>

<template>
    <div class="folder-tree-item">
        <a
            class="folder-tree-link clickable"
            :class="{
                'folder-tree-link_active': active, 
                'folder-tree-link_styled': depth > 1, 
                'folder-tree-link_expanded': expanded
            }"
            :key="folder.path"
            @click.prevent="selectFolder()"
        >
            <div class="folder-tree-link__icon" :style="{marginLeft: depth * 0.7 + 'em'}"></div>
            <div class="folder-tree-link__name">{{ folder.caption }}</div>
        </a>
        <fragment v-if="folder.subfolders && folder.subfolders.length && expanded">
            <FolderTreeItem v-for="s in folder.subfolders" :folder="s" :key="s.path" :depth="depth + 1" :current-path="currentPath" />
        </fragment>
    </div>
</template>

<style lang="scss">
@import "../../../../../Common/styles/variables";
@import "./node_modules/bootstrap/scss/functions";
@import "./node_modules/bootstrap/scss/variables";

.folder-tree-link {
    display: flex;
    flex-direction: row;
    padding: 0.5em 1em;
    color: $gray-800;

    &:hover:not(#{&}_active) {
        color: $gray-800;
        text-decoration: none;
        background-color: $gray-200;
    }

    &-icon,
    &-name {
        flex: 0 0 auto;
    }

    &__icon {
        $size: 1.4em;

        width: $size;
        height: $size;
        background-image: url(../../../images/folder.svg);
        background-position: 0 0;
        background-size: auto 200%;
        background-repeat: no-repeat;
    }

    &__name {
        flex-grow: 1;
        padding: 0 1em;
        flex: 0 1 auto;
    }
    
    &_active #{&}__icon ,
    &_expanded #{&}__icon {
        background-position: 0 100%;
    }

    &_active,
    &_active:hover {
        color: $white;
        background-color: $primary;
        border-color: $primary;
        text-decoration: none;
    }
}
</style>