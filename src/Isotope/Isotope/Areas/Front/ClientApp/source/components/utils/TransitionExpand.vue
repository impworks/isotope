<script lang="ts">
import { Vue, Component } from "vue-property-decorator";

@Component
export default class TransitionExpand extends Vue {
    enter(element) {
        const width = getComputedStyle(element).width;

        element.style.width = width;
        element.style.position = 'absolute';
        element.style.visibility = 'hidden';
        element.style.height = 'auto';

        const height = getComputedStyle(element).height;

        element.style.width = null;
        element.style.position = null;
        element.style.visibility = null;
        element.style.height = 0;

        // Force repaint to make sure the
        // animation is triggered correctly.
        getComputedStyle(element).height;

        // Trigger the animation.
        // We use `requestAnimationFrame` because we need
        // to make sure the browser has finished
        // painting after setting the `height`
        // to `0` in the line above.
        requestAnimationFrame(() => {
            element.style.height = height;
        });
    } 

    afterEnter(element) {
        element.style.height = 'auto';
    }

    leave(element) {
        const height = getComputedStyle(element).height;
        
        element.style.height = height;

        // Force repaint to make sure the
        // animation is triggered correctly.
        getComputedStyle(element).height;

        requestAnimationFrame(() => {
            element.style.height = 0;
        });
    }
}
</script>

<template>
    <transition
        name="expand"
        @enter="enter"
        @after-enter="afterEnter"
        @leave="leave"
    >
        <slot/>
    </transition>
</template>

<style lang="scss" scoped>
    * {
        will-change: height;
        transform: translateZ(0);
        backface-visibility: hidden;
        perspective: 1000px;
    }

    .expand-enter-active,
    .expand-leave-active {
        transition: height 400ms cubic-bezier(.645,.045,.355,1);
        overflow: hidden;
    }

    .expand-enter,
    .expand-leave-to {
        height: 0;
    }
</style>