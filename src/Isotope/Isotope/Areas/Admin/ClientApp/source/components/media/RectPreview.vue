<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Rect } from "../../../../../Common/source/vms/Rect";

@Component
export default class RectPreview extends Vue {
    @Prop({ required: true }) rect: Rect;
    
    get style() {
        return {
            left: this.rect.x + 'px',
            top: this.rect.y + 'px',
            width: this.rect.width + 'px',
            height: this.rect.height + 'px'
        };
    }
}
</script>

<template>
    <div class="rect-preview" :style="style"></div>
</template>

<style scoped lang="scss">
.rect-preview {
    position: absolute;
    border: 2px solid rgba(255, 255, 255, 0.5);
}
</style>