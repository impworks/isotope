<script lang="ts">
import { Component, Mixins, Watch } from "vue-property-decorator";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins";

import { create } from "vue-modal-dialogs";
import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { MediaListRequest } from "../../vms/MediaListRequest";

const confirmation = create<{text: string}>(ConfirmationDlg);

@Component
export default class MediaPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    media: MediaThumbnail[] = [];
    filter: MediaListRequest = null;
    folderTitle: string = null;

    async mounted() {
        const folder = this.$route.params['key'] || null;
        this.filter = {
            folder: folder,
            orderBy: "Order",
            orderDesc: false,
            page: 0
        };
    }

    async load() {
        await this.showLoading(
            async () => {
                if(this.filter.folder && !this.folderTitle) {
                    this.folderTitle = (await this.$api.folders.get(this.filter.folder)).caption;
                }
                this.media = await this.$api.media.getList(this.filter);
            },
            'Failed to load media!'
        );
    }
    
    @Watch('filter', { deep: true })
    onFilterChanged() {
        this.load();
    }

    async remove(m: MediaThumbnail) {
        const hint = 'Are you sure you want to remove this media?';
        if(!await confirmation({ text: hint }))
            return;

        await this.showSaving(
            async () => {
                await this.$api.media.remove(m.key);
                await this.load();

                this.$toast.success('Media removed');
            },
            'Failed to remove media'
        );
    }
    
    setPage(i: number) {
        const newPage = this.filter.page + i;
        if(newPage < 0) return;
        if(this.media.length === 0 && i > 0) return;
        this.filter.page = newPage;
    }
    
    async edit(m: MediaThumbnail) {
        alert('edit');
    }

    async editThumb(m: MediaThumbnail) {
        alert('editThumb');
    }
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <div class="mb-2">
            <h5 class="pull-left">
                Media
                <span v-if="folderTitle">({{folderTitle}})</span>
            </h5>
            <button v-if="filter.folder" class="btn btn-outline-secondary btn-sm pull-right" type="button" @click.prevent="upload()">
                <span class="fa fa-upload"></span> Upload media
            </button>
            <div class="clearfix"></div>
        </div>
        <table class="table table-bordered mb-0">
            <thead>
            <tr>
                <th></th>
                <th width="60%">Upload date</th>
                <th>Type</th>
                <th>Tags</th>
                <th width="1"></th>
            </tr>
            </thead>
            <tbody v-if="media.length === 0">
            <tr>
                <td colspan="5">
                    <div class="alert alert-info mb-0">
                        <span v-if="filter.page > 0">
                            No media on current page. Please go back.
                        </span>
                        <span v-else>
                            <span v-if="folderTitle">No media in this folder. Click "upload media" to create some.</span>
                            <span v-else>No media found. Please create a folder to upload it.</span>
                        </span>
                    </div>
                </td>
            </tr>
            </tbody>
            <tbody v-else>
                <tr v-for="m in media" v-action-row class="hover-actions">
                    <td class="media-thumb" :style="{'background-image': 'url(' + m.thumbnailPath + ')'}">
                    </td>
                    <td>{{m.uploadDate}}</td>
                    <td>
                        <span v-if="m.type === 1">Photo</span>
                        <span v-if="m.type === 2">Video</span>
                    </td>
                    <td>{{m.tags}}</td>
                    <td>
                        <a class="hover-action" @click.prevent="remove(m)" title="Remove">
                            <span class="fa fa-fw fa-remove"></span>
                        </a>
                        <a class="hover-action" @click.prevent="editThumb(m)" title="Update thumbnail">
                            <span class="fa fa-fw fa-crop"></span>
                        </a>
                        <a class="hover-action" @click.prevent="edit(m)" title="Edit">
                            <span class="fa fa-fw fa-edit"></span>
                        </a>
                    </td>
                </tr>
            </tbody>
        </table>
        <div class="mt-2">
            <div class="pull-right">
                <ul class="pagination">
                    <li class="page-item" :class="{disabled: filter.page === 0}">
                        <a class="page-link" href @click.prevent="setPage(-1)" title="Back">&laquo;</a>
                    </li>
                    <li class="page-item active">
                        <span class="page-link">{{filter.page + 1}}</span>
                    </li>
                    <li class="page-item" :class="{disabled: media.length === 0}">
                        <a class="page-link" href @click.prevent="setPage(1)" title="Forward">&raquo;</a>
                    </li>
                </ul>
            </div>
            <div class="clearfix"></div>
        </div>
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