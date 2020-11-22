<script lang="ts">
import { Component, Mixins} from "vue-property-decorator";
import { create } from "vue-modal-dialogs";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { FolderTitle } from "../../vms/FolderTitle";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { HasAsyncState } from "../mixins";

import FilePicker from "./FilePicker.vue";
import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import MediaPropsEditorDlg from "./MediaPropsEditorDlg.vue";
import MediaTagsEditorDlg from "./MediaTagsEditorDlg.vue";
import MediaThumbEditorDlg from "./MediaThumbEditorDlg.vue";

const confirmation = create<{text: string}>(ConfirmationDlg);
const propsEditor = create<{mediaKey: string}>(MediaPropsEditorDlg);
const tagEditor = create<{mediaKey: string}>(MediaTagsEditorDlg);
const thumbEditor = create<{mediaKey: string}>(MediaThumbEditorDlg);

@Component({
    components: { FilePicker }
})
export default class MediaUploadPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;
    
    folder: FolderTitle = null;
    uploads: IUploadWrapper[] = [];
    
    async mounted() {
        const key = this.$route.params['key'];
        await this.showLoading(
            async () => this.folder = await this.$api.folders.get(key),
            'Failed to load folder info'
        );
    }
    
    upload(files: File[]) {
        if(!this.folder)
            return;
        
        for(let file of files)
            this.uploadOne(file); // sic! no await
    }
    
    async uploadOne(file: File) {
        const wrap: IUploadWrapper = { isUploading: true, result: null, error: null };
        this.uploads.push(wrap);
        
        try {
            wrap.result = await this.$api.media.upload(this.folder.key, file);
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

    editProps(m: MediaThumbnail) {
        propsEditor({ mediaKey: m.key });
    }

    editTags(m: MediaThumbnail) {
        tagEditor({ mediaKey: m.key });
    }

    editThumb(m: MediaThumbnail) {
        thumbEditor({ mediaKey: m.key });
    }

    async remove(u: IUploadWrapper) {
        const hint = 'Are you sure you want to remove this media?';
        if(!await confirmation({ text: hint }))
            return;

        await this.showSaving(
            async () => {
                await this.$api.media.remove(u.result.key);
                const idx = this.uploads.indexOf(u);
                this.uploads.splice(idx, 1);

                this.$toast.success('Media removed');
            },
            'Failed to remove media'
        );
    }
}

interface IUploadWrapper {
    isUploading: boolean;
    error?: string;
    result?: MediaThumbnail;
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
                    <FilePicker :multiple="true" @change="upload"></FilePicker>
                </div>
            </div>
            <div class="row row-cols-1 row-cols-sm-4">
                <div class="col mt-4" v-for="u in uploads">
                    <div class="card h-100 upload-card">
                        <loading :is-loading="u.isUploading" :is-full-page="true">
                            <template v-if="u.result">
                                <img :src="u.result.thumbnailPath" class="card-img-top" />
                                <div class="card-body">
                                    <div class="card-actions">
                                        <div class="pull-left">
                                            <a class="clickable" @click.prevent="editProps(u.result)" title="Edit">
                                                <span class="fa fa-fw fa-edit"></span>
                                            </a>
                                            <a class="clickable" @click.prevent="editThumb(u.result)" title="Edit thumbnail">
                                                <span class="fa fa-fw fa-crop"></span>
                                            </a>
                                            <a class="clickable" @click.prevent="editTags(u.result)" title="Edit tags">
                                                <span class="fa fa-fw fa-tags"></span>
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
                        </loading>
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