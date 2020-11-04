<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins/HasAsyncState";
import { SharedLinkDetails } from "../../vms/SharedLinkDetails";
import { create } from "vue-modal-dialogs";

import ConfirmationDlg from "../utils/ConfirmationDlg.vue";

const confirmation = create<{text: string}>(ConfirmationDlg);

@Component
export default class SharedLinksPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    links: SharedLinkDetails[] = [];

    async mounted() {
        await this.load();
    }
    
    async load() {
        await this.showLoading(
            async () => this.links = await this.$api.sharedLinks.getList(),
            'Failed to load config state!'
        );
    }
    
    async remove(l: SharedLinkDetails) {
        if(!await confirmation({text: 'Are you sure you want to remove this shared link?'}))
            return;
        
        await this.$api.sharedLinks.remove(l.key);
        await this.load();
        this.$toast.success('Shared link removed.');
    }
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <table class="table table-bordered mb-0">
            <thead>
                <tr>
                    <th>Created at</th>
                    <th>Folder</th>
                    <th>Tags</th>
                    <th width="1"></th>
                </tr>
            </thead>
            <tbody v-if="links.length === 0">
                <tr>
                    <td colspan="4">
                        <div class="alert alert-info mb-0">
                            There are no shared links yet. To create one, use the "share" icon in the gallery.
                        </div>
                    </td>
                </tr>
            </tbody>
            <tbody v-else>
                <tr v-for="l in links" v-action-row class="hover-actions">
                    <td>{{ l.creationDate }}</td>
                    <td><span :title="l.folder">{{l.folderCaption}}</span></td>
                    <td>{{l.tags ? l.tags.length : '-'}}</td>
                    <td>
                        <a class="hover-action" href @click.prevent="remove(l)" title="Remove">
                            <span class="fa fa-remove"></span>
                        </a>
                    </td>
                </tr>
            </tbody>
        </table>
    </loading>
</template>