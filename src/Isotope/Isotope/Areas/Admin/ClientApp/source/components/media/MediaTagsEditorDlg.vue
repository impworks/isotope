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

    async loadNext(skip: boolean) {
        await this.showProgress(
            skip ? 'isSkipping' : 'isWorking',
            async () => {
                const next = await this.$api.media.getNextUntagged(this.currentKey);
                if (next.key) {
                    this.currentKey = next.key;
                    await this.load();
                } else {
                    this.$close(this.result);
                }
            },
            'Failed to load next media'
        );
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
        
        this.tagOrigin = this.getOffset(evt);
        this.newTagRect = {
            x: this.tagOrigin.x,
            y: this.tagOrigin.y,
            width: 0,
            height: 0
        };
    }

    createTagMoved(evt: MouseEvent) {
        if(!this.newTagRect)
            return;

        const { x, y } = this.getOffset(evt);
        const r = this.newTagRect;
        const o = this.tagOrigin;
        
        const w = x - o.x;
        if(w >= 0) {
            r.x = o.x;
            r.width = w;
        } else {
            r.x = o.x + w;
            r.width = Math.abs(w);
        }

        const h = y - o.y;
        if(h >= 0) {
            r.y = o.y;
            r.height = h;
        } else {
            r.y = o.y + h;
            r.height = Math.abs(h);
        }
    }
    
    createTagCompleted() {
        if(!this.newTagRect)
            return;
        
        const elem = this.$refs.wrapper;
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
            const rects = this.$refs['rects'];
            const lastElem = rects[rects.length - 1].$el as HTMLElement;
            EventHelper.sendMouseEvent(lastElem, 'mousedown');
            EventHelper.sendMouseEvent(lastElem, 'mouseup');
        }, 50);
    }
    
    private getOffset(evt: MouseEvent) {
        const wp = this.$refs.wrapper.getBoundingClientRect();
        return {
            x: evt.clientX - wp.left,
            y: evt.clientY - wp.top
        };
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