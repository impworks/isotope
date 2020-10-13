<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";
import { Media } from "../../vms/Media";
import { Dep } from "../../utils/VueInjectDecorator";

@Component
export default class MediaContent extends Vue {
    @Dep('$host') $host: string;
    @Prop({ required: true }) elem: ICachedMedia;

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
    <div class="media-viewer__content__item">
        <div v-if="elem">
            <loading :is-loading="elem.isLoading">
                <img v-if="elem.media"
                     :src="getAbsolutePath(elem.media.fullPath)"
                     :alt="elem.media.description" />
            </loading>
        </div>
    </div>
</template>

<style lang="scss">
    .media-viewer {
        &__content {
            &__item {
                height: 100%;
                min-width: 100vw;
                position: relative;

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
        }
    }
</style>