<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { DialogComponent } from "vue-modal-dialogs";
import { Media } from "../../vms/Media";
import { Tag } from "../../vms/Tag";
import { TagBindingType } from "../../../../../Common/source/vms/TagBindingType";
import { OverlayTagBinding } from "../../vms/OverlayTagBinding";
import RectEditor from "./RectEditor.vue";
@Component({
    components: { RectEditor }
})
export default class MediaTagsEditorDlg extends Mixins(HasAsyncState(), DialogComponent) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: true }) mediaKey: string;

    value: Media = null;
    extraTags: number[] = [];
    
    tagsLookup: Tag[] = []; 

    async mounted() {
        await this.showLoading(
            async () => {
                this.tagsLookup = await this.$api.tags.getList();
                const data = await this.$api.media.get(this.mediaKey);
                await this.loadImage(data.fullPath);
                this.value = data;
                this.extraTags = data.extraTags?.map(x => x.tagId) ?? [];
            },
            'Failed to load media data'
        );
    }

    async save() {
        if(this.asyncState.isSaving)
            return;
        
        await this.showSaving(
            async () => {
                this.value.extraTags = this.extraTags.map(x => ({ tagId: x, type: TagBindingType.Custom }));
                await this.$api.media.update(this.mediaKey, this.value);

                this.$close(true);

                this.$toast.success('Media tags updated');
            },
            'Failed to update media tags'
        )
    }
    
    private loadImage(path: string): Promise<void> {
        return new Promise((resolve, reject) => {
            const img = new Image();
            img.addEventListener('load', () => resolve());
            img.addEventListener('error', err => reject(err));
            img.src = path;
        })
    }
    
    addTag() {
        alert('new tag');
    }
    
    removeTag(b: OverlayTagBinding) {
        const list = this.value.overlayTags;
        const idx = list.indexOf(b);
        if(idx !== -1)
            list.splice(idx, 1);
    }
}
</script>

<template>
    <div>
        <div class="modal fade show">
            <form @submit.prevent="save()">
                <div class="modal-dialog modal-lg" v-if="value">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Update media tags</h5>
                        </div>
                        <div class="modal-body">
                            <div class="form-group">
                                <div class="mb-2">
                                    <div class="pull-right">
                                        <button class="btn btn-sm btn-outline-secondary" type="button" @click.prevent="addTag()" :disabled="asyncState.isSaving">
                                            <span class="fa fa-plus-circle"></span> Add tag
                                        </button>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="media-wrapper">
                                    <img :src="value.fullPath" ref="img" />
                                    <div class="tag-wrapper">
                                        <RectEditor v-for="(b, idx) in value.overlayTags"
                                                    :rect="b.location"
                                                    :tag-binding="b"
                                                    :tags-lookup="tagsLookup"
                                                    :key="idx"
                                                    @removed="removeTag(b)">
                                        </RectEditor>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Custom tags</label>
                                <div class="col-sm-9">
                                    <v-select multiple :options="tagsLookup" v-model="extraTags" label="caption" :reduce="x => x.id">
                                        <template slot="no-options">No tags created yet.</template>
                                    </v-select>
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

<style lang="scss" scoped>
.media-wrapper {
    width: 100%;
    position: relative;
    
    img {
        width: 100%;
        height: auto;
    }
    
    .tag-wrapper {
        position: absolute;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
        z-index: 1000;
    }
}
</style>