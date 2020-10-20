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
        content: HTMLElement
    }
    
    isOpen: boolean = false;
    height: number = 0;
    
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
        this.height = this.isOpen 
            ? this.$refs.content.clientHeight
            : 0;
    }
}
</script>

<template>
    <div class="desktop-filters">
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
            class="desktop-filters__content"
            :style="{height: height + 'px'}" 
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
        overflow: hidden;

        .filter:last-child {
            border-bottom: 1px solid $gray-300;
        }

        .filter-icon {
            background-image: url(../../../images/filter.svg);
        }

        &__content {
            transition: height 400ms cubic-bezier(.645,.045,.355,1);
        }
    }
</style>