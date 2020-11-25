<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { Folder } from "../../vms/Folder";
import { Tag } from "../../vms/Tag";
import { FolderTitle } from "../../vms/FolderTitle";

@Component
export default class FolderEditorDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop() folder: FolderTitle;
    @Prop() parent: FolderTitle;
    
    value: Folder = null;
    tags: Tag[] = [];
    
    get isNew() {
        return !this.folder;
    }
    
    get canSave() {
        return !this.asyncState.isSaving
        && !this.asyncState.isLoading
        && !!this.value.caption
        && !!this.value.slug;
    }
    
    async mounted() {
        await this.showLoading(
            async () => {
                this.value = this.folder
                    ? await this.$api.folders.get(this.folder.key)
                    : { caption: '', slug: '', tags: [], thumbnailKey: '' };
                this.tags = await this.$api.tags.getList();
            },
            'Failed to load data'
        );
    }
    
    async save() {
        await this.showSaving(
            async () => {
                if(this.isNew) {
                    await this.$api.folders.create(this.parent.key, this.value);
                    this.$toast.success('Folder created');
                } else {
                    await this.$api.folders.update(this.folder.key, this.value);
                    this.$toast.success('Folder updated');
                }
                
                this.$close(true);
            },
            this.isNew ? 'Failed to create folder' : 'Failed to update folder'
        )
    }
}
</script>

<template>
    <div>
        <div class="modal fade show">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <loading :is-loading="asyncState.isLoading" :is-full-page="true">
                            <form @submit.prevent="save()" v-if="value">
                                <div class="modal-header">
                                    <h5 class="modal-title">
                                        <span v-if="isNew">Create folder</span>
                                        <span v-else>Update folder '{{folder.caption}}'</span>
                                    </h5>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group row" v-if="parent">
                                        <label class="col-sm-3 col-form-label">Parent</label>
                                        <div class="col-sm-9">
                                            <input type="text" readonly class="form-control-plaintext" :value="parent.caption" />
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label class="col-sm-3 col-form-label">Caption</label>
                                        <div class="col-sm-9">
                                            <input type="text" class="form-control" v-model="value.caption" v-autofocus />
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label class="col-sm-3 col-form-label">URL Slug</label>
                                        <div class="col-sm-9">
                                            <input type="text" class="form-control" v-model="value.slug" />
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label class="col-sm-3 col-form-label">Tags</label>
                                        <div class="col-sm-9">
                                            <v-select multiple :options="tags" v-model="value.tags" label="caption" :reduce="x => x.id">
                                                <template slot="no-options">No tags created yet.</template>                                       
                                            </v-select>
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="submit" class="btn btn-primary" :disabled="!canSave">
                                        <span v-if="asyncState.isSaving">Saving...</span>
                                        <span v-else>
                                            <span v-if="isNew">Create</span>
                                            <span v-else>Update</span>
                                        </span>
                                    </button>
                                    <button type="button" class="btn btn-secondary" @click.prevent="$close(false)">Cancel</button>
                                </div>
                            </form>
                        </loading>
                    </div>
                </div>
        </div>
        <div class="modal-backdrop show fade">
        </div>
    </div>
</template>