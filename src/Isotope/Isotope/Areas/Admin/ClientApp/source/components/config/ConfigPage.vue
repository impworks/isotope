<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { ApiService } from "../../services/ApiService";
import { Dep } from "../../common/utils/VueInjectDecorator";
import { Config } from "../../vms/Config";
import { HasAsyncState } from "../mixins/HasAsyncState";

@Component
export default class ConfigPage extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;

    value: Config = null;
    
    async mounted() {
        await this.showLoading(
            async () => this.value = await this.$api.config.get(),
            'Failed to load config state!'
        );
    }
    
    get canSave() {
        return this.value && !!this.value.title;
    }
    
    async save() {
        await this.showSaving(
            async () => {
                await this.$api.config.update({ ...this.value });
                this.$toast.success('Config updated');
            },
            'Failed to update config state!'
        );
    }
}
</script>

<template>
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <form @submit.prevent="save()" v-if="value">
            <div class="form-group row">
                <label class="col-sm-2 col-form-label" for="title">Title</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="title" v-model="value.title" :disabled="asyncState.isSaving" />
                </div>
            </div>
            <div class="form-group row">
                <label class="col-sm-2 col-form-label">Access</label>
                <div class="col-sm-10">
                    <div class="form-check">
                        <input type="radio" class="form-check-input" name="access" id="access-true" :value="true" v-model="value.allowGuests" :disabled="asyncState.isSaving" />
                        <label class="form-check-label" for="access-true">
                            Allow anyone to view the gallery
                        </label>
                    </div>
                    <div class="form-check">
                        <input type="radio" class="form-check-input" name="access" id="access-false" :value="false" v-model="value.allowGuests" :disabled="asyncState.isSaving" />
                        <label class="form-check-label" for="access-false">
                            Only allow authorized users and shared links
                        </label>
                    </div>
                </div>
            </div>
            <div class="form-group row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" type="submit" :disabled="!canSave">
                        <span v-if="asyncState.isSaving">Saving...</span>
                        <span v-else>Save</span>
                    </button>
                </div>
            </div>
        </form>
    </loading>
</template>