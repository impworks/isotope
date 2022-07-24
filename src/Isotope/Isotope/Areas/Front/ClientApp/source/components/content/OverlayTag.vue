<script lang="ts">
import Popper from 'popper.js';
import { Component, Mixins, Prop, Watch } from "vue-property-decorator";
import { TagBindingWithLocation } from "../../vms/TagBinding";
import { FilterStateService } from "../../services/FilterStateService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { SearchScope } from "../../../../../Common/source/vms/SearchScope";
import { EventBusService } from "../../services/EventBusService";
import { HasLifetime } from "../mixins/HasLifetime";

@Component
export default class OverlayTag extends Mixins(HasLifetime) {
    @Dep('$filter') $filter: FilterStateService;
    @Dep('$eventBus') $eventBus: EventBusService;
    
    @Prop({ required: true }) value: TagBindingWithLocation;
    @Prop({ required: false }) isMobile: boolean;
    @Prop({ required: false }) isMobileOverlayVisible: boolean;
    @Prop({ required: false }) tappedTag: TagBindingWithLocation;

    isTransitioning: boolean = false;
    isTapped: boolean = false;
    
    $refs: {
        frame: HTMLElement;
        popover: HTMLElement;
    };
    
    private p: Popper;
    
    get canFilter() {
        return !this.$filter.shareId;
    }
    
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
    
    created() {
        this.$nextTick(() => {
            this.p = new Popper(this.$refs.frame, this.$refs.popover, {
                placement: 'bottom',
                eventsEnabled: true,
                modifiers: {
                    arrow: {
                        element: '.arrow'
                    }
                }
            });
            this.observe(this.$eventBus.uiUpdated, () => this.p.scheduleUpdate());
        });
    }

    onTap() {
        this.$emit('tagTapped', this.value);
    }
    
    filterByTag() {
        if(!this.canFilter)
            return;
        
        this.$filter.update(
            'tag', 
            {
                folder: '/',
                tags: [ this.value.tag.id ],
                from: null,
                to: null,
                mediaKey: null,
                scope: SearchScope.CurrentFolderAndSubfolders
            }
        );
    }

    @Watch('isMobile')
    @Watch('isMobileOverlayVisible')
    onOverlayVisibilityChanged(value: boolean) {
        if(!value) {
            this.isTapped = false;
        }
    }

    @Watch('tappedTag')
    onTappedTagChanged(value: TagBindingWithLocation) {
        this.isTapped = value == this.value;
    }
}
</script>

<template>
    <div>
        <div 
            v-hammer:tap="onTap"
            ref="frame"
            class="overlay-tag tooltip-target" 
            :class="{
                'overlay-tag_mobile' : isMobile, 
                'overlay-tag_mobile-visible' : isMobileOverlayVisible,
                'overlay-tag_transitioning': isTransitioning,
                'overlay-tag_tapped': isTapped
            }"
            :style="style"
            @transitionstart.self="isTransitioning = true" 
            @transitionend.self="isTransitioning = false"
        >
            <div class="popover bs-popover-bottom tag-popover" ref="popover">
                <div class="arrow"></div>
                <div class="popover-body">
                    <a
                        v-if="canFilter"
                        class="clickable"
                        @click.prevent="filterByTag()"
                    >
                        {{value.tag.caption}}
                    </a>
                    <span v-else>
                    {{value.tag.caption}}
                </span>
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

    .overlay-tag {
        position: absolute;
        border: 2px solid rgba($white, 0.2);
        transition: 
            border-color 200ms linear,
            opacity 200ms linear;

        @include media-breakpoint-down(md) {
            visibility: hidden;
        }

        &_mobile {
            opacity: 0;
        }

        &_mobile-visible {
            opacity: 1;
            visibility: visible;
        }

        &_transitioning {
            visibility: visible;
        }

        &_tapped .popover {
            @include media-breakpoint-down(md) {
                opacity: 1 !important;
                border-color: rgba($white, 0.5);
            }
        }

        .popover {
            opacity: 0;
            transition: opacity 200ms linear;

            &:focus {
                outline: none;
            }
        }

        &:hover {
            @include media-breakpoint-up(lg) {
                border-color: rgba($white, 0.5);
            }

            .popover {
                @include media-breakpoint-up(lg) {
                    opacity: 1;
                }
            }
        }
    }
</style>