<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { DialogComponent } from "vue-modal-dialogs";
import { Media } from "../../vms/Media";

@Component
export default class MediaPropsEditorDlg extends Mixins(HasAsyncState(), DialogComponent) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: true }) mediaKey: string;

    value: Media = null;
    date: Date = null;
    
    async mounted() {
        await this.showLoading(
            async () => {
                this.value = await this.$api.media.get(this.mediaKey);
                this.date = this.value.date ? new Date(this.value.date) : null;
            },
            'Failed to load media data'
        );
    }

    async save() {
        if(this.asyncState.isSaving)
            return;
        
        await this.showSaving(
            async () => {
                this.value.date = this.date ? this.date.toISOString() : null;
                await this.$api.media.update(this.mediaKey, this.value);
                
                this.$close(true);
                
                this.$toast.success('Media properties updated');
            },
            'Failed to update media properties'
        )
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
                            <h5 class="modal-title">Update media properties</h5>
                            <button type="button" class="close" @click="$close(false)">&times;</button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Date</label>
                                <div class="col-sm-5">
                                    <datepicker v-model="date" :monday-first="true"></datepicker>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Description</label>
                                <div class="col-sm-9">
                                    <textarea class="form-control" v-model="value.description" rows="5"></textarea>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="submit" class="btn btn-primary" :disabled="asyncState.isSaving">
                                <span v-if="asyncState.isSaving">Saving...</span>
                                <span v-else>Update</span>
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