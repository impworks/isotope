<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import { OverlayTagBinding } from "../../../vms/OverlayTagBinding";
import { Rect } from "../../../../../../Common/source/vms/Rect";
import { TagBindingType } from "../../../../../../Common/source/vms/TagBindingType";
import { EventHelper } from "../../../../../../Common/source/utils/EventHelper";
import { Media } from "../../../vms/Media";
import { Tag } from "../../../vms/Tag";

import RectPreview from "../RectPreview.vue";
import RectEditor from "../RectEditor.vue";

@Component({
    components: { RectPreview, RectEditor }
})
export default class MediaTagsEditor extends Vue {

    // -----------------------------------
    // Dependencies
    // -----------------------------------
    
    @Prop({ required: true }) media: Media;
    @Prop({ required: true }) tagsLookup: Tag[];

    // -----------------------------------
    // Properties
    // -----------------------------------
    
    $refs: {
        img: HTMLImageElement;
        rects: Vue[];
        wrapper: HTMLElement;
    };
    
    extraTags: number[] = [];
    
    isCreatingTag: boolean = false;
    tagOrigin: { x: number, y: number } = null;
    newTagRect: Rect = null;

    // -----------------------------------
    // Methods
    // -----------------------------------
    
    toggleCreatingTagMode(state: boolean) {
        this.isCreatingTag = state;
        this.newTagRect = null;
    }

    removeTag(b: OverlayTagBinding) {
        const list = this.media.overlayTags;
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
        this.media.overlayTags.push({ type: TagBindingType.Depicted, location: relRect, tagId: null});

        this.newTagRect = null;
        this.isCreatingTag = false;

        // activate the newly created tag
        setTimeout(() => {
            const rects = this.$refs['rects'];
            const lastElem = rects[rects.length - 1].$el as HTMLElement;
            EventHelper.sendMouseClick(lastElem);
        }, 50);
    }

    private getOffset(evt: MouseEvent) {
        const wp = this.$refs.wrapper.getBoundingClientRect();
        return {
            x: evt.clientX - wp.left,
            y: evt.clientY - wp.top
        };
    }

    // -----------------------------------
    // Watches
    // -----------------------------------
    
    @Watch('extraTags')
    onExtraTagsChanged() {
        this.media.extraTags = this.extraTags.map(x => ({ tagId: x, type: TagBindingType.Custom }));
    }
}
</script>

<template>
    <div>
        <div class="form-group">
            <div class="pull-right">
                <button class="btn btn-sm btn-outline-secondary mr-2" type="button"
                        @click.prevent="toggleCreatingTagMode(true)"
                        :disabled="isCreatingTag">
                    <span class="fa fa-plus-circle"></span>
                    <span v-if="isCreatingTag">Adding tag: Esc to cancel</span>
                    <span v-else title="Add a new tag (Ctrl + Space)">Add tag</span>
                </button>
                <button class="btn btn-sm btn-outline-secondary" type="button"
                        @click.prevent="$emit('pick-tags')"
                        title="Configure tags">
                    <span class="fa fa-cog"></span>
                </button>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="form-group media-container">
            <div class="media-wrapper" :class="{'crosshair': isCreatingTag }"
                 @mousedown.prevent="createTagStarted($event)"
                 @mousemove.prevent="createTagMoved($event)"
                 @mouseup.prevent="createTagCompleted()"
                 ref="wrapper">
                <img :src="media.fullPath" />
                <RectPreview v-if="newTagRect" :rect="newTagRect"></RectPreview>
                <div class="tag-wrapper" v-if="!isCreatingTag">
                    <RectEditor v-for="(b, idx) in media.overlayTags"
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
                          :disabled="isCreatingTag" v-burst-selection>
                    <template slot="no-options">No tags created yet.</template>
                </v-select>
            </div>
        </div>
        <GlobalEvents @keydown.esc="toggleCreatingTagMode(false)"
                      @keydown.ctrl.space.stop.prevent="toggleCreatingTagMode(true)">
        </GlobalEvents>
    </div>
</template>