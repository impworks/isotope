<script lang="ts">
    import { Component, Mixins } from "vue-property-decorator";
    import { Dep } from "../utils/VueInjectDecorator";
    import { ApiService } from "../services/ApiService";
    import FilterStateService from "../services/FilterStateService";
    import { HasLifetime } from "./mixins/HasLifetime";
    import { Folder } from "../vms/Folder";
    import { GalleryInfo } from "../vms/GalleryInfo";

    @Component
    export default class Breadcrumbs extends Mixins(HasLifetime) {
        @Dep('$api') $api: ApiService;
        @Dep('$filter') $filter: FilterStateService;
        
        info: GalleryInfo = null;
        folderTree: Folder[] = [];
        crumbs: Crumb[] = [];
        currentFolder: string = null;
        
        async mounted() {
            try {
                this.info = await this.$api.getInfo();
                this.folderTree = await this.$api.getFolderTree();
            } catch (e) {
            }
            
            this.observe(this.$filter.onStateChanged, x => this.update(x.folder))
            this.update(this.$filter.state.folder);            
        }
        
        update(path: string) {
            if(path === '/' || !path) {
                this.crumbs = [];
                this.currentFolder = this.info.caption;
                return;
            }
            
            const parts = path.split('/');
            const crumbs: Crumb[] = [{ caption: this.info.caption, path: '/' }];
            let scope = this.folderTree;
            let tmpPath = '';
            for(let i = 1; i < parts.length - 1; i++) {
                tmpPath += '/' + parts[i];
                const sub = scope.find(x => x.path === tmpPath);
                if(!sub) return;
                crumbs.push({ caption: sub.caption, path: sub.path });
                scope = sub.subfolders || [];
            }
            
            this.crumbs = crumbs;
            this.currentFolder = scope.find(x => x.path == path)?.caption || 'Not found';
        }
        
        selectFolder(crumb: Crumb) {
            this.$filter.update('breadcrumb', {folder: crumb.path});
        }
    } 
    
    interface Crumb {
        caption: string;
        path: string;
    }
</script>

<template>
    <ul class="breadcrumbs">
        <li v-for="c in crumbs" class="breadcrumbs__item">
            <a href="#" @click.prevent="selectFolder(c)">{{c.caption}}</a>
        </li>
        <li class="breadcrumbs__item breadcrumbs__item_active">
            {{currentFolder}}
        </li>
    </ul>
</template>

<style lang="scss">
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";
    
    .breadcrumbs {
        list-style-type: none;
        margin: 0;
        padding: 1rem;
        display: flex;
        flex-direction: row;
        line-height: 1.625;

        &__item {
            position: relative;
            padding: 0 2em 0 0;
            
            &:not(:last-child):after {
                top: 0;
                right: 0;
                width: 2em;
                content: '>';
                color: $gray-500;
                position: absolute;
                text-align: center;
                display: inline-block;
            }

            &_active {
                font-weight: 500;
            }
        }
    }
</style>