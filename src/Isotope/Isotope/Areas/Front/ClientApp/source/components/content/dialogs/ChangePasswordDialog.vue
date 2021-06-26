<script lang="ts">
import { AxiosResponse } from "axios";
import { Component, Mixins, Model, Watch } from 'vue-property-decorator';
import { Dep } from "../../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../../services/ApiService";
import { isAxiosError } from "../../../utils/AxiosHelpers";
import { DialogBase } from "../../utils/DialogBase";
import { HasAsyncState } from "../../mixins/HasAsyncState";

import ModalWindow from "../../utils/ModalWindow.vue";

@Component({
    components: { ModalWindow }
})
export default class ChangePasswordDialog extends Mixins(DialogBase, HasAsyncState()) {
    @Dep('$api') $api: ApiService;
    
    @Model() model: boolean;
    isVisible: boolean = false;
    
    oldPassword: string = null;
    newPassword: string = null;
    newPassword2: string = null;
    
    errorMessage: string = null;    
    
    get canReset() {
        return this.oldPassword
            && this.newPassword
            && this.newPassword2
            && !this.asyncState.isSaving;
    }
    
    mounted() {
        this.isVisible = this.model;
    }
    
    async reset() {
        if(!this.canReset)
            return;
        
        this.errorMessage = this.validate();
        if(this.errorMessage)
            return;
        
        try {
            await this.showSaving(async () => {
                await this.$api.changePassword({
                    oldPassword: this.oldPassword,
                    newPassword: this.newPassword,
                    newPasswordRepeat: this.newPassword2
                });
            });
            
            this.close();
        } catch (e) {
            if(isAxiosError(e)) {
                const resp = e.response as AxiosResponse;
                this.errorMessage = resp.data.error;
            } else {
                console.log('Failed to change password!', e);
            }
        }
    }
    
    private validate(): string {
        if(!this.oldPassword || !this.newPassword || !this.newPassword2)
            return 'Not all fields are filled!';
        
        if(this.newPassword != this.newPassword2)
            return 'Passwords do not match!';
        
        if(this.newPassword.length < 6)
            return 'Password must be at least 6 characters long.';
        
        return null;
    }
    
    @Watch('model')
    onModelChanged() {
        this.isVisible = this.model;
    }

    @Watch('isVisible')
    onVisibilityChanged(value: boolean) {
        this.$emit('model', value);
    }
}
</script>

<template>
    <modal-window v-model="isVisible">
        <template v-slot:header>
            <div class="modal-window__header__caption">
                Change password
            </div>
            <div class="modal-window__header__actions">
                <button
                    href="#"
                    class="btn-header"
                    @click.prevent="isVisible = !isVisible"
                >
                    <div class="btn-header__content">
                        <i class="icon icon-cross"></i>
                    </div>
                </button>
            </div>
        </template>
        <template v-slot:content>
            <div>
                <div class="row">
                    <label class="col-form-label col-sm-4">Old password</label>
                    <div class="col-sm-8">
                        <input type="password" class="form-control" v-model="oldPassword" v-autofocus />
                    </div>
                </div>
                <div class="row mt-3">
                    <label class="col-form-label col-sm-4">New password</label>
                    <div class="col-sm-8">
                        <input type="password" class="form-control" v-model="newPassword" />
                        <small class="text-muted">At least 6 characters</small>
                    </div>
                </div>
                <div class="row mt-3">
                    <label class="col-form-label col-sm-4">Confirmation</label>
                    <div class="col-sm-8">
                        <input type="password" class="form-control" v-model="newPassword2" />
                    </div>
                </div>
                <div class="row mt-3" v-if="errorMessage">
                    <div class="col-sm-12">
                        <div class="alert alert-danger mb-0">
                            {{errorMessage}}
                        </div>
                    </div>
                <div>
            </div>
        </template>
        <template v-slot:footer>
            <button
                type="button"
                class="btn btn-block btn-primary"
                :disabled="!canReset"
                @click.prevent="reset"
            >
                <span v-if="asyncState.isSaving">Resetting...</span>
                <span v-else>Reset</span>
            </button>
        </template>
    </modal-window>
</template>