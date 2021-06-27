<script lang="ts">
import { Component, Mixins, Watch } from "vue-property-decorator";
import { Dep } from "../../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../../services/ApiService";
import { FilterStateService, IFilterState } from "../../../services/FilterStateService";
import { HasAsyncState } from "../../mixins/HasAsyncState";
import { SearchScope } from "../../../../../../Common/source/vms/SearchScope";
import { ArrayHelper } from "../../../../../../Common/source/utils/ArrayHelper";
import { DateHelper } from "../../../../../../Common/source/utils/DateHelper";
import { DialogBase } from "../../utils/DialogBase";

import ModalWindow from "../../utils/ModalWindow.vue";

@Component({
    components: { ModalWindow }
})
export default class ShareLinkDialog extends Mixins(DialogBase, HasAsyncState()) {
    @Dep('$api') $api: ApiService;
    @Dep('$filter') $filter: FilterStateService;
    
    caption: string = null;
    tags: string[] = [];
    scope: string = null;
    dates: string = null;
    
    state: LinkCreationState = 'new';
    link: string = null;

    @Watch('model')
    onModelChanged() {
        this.isVisible = this.model;
        if(this.isVisible)
            this.load();
    }
    
    
    get canCreate(): boolean {
        return this.state == 'new'
            && !this.asyncState.isLoading
            && !this.asyncState.isSaving;
    }
    
    async load() {
        this.state = 'new';
        this.link = null;
        
        try {
            await this.showLoading(async () => {
                const state = this.$filter.state;
                this.tags = await this.getTagNames(state);
                this.scope = await this.getFolderName(state);
                this.dates = this.getDateDescription(state);
            });
        } catch (e) {
            console.log('Failed to load filter data', e);
            this.isVisible = false;
        }
    }
    
    async create() {
        if(!this.canCreate)
            return;
        
        try {
            await this.showSaving(async () => {
                const key = await this.$api.createSharedLink(this.caption, this.$filter.state);
                this.link = window.location.origin + '/?sh=' + key;
                this.state = 'succeeded';
            });
        } catch (e) {
            console.log('Link creation failed', e);
            this.state = 'failed';
        }
    }

    /**
     * Returns the folder name for filter display.
     */
    private async getFolderName(state: IFilterState): Promise<string> {
        const empty = this.$filter.isEmpty(state);
        const root = state.folder === '/';
        const tree = await this.$api.getFolderTree();
        const folder = ArrayHelper.findRecursive(tree, x => x.path === state.folder, x => x.subfolders);
        if(!folder && !root)
            throw Error('Folder not found');
        
        if(empty)
            return root ? 'Everywhere' : 'Folder "' + folder.caption + '" and subfolders';
        
        if(state.scope === SearchScope.Everywhere || (root && state.scope === SearchScope.CurrentFolderAndSubfolders))
            return 'Everywhere';
        
        if(root && state.scope === SearchScope.CurrentFolder)
            return "Root folder";
        
        return 'Folder "' + folder.caption + '"' + (state.scope === SearchScope.CurrentFolder ? '' : ' and subfolders');
    }

    /**
     * Returns the specified tag names.
     */
    private async getTagNames(state: IFilterState): Promise<string[]> {
        if(!state.tags || !state.tags.length)
            return [];
        
        const tags = await this.$api.getTags();
        return tags.filter(x => state.tags.indexOf(x.id) !== -1).map(x => x.caption);
    }

    /**
     * Returns formatted date values.
     */
    private getDateDescription(state: IFilterState): string {
        const parts: string[] = [];
        if(state.from)
            parts.push('From ' + DateHelper.format(state.from));
        if(state.to)
            parts.push('to ' + DateHelper.format(state.to));
        
        return parts.length ? parts.join(' ') : null;
    }
}

type LinkCreationState = 'new' | 'succeeded' | 'failed';
</script>

<template>
    <modal-window v-model="isVisible">
        <template v-slot:header>
            <div class="modal-window__header__caption">
                Link sharing
            </div>
            <div class="modal-window__header__actions">
                <button
                    href="#" 
                    class="btn-header" 
                    @click.prevent="isVisible = !isVisible"
                >
                    <div class="btn-header__content">
                        <i class="icon icon-cross"></i>
                    </div>
                </button>
            </div>
        </template>
        <template v-slot:content>
            <loading :is-loading="asyncState.isLoading" :is-full-page="true">
                <div v-if="state === 'new'">
                    <p>You are creating a public link with the following properties:</p>
                    <div class="row mb-3">
                        <label class="col-form-label col-sm-3">Caption</label>
                        <div class="col-sm-9">
                            <input type="text" class="form-control" v-model="caption" v-autofocus />
                        </div>
                    </div>
                    <div class="row mb-4">
                        <div class="col-sm-3">Scope</div>
                        <div class="col-sm-9">{{scope}}</div>
                    </div>
                    <div class="row mb-4" v-if="dates">
                        <div class="col-sm-3">Date range</div>
                        <div class="col-sm-9">{{dates}}</div>
                    </div>
                    <div class="row" v-if="tags && tags.length">
                        <div class="col-sm-3">Tags</div>
                        <div class="col-sm-9">
                            <ul class="mb-0 pl-3">
                                <li v-for="(tag, i) in tags" :key="i">{{tag}}</li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div v-if="state === 'succeeded'">
                    <p>Please copy the link:</p>
                    <div>
                        <input type="text" class="form-control" readonly="readonly" :value="link" />
                    </div>
                </div>
                <div v-if="state === 'failed'">
                    <div class="alert alert-danger mb-0">
                        <strong>Error</strong><br />
                        Link could not be created.
                    </div>
                </div>
            </loading>            
        </template>
        <template v-slot:footer>
            <button 
                v-if="state === 'new'"
                type="button" 
                class="btn btn-block btn-primary"
                :disabled="!canCreate"
                @click.prevent="create"
            >
                <span v-if="asyncState.isSaving">Creating...</span>
                <span v-else>Create</span>
            </button>
            <button
                v-if="state !== 'new'"
                type="button"
                class="btn btn-block btn-primary"
                @click.prevent="close"
            >
                Close
            </button>
        </template>
    </modal-window>
</template>