<script lang="ts">
import { Component, Prop, Vue, Watch } from "vue-property-decorator";
import { Bind } from 'lodash-decorators';
import { Debounce } from 'lodash-decorators';
import { Media } from "../../vms/Media";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { FilterStateService } from "../../services/FilterStateService";
import { TagBinding } from "../../vms/TagBinding";
import { GalleryInfo } from "../../vms/GalleryInfo";
import { SearchScope } from "../../../../../Common/source/vms/SearchScope";

@Component
export default class MediaDetails extends Vue {
    @Prop({ required: true }) media: Media;
    @Prop({ required: false }) isMobile: boolean;
    @Prop({ required: false }) isOpenOnMobile: boolean;
    
    @Dep('$filter') $filter: FilterStateService;
    
    height: string = '0px';
    isAnimated: boolean = false;
    isOpen: boolean = false;
    isTransitioning: boolean = false;
    info: GalleryInfo = null;

    $refs: {
        wrapper: HTMLElement,
        content: HTMLElement
    }
    
    get canFilter() {
        return !this.$filter.shareId;
    }

    filterByTag(binding: TagBinding) {
        if(!this.canFilter)
            return;
        
        this.$filter.update(
            'tag', 
            {
                folder: '/',
                tags: [ binding.tag.id ],
                from: null,
                to: null,
                mediaKey: null,
                scope: SearchScope.CurrentFolderAndSubfolders
            }
        );
    }

    @Watch('isOpen')
    onOpenChanged(value: boolean) {
        if (this.isMobile) {
            return;
        }

        const wrapper = this.$refs.wrapper;
        const content = this.$refs.content;

        if (value) {
            this.isAnimated = true;
            this.height = content.clientHeight + 'px';
        } else {
            this.height = content.clientHeight + 'px';
            
            setTimeout(() => {
                this.isAnimated = true;
                this.height = '0px';
            }, 1);
        }
    }

    @Watch('isTransitioning')
    onTransitioningChanged(value: boolean) {
        if(!value && !this.isMobile) {
            this.isAnimated = false;

            if(this.isOpen) {
                this.height = 'auto';
            }
        }
    }
}
</script>

<template>
    <div 
        v-if="media"
        class="media-details"
        :class="{ 
            'media-details_open': isOpen || isOpenOnMobile, 
            'media-details_mobile': isMobile,
            'media-details_transitioning': isTransitioning
        }"
        @transitionstart.self="isTransitioning = true" 
        @transitionend.self="isTransitioning = false"
    >
        <div class="media-details__header">
            <button 
                ref="button"
                class="media-details__button"
                @click.prevent="isOpen = !isOpen"
            >
                <div class="media-details__button__caption">
                    Details
                </div>
                <div class="media-details__button__arrow">
                    <i class="icon icon-arrow"></i>
                </div>
            </button>
        </div>
        <div 
            ref="wrapper"
            :style="{height: isMobile ? null : height}" 
            @transitionstart.self="isTransitioning = true" 
            @transitionend.self="isTransitioning = false"
            :class="{ 'media-details__wrapper_animated': isAnimated }"
        >   
            <div 
                ref="content" 
                class="media-details__content"
            >
                <div v-if="media.date">{{media.date}}</div>
                <div v-if="media.description">{{media.description}}</div>
                <div 
                    class="media-details__tags"
                    v-if="media.extraTags.length"
                >
                    <template v-for="t in media.extraTags">
                        <a 
                            class="media-details__tags__item clickable" 
                            v-if="canFilter" 
                            :key="t.tag.id"
                            @click.prevent="filterByTag(t)"
                        >
                            {{t.tag.caption}}
                        </a>
                        <span v-else
                            class="media-details__tags__item"
                            :key="t.tag.id">
                            {{t.tag.caption}}
                        </span>
                    </template>
                </div>
                <div class="media-details__original">
                    <a class="media-details__original__button"
                    :href="media.originalPath" target="_blank">
                    <span class="fa fa-fw fa-download"></span> View original
                    </a>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../../../Common/styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .media-details {
        left: 0;
        bottom: 0;
        color: $light;

        @include media-breakpoint-down(md) {
            opacity: 0;
            width: 100%;
            position: fixed;
            z-index: $zindex-tooltip;
            visibility: hidden;
            transition: opacity 200ms linear;
        }

        @include media-breakpoint-up(lg) {
            z-index: 2;
            min-width: 30%;
            min-height: 2.5rem;
            max-width: 100%;
            overflow: hidden;
            position: absolute;
        }

        $background: rgba(0,0,0, 0.7);

        &_open {
            z-index: $zindex-tooltip;

            @include media-breakpoint-down(md) {
                opacity: 1;
                visibility: visible;
            }

            .media-details__button {
                opacity: 1;
            }

            .media-details__button__arrow {
                transform: rotate(0);
            }
        }

        &_transitioning {
            visibility: visible;
        }

        &_mobile {

            @include media-breakpoint-up(lg) {
                display: none;
            }

            .media-details__header {
                display:none
            }
        }

        &__button {
            margin: 0;
            border: 0;
            opacity: 0.7;
            color: $light;
            display: flex;
            flex-direction: row;
            padding: 0.5rem 1rem;
            border-radius: $border-radius $border-radius 0 0;
            background-color: $background;
            transition: opacity 200ms linear;

            @include media-breakpoint-down(md) {
                display: none;
            }

            &:hover {
                opacity: 1;
            }

            &:focus {
                outline: none;
            }

            &__caption {
                padding-right: 1rem;
            }

            &__arrow {
                transform: rotate(180deg);
                transition: transform 150ms ease;
            }
        }

        &__wrapper {
            &_animated {
                transition: height 300ms cubic-bezier(.645,.045,.355,1);
            }
        }

        &__content {
            padding: 1rem;
            background-color: $background;

            @supports(padding: max(0px)) {
                padding-left: max(1rem, env(safe-area-inset-left));
                padding-right: max(1rem, env(safe-area-inset-right));
            }

            @include media-breakpoint-up(lg) {
                border-radius: 0 $border-radius 0 0;
            }
        }

        &__tags {
            font-size: 0;

            &__item {
                color: $white;
                font-size: 1rem;
                padding: 0 0.5rem;
                margin: 0.5rem 0.5rem 0 0;
                display: inline-block;
                border-radius: $border-radius;
                background-color: $primary;

                &:hover {
                    color: $white;
                    text-decoration: none;
                    background-color: darken($primary, 3%);
                }
            }
        }
        
        &__original {
            font-size: 0;
            
            &:not(:first-child) {
                margin-top: 0.5rem;
            }
            
            &__button {
                font-size: 1rem;
                color: $light;
                text-decoration: underline;

                &:hover {
                    color: $white;
                }
            }
        }
    }
</style>