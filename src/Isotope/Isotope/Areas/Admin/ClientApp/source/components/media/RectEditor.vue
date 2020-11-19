<script lang="ts">
import { Component, Prop, Vue, Watch } from "vue-property-decorator";
import { Rect } from "../../../../../Common/source/vms/Rect";
import { TagBinding } from "../../vms/TagBinding";
import { Tag } from "../../vms/Tag";

@Component
export default class RectEditor extends Vue {
    @Prop({ required: true }) rect: Rect;
    @Prop({ required: false }) tagBinding: TagBinding;
    @Prop({ required: false }) tagsLookup: Tag[];
    @Prop({ required: false, default: false }) lockRatio: boolean;
    
    private _container: HTMLElement = null;
    active: boolean = false;
    
    origin: IPoint = { x: 0, y: 0 };
    size: IPoint = { x: 1, y: 1 };    
    
    get container(): HTMLElement {
        if(this._container)
            return this._container;
        
        let elem = this.$el as HTMLElement;
        while (elem && !elem.classList.contains('media-wrapper'))
            elem = elem.parentElement;
        
        return this._container = elem;
    }
    
    get popoverStyle() {
        return {
            left: Math.round((this.size.x - 280) / 2) + 'px'
        };
    }
    
    get isIncorrect() {
        return this.tagBinding && !this.tagBinding.tagId;
    }
    
    mounted() {
        const w = this.container.offsetWidth;
        const h = this.container.offsetHeight;
        const r = this.rect;
        this.origin = { x: Math.round(w * r.x), y: Math.round(h * r.y) };
        this.size = { x: Math.round(w * r.width), y: Math.round(h * r.height) };
    }
    
    @Watch('origin', { deep: true })
    @Watch('size', { deep: true })
    onPointsChanged() {
        const w = this.container.offsetWidth;
        const h = this.container.offsetHeight;
        this.rect.x = this.origin.x / w;
        this.rect.y = this.origin.y / h;
        this.rect.width = this.size.x / w;
        this.rect.height = this.size.y / h;
    }
    
    onDrag(x: number, y: number) {
        this.origin = { x, y };
    }
    
    onResize(x: number, y: number, w: number, h: number) {
        this.origin = { x, y };
        this.size = { x: w, y: h };
    }
    
    onRemoveRequested() {
        this.$emit('removed');
    }
}

interface IPoint {
    x: number;
    y: number;
}
</script>

<template>
    <vue-drag-resize :x="origin.x" :y="origin.y" :w="size.x" :h="size.y" :min-width="50" :min-height="50"
                     :active.sync="active" :parent="true" :lock-aspect-ratio="lockRatio"
                     :class="{'unset': isIncorrect }"
                     :title="isIncorrect ? 'Please select a tag' : null"
                     @dragging="onDrag" @resizing="onResize">
        <div v-if="tagBinding && active" class="popover bs-popover-bottom show tag-popover" :style="popoverStyle">
            <div class="arrow" style="left: 124px"></div>
            <div class="popover-body">
                <div class="form-inline">
                    <v-select :options="tagsLookup" v-model="tagBinding.tagId" label="caption" :reduce="x => x.id"
                              class="tag-popover-select">
                        <template slot="no-options">No tags created yet.</template>
                    </v-select>
                    <button type="button" class="btn btn-sm btn-outline-danger ml-2" @click.prevent="onRemoveRequested" title="Remove tag">
                        <span class="fa fa-remove"></span>
                    </button>
                </div>
            </div>
        </div>
    </vue-drag-resize>
</template>

<style lang="scss">
.v-select.tag-popover-select {
    width: 213px;
}

.vdr.unset {
    border-color: rgba(255, 0, 0, 0.5);
    
    &.active {
        border-color: rgba(255, 0, 0, 1);    
    }
}

.popover.tag-popover {
    position: absolute;
    bottom: -68px;
    top: unset;
    width: 300px;
    height: 50px;
}
</style>