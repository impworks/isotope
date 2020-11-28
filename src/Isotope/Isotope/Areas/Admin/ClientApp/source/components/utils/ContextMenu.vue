<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

@Component
export default class ContextMenu extends Vue {
    isShown: boolean = false;
    data: any = null;
    pos: { top: string, left: string } = null;
    
    open(e: MouseEvent, data: any) {
        this.isShown = true;
        this.data = data;
        setTimeout(() => this.pos = this.getPosition(e), 1);
    }
    
    close() {
        this.isShown = false;
        this.data = null;
        this.pos = null;
    }
    
    private getPosition(e: MouseEvent) {
        const offset = 5;
        let x = e.clientX + offset;
        let y = e.clientY + offset;
        const w = this.$el.clientWidth;
        const h = this.$el.clientHeight;
        const vw = window.scrollX + window.innerWidth;
        const vh = window.scrollY + window.innerHeight;
        
        console.log({ x, y, w, h, vw, vh });
        
        if(x + w >= vw)
            x -= (w + offset * 2);
        
        if(y + h >= vh)
            y -= (h + offset * 2);
        
        return {
            left: x + 'px',
            top: y + 'px'
        };
    }
}
</script>

<template>
    <div :class="{'dropdown-menu': true, 'show': isShown, 'placed': !!pos}" :style="pos" v-click-outside="close" @click="close">
        <slot :data="data"></slot>
    </div>
</template>

<style scoped lang="scss">
.dropdown-menu {
    opacity: 0.01;
}
.placed {
    opacity: 1;
}
</style>