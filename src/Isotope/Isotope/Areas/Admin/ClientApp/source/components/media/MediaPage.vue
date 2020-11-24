<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { create } from "vue-modal-dialogs";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { DateHelper } from "../../../../../Common/source/utils/DateHelper";
import { FolderTitle } from "../../vms/FolderTitle";

import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import MediaPropsEditorDlg from "./MediaPropsEditorDlg.vue";
import MediaTagsEditorDlg from "./MediaTagsEditorDlg.vue";
import MediaThumbEditorDlg from "./MediaThumbEditorDlg.vue";
import MediaOrderEditorDlg from "./MediaOrderEditorDlg.vue";

const confirmation = create<{text: string}>(ConfirmationDlg);
const propsEditor = create<{mediaKey: string}>(MediaPropsEditorDlg);
const tagEditor = create<{mediaKey: string}>(MediaTagsEditorDlg);
const thumbEditor = create<{mediaKey: string}>(MediaThumbEditorDlg);
const orderEditor = create<{folderKey: string}>(MediaOrderEditorDlg);

@Component
export default class MediaPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    media: MediaThumbnail[] = [];
    folder: FolderTitle = null;

    async mounted() {
        const key = this.$route.params['key'];
        await this.showLoading(
            async () => this.folder = await this.$api.folders.get(key),
            'Folder was not found!'
        );
        
        if(this.folder)
            await this.load();
    }

    async load() {
        await this.showLoading(
            async () => this.media = await this.$api.media.getList(this.folder.key),
            'Failed to load media!'
        );
    }

    async remove(m: MediaThumbnail) {
        const hint = 'Are you sure you want to remove this media?';
        if(!await confirmation({ text: hint }))
            return;

        await this.showSaving(
            async () => {
                await this.$api.media.remove(m.key);
                const idx = this.media.indexOf(m);
                this.media.splice(idx, 1);

                this.$toast.success('Media removed');
            },
            'Failed to remove media'
        );
    }
    
    async editProps(m: MediaThumbnail) {
        if(await propsEditor({ mediaKey: m.key }))
            await this.load();
    }
    
    async editTags(m: MediaThumbnail) {
      if(await tagEditor({ mediaKey: m.key }))
          await this.load();
    }

    async editThumb(m: MediaThumbnail) {
        if(await thumbEditor({ mediaKey: m.key }))
            await this.load();
    }
    
    async reorder() {
        if(await orderEditor({ folderKey: this.folder.key }))
            await this.load();
    }
    
    formatDate(d: string) {
        return DateHelper.formatFull(d);
    }
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
                    <button class="btn btn-outline-secondary btn-sm mr-2" type="button" @click.prevent="reorder()" :disabled="!media || media.length < 2">
                        <span class="fa fa-sort"></span> Reorder
                    </button>
                    <router-link class="btn btn-outline-secondary btn-sm" :to="'/folders/' + folder.key + '/upload'">
                        <span class="fa fa-upload"></span> Upload
                    </router-link>
                </div>
                <div class="clearfix"></div>
            </div>
            <table class="table table-bordered mb-0">
                <thead>
                <tr>
                    <th></th>
                    <th>Type</th>
                    <th width="60%">Upload date</th>
                    <th>Tags</th>
                    <th width="1"></th>
                </tr>
                </thead>
                <tbody v-if="media.length === 0">
                <tr>
                    <td colspan="5">
                        <div class="alert alert-info mb-0">
                            No media in this folder. Click "upload media" to create some.
                        </div>
                    </td>
                </tr>
                </tbody>
                <tbody v-else>
                    <tr v-for="m in media" v-action-row class="hover-actions">
                        <td class="media-thumb" :style="{'background-image': 'url(' + m.thumbnailPath + ')'}">
                        </td>
                        <td>
                            <span v-if="m.type === 1">Photo</span>
                            <span v-if="m.type === 2">Video</span>
                        </td>
                        <td>{{formatDate(m.uploadDate)}}</td>
                        <td>{{m.tags}}</td>
                        <td>
                            <a class="hover-action" @click.prevent="remove(m)" title="Remove">
                                <span class="fa fa-fw fa-remove"></span>
                            </a>
                            <a class="hover-action" @click.prevent="editThumb(m)" title="Update thumbnail">
                                <span class="fa fa-fw fa-crop"></span>
                            </a>
                            <a class="hover-action" @click.prevent="editTags(m)" title="Update tags">
                                <span class="fa fa-fw fa-tags"></span>
                            </a>
                            <a class="hover-action" @click.prevent="editProps(m)" title="Edit">
                                <span class="fa fa-fw fa-edit"></span>
                            </a>
                        </td>
                    </tr>
                </tbody>
            </table>
        </template>
        <template v-else>
            <div class="alert alert-danger mb-0">Folder does not exist.</div>
        </template>
    </loading>
</template>

<style scoped lang="scss">
    .media-thumb {
        width: 48px;
        height: 48px;
        background-position: center center;
        background-size: cover;
    }
</style>