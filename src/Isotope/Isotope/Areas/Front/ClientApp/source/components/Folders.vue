<script lang="ts">
    import { Component, Mixins } from "vue-property-decorator";
    import { HasAsyncState } from "./mixins/HasAsyncState";
    import { Dep } from "../utils/VueInjectDecorator";
    import { ApiService } from "../services/ApiService";
    import { Folder } from "../vms/Folder";
    import FolderTreeItem from "./FolderTreeItem.vue";
    import { FilterStateService } from "../services/FilterStateService";
    import { HasLifetime } from "./mixins/HasLifetime";
    @Component({
        components: { FolderTreeItem }
    })
    export default class Folders extends Mixins(HasAsyncState(), HasLifetime) {
        @Dep('$api') $api: ApiService;
        @Dep('$filter') $filter: FilterStateService;
        
        currentPath: string = null;
        folders: Folder[] = null;
        
        async mounted() {
            this.currentPath = this.$filter.state.folder;
            this.observe(this.$filter.onStateChanged, x => {
                this.currentPath = x.folder;
                if(x.source !== 'tree')
                    this.scrollToSelected();
            });
            try {
                await this.showLoading(async () => {
                    this.folders = await this.$api.getFolderTree();
                    this.scrollToSelected();
                })
            } catch (e) {
                console.log('Failed to get folders.', e);
            }
        }
        
        private scrollToSelected() {
            setTimeout(() => {
                const scroll = this.$refs.scroll.$el as HTMLElement;
                if(scroll.scrollHeight <= scroll.clientHeight)
                    return;

                const selected = scroll.querySelector('.folder-tree-item.active');
                if(!selected)
                    return;

                let scrollRect = scroll.getBoundingClientRect();
                let selectedRect = selected.getBoundingClientRect();
                const top = selectedRect.top - scrollRect.top - selectedRect.height * 2 + 1;
                scroll.scrollTo({behavior: "smooth", top: top});
            }, 10);
        }
    }
</script>

<template>
    <perfect-scrollbar class="folder-tree" ref="scroll">
        <loading :is-loading="asyncState.isLoading" :is-full-page="true">
            <FolderTreeItem v-for="f in folders" :folder="f" :key="f.path" :depth="0" :current-path="currentPath" />
        </loading>
    </perfect-scrollbar>
</template>

<style lang="scss">
    .folder-tree {
        flex: 1 1 auto;
    }
</style>