<script lang="ts">
import { Component, Mixins} from "vue-property-decorator";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { FolderTitle } from "../../vms/FolderTitle";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { HasAsyncState } from "../mixins";

import FilePicker from "./FilePicker.vue";

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
    
    async upload(files: File[]) {
        if(!this.folder)
            return;
        
        await Promise.all(files.map(x => this.uploadOne(x)));
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
    
    async edit(m: MediaThumbnail) {
        alert('edit');
    }

    async editTags(m: MediaThumbnail) {
        alert('edit tags');
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
                    {{folder.caption}}: Upload
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
                                    <div class="card-text">
                                        <a class="clickable" @click.prevent="edit(u.result)" title="Edit">
                                            <span class="fa fa-edit"></span>
                                        </a>
                                        <a class="clickable" @click.prevent="editTags(u.result)" title="Edit tags">
                                            <span class="fa fa-tags"></span>
                                        </a>
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
    </loading>
</template>

<style scoped>
.upload-card {
    min-height: 200px;
}
</style>