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
    
    active: boolean = false;
    
    origin: IPoint = { x: 0, y: 0 };
    size: IPoint = { x: 1, y: 1 };    
    
    get container(): HTMLElement {
        let elem = this.$el;
        while(elem && !elem.classList.contains('media-wrapper'))
            elem = elem.parentElement;
        return elem;
    }
    
    mounted() {
        const c = this.container;
        const w = c.offsetWidth;
        const h = c.offsetHeight;
        const r = this.rect;
        this.origin = { x: w * r.x, y: h * r.y };
        this.size = { x: w * r.width, y: h * r.height };
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
        // todo
    }
}

interface IPoint {
    x: number;
    y: number;
}
</script>

<template>
    <vue-drag-resize :x="origin.x" :y="origin.y" :w="size.x" :h="size.y" :parent="true">
    </vue-drag-resize>
</template>

<style lang="scss" scoped>

</style>