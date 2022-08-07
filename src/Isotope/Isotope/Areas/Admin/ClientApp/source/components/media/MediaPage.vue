<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { create } from "vue-modal-dialogs";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { DateHelper } from "../../../../../Common/source/utils/DateHelper";
import { Folder } from "../../vms/Folder";

import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import MediaOrderEditorDlg from "./MediaOrderEditorDlg.vue";
import MediaEditorDlg from "./MediaEditorDlg.vue";

const confirmation = create<{text: string}>(ConfirmationDlg);
const mediaEditor = create<{mediaKey: string, otherMedia: MediaThumbnail[], tabKey: MediaEditorDlgTab}>(MediaEditorDlg);
const orderEditor = create<{folderKey: string}>(MediaOrderEditorDlg);

@Component
export default class MediaPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    media: MediaThumbnail[] = [];
    folder: Folder = null;

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
    
    async edit(m: MediaThumbnail, tab: MediaEditorDlgTab) {
        if(await mediaEditor({ mediaKey: m.key, otherMedia: this.media, tabKey: tab }))
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
            <div class="media-thumb-list">
                <div v-for="m in media" v-action-row
                     class="media-thumb mr-2 hover-actions" :style="{'background-image': 'url(' + m.thumbnailPath + ')'}"
                     @contextmenu.prevent="$refs.menu.open($event, m)">
                    <a class="hover-action" @click.stop="$refs.menu.open($event, m)">
                        <span class="fa fa-fw fa-ellipsis-v"></span>
                    </a>
                </div>
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
</style>