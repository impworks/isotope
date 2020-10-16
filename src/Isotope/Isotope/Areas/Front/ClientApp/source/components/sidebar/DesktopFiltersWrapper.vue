<script lang="ts">
import { Component, Mixins, Vue } from "vue-property-decorator";
import { FilterStateService } from "../../services/FilterStateService";
import { Dep } from "../../utils/VueInjectDecorator";
import TransitionExpand from '../utils/TransitionExpand.vue';
import Filters from "./Filters.vue";
import { HasLifetime } from "../mixins/HasLifetime";

@Component({
    components: {
        Filters, 
        TransitionExpand
    }
})
export default class DesktopFiltersWrapper extends Mixins(HasLifetime) {
    @Dep('$filter') $filter: FilterStateService;
    
    isOpen: boolean = false;
    
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
        <transition-expand>
            <Filters v-if="isOpen"></Filters>
        </transition-expand>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";

    .desktop-filters {
        flex: 0 0 auto;
        position: relative;
        z-index: 3;

        .filter:last-child {
            border-bottom: 1px solid $gray-300;
        }

        .filter-icon {
            background-image: url(../../../images/filter.svg);
        }
    }
</style>