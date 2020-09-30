<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { TagBindingWithLocation } from "../../vms/TagBinding";

@Component
export default class OverlayTag extends Vue {
    @Prop({ required: true }) value: TagBindingWithLocation;
    
    get style() {
        const pc = (v: number) => (v * 100) + '%';
        const loc = this.value.location;
        
        return {
            left: pc(loc.x),
            top: pc(loc.y),
            width: pc(loc.width),
            height: pc(loc.height)
        }
    }
}
</script>

<template>
    <div class="overlay-tag" :style="style"></div>
</template>

<style lang="scss" scoped>
.overlay-tag {
    position: absolute;
    border: 2px #ffffff solid;
    opacity: 0.2;
    
    &:hover {
        opacity: 0.4;
    }
}
</style>