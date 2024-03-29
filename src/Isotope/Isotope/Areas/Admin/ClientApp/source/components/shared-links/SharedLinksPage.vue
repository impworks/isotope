<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins";
import { SharedLinkDetails } from "../../vms/SharedLinkDetails";
import { create } from "vue-modal-dialogs";

import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import { DateHelper } from "../../../../../Common/source/utils/DateHelper";
import SharedLinkEditorDlg from "./SharedLinkEditorDlg.vue";
import { Tag } from "../../vms/Tag";

const confirmation = create<{text: string}>(ConfirmationDlg);
const editor = create<{link: SharedLinkDetails}>(SharedLinkEditorDlg);

@Component
export default class SharedLinksPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    links: SharedLinkDetails[] = [];

    async mounted() {
        await this.load(true);
    }
    
    async load(showPreloader: boolean = false) {
        await this.showProgress(
            showPreloader ? 'isLoading' : 'isWorking',
            async () => this.links = await this.$api.sharedLinks.getList(),
            'Failed to load links list!'
        );
    }
    
    async remove(sl: SharedLinkDetails) {
        const hint = 'Are you sure you want to remove this shared link?<br/><br/>This operation cannot be undone.';
        if(!await confirmation({text: hint}))
            return;
        
        await this.$api.sharedLinks.remove(sl.key);
        await this.load();
        this.$toast.success('Shared link removed.');
    }
    
    async edit(sl: SharedLinkDetails) {
        if(await editor({link: sl}))
            await this.load();
    }
    
    formatDate(d: string) {
        return DateHelper.formatFull(d);
    }
    
    externalLink(sl: SharedLinkDetails) {
        window.open('/?sh=' + sl.key, '_blank');
    }
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <div class="mb-2">
            <h5>Shared links</h5>
        </div>
        <table class="table table-bordered mb-0">
            <thead>
                <tr>
                    <th>Caption</th>
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
                <tr v-for="l in links" v-action-row class="hover-actions" @contextmenu.prevent="$refs.menu.open($event, l)">
                    <td>
                        <a href="#" @click.prevent="edit(l)">{{l.caption || '-'}}</a>
                    </td>
                    <td>{{ formatDate(l.creationDate) }}</td>
                    <td><span :title="l.folder">{{l.folderCaption}}</span></td>
                    <td>{{l.tags ? l.tags.length : '-'}}</td>
                    <td>
                        <a class="hover-action" @click.stop="$refs.menu.open($event, l)" title="Actions">
                            <span class="fa fa-fw fa-ellipsis-v"></span>
                        </a>
                    </td>
                </tr>
            </tbody>
        </table>
        <portal to="context-menu">
            <context-menu ref="menu" v-slot="{data}">
                <a class="dropdown-item clickable" @click.prevent="edit(data)">
                    <span class="fa fa-fw fa-edit"></span> Edit / copy
                </a>
                <a class="dropdown-item clickable" @click.prevent="externalLink(data)">
                    <span class="fa fa-fw fa-share"></span> View
                </a>
                <div class="dropdown-divider"></div>
                <a class="dropdown-item clickable" @click.prevent="remove(data)">
                    <span class="fa fa-fw fa-remove"></span> Remove
                </a>
            </context-menu>
        </portal>
    </loading>
</template>