<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Tag } from "../../vms/Tag";
import { TagType } from "../../../../../Common/source/vms/TagType";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";

@Component
export default class TagEditorDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: false }) tag: Tag;
    
    value: Tag = null;
    tagTypes: {type: TagType, caption: string}[] = [
        { type: TagType.Person, caption: 'Person' },
        { type: TagType.Location, caption: 'Location' },
        { type: TagType.Pet, caption: 'Pet' },
        { type: TagType.Custom, caption: 'Other' }
    ];

    createMore: boolean = false;
    result: boolean = false;

    $refs: {
        caption: HTMLInputElement;
    }
    
    get isNew(): boolean {
        return !this.value?.id;
    }
    
    mounted() {
        this.value = this.tag
            ? { ...this.tag }
            : { id: 0, caption: '', type: TagType.Person };
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
                if(this.isNew) {
                    await this.$api.tags.create(this.value)
                    this.$toast.success('Tag created');
                    
                    if(this.createMore) {
                        this.value = { id: 0, caption: '', type: this.value.type };
                        this.result = true;
                        this.$refs.caption.focus();
                        return;
                    }
                }
                else {
                    await this.$api.tags.update(this.value.id, this.value);
                    this.$toast.success('Tag updated');
                }
                
                this.$close(true);
            },
            this.isNew ? 'Failed to create tag' : 'Failed to update tag'
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
                            <h5 class="modal-title">
                                <span v-if="isNew">Create a new tag</span>
                                <span v-else>Update tag '{{tag.caption}}'</span>
                            </h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label class="col-sm-2 col-form-label">Caption</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" v-model="value.caption" v-autofocus ref="caption" />
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-2 col-form-label">Type</label>
                                <div class="col-sm-10">
                                    <div class="btn-group">
                                        <button type="button" class="btn btn-outline-secondary"
                                                v-for="t in tagTypes"
                                                :class="{active: t.type === value.type}"
                                                @click="value.type = t.type">
                                            {{t.caption}}
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <label class="mr-auto" v-if="isNew" title="Keep the dialog open to create another folder after saving">
                                <input type="checkbox" v-model="createMore" /> Create more
                            </label>
                            <button type="submit" class="btn btn-primary" :disabled="!canSave">
                                <span v-if="asyncState.isSaving">Saving...</span>
                                <span v-else>
                                    <span v-if="isNew">Create</span>
                                    <span v-else>Update</span>
                                </span>
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