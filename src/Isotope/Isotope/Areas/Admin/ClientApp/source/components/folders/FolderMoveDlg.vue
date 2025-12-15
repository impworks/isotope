<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../common/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { FolderTitle } from "../../vms/FolderTitle";
import { FolderHelper, NestedFolderTitle } from "../utils/FolderHelper";

@Component
export default class FolderMoveDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop() folder: FolderTitle;
    
    targets: NestedFolderTitle[] = [];
    canMoveToRoot: boolean = false;
    targetRoot: boolean = false;
    targetKey: string = null;

    get canSave() {
        return !this.asyncState.isSaving
            && !this.asyncState.isLoading
            && ((this.canMoveToRoot && this.targetRoot) || !!this.targetKey);
    }

    async mounted() {
        await this.showLoading(
            async () => {
                this.targets = FolderHelper.flatten(await this.$api.folders.getTree(), this.folder.key);
                this.canMoveToRoot = this.targets.find(x => x.key == this.folder.key).prefix !== '';
            },
            'Failed to load data'
        );
    }

    async save() {
        await this.showSaving(
            async () => {
                await this.$api.folders.move(this.folder.key, this.targetRoot ? null : this.targetKey);
                this.$close(true);
            },
            'Failed to move folder'
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
                        <form @submit.prevent="save()" v-if="targets">
                            <div class="modal-header">
                                <h5 class="modal-title">Move folder</h5>
                            </div>
                            <div class="modal-body">
                                <div class="form-group row">
                                    <label class="col-sm-3 col-form-label">Folder</label>
                                    <div class="col-sm-9">
                                        <input type="text" readonly class="form-control-plaintext" :value="folder.caption" />
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-sm-3 col-form-label">Move to</label>
                                    <div class="col-sm-9">
                                        <template  v-if="canMoveToRoot">
                                            <div>
                                                <label>
                                                    <input type="radio" v-model="targetRoot" :value="true" /> Gallery root
                                                </label>
                                            </div>
                                            <div>
                                                <label>
                                                    <input type="radio" v-model="targetRoot" :value="false" /> Other folder:
                                                </label>
                                            </div>
                                        </template>
                                        <v-select :options="targets" v-model="targetKey" label="caption" :reduce="x => x.key"
                                                  :disabled="targetRoot" :selectable="x => x.selectable">
                                            <template v-slot:option="option">
                                                <div class="text-nowrap">
                                                    <span style="opacity: 0.2">{{option.prefix}}</span>
                                                    <span>{{option.caption}}</span>
                                                </div>
                                            </template>
                                            <template slot="no-options">There are no other folders.</template>
                                        </v-select>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="submit" class="btn btn-primary" :disabled="!canSave">
                                    <span v-if="asyncState.isSaving">Moving...</span>
                                    <span v-else>Move</span>
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