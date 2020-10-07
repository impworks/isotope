<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { Dep } from "../../utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { FilterStateService } from "../../services/FilterStateService";
import { HasLifetime } from "../mixins/HasLifetime";
import { Folder } from "../../vms/Folder";
import { GalleryInfo } from "../../vms/GalleryInfo";

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
        
        path = path.replace(/\/$/, '');
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
    <div>
        <ul class="breadcrumbs">
            <li 
                class="breadcrumbs__item"
                v-for="c in crumbs"
            >
                <a 
                    href="#" 
                    @click.prevent="selectFolder(c)"
                >
                    {{c.caption}}
                </a>
            </li>
            <li class="breadcrumbs__item breadcrumbs__item_active">
                {{currentFolder}}
            </li>
        </ul>
        <div class="mobile-navigation">
            <div 
                class="mobile-navigation__actions"
            >
                <button 
                    class="btn-header"
                    v-if="crumbs.length"
                    @click.prevent="selectFolder(crumbs[crumbs.length - 1])"
                >
                    <div class="btn-header__content">
                        <i class="icon icon-back"></i>
                    </div>
                </button>
            </div>
            <div class="mobile-navigation__caption">
                {{currentFolder}}
            </div>
            <div class="mobile-navigation__actions">
                <button class="btn-header">
                    <div class="btn-header__content">
                        <i class="icon icon-share"></i>
                    </div>
                </button>
            </div>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .breadcrumbs {
        list-style-type: none;
        margin: 0;
        padding: 1rem;
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
        line-height: 1.625;
        
        @include media-breakpoint-down(sm) {
            display: none;
        }

        &__item {
            position: relative;
            padding: 0 2em 0 0;
            
            a {
                color: $gray-700;

                &:hover {
                    color: $gray-800;
                }
            }
            
            &:not(:last-child):after {
                top: 0;
                right: 0;
                width: 2em;
                content: '>';
                color: $gray-400;
                position: absolute;
                text-align: center;
                display: inline-block;
            }

            &_active {
                font-weight: 500;
            }
        }
    }

    .mobile-navigation {
        flex: 0 0 auto;
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: center;
        background: $gray-200;

        @include media-breakpoint-up(md) {
            display: none;
        }

        &__caption {
            font-weight: bold;
            flex: 1 1 auto;
            padding: 0 0.5rem;
            white-space: nowrap;
            overflow: hidden;
            text-align: center;
            text-overflow: ellipsis;

            &:first-child {
                padding-left: 1rem;
            }

            &:last-child {
                padding-right: 1rem;
            }
        }

        &__actions {
            min-width: 3.625rem;

            .btn-header {
                padding: 0.5rem 0.5rem;
                
                &__content {
                    background: none !important;
                }
            }

            &:first-child .btn-header {
                padding-left: 1rem;
            }

            &:last-child .btn-header {
                padding-right: 1rem;
            }
        }
    }
</style>