<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { SharedLinkDetails } from "../../vms/SharedLinkDetails";

@Component
export default class SharedLinkEditorDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: false }) link: SharedLinkDetails;

    value: SharedLinkDetails = null;
    copied: boolean = false;

    mounted() {
        this.value = { ...this.link };
    }
    
    get url(): string {
        return window.location.origin + '/?sh=' + this.link.key;
    }

    get canSave(): boolean {
        return !this.asyncState.isSaving
            && !!this.value?.caption;
    }

    async save() {
        if(!this.canSave)
            return;

        await this.showSaving(
            async () => {
                await this.$api.sharedLinks.update(this.value.key, this.value);
                this.$close(true);
            },
            'Failed to update link'
        )
    }
    
    async copy() {
        try {
            await navigator.clipboard.writeText(this.url);
            this.copied = true;
            setTimeout(() => this.copied = false, 1500);
        } catch {
            this.$toast.error('Failed to copy link');
        }
    }
}
</script>

<template>
    <div>
        <div class="modal-backdrop show fade">
        </div>
        <div class="modal fade show">
            <form @submit.prevent="save()">
                <div class="modal-dialog" v-if="value">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Update shared link</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-2 col-form-label">Caption</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" v-model="value.caption" v-autofocus />
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-2 col-form-label">Link</label>
                                <div class="col-sm-10">
                                    <div class="input-group">
                                        <input type="text" class="form-control" readonly="readonly" :value="url" />
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" title="Copy link to clipboard" type="button" @click.prevent="copy">
                                                <i :class="copied ? 'fa fa-check' : 'fa fa-copy'"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="submit" class="btn btn-primary" :disabled="!canSave" title="Ctrl + S">
                                <span v-if="asyncState.isSaving">Saving...</span>
                                <span v-else>Update</span>
                            </button>
                            <button type="button" class="btn btn-secondary" @click.prevent="$close(false)">Cancel</button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <GlobalEvents @keydown.ctrl.83.stop.prevent="save()"></GlobalEvents>
    </div>
</template>