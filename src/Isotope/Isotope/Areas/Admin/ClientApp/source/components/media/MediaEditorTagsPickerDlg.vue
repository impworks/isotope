<script lang="ts">
import { Component, Mixins, Prop } from 'vue-property-decorator';
import { DialogBase } from "../mixins";
import { Tag } from "../../vms/Tag";

@Component
export default class MediaEditorTagsPickerDlg extends Mixins(DialogBase) {
    @Prop({ required: true }) allTags: Tag[];
    @Prop({ required: true }) pickedTags: Tag[];
    
    result: Tag[] = [];
    
    mounted() {
        this.result = this.pickedTags?.slice(0) || [];
    }
}
</script>

<template>
    <div>
        <div class="modal-backdrop show fade">
        </div>
        <div class="modal fade show">
            <form @submit.prevent="$close(result)">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Tag picker</h5>
                            <button type="button" class="close" @click="$close([])">&times;</button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <div class="col-sm-12">
                                    You can temporarily select a subset of relevant tags to speed up tagging multiple photos.
                                    Other tags will be hidden until you reopen the media editor.
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Tags</label>
                                <div class="col-sm-9">
                                    <v-select multiple :options="allTags" v-model="result" label="caption">
                                        <template slot="no-options">No tags created yet.</template>
                                    </v-select>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="submit" class="btn btn-primary">Save</button>
                            <button type="button" class="btn btn-secondary" @click.prevent="$close([])">Cancel</button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
</template>