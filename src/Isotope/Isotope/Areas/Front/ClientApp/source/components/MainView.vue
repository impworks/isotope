<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { HasAsyncState } from "./mixins/HasAsyncState";
import { Dep } from "../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../services/ApiService";
import { FilterStateService } from "../services/FilterStateService";
import { Folder } from "../vms/Folder";
import Gallery from './content/Gallery.vue';
import MainHeader from './content/MainHeader.vue';
import Sidebar from './sidebar/Sidebar.vue';
import GalleryHeader from './content/GalleryHeader.vue';

@Component({
    components: { MainHeader, GalleryHeader, Gallery, Sidebar,  }
})
export default class MainView extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;
    @Dep('$filter') $filter: FilterStateService;

    folderTree: Folder[] = [];
    
    async mounted() {
        try {
            this.showLoading(async () => {
                this.folderTree = await this.$api.getFolderTree();
            })
        } catch (e) {
        }
    }

    get isSidebarHidden() {
        return this.$filter.shareId !== null 
            && this.folderTree.length == 0
            && !this.asyncState.isLoading;
    }
}
</script>

<template>
    <div 
        class="main-view"
        :class="{'main-view_no-sidebar': isSidebarHidden}"
    >
        <div class="main-view__header">
            <main-header></main-header>
            <gallery-header ></gallery-header>
        </div>
        <div class="main-view__content">
            <sidebar></sidebar>
            <gallery></gallery>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../../Common/styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";
    
    .main-view {
        display: flex;
        min-height: 0;
        flex-direction: column;

        @include media-breakpoint-down(md) {
            padding-top: 3.6875rem;
            min-height: 100%;
        }

        @include media-breakpoint-up(lg) {
            height: 100%;
            overflow: hidden;
        }

        &_no-sidebar {

            .main-header {
                flex: 1 0 auto;
                border-right: none;
            }

            .gallery-header {
                display: none;
            }

            .sidebar {
                display: none;
            }
        }

        &__header {
            flex: 0 0 auto;
            display: flex;

            @include media-breakpoint-down(md) {
                flex-direction: column;
            }

            @include media-breakpoint-up(lg) {
                flex-direction: row;
                border-bottom: 1px solid $gray-300;
            }

            .gallery-header {
                flex: 1 0 auto;
            }
        }

        &__content {
            flex: 1 0 auto;
            display: flex;
            
            @include media-breakpoint-down(md) {
                flex-direction: column;
            }

            @include media-breakpoint-up(lg) {
                flex-direction: row;
            }
        }
    }
</style>
