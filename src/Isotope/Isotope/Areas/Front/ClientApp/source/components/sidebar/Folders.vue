<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { HasAsyncState } from "../mixins/HasAsyncState";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { Folder } from "../../vms/Folder";
import { FilterStateService } from "../../services/FilterStateService";
import { HasLifetime } from "../mixins/HasLifetime";

import FolderTreeItem from "./FolderTreeItem.vue";

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
            const scroll = this.$refs.scroll.scrollElement as HTMLElement;
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
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <simplebar class="folder-tree" ref="scroll">
            <div 
                class="alert alert-warning m-3"
                v-if="folders && folders.length === 0" 
            >
                <span>There are no folders yet.</span>
            </div>
            <FolderTreeItem v-for="f in folders" :folder="f" :key="f.path" :depth="0" :current-path="currentPath" />
        </simplebar>
    </loading>
</template>

<style lang="scss">
    @import "../../../../../Common/styles/variables";

    .folder-tree {
        height: 0;
        flex: 1 1 auto;
    }
</style>