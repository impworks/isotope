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
    @Prop({ required: false }) ratio: number;
    
    private _container: HTMLElement = null;
    active: boolean = false;
    
    origin: IPoint = { x: 0, y: 0 };
    size: IPoint = { x: 1, y: 1 };    
    
    get container(): HTMLElement {
        if(this._container)
            return this._container;
        
        let elem = this.$el;
        while (elem && !elem.classList.contains('media-wrapper'))
            elem = elem.parentElement;
        
        return this._container = elem;
    }
    
    mounted() {
        const w = this.container.offsetWidth;
        const h = this.container.offsetHeight;
        const r = this.rect;
        this.origin = { x: Math.round(w * r.x), y: Math.round(h * r.y) };
        this.size = { x: Math.round(w * r.width), y: Math.round(h * r.height) };
    }
    
    focus() {
        this.active = true;
    }
    
    unfocus() {
        this.active = false;
    }
    
    @Watch('origin', { deep: true })
    @Watch('size', { deep: true })
    onPointsChanged() {
        const w = this.container.offsetWidth;
        const h = this.container.offsetHeight;
        const o = this.origin;
        const s = this.size;
        this.rect.x = o.x / w;
        this.rect.y = o.y / h;
        this.rect.width = (s.x - o.x) / w;
        this.rect.height = (s.y - o.y) / h;
    }
    
    onDrag(x: number, y: number) {
        this.origin = { x, y };
    }
    
    onResize(x: number, y: number, w: number, h: number) {
        this.origin = { x, y };
        this.size = { x: w, y: h };
    }
}

interface IPoint {
    x: number;
    y: number;
}
</script>

<template>
    <vue-drag-resize :x="origin.x" :y="origin.y" :w="size.x" :h="size.y" @dragging="onDrag" @resizing="onResize" :parent="true">
    </vue-drag-resize>
</template>

<style lang="scss" scoped>

</style>