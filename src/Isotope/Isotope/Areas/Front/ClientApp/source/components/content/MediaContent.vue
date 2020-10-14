<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";
import { Media } from "../../vms/Media";
import { Dep } from "../../utils/VueInjectDecorator";
import OverlayTag from "./OverlayTag.vue";
import { Bind } from 'lodash-decorators';
import { Debounce } from 'lodash-decorators';

@Component({
    components: { OverlayTag }
})
export default class MediaContent extends Vue {
    @Dep('$host') $host: string;
    @Prop({ required: true }) elem: ICachedMedia;

    maxHeight: number = 0;
    showOverlay: boolean = false;

    $refs: {
        card: HTMLElement
    }

    getAbsolutePath(path: string) {
        return this.$host + path;
    }

    mounted () {
        window.addEventListener("resize", this.resizeHandler);
    }

    beforeDestroy() {
        window.removeEventListener('resize', this.resizeHandler);
    }

    @Debounce(50)
    @Bind()
    resizeHandler() {
        if (this.$refs.card) {
            const imageHeight = this.elem.media.height;
            const windowHeight = window.innerHeight;
            const cardStyles = getComputedStyle(this.$refs.card, null);
            const paddingSize = parseFloat(cardStyles.paddingTop) + parseFloat(cardStyles.paddingBottom);
            const marginSize = parseFloat(cardStyles.marginTop) + parseFloat(cardStyles.marginBottom);

            if (windowHeight >= imageHeight + marginSize + paddingSize) {
                this.maxHeight = imageHeight;
            } else {
                this.maxHeight = windowHeight - paddingSize - marginSize;
            }
        }
    }

    @Watch('elem.isLoading')
    onLoaded(value: boolean) {
        if (!value) {
            this.resizeHandler();
        }
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
        <div 
            class="media-content" 
            v-if="elem"
        >
            <div class="media-content__card" ref="card">
                <loading 
                    :is-loading="elem.isLoading"
                    :is-full-page="true"
                >
                    <div class="media-content__wrapper">
                        <div class="media-content__wrapper__overlay">
                            I am overlay!
                        </div>
                        <img 
                            :style="{maxHeight : maxHeight + 'px'}"
                            v-if="elem.media"
                            :src="getAbsolutePath(elem.media.fullPath)"
                            :alt="elem.media.description" 
                        />
                    </div>
                </loading>
            </div>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .media-viewer__item {
        width: 100%;
        height: 100%;
        min-width: 100%;
        box-sizing: border-box;
        position: relative;
        -webkit-transform: translateZ(0);
        -webkit-backface-visibility: hidden;
    }

    .media-content {
        display: flex;
        width: 100%;
        height: 100%;
        align-items: center;
        justify-content: center;
        will-change: contents;

        &__card {
            margin: auto;
            max-width: 100%;
            max-height: 100%;
            background: $white;
            border-radius: $border-radius;

            @include media-breakpoint-down(sm) {
                margin: 0.5rem;
                padding: 0.5rem;
                box-shadow: $box-shadow;
            }

            @include media-breakpoint-up(md) {
                margin: 1rem;
                padding: 1rem;
                box-shadow: $box-shadow-lg;
            }
        }

        &__wrapper {
            margin: auto;
            position: relative;
            border-radius: $border-radius;

            &__overlay {
                top: 0;
                left: 0;
                padding: 0.5rem;
                // width: 100%;
                // height: 100%;
                position: absolute;
                background-color: rgba($white, 0.5);
            }
        }

        img {
            max-width: 100%;
            max-height: 100%;
        }
    }
</style>