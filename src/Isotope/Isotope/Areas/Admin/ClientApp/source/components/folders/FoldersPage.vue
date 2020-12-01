<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins";
import { FolderTitle } from "../../vms/FolderTitle";

import { create } from "vue-modal-dialogs";

import FolderRow from "./FolderRow.vue";
import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import FolderEditorDlg from "./FolderEditorDlg.vue";
import FolderMoveDlg from "./FolderMoveDlg.vue";

const confirmation = create<{text: string}>(ConfirmationDlg);
const editDlg = create<{folder: FolderTitle, parent: FolderTitle}>(FolderEditorDlg);
const moveDlg = create<{folder: FolderTitle}>(FolderMoveDlg); 

@Component({
    components: { FolderRow }
})
export default class FoldersPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    folders: FolderTitle[] = [];

    async mounted() {
        await this.load(true);
        this.$root.$on('menu-requested', e => this.$refs.menu.open(e.event, e.folder));
    }
    
    beforeDestroy() {
        this.$root.$off('menu-requested');
    }

    async load(showPreloader: boolean = false) {
        await this.showProgress(
            showPreloader ? 'isLoading' : 'isWorking',
            async () => this.folders = await this.$api.folders.getTree(),
            'Failed to load folders tree!'
        );
    }

    async create(root: FolderTitle) {
        if(await editDlg({ parent: root }))
            await this.load();
    }

    async remove(f: FolderTitle) {
        const hint = 'Are you sure you want to remove folder "<b>' + f.caption + '</b>", with all subfolders and media inside?<br/><br/>This operation cannot be undone!';
        if(!await confirmation({ text: hint }))
            return;
        
        await this.showSaving(
            async () => {
                await this.$api.folders.remove(f.key);
                await this.load();
                
                this.$toast.success('Folder removed');
            },
            'Failed to remove folder'
        );
    }

    async edit(f: FolderTitle) {
        if(await editDlg({ folder: f }))
            await this.load();
    }

    async move(f: FolderTitle) {
        if(await moveDlg({ folder: f }))
            await this.load();
    }
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <div class="mb-2">
            <h5 class="pull-left">Folders</h5>
            <button class="btn btn-outline-secondary btn-sm pull-right" type="button" @click.prevent="create(null)">
                <span class="fa fa-plus"></span> Create top-level folder
            </button>
            <div class="clearfix"></div>
        </div>
        <table class="table table-bordered mb-0">
            <thead>
            <tr>
                <th width="75%">Caption</th>
                <th>Media count</th>
                <th width="1"></th>
            </tr>
            </thead>
            <tbody v-if="folders.length === 0">
            <tr>
                <td colspan="3">
                    <div class="alert alert-info mb-0">
                        There are no folders yet. Please create a folder to upload media.
                    </div>
                </td>
            </tr>
            </tbody>
            <tbody v-else>
                <FolderRow v-for="f in folders" :folder="f" :depth="0"></FolderRow>
            </tbody>
        </table>
        <portal to="context-menu">
            <context-menu ref="menu" v-slot="{data}">
                <template v-if="data">
                    <router-link :to="'/folders/' + data.key" class="dropdown-item clickable">
                        <span class="fa fa-fw fa-list"></span> View media
                    </router-link>
                    <router-link :to="'/folders/' + data.key + '/upload'" class="dropdown-item clickable">
                        <span class="fa fa-fw fa-upload"></span> Upload
                    </router-link>
                    <div class="dropdown-divider"></div>
                    <a class="dropdown-item clickable" @click.prevent="edit(data)">
                        <span class="fa fa-fw fa-edit"></span> Edit folder
                    </a>
                    <a class="dropdown-item clickable" @click.prevent="move(data)">
                        <span class="fa fa-fw fa-arrow-circle-right"></span> Move folder
                    </a>
                    <a class="dropdown-item clickable" @click.prevent="remove(data)">
                        <span class="fa fa-fw fa-remove"></span> Remove
                    </a>
                    <div class="dropdown-divider"></div>
                    <a class="dropdown-item clickable" @click.prevent="create(data)">
                        <span class="fa fa-fw fa-plus"></span> Create subfolder
                    </a>
                </template>
            </context-menu>
        </portal>
    </loading>
</template>