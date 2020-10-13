<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";
import { Media } from "../../vms/Media";
import { Dep } from "../../utils/VueInjectDecorator";
import OverlayTag from "./OverlayTag.vue";

@Component({
    components: { OverlayTag }
})
export default class MediaContent extends Vue {
    @Dep('$host') $host: string;
    @Prop({ required: true }) elem: ICachedMedia;

    showOverlay: boolean = false;

    getAbsolutePath(path: string) {
        return this.$host + path;
    }
}

interface IMedia {
    media?: Media;
    img?: HTMLImageElement;
}

interface ICachedMedia extends IMedia {
    isLoading: boolean;
}
</script>

<template>
    <div class="media-viewer__item">
        <div class="media-content">
            <loading :is-loading="elem.isLoading">
                <img 
                    v-if="elem.media"
                    :src="getAbsolutePath(elem.media.fullPath)"
                    :alt="elem.media.description" 
                />
            </loading>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .media-viewer {

        &__item {
            width: 100%;
            height: 100%;
            margin: auto;
            flex-grow: 1;
            flex-basis: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            position: relative;
        }
    }

    .media-content {

        img {
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            margin: auto;
            max-width: 100%;
            max-height: 100%;
            position: absolute;
            vertical-align: bottom;
        }
    }
</style>