<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { create } from "vue-modal-dialogs";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { Folder } from "../../vms/Folder";
import { ArrayHelper } from "../../../../../Common/source/utils/ArrayHelper";

import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import MediaEditorDlg from "./MediaEditorDlg.vue";
import FilePicker from "./FilePicker.vue";

const confirmation = create<{text: string}>(ConfirmationDlg);
const mediaEditor = create<{mediaKey: string, otherMedia: MediaThumbnail[], tabKey: MediaEditorDlgTab}>(MediaEditorDlg);

@Component({
    components: { FilePicker }
})
export default class MediaPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    mode: Mode = 'View';
    
    media: MediaThumbnail[] = [];
    mediaWraps: MediaWrapper[] = [];
    folder: Folder = null;
    
    mediaReorder: MediaThumbnail[] = [];
    
    batchSize: number = null;
    batchIndex: number = null;

    async mounted() {
        const key = this.$route.params['key'];
        await this.showLoading(
            async () => this.folder = await this.$api.folders.get(key),
            'Folder was not found!'
        );
        
        if(this.folder)
            await this.load(true);
    }

    async load(showPreloader: boolean = false) {
        await this.showProgress(
            showPreloader ? 'isLoading' : 'isWorking',
            async () => {
                this.media = await this.$api.media.getList(this.folder.key);
                this.mediaWraps = this.media.map(x => this.wrap(x));
            },
            'Failed to load media!'
        );
    }

    async remove(m: MediaWrapper) {
        const hint = 'Are you sure you want to remove this media?';
        if(!await confirmation({ text: hint }))
            return;

        await this.showSaving(
            async () => {
                await this.$api.media.remove(m.media.key);
                
                ArrayHelper.removeItem(this.mediaWraps, m);
                ArrayHelper.removeItem(this.media, m.media);

                this.$toast.success('Media removed');
            },
            'Failed to remove media'
        );
    }
    
    async edit(m: MediaWrapper, tab: MediaEditorDlgTab) {
        if(await mediaEditor({ mediaKey: m.media.key, otherMedia: this.media, tabKey: tab }))
            await this.load();
    }
    
    startReorder() {
        this.mode = 'Reorder';
        this.mediaReorder = [...this.media];
        this.mediaWraps = this.mediaWraps.filter(x => !!x.media);
    }
    
    async confirmReorder() {
        if(this.mode !== 'Reorder')
            return;
        
        await this.showSaving(
            async () => {
                await this.$api.media.reorder(this.folder.key, this.mediaReorder.map(x => x.key));
                
                this.media = this.mediaReorder;
                this.mediaWraps = this.media.map(x => this.wrap(x));
                this.mediaReorder = [];
                
                this.$toast.success('Media order updated');
            },
            'Failed to update media order'
        );
        
        this.mode = 'View';
    }
    
    async cancelReorder() {
        if(this.mode !== 'Reorder')
            return;
        
        this.mediaReorder = [];
        this.mode = 'View';
    }
    
    async startUpload() {
        this.mode = 'Upload';
    }
    
    async confirmUpload(files: File[]) {
        try {
            this.batchSize = files.length;
            this.batchIndex = 1;

            for (let file of files) {
                await this.uploadOne(file);
                this.batchIndex++;
            }
        } finally {
            this.cancelUpload();
        }
    }
    
    async cancelUpload() {
        this.mode = 'View';
        this.batchSize = this.batchIndex = null;
    }

    async uploadOne(file: File) {
        const wrap: MediaWrapper = this.wrap(null);
        this.mediaWraps.push(wrap);

        try {
            wrap.isUploading = true;
            const media = wrap.media = await this.$api.media.upload(this.folder.key, file, pc => wrap.progress = pc);
            if(media)
                this.media.push(media);
        } catch (e) {
            if(e.response?.status === 400) {
                wrap.error = e.response.data.error;
            } else {
                wrap.error = e;
            }
        } finally {
            // uncomment this when debugging finishes
            //wrap.isUploading = false;
        }
    }
    
    private wrap(m: MediaThumbnail): MediaWrapper {
        return {
            media: m,
            isUploading: false,
            progress: null,
            error: null
        };
    }
}

type Mode = 'View' | 'Reorder' | 'Upload';

type MediaWrapper = {
    media: MediaThumbnail;
    progress: number;
    error: string;
    isUploading: boolean;
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <template v-if="folder">
            <div class="mb-2">
                <h5 class="pull-left">
                    {{folder.caption}}
                </h5>
                <div class="pull-right" v-if="mode === 'View'">
                    <button class="btn btn-outline-secondary btn-sm mr-2" type="button" @click.prevent="startReorder" :disabled="!media || media.length < 2">
                        <span class="fa fa-sort"></span> Reorder
                    </button>
                    <button class="btn btn-outline-secondary btn-sm" type="button" @click.prevent="startUpload">
                        <span class="fa fa-upload"></span> Upload
                    </button>
                </div>
                <div class="pull-right" v-if="mode === 'Reorder'">
                    <button class="btn btn-outline-primary btn-sm mr-2" type="button" @click.prevent="confirmReorder">
                        <span class="fa fa-check"></span> Save
                    </button>
                    <button class="btn btn-outline-secondary btn-sm" type="button" @click.prevent="cancelReorder">
                        <span class="fa fa-times"></span> Cancel
                    </button>
                </div>
                <div class="pull-right" v-if="mode === 'Upload'">
                    <button class="btn btn-outline-secondary btn-sm" type="button" @click.prevent="cancelUpload">
                        <span class="fa fa-times"></span> Cancel
                    </button>
                </div>
                <div class="clearfix"></div>
            </div>
            
            <FilePicker v-if="mode === 'Upload'" :multiple="true" @change="confirmUpload" :disabled="!!batchSize" class="mb-4">
                <div v-if="!batchSize">
                    <i class="fa fa-upload"></i> Drag files here or click for file picker
                </div>
                <div v-else>
                    <loading :is-loading="true" :text="'Uploading ' + batchIndex + ' of ' + batchSize + '...'"></loading>
                </div>
            </FilePicker>
            
            <div class="media-thumb-list">
                <template v-if="media.length > 0">
                    <template v-if="mode === 'Reorder'">
                        <Draggable v-model="mediaReorder" ghost-class="media-thumb-ghost">
                            <div v-for="m in mediaReorder"
                                 class="media-thumb mr-2 media-thumb-reorder"
                                 :style="{'background-image': 'url(' + m.thumbnailPath + ')'}">
                            </div>
                        </Draggable>
                    </template>
                    <template v-if="mode === 'View' || mode === 'Upload'">
                        <template v-for="w in mediaWraps">
                            <div v-if="w.isUploading" class="media-thumb-ghost mr-2">
                                <p>{{ w.progress }}%</p>
                            </div>
                            <template v-else>
                                <div v-if="w.error" class="media-thumb mr-2">
                                    <div class="alert alert-danger mb-0">
                                        <span class="fa fa-exclamation-circle"></span>
                                        <span>{{ w.error }}</span>
                                    </div>
                                </div>
                                <div v-else
                                     v-action-row
                                     class="media-thumb mr-2 hover-actions"
                                     :style="{'background-image': 'url(' + w.media.thumbnailPath + ')'}"
                                     @contextmenu.prevent="$refs.menu.open($event, w)">
                                    <a class="hover-action" @click.stop="$refs.menu.open($event, w)">
                                        <span class="fa fa-fw fa-ellipsis-v"></span>
                                    </a>
                                </div>
                            </template>
                        </template>
                    </template>
                </template>
                <div v-if="media.length === 0" class="alert alert-info mb-0">
                    No media in this folder. Click "upload media" to create some.
                </div>
            </div>
        </template>
        <template v-else>
            <div class="alert alert-danger mb-0">Folder does not exist.</div>
        </template>
        <portal to="context-menu">
            <context-menu ref="menu" v-slot="{data}">
                <a class="dropdown-item clickable" @click.prevent="edit(data, 'props')">
                    <span class="fa fa-fw fa-edit"></span> Edit properties
                </a>
                <a class="dropdown-item clickable" @click.prevent="edit(data, 'tags')">
                    <span class="fa fa-fw fa-tags"></span> Edit tags
                </a>
                <a class="dropdown-item clickable" @click.prevent="edit(data, 'thumb')">
                    <span class="fa fa-fw fa-crop"></span> Update thumbnail
                </a>
                <div class="dropdown-divider"></div>
                <a class="dropdown-item clickable" @click.prevent="remove(data)">
                    <span class="fa fa-fw fa-remove"></span> Remove
                </a>
            </context-menu>
        </portal>
    </loading>
</template>

<style scoped lang="scss">
    .media-thumb-list {
        margin-right: -0.6rem;
    }
    
    .media-thumb {
        display: inline-block;
        width: 162px;
        height: 162px;
        background-position: center center;
        background-size: cover;
        position: relative;
        
        .hover-action {
            display: inline-block;
            width: 30px;
            height: 30px;
            background: rgba(100%, 100%, 100%, 70%);
            box-shadow: rgba(0,0,0, 70%) 0 0 5px;
            border-radius: 15px;
            position: absolute;
            top: 5px;
            right: 5px;
            vertical-align: center;
            
            .fa {
                vertical-align: top;
                margin-top: 3px;
            }
        }
    }
    
    .media-thumb-ghost {
        display: inline-block;
        width: 162px;
        height: 162px;
        border: 1px #ccc solid;
        background-image: none !important;
        padding: 8px;
    }
    
    .media-thumb-reorder {
        cursor: move;
    }
    
    .upload-progress {
        height: 48px;
    }
</style>