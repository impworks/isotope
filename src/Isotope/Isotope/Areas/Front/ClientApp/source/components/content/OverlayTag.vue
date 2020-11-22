<script lang="ts">
import { Component, Prop, Vue, Watch } from "vue-property-decorator";
import { TagBindingWithLocation } from "../../vms/TagBinding";
import { FilterStateService } from "../../services/FilterStateService";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { SearchScope } from "../../../../../Common/source/vms/SearchScope";

@Component
export default class OverlayTag extends Vue {
    @Dep('$filter') $filter: FilterStateService;
    @Prop({ required: true }) value: TagBindingWithLocation;
    @Prop({ required: false }) isMobile: boolean;
    @Prop({ required: false }) isMobileOverlayVisible: boolean;
    @Prop({ required: false }) tappedTag: TagBindingWithLocation;

    isTransitioning: boolean = false;
    isTapped: boolean = false;
    
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
    
    get id() {
        return 'tag-' + this.value.id;
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
    <fragment>
        <div 
            v-hammer:tap="onTap"
            class="overlay-tag tooltip-target" 
            :class="{
                'overlay-tag_mobile' : isMobile, 
                'overlay-tag_mobile-visible' : isMobileOverlayVisible,
                'overlay-tag_transitioning': isTransitioning,
                'overlay-tag_tapped': isTapped
            }"
            :style="style" 
            :id="id"
            @transitionstart.self="isTransitioning = true" 
            @transitionend.self="isTransitioning = false"
        ></div>
        <b-popover 
            :target="id" 
            :container="id" 
            :show="true"
            triggers="manual"
            placement="bottom" 
        >
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
        </b-popover>
    </fragment>
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

        @include media-breakpoint-down(sm) {
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
            @include media-breakpoint-down(sm) {
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
            @include media-breakpoint-up(md) {
                border-color: rgba($white, 0.5);
            }

            .popover {
                @include media-breakpoint-up(md) {
                    opacity: 1;
                }
            }
        }
    }
</style>