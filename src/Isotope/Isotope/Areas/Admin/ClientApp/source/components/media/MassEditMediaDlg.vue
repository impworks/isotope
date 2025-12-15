<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../common/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { MassMediaUpdate } from "../../vms/MassMediaAction";

@Component
export default class MassEditMediaDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop() media: MediaThumbnail[];
    
    request: MassMediaUpdate = null;
    date: Date = null;

    get canSave() {
        const r = this.request;
        return !this.asyncState.isSaving
               && (r.date.isSet || r.description.isSet);
    }

    async mounted() {
        this.request = {
            keys: this.media.map(x => x.key),
            date: {
                isSet: true,
                value: null,
            },
            description: {
                isSet: true,
                value: null
            }
        };
    }

    async save() {
        if(!this.canSave)
            return;
        
        const r = this.request;
        await this.showSaving(
            async () => {
                if(r.date.isSet)
                    r.date.value = this.date ? this.date.toISOString() : null;
                if(r.description.isSet)
                    r.description.value = r.description.value || null;
                await this.$api.media.massUpdate(r);
                this.$close(true);
            },
            'Failed to update media'
        )
    }
}
</script>

<template>
    <div>
        <div class="modal-backdrop show fade">
        </div>
        <div class="modal fade show">
            <div class="modal-dialog">
                <div class="modal-content">
                    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
                        <form @submit.prevent="save()">
                            <div class="modal-header">
                                <h5 class="modal-title">Edit {{ media.length }} media file(s)</h5>
                            </div>
                            <div class="modal-body">
                                <div class="form-group row">
                                    <label class="col-sm-3 col-form-label no-wrap">
                                        <input type="checkbox" v-model="request.date.isSet" />
                                        Date
                                    </label>
                                    <div class="col-sm-5">
                                        <datepicker v-model="date" :monday-first="true" :disabled="!request.date.isSet"></datepicker>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-sm-3 col-form-label no-wrap">
                                        <input type="checkbox" v-model="request.description.isSet" />
                                        Description
                                    </label>
                                    <div class="col-sm-9">
                                        <textarea class="form-control" v-model="request.description.value" rows="5" :disabled="!request.description.isSet"></textarea>
                                    </div>
                                </div>
                                <div class="form-group mb-0">
                                    <span class="text-muted small">
                                        <span class="fa fa-question-circle"></span>
                                        Uncheck the fields which you do not want to change.
                                    </span>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="submit" class="btn btn-primary" :disabled="!canSave">
                                    <span v-if="asyncState.isSaving">Saving...</span>
                                    <span v-else>Save</span>
                                </button>
                                <button type="button" class="btn btn-secondary" @click.prevent="$close(false)">Cancel</button>
                            </div>
                        </form>
                    </loading>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="scss">
.no-wrap {
    white-space: nowrap;
}
</style>