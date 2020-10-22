<script lang="ts">
import { Component, Mixins, Vue, Watch } from "vue-property-decorator";
import { FilterStateService } from "../../services/FilterStateService";
import { Dep } from "../../utils/VueInjectDecorator";
import Filters from "./Filters.vue";
import { HasLifetime } from "../mixins/HasLifetime";

@Component({
    components: { Filters }
})
export default class DesktopFiltersWrapper extends Mixins(HasLifetime) {
    @Dep('$filter') $filter: FilterStateService;

    $refs: {
        wrapper: HTMLElement,
        content: HTMLElement
    }
    
    isOpen: boolean = false;
    height: string = '0px';
    isTransitioning: boolean = false;
    
    mounted() {
        this.isOpen = !this.$filter.isEmpty(this.$filter.state);
        this.observe(this.$filter.onStateChanged, x => {
            if(!this.$filter.isEmpty(x) && !this.isOpen)
                this.isOpen = true;
        });
    }

    toggleOpen() {
        if(this.isOpen)
            this.$filter.clear('filters');
        this.isOpen = !this.isOpen;
    }

    @Watch('isOpen')
    onOpenChanged(value: boolean) {
        const wrapper = this.$refs.wrapper;
        const content = this.$refs.content;

        if (value) {
            this.height = content.clientHeight == 0 
                ? 'auto' 
                : content.clientHeight + 'px';
            return;
        }
        
        if (wrapper.style.height == 'auto') {
            this.height = content.clientHeight + 'px';
            setTimeout(() => this.height = '0px', 1);
        } else {
            this.height = '0px';
        }
    }

    @Watch('isTransitioning')
    onTransitioningChanged(value: boolean) {
        if (!value && this.isOpen) {
            this.height = 'auto';
        }
    }
}
</script>

<template>
    <div 
        class="desktop-filters"
        :class="{ 
            'desktop-filters_opened': isOpen,
            'desktop-filters_transitioning': isTransitioning,
        }"
    >
        <a 
            class="sidebar-button clickable"
            :class="{ 'sidebar-button_opened': isOpen }"
            @click.prevent="toggleOpen()"
        >
            <div class="sidebar-button__icon">
                <div class="filter-icon"></div>
            </div>
            <div class="sidebar-button__text">
                Filters
            </div>
            <div class="sidebar-button__arrow">
                <i class="icon icon-arrow"></i>
            </div>
        </a>
        <div 
            ref="wrapper"
            class="desktop-filters__content"
            :style="{ height: height }"
            @transitionstart.self="isTransitioning = true" 
            @transitionend.self="isTransitioning = false"
        >
            <div ref="content">
                <filters></filters>
            </div>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";

    .desktop-filters {
        flex: 0 0 auto;
        position: relative;
        z-index: 3;

        &_opened {
            border-bottom: 1px solid $gray-300;
        }

        &_transitioning,
        &:not(.desktop-filters_opened)  {
            .desktop-filters__content {
                overflow: hidden;
            }
        }

        .filter-icon {
            background-image: url(../../../images/filter.svg);
        }

        &__content {
            transition: height 400ms cubic-bezier(.645,.045,.355,1);

            .filter {
                padding: 0 1rem 1rem;

                &:first-child {
                    padding-top: 1rem;
                }
            }
        }
    }
</style>