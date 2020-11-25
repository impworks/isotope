<script lang="ts">
import { Component, Mixins, Prop, Vue } from "vue-property-decorator";
import GlobalEvents from "vue-global-events";

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
export default class MediaTagsEditorDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: true }) mediaKey: string;

    value: Media = null;
    extraTags: number[] = [];
    
    tagsLookup: Tag[] = [];
    
    isCreatingTag: boolean = false;
    newTagRect: Rect = null;
    
    get isIncorrect() {
        return this.value?.overlayTags.some(x => !x.tagId);
    }
    
    get canSave() {
        return !this.asyncState.isSaving
            && !this.isCreatingTag
            && !this.isIncorrect;
    }

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
    
    toggleCreatingTagMode(state: boolean) {
        this.isCreatingTag = state;
        this.newTagRect = null;
    }
    
    removeTag(b: OverlayTagBinding) {
        const list = this.value.overlayTags;
        const idx = list.indexOf(b);
        if(idx !== -1)
            list.splice(idx, 1);
    }
    
    createTagStarted(evt: MouseEvent) {
        if(!this.isCreatingTag || this.newTagRect)
            return;
        
        this.newTagRect = {
            x: evt.offsetX,
            y: evt.offsetY,
            width: 0,
            height: 0
        };
    }

    createTagMoved(evt: MouseEvent) {
        if(!this.newTagRect)
            return;
        
        const x = evt.offsetX;
        const y = evt.offsetY;
        const r = this.newTagRect;
        r.width = Math.max(0, x - r.x);
        r.height = Math.max(0, y - r.y);
    }
    
    createTagCompleted() {
        if(!this.newTagRect)
            return;
        
        const elem = this.$refs['media-wrapper'] as HTMLElement;
        const w = elem.offsetWidth;
        const h = elem.offsetHeight;
        const r = this.newTagRect;
        
        const relRect: Rect = {
            x: r.x / w,
            y: r.y / h,
            width: Math.min(r.width / w, 1 - (r.x / w)),
            height: Math.min(r.height / h, 1 - (r.y / h))
        };
        this.value.overlayTags.push({ type: TagBindingType.Depicted, location: relRect, tagId: null});
        
        this.newTagRect = null;
        this.isCreatingTag = false;
        
        // activate the newly created tag
        setTimeout(() => {
            const rects = this.$refs['rects'] as Vue[];
            const lastElem = rects[rects.length - 1].$el as HTMLElement;
            EventHelper.sendMouseEvent(lastElem, 'mousedown');
            EventHelper.sendMouseEvent(lastElem, 'mouseup');
        }, 50);
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
                            <button type="button" class="close" @click="$close(false)">&times;</button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group">
                                <div class="mb-2">
                                    <div class="pull-right">
                                        <button class="btn btn-sm btn-outline-secondary" type="button"
                                                @click.prevent="toggleCreatingTagMode(true)"
                                                :disabled="asyncState.isSaving || isCreatingTag">
                                            <span class="fa fa-plus-circle"></span>
                                            <span v-if="isCreatingTag">Adding tag: Esc to cancel</span>
                                            <span v-else>Add tag</span>
                                        </button>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="media-wrapper" :class="{'crosshair': this.isCreatingTag}"
                                     @mousedown.prevent="createTagStarted($event)"
                                     @mousemove.prevent="createTagMoved($event)"
                                     @mouseup.prevent="createTagCompleted()"
                                     ref="media-wrapper">
                                    <img :src="value.fullPath" ref="img" />
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
                            <button type="submit" class="btn btn-primary" :disabled="!canSave">
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
        <GlobalEvents @keydown.esc="toggleCreatingTagMode(false)"></GlobalEvents>
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
    
    &.crosshair {
        cursor: crosshair;
    }
}
</style>