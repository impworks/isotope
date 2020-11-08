<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { DialogComponent } from "vue-modal-dialogs";
import { User } from "../../vms/User";
import { UserCreation } from "../../vms/UserCreation";

@Component
export default class UserCreatorDlg extends Mixins(HasAsyncState(), DialogComponent) {
    @Dep('$api') $api: ApiService;

    value: UserCreation = {
        userInfo: {
            id: '',
            isAdmin: false,
            userName: ''
        },
        passwordInfo: {
            password: '',
            passwordConfirmation: ''
        }
    };
    accessLevels: { caption: string, isAdmin: boolean }[] = [
        { caption: 'Admin', isAdmin: true },
        { caption: 'User', isAdmin: false },
    ]
    
    get canSave() {
        const u = this.value;
        return !this.asyncState.isSaving
            && !!u.userInfo.userName
            && !!u.passwordInfo.password
            && u.passwordInfo.password === u.passwordInfo.passwordConfirmation;
    }

    async save() {
        if(!this.canSave)
            return;
        
        await this.showSaving(
            async () => {
                await this.$api.users.create(this.value);
                this.$close(true);
            },
            'Failed to create user!'
        );
    }
}
</script>

<template>
    <div>
        <div class="modal fade show">
            <form @submit.prevent="save()">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Create user</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Email</label>
                                <div class="col-sm-9">
                                    <input type="text" class="form-control" v-model="value.userInfo.userName" v-autofocus />
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Access</label>
                                <div class="col-sm-9">
                                    <div class="btn-group">
                                        <button type="button" class="btn btn-outline-secondary"
                                                v-for="al in accessLevels"
                                                :class="{active: al.isAdmin === value.userInfo.isAdmin}"
                                                @click="value.userInfo.isAdmin = al.isAdmin">
                                            {{al.caption}}
                                        </button>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Password</label>
                                <div class="col-sm-9">
                                    <input type="password" class="form-control" v-model="value.passwordInfo.password"/>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Confirmation</label>
                                <div class="col-sm-9">
                                    <input type="password" class="form-control" v-model="value.passwordInfo.passwordConfirmation"
                                           :class="{'is-invalid': !!value.passwordInfo.passwordConfirmation && value.passwordInfo.passwordConfirmation !== value.passwordInfo.password }"/>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="submit" class="btn btn-primary" :disabled="!canSave">
                                <span v-if="asyncState.isSaving">Saving...</span>
                                <span v-else>Create</span>
                            </button>
                            <button type="button" class="btn btn-secondary" @click.prevent="$close(false)">Cancel</button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <div class="modal-backdrop show fade">
        </div>
    </div>
</template>