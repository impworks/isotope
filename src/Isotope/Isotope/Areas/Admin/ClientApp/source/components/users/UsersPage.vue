<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "../mixins";
import { User } from "../../vms/User";
import { create } from "vue-modal-dialogs";

import ConfirmationDlg from "../utils/ConfirmationDlg.vue";
import UserCreatorDlg from "./UserCreatorDlg.vue";
import UserEditorDlg from "./UserEditorDlg.vue";
import UserPasswordDlg from "./UserPasswordDlg.vue";
import ContextMenu from "../utils/ContextMenu.vue";

const confirmation = create<{ text: string }>(ConfirmationDlg);
const creator = create<{}>(UserCreatorDlg);
const profileEditor = create<{ user: User }>(UserEditorDlg);
const pwdEditor = create<{ user: User }>(UserPasswordDlg);
@Component({
    components: { ContextMenu }
})
export default class UsersPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    users: User[] = [];

    async mounted() {
        await this.load(true);
    }

    async load(showPreloader: boolean = false) {
        await this.showProgress(
            showPreloader ? 'isLoading' : 'isWorking',
            async () => this.users = await this.$api.users.getList(),
            'Failed to load users list!'
        );
    }

    async remove(user: User) {
        const hint = 'Are you sure you want to remove the user "<b>' + user.userName + '"</b>?<br/><br/>This operation cannot be undone.';
        if (!await confirmation({ text: hint }))
            return;
        
        await this.showSaving(
            async () => {
                await this.$api.users.remove(user.id);
                this.$toast.success('User removed.');
            },
            'Failed to remove the user'
        );

        await this.load();
    }

    async create() {
        if (await creator({}))
            await this.load();
    }

    async edit(user: User) {
        if (await profileEditor({ user: user }))
            await this.load();
    }

    async setPassword(user: User) {
        await pwdEditor({ user: user });
    }
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <div class="mb-2">
            <h5 class="pull-left">Users</h5>
            <button class="btn btn-outline-secondary btn-sm pull-right" type="button" @click.prevent="create()">
                <span class="fa fa-plus"></span> Create user
            </button>
            <div class="clearfix"></div>
        </div>
        <table class="table table-bordered mb-0">
            <thead>
            <tr>
                <th width="80%">Username</th>
                <th style="white-space: nowrap">Access level</th>
                <th width="1"></th>
            </tr>
            </thead>
            <tbody v-if="users.length === 0">
            <tr>
                <td colspan="3">
                    <div class="alert alert-info mb-0">
                        No users found.
                    </div>
                </td>
            </tr>
            </tbody>
            <tbody v-else>
            <tr v-for="u in users" v-action-row class="hover-actions" @contextmenu.prevent="$refs.menu.open($event, u)">
                <td>{{ u.userName }}</td>
                <td>
                    <span v-if="u.isAdmin" class="badge badge-danger">Admin</span>
                    <span v-else class="badge badge-primary">User</span>
                </td>
                <td>
                    <a class="hover-action" @click.stop="$refs.menu.open($event, u)" title="Actions">
                        <span class="fa fa-fw fa-ellipsis-v"></span>
                    </a>
                </td>
            </tr>
            </tbody>
        </table>
        <portal to="context-menu">
            <context-menu ref="menu" v-slot="{data}">
                <a class="dropdown-item clickable" @click.prevent="edit(data)">
                    <span class="fa fa-fw fa-edit"></span> Edit profile
                </a>
                <a class="dropdown-item clickable" @click.prevent="setPassword(data)">
                    <span class="fa fa-fw fa-lock"></span> Change password
                </a>
                <div class="dropdown-divider"></div>
                <a class="dropdown-item clickable" @click.prevent="remove(data)">
                    <span class="fa fa-fw fa-remove"></span> Remove
                </a>
            </context-menu>
        </portal>
    </loading>
</template>