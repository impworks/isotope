<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { DialogComponent } from "vue-modal-dialogs";
import { UserPassword } from "../../vms/UserPassword";

@Component
export default class UserPasswordDlg extends Mixins(HasAsyncState(), DialogComponent) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: true}) userId: string;
    
    value: UserPassword = { password: null, passwordConfirmation: null };

    get canSave(): boolean {
        return !!this.value.password
               && this.value.password === this.value.passwordConfirmation;
    }

    async save() {
        if (!this.canSave)
            return;

        await this.showSaving(
            async () => this.$api.users.updatePassword(this.userId, { ...this.value}),
            'Failed to update password!'
        );
        this.$close(true);
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
                            <h5 class="modal-title">Update password</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-2 col-form-label">Password</label>
                                <div class="col-sm-10">
                                    <input type="password" class="form-control" v-model="value.password"/>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-2 col-form-label">Confirmation</label>
                                <div class="col-sm-10">
                                    <input type="password" class="form-control" v-model="value.passwordConfirmation"/>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="submit" class="btn btn-primary" :disabled="!canSave">Save</button>
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