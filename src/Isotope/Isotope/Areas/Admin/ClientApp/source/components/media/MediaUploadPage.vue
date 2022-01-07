<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { create } from "vue-modal-dialogs";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { Folder } from "../../vms/Folder";
import { HasAsyncState } from "../mixins";
import { ArrayHelper } from "../../../../../Common/source/utils/ArrayHelper";

import FilePicker from "./FilePicker.vue";
import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import MediaEditorDlg from "./MediaEditorDlg.vue";

const confirmation = create<{text: string}>(ConfirmationDlg);
const mediaEditor = create<{mediaKey: string, otherMedia: MediaThumbnail[], tabKey: MediaEditorDlgTab}>(MediaEditorDlg);

@Component({
    components: { FilePicker }
})
export default class MediaUploadPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;
    
    folder: Folder = null;
    batchSize: number = null;
    batchIndex: number = 1;
    uploads: IUploadWrapper[] = [];
    media: MediaThumbnail[] = [];
    
    async mounted() {
        const key = this.$route.params['key'];
        await this.showLoading(
            async () => this.folder = await this.$api.folders.get(key),
            'Failed to load folder info'
        );
    }
    
    async upload(files: File[]) {
        if(!this.folder)
            return;
        
        try {
            this.batchSize = files.length;
            this.batchIndex = 1;
            
            for (let file of files) {
                await this.uploadOne(file);
                this.batchIndex++;
            }
        } finally {
            this.batchSize = null;
        }
    }
    
    async uploadOne(file: File) {
        const wrap: IUploadWrapper = { isUploading: true, result: null, error: null, progress: 0 };
        this.uploads.push(wrap);
        
        try {
            wrap.result = await this.$api.media.upload(this.folder.key, file, pc => wrap.progress = pc);
            if(wrap.result)
                this.media.push(wrap.result);
        } catch (e) {
            if(e.response?.status === 400) {
                wrap.error = e.response.data.error;
            } else {
                wrap.error = e;
            }
        } finally {
            wrap.isUploading = false;
        }
    }

    async edit(m: MediaThumbnail, tab: MediaEditorDlgTab) {
        const result = await mediaEditor({ mediaKey: m.key, otherMedia: this.media, tabKey: tab });
        
        if(!result)
            return;
        
        // refresh all thumbnails because it's impossible to know which ones have been modified
        for(let m of this.media) {
            const newNonce = Math.ceil(Math.random() * 10000);
            m.thumbnailPath = m.thumbnailPath.replace(/nonce=([0-9]+)$/, 'nonce=' + newNonce);
        }
    }

    async remove(u: IUploadWrapper) {
        const hint = 'Are you sure you want to remove this media?';
        if(!await confirmation({ text: hint }))
            return;

        await this.showSaving(
            async () => {
                await this.$api.media.remove(u.result.key);
                
                ArrayHelper.removeItem(this.media, u.result);
                ArrayHelper.removeItem(this.uploads, u);

                this.$toast.success('Media removed');
            },
            'Failed to remove media'
        );
    }
}

interface IUploadWrapper {
    isUploading: boolean;
    progress: number;
    error: string;
    result: MediaThumbnail;
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <template v-if="folder">
            <div class="mb-2">
                <h5 class="pull-left">
                    {{folder.caption}}
                </h5>
                <div class="pull-right">
                    <router-link :to="'/folders/' + folder.key" :exact="true" class="btn btn-outline-secondary btn-sm">
                        <span class="fa fa-backward"></span> Back to media list
                    </router-link>
                </div>
                <div class="clearfix"></div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <FilePicker :multiple="true" @change="upload" :disabled="!!batchSize">
                        <div v-if="!batchSize">
                            <i class="fa fa-upload"></i> Upload
                        </div>
                        <div v-else>
                            <loading :is-loading="true" :text="'Uploading ' + batchIndex + ' of ' + batchSize + '...'"></loading>
                        </div>
                    </FilePicker>
                </div>
            </div>
            <div class="row row-cols-1 row-cols-sm-4">
                <div class="col mt-4" v-for="u in uploads">
                    <div class="card h-100 upload-card">
                        <template v-if="u.isUploading">
                            <div class="card-body">
                                <div class="progress upload-progress">
                                    <div class="progress-bar progress-bar-animated progress-bar-striped" :style="{width: u.progress + '%'}"></div>
                                </div>
                            </div>
                        </template>
                        <template v-else>
                            <template v-if="u.result">
                                <img :src="u.result.thumbnailPath" class="card-img-top" />
                                <div class="card-body">
                                    <div class="card-actions">
                                        <div class="pull-left">
                                            <a class="clickable" @click.prevent="edit(u.result, 'props')" title="Edit properties">
                                                <span class="fa fa-fw fa-edit"></span>
                                            </a>
                                            <a class="clickable" @click.prevent="edit(u.result, 'tags')" title="Edit tags">
                                                <span class="fa fa-fw fa-tags"></span>
                                            </a>
                                            <a class="clickable" @click.prevent="edit(u.result, 'thumb')" title="Edit thumbnail">
                                                <span class="fa fa-fw fa-crop"></span>
                                            </a>
                                        </div>
                                        <div class="pull-right">
                                            <a class="clickable" @click.prevent="remove(u)" title="Remove">
                                                <span class="fa fa-fw fa-remove"></span>
                                            </a>
                                        </div>
                                        <div class="clearfix"></div>
                                    </div>
                                </div>
                            </template>
                            <template v-else>
                                <div class="card-body">
                                    <div class="alert alert-danger mb-0">
                                        <strong>Upload error:</strong><br/>
                                        <span>{{u.error}}</span>
                                    </div>
                                </div>
                            </template>
                        </template>
                    </div>
                </div>
            </div>
        </template>
        <template v-else>
            <div class="alert alert-danger mb-0">
                Folder does not exist.
            </div>
        </template>
    </loading>
</template>

<style lang="scss">
.upload-progress {
    height: 48px;
}

.upload-card {
    min-height: 200px;
    
    .card-body {
        padding: 0.5rem 1rem;
    }

    .card-actions {
        font-size: 24px;
    }
}
</style>