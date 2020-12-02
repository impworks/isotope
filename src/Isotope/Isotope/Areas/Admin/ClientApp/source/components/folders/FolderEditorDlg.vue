<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { Folder } from "../../vms/Folder";
import { Tag } from "../../vms/Tag";
import { FolderTitle } from "../../vms/FolderTitle";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { MediaType } from "../../../../../Common/source/vms/MediaType";

@Component
export default class FolderEditorDlg extends Mixins(HasAsyncState({isLoadingThumbs: false}), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop() folder: FolderTitle;
    @Prop() parent: FolderTitle;
    
    value: Folder = null;
    thumb: MediaThumbnail = null;
    tags: Tag[] = [];
    isThumbPickerOpen: boolean = false;
    thumbs: MediaThumbnail[] = [];
    
    createMore: boolean = false;
    result: boolean = false;
    
    $refs: {
        caption: HTMLInputElement;
    }
    
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
                    : { key: '', caption: '', slug: '', tags: [], thumbnailKey: '' };
                this.tags = await this.$api.tags.getList();
                if(this.value.thumbnailKey) {
                    const thumb = await this.$api.media.get(this.value.thumbnailKey);
                    this.thumb = {
                        thumbnailPath: thumb.thumbnailPath,
                        key: this.value.thumbnailKey,
                        uploadDate: '',
                        tags: 0,
                        type: MediaType.Photo
                    };
                }
            },
            'Failed to load data'
        );
    }
    
    async save() {
        await this.showSaving(
            async () => {
                if(this.isNew) {
                    await this.$api.folders.create(this.parent?.key, this.value);
                    this.$toast.success('Folder created');
                    
                    if(this.createMore) {
                        this.value = { key: '', caption: '', slug: '', tags: [], thumbnailKey: '' };
                        this.thumb = null;
                        this.result = true;
                        this.$refs.caption.focus();
                        return;
                    }
                } else {
                    await this.$api.folders.update(this.folder.key, this.value);
                    this.$toast.success('Folder updated');
                }
                
                this.$close(true);
            },
            this.isNew ? 'Failed to create folder' : 'Failed to update folder'
        )
    }
    
    async openThumbPicker() {
        if(this.isThumbPickerOpen)
            return;
        
        this.isThumbPickerOpen = true;
        await this.showProgress(
            'isLoadingThumbs',
            async () => this.thumbs = await this.$api.media.getList(this.folder.key),
            'Failed to load thumbnails'
        );
    }
    
    closeThumbPicker() {
        this.isThumbPickerOpen = false;
    }
    
    pickThumb(t: MediaThumbnail) {
        this.thumb = t;
        this.value.thumbnailKey = t?.key;
        this.isThumbPickerOpen = false;
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
                                    <div class="row">
                                        <div class="col-sm-9">
                                            <div class="form-group row">
                                                <label class="col-sm-4 col-form-label">Caption</label>
                                                <div class="col-sm-8">
                                                    <input type="text" class="form-control" v-model="value.caption" v-autofocus ref="caption" />
                                                </div>
                                            </div>
                                            <div class="form-group row">
                                                <label class="col-sm-4 col-form-label">URL Slug</label>
                                                <div class="col-sm-8">
                                                    <input type="text" class="form-control" v-model="value.slug" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-3" v-if="!isNew">
                                            <div class="clickable thumb-picker" :style="{'background-image': thumb ? 'url(' + thumb.thumbnailPath + ')' : 'none'}"
                                                 @click="openThumbPicker()" v-click-outside="closeThumbPicker" title="Pick thumbnail...">
                                                <div v-if="isThumbPickerOpen" class="popover bs-popover-bottom show thumb-picker-popover">
                                                    <div class="arrow"></div>
                                                    <h3 class="popover-header">
                                                        <span class="pull-left">Folder thumbnail</span>
                                                        <span class="pull-right clickable" v-if="value.thumbnailKey" @click="pickThumb(null)" title="Remove">
                                                            <span class="fa fa-fw fa-trash-o"></span>
                                                        </span>
                                                        <span class="clearfix"></span>
                                                    </h3>
                                                    <div class="popover-body">
                                                        <loading :is-loading="asyncState.isLoadingThumbs">
                                                            <div v-if="thumbs.length" class="thumb-picker-list">
                                                                <div v-for="t in thumbs" class="thumb-picker-item"
                                                                     :style="{'background-image': 'url(' + t.thumbnailPath + ')' }"
                                                                     @click="pickThumb(t)">
                                                                </div>
                                                            </div>
                                                            <div v-else class="text-muted">
                                                                This folder is empty.
                                                            </div>
                                                        </loading>
                                                    </div>
                                                </div>
                                            </div>
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
                                    <label class="mr-auto" v-if="isNew" title="Keep the dialog open to create another tag after saving">
                                        <input type="checkbox" v-model="createMore" /> Create one more
                                    </label>
                                    <button type="submit" class="btn btn-primary" :disabled="!canSave">
                                        <span v-if="asyncState.isSaving">Saving...</span>
                                        <span v-else>
                                            <span v-if="isNew">Create</span>
                                            <span v-else>Update</span>
                                        </span>
                                    </button>
                                    <button type="button" class="btn btn-secondary" @click.prevent="$close(result)">Cancel</button>
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

<style lang="scss">
.thumb-picker {
    width: 92px;
    height: 92px;
    border: 1px #ccc solid;
    background-size: cover;
    background-position: center center;
}

.popover.thumb-picker-popover {
    position: absolute;
    left: -155px;
    bottom: -295px;
    top: unset;
    width: 406px;
    max-width: 406px;
    height: 300px;

    .arrow {
        left: 204px;
        
        &:after {
            border-bottom-color: #f7f7f7;
        }
    }

    .popover-body {
        padding: 0.5rem 0 0 0.5rem;
        height: 260px;
        max-height: 260px;
        overflow: hidden;
        position: relative;
    }

    .thumb-picker-list {
        position: absolute;
        top: 0;
        left: 0;
        bottom: 0;
        right: -17px;
        padding: 8px 0 0 8px;
        overflow-y: scroll;

        .thumb-picker-item {
            display: inline-block;
            width: 92px;
            height: 92px;
            margin: 0 0.5em 0.5em 0;
            background-size: cover;
            background-position: center center;
        }
    }
}
</style>