<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins";
import { FolderTitle } from "../../vms/FolderTitle";

import { create } from "vue-modal-dialogs";

import FolderRow from "./FolderRow.vue";
@Component({
    components: { FolderRow }
})
export default class FoldersPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    folders: FolderTitle[] = [];

    async mounted() {
        await this.load();
    }

    async load() {
        await this.showLoading(
            async () => this.folders = await this.$api.folders.getTree(),
            'Failed to load folders tree!'
        );
    }

    async remove(f: FolderTitle) {
    }

    async create(root: FolderTitle) {
    }

    async edit(f: FolderTitle) {
    }
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <div class="mb-2">
            <button class="btn btn-outline-secondary btn-sm pull-right" type="button" @click.prevent="create(null)">
                <span class="fa fa-plus"></span> Create top-level folder
            </button>
            <div class="clearfix"></div>
        </div>
        <table class="table table-bordered mb-0">
            <thead>
            <tr>
                <th width="80%">Caption</th>
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
    </loading>
</template>