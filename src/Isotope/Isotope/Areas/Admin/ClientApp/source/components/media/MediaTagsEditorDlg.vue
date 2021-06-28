<script lang="ts">
import { Component, Mixins, Prop, Vue } from "vue-property-decorator";
import { GlobalEvents } from "vue-global-events";

import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { Media } from "../../vms/Media";
import { Tag } from "../../vms/Tag";
import { TagBindingType } from "../../../../../Common/source/vms/TagBindingType";
import { OverlayTagBinding } from "../../vms/OverlayTagBinding";
import { Rect } from "../../../../../Common/source/vms/Rect";
import { EventHelper } from "../../../../../Common/source/utils/EventHelper";

import RectEditor from "./RectEditor.vue";
import RectPreview from "./RectPreview.vue";

@Component({
    components: { RectPreview, RectEditor, GlobalEvents }
})
export default class MediaTagsEditorDlg extends Mixins(HasAsyncState({ isSkipping: false }), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: true }) mediaKey: string;
    
    $refs: {
        img: HTMLImageElement;
        rects: Vue[];
        wrapper: HTMLElement;
    };

    value: Media = null;
    extraTags: number[] = [];
    
    tagsLookup: Tag[] = [];
    isCreatingTag: boolean = false;
    tagOrigin: { x: number, y: number } = null;
    newTagRect: Rect = null;
    
    currentKey: string = null;
    tagMore: boolean = false;
    result: boolean = false;
    
    get isIncorrect() {
        return this.value?.overlayTags.some(x => !x.tagId);
    }
    
    get canSave() {
        return !this.asyncState.isSaving
            && !this.asyncState.isSkipping
            && !this.isCreatingTag
            && !this.isIncorrect;
    }

    async mounted() {
        await this.showLoading(
            async () => this.tagsLookup = await this.$api.tags.getList(),
            'Failed to load tag list'
        );
        this.currentKey = this.mediaKey;
        await this.load();
    }
    
    async load() {
        await this.showLoading(
            async () => {
                const data = await this.$api.media.get(this.currentKey);
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
                await this.$api.media.update(this.currentKey, this.value);

                this.$toast.success('Media tags updated');
                this.result = true;
                
                if (this.tagMore)
                    await this.loadNext(false);
                else
                    this.$close(true);
            },
            'Failed to update media tags'
        );        
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
                            <button type="button" class="close" @click="$close(result)">&times;</button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group">
                                <div class="pull-right">
                                    <button class="btn btn-sm btn-outline-secondary" type="button"
                                            @click.prevent="toggleCreatingTagMode(true)"
                                            :disabled="asyncState.isSaving || isCreatingTag">
                                        <span class="fa fa-plus-circle"></span>
                                        <span v-if="isCreatingTag">Adding tag: Esc to cancel</span>
                                        <span v-else title="Add a new tag (Ctrl + Space)">Add tag</span>
                                    </button>
                                </div>
                                <div class="clearfix"></div>
                            </div>
                            <div class="form-group media-container">
                                <div class="media-wrapper" :class="{'crosshair': this.isCreatingTag}"
                                     @mousedown.prevent="createTagStarted($event)"
                                     @mousemove.prevent="createTagMoved($event)"
                                     @mouseup.prevent="createTagCompleted()"
                                     ref="wrapper">
                                    <img :src="value.fullPath" />
                                    <RectPreview v-if="newTagRect" :rect="newTagRect"></RectPreview>
                                    <div class="tag-wrapper" v-if="!isCreatingTag">
                                        <RectEditor v-for="(b, idx) in value.overlayTags"
                                                    :rect="b.location"
                                                    :tag-binding="b"
                                                    :tags-lookup="tagsLookup"
                                                    :key="idx"
                                                    ref="rects"
                                                    @removed="removeTag(b)">
                                        </RectEditor>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label class="col-sm-3 col-form-label">Custom tags</label>
                                <div class="col-sm-9">
                                    <v-select multiple :options="tagsLookup" v-model="extraTags" label="caption" :reduce="x => x.id"
                                              :disabled="asyncState.isSaving || isCreatingTag">
                                        <template slot="no-options">No tags created yet.</template>
                                    </v-select>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <label class="mr-auto" title="Keep the dialog open and find next untagged media after saving">
                                <input type="checkbox" v-model="tagMore" /> Tag next untagged
                            </label>
                            <button v-if="tagMore" type="button" class="btn btn-outline-primary"
                                    :disabled="asyncState.isSaving || asyncState.isSkipping"
                                    @click="loadNext(true)" title="View next untagged (Ctrl + RightArrow)">
                                <span v-if="asyncState.isSkipping">Skipping...</span>
                                <span v-else>Skip</span>
                            </button>
                            <button type="submit" class="btn btn-primary" :disabled="!canSave" title="Ctrl + S">
                                <span v-if="asyncState.isSaving">Saving...</span>
                                <span v-else>Update</span>
                            </button>
                            <button type="button" class="btn btn-secondary" @click.prevent="$close(result)">Cancel</button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <div class="modal-backdrop show fade">
        </div>
        <GlobalEvents @keydown.esc="toggleCreatingTagMode(false)"
                      @keydown.ctrl.right="loadNext(true)"
                      @keydown.ctrl.space.stop.prevent="toggleCreatingTagMode(true)"
                      @keydown.ctrl.83.stop.prevent="save()">
        </GlobalEvents>
    </div>
</template>

<style lang="scss" scoped>
.media-container {
    display: flex;
    justify-content: center;
  
    .media-wrapper {
        position: relative;
        
        img {
            max-width: 100%;
            max-height: 700px;
        }
        
        .tag-wrapper {
            position: absolute;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            z-index: 1000;
        }
        
        &.crosshair {
            cursor: crosshair;
        }
    }
}
</style>