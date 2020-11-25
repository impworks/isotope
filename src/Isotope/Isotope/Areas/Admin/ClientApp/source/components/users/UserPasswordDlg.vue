<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { UserPassword } from "../../vms/UserPassword";
import { User } from "../../vms/User";

@Component
export default class UserPasswordDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: true }) user: User;
    
    value: UserPassword = { password: null, passwordConfirmation: null };

    get canSave(): boolean {
        return !this.asyncState.isSaving
            && !!this.value.password
            && this.value.password === this.value.passwordConfirmation;
    }

    async save() {
        if (!this.canSave)
            return;

        await this.showSaving(
            async () => {
                await this.$api.users.updatePassword(this.user.id, { ...this.value });
                this.$toast.success('User password updated');
                this.$close(true);
            },
            'Failed to update password!'
        );
    }
}
</script>

<template>
    <div>
        <div class="modal fade show">
            <form @submit.prevent="save()">
                <div class="modal-dialog" v-if="value">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Update password for {{user.userName}}</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Password</label>
                                <div class="col-sm-9">
                                    <input type="password" class="form-control" v-model="value.password" v-autofocus />
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Confirmation</label>
                                <div class="col-sm-9">
                                    <input type="password" class="form-control" v-model="value.passwordConfirmation"
                                           :class="{'is-invalid': !!value.passwordConfirmation && value.passwordConfirmation !== value.password }"/>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="submit" class="btn btn-primary" :disabled="!canSave">
                                <span v-if="asyncState.isSaving">Saving...</span>
                                <span v-else>Save</span>
                            </button>
                            <button type="button" class="btn btn-secondary" @click.prevent="$close(false)">Cancel
                            </button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <div class="modal-backdrop show fade">
        </div>
    </div>
</template>