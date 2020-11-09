<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins";
import { Tag } from "../../vms/Tag";
import { create } from "vue-modal-dialogs";

import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import TagEditorDlg from "./TagEditorDlg.vue";

const confirmation = create<{text: string}>(ConfirmationDlg);
const editor = create<{tag: Tag}>(TagEditorDlg);

@Component
export default class TagsPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    tags: Tag[] = [];

    async mounted() {
        await this.load();
    }

    async load() {
        await this.showLoading(
            async () => this.tags = await this.$api.tags.getList(),
            'Failed to load tags list!'
        );
    }

    async remove(tag: Tag) {
        const hint = 'Are you sure you want to remove the tag "<b>' + tag.caption + '</b>"?<br/><br/>This operation cannot be undone.';
        if(!await confirmation({text: hint}))
            return;

        await this.$api.tags.remove(tag.id);
        await this.load();
        this.$toast.success('Tag removed.');
    }
    
    async create() {
        if(await editor({tag: null}))
            await this.load();
    }
    
    async edit(t: Tag) {
        if(await editor({tag: t}))
            await this.load();
    }
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <div class="mb-2">
            <h5 class="pull-left">Tags</h5>
            <button class="btn btn-outline-secondary btn-sm pull-right" type="button" @click.prevent="create()">
                <span class="fa fa-plus"></span> Create tag
            </button>
            <div class="clearfix"></div>
        </div>
        <table class="table table-bordered mb-0">
            <thead>
            <tr>
                <th width="80%">Caption</th>
                <th>Type</th>
                <th width="1"></th>
            </tr>
            </thead>
            <tbody v-if="tags.length === 0">
            <tr>
                <td colspan="3">
                    <div class="alert alert-info mb-0">
                        There are no tags yet.
                    </div>
                </td>
            </tr>
            </tbody>
            <tbody v-else>
            <tr v-for="t in tags" v-action-row class="hover-actions">
                <td>{{ t.caption }}</td>
                <td>
                    <span v-if="t.type === 1" class="badge badge-primary">Person</span>
                    <span v-if="t.type === 2" class="badge badge-warning">Location</span>
                    <span v-if="t.type === 3" class="badge badge-danger">Other</span>
                </td>
                <td>
                    <a class="hover-action" href @click.prevent="remove(t)" title="Remove">
                        <span class="fa fa-fw fa-remove"></span>
                    </a>
                    <a class="hover-action" href @click.prevent="edit(t)" title="Edit">
                        <span class="fa fa-fw fa-edit"></span>
                    </a>
                </td>
            </tr>
            </tbody>
        </table>
    </loading>
</template>