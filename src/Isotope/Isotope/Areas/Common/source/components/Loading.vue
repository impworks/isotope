<script lang="ts">
import { Vue, Component, Prop } from "vue-property-decorator";

@Component
export default class Loading extends Vue {
    @Prop({ type: Boolean }) isLoading: boolean;
    @Prop({ type: Boolean }) isFullPage: boolean;
    @Prop({ type: String }) text: string;
}
</script>

<template>
    <fragment>
        <div v-if="isLoading && isFullPage" class="d-flex align-items-center justify-content-center flex-grow-1 overflow-hidden">
            <div class="spinner-border spinner-large"></div>
        </div>
        <div v-if="isLoading && !isFullPage">
            <span class="spinner-border spinner-border-sm spinner-small"></span>
            <span>{{text}}</span>
        </div>
        <slot v-if="!isLoading"></slot>
    </fragment>
</template>

<style lang="scss">
.spinner-border.spinner-small {
    vertical-align: middle;
}

.spinner-border.spinner-large {
    display: inline-block;
    width: 3rem;
    height: 3rem;
    margin: 0.5em;
}
</style>


