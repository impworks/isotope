<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Rect } from "../../../../../Common/source/vms/Rect";
import { TagBinding } from "../../vms/TagBinding";
import { Tag } from "../../vms/Tag";

@Component
export default class RectEditor extends Vue {
    @Prop({ required: true }) rect: Rect;
    @Prop({ required: false }) tagBinding: TagBinding;
    @Prop({ required: false }) tagsLookup: Tag[];
    
    get dimensions() {
        const pc = (v: number) => (v * 100) + '%';
        return {
            left: pc(this.rect.x),
            top: pc(this.rect.y),
            width: pc(this.rect.width),
            height: pc(this.rect.height)
        };
    }
}
</script>

<template>
    <div class="rect" :style="dimensions"></div>
</template>

<style lang="scss" scoped>
.rect {
    position: absolute;
    display: block;
    border: 2px #FFFFFF solid;
    opacity: 0.2;
    background: transparent;
}
</style>