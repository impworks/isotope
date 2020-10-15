<script lang="ts">
import { Component, Prop, Vue, Watch } from "vue-property-decorator";
import { Bind } from 'lodash-decorators';
import { Debounce } from 'lodash-decorators';
import { Media } from "../../vms/Media";
import { Dep } from "../../utils/VueInjectDecorator";
import { FilterStateService } from "../../services/FilterStateService";
import { SearchMode } from "../../vms/SearchMode";
import { TagBinding } from "../../vms/TagBinding";

@Component
export default class MediaDetails extends Vue {
    @Prop({ required: true }) media: Media;
    @Dep('$filter') $filter: FilterStateService;
    
    isOpen: boolean = false;
    height: number = 0;

    $refs: {
        button: HTMLElement,
        content: HTMLElement
    }

    mounted () {
        this.height = this.$refs.button.clientHeight;

        window.addEventListener("resize", this.resizeHandler);
    }

    beforeDestroy() {
        window.removeEventListener('resize', this.resizeHandler);
    }

    countHeight() {
        this.height = this.isOpen 
            ? this.$refs.button.clientHeight + this.$refs.content.clientHeight
            : this.$refs.button.clientHeight;
    }

    get hasDetails(): boolean {
        if (this.media.date != null || this.media.description != null || this.media.extraTags.length > 0 ) {
            return true;
        } else {
            return false;
        }
    }

    filterByTag(tag: TagBinding) {
        this.$filter.update(
            'tag', 
            {
                folder: '/',
                tags: [ tag.id ],
                dateFrom: null,
                dateTo: null,
                mediaKey: null,
                searchMode: SearchMode.CurrentFolderAndSubfolders
            }
        );
    }

    @Debounce(50)
    @Bind()
    resizeHandler() {
        this.countHeight();
    }

    @Watch('isOpen')
    onOpenChanged() {
        this.countHeight();
    }
}
</script>

<template>
    <div 
        v-show="hasDetails"
        class="media-details"
        :style="{height: height + 'px'}" 
        :class="{ 'media-details_open': isOpen }"
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
            ref="content"
            class="media-details__content"
        >
            <div v-if="media.date">{{media.date}}</div>
            <div v-if="media.description">{{media.description}}</div>
            <div 
                class="media-details__tags"
                v-if="media.extraTags.length"
            >
                <a 
                    class="media-details__tags__item clickable" 
                    v-for="t in media.extraTags" 
                    :key="t.tag.id"
                    @click.prevent="filterByTag(t)"
                >
                    {{t.tag.caption}}
                </a>
            </div>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .media-details {
        left: 0;
        bottom: 0;
        z-index: 2;
        color: $light;
        min-width: 30%;
        max-width: 100%;
        overflow: hidden;
        position: absolute;
        min-height: 2.5rem;
        transition: height 300ms cubic-bezier(.645,.045,.355,1);

        $background: rgba(0,0,0, 0.7);

        &_open {
            z-index: $zindex-tooltip;

            .media-details__button {
                opacity: 1;
            }

            .media-details__button__arrow {
                transform: rotate(180deg);
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
                transition: transform 150ms ease;
            }
        }

        &__content {
            padding: 1rem;
            border-radius: 0 $border-radius 0 0;
            background-color: $background;
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
    }
</style>