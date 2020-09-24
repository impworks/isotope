<script lang="ts">
    import { Component, Mixins } from "vue-property-decorator";
    import { HasAsyncState } from "./mixins/HasAsyncState";
    import { ClientApiService } from "../services/ClientApiService";
    import { Dep } from "../utils/VueInjectDecorator";
    import { Folder } from "../vms/Folder";
    import FolderTreeItem from "./FolderTreeItem.vue";
    @Component({
        components: { FolderTreeItem }
    })
    export default class Folders extends Mixins(HasAsyncState()) {
        @Dep('$api') $api: ClientApiService;
        
        folders: Folder[] = null;
        
        async mounted() {
            try {
                await this.showLoading(async () => {
                    this.folders = await this.$api.getFolderTree();
                    setTimeout(() => this.scrollToSelected(), 10);
                })
            } catch (e) {
                console.log('Failed to get folders.');
            }
        }
        
        private scrollToSelected() {
            const scroll = this.$refs.scroll.$el as HTMLElement;
            if(scroll.scrollHeight <= scroll.clientHeight)
                return;
            
            const selected = scroll.querySelector('.folder.active');
            if(!selected)
                return;
            
            let scrollRect = scroll.getBoundingClientRect();
            let selectedRect = selected.getBoundingClientRect();
            const top = selectedRect.top - scrollRect.top - selectedRect.height * 2 + 1;
            scroll.scrollTo({behavior: "smooth", top: top});
        }
    }
</script>

<template>
    <perfect-scrollbar class="folders" ref="scroll">
        <loading :is-loading="asyncState.isLoading" :is-full-page="true">
            <FolderTreeItem v-for="f in folders" :folder="f" :key="f.path" :depth="0" />
        </loading>
    </perfect-scrollbar>
</template>

<style lang="scss">
.folders {
    flex: 1 1 auto;
}
</style>