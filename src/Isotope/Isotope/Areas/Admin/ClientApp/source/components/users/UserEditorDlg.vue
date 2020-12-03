<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { User } from "../../vms/User";

@Component
export default class UserEditorDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: true }) user: User;
    
    value: User = null;
    accessLevels: { caption: string, isAdmin: boolean }[] = [
        { caption: 'Admin', isAdmin: true },
        { caption: 'User', isAdmin: false },
    ]
    
    async mounted() {
        this.value = { ...this.user };
    }
    
    async save() {
        await this.showSaving(
            async () => {
                await this.$api.users.update(this.user.id, { ...this.value });
                this.$toast.success('User updated');
                this.$close(true);
            },
            'Failed to update user!'
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
                            <h5 class="modal-title">Update user {{user.userName}}</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-2 col-form-label">Access</label>
                                <div class="col-sm-10">
                                    <div class="btn-group">
                                        <button type="button" class="btn btn-outline-secondary"
                                                v-for="al in accessLevels"
                                                :class="{active: al.isAdmin === value.isAdmin}"
                                                @click="value.isAdmin = al.isAdmin">
                                            {{al.caption}}
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="submit" class="btn btn-primary" title="Ctrl + S">
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
        <GlobalEvents @keydown.ctrl.83.stop.prevent="save()"></GlobalEvents>
    </div>
</template>