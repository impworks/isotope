<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { TagBindingWithLocation } from "../../vms/TagBinding";
import { FilterStateService } from "../../services/FilterStateService";
import { Dep } from "../../utils/VueInjectDecorator";
import { SearchMode } from "../../vms/SearchMode";

@Component
export default class OverlayTag extends Vue {
    @Dep('$filter') $filter: FilterStateService;
    @Prop({ required: true }) value: TagBindingWithLocation;
    @Prop({ required: true }) show: boolean;
    
    get hasFilter() {
        return !this.$filter.shareId;
    }
    
    get style() {
        const pc = (v: number) => (v * 100) + '%';
        const loc = this.value.location;
        
        return {
            opacity: this.show ? undefined : 0,
            left: pc(loc.x),
            top: pc(loc.y),
            width: pc(loc.width),
            height: pc(loc.height)
        }
    }
    
    get id() {
        return 'tag-' + this.value.id;
    }
    
    filterByTag() {
        this.$filter.update(
            'tag', 
            {
                folder: '/',
                tags: [ this.value.tag.id ],
                dateFrom: null,
                dateTo: null,
                mediaKey: null,
                searchMode: SearchMode.CurrentFolderAndSubfolders
            }
        );
    }
}
</script>

<template>
    <fragment>
        <div class="overlay-tag tooltip-target" :style="style" :id="id"></div>
        <b-popover :target="id" placement="bottom" :container="id" triggers="hover">
            <a v-if="hasFilter" class="clickable" @click.prevent="filterByTag()">{{value.tag.caption}}</a>
            <span v-if="!hasFilter">{{value.tag.caption}}</span>
        </b-popover>
    </fragment>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";

    .overlay-tag {
        position: absolute;
        border: 2px solid rgba($white, 0.2);
        transition: border-color 200ms ease;
        
        &:hover {
            border-color: rgba($white, 0.5);
        }
    }
</style>