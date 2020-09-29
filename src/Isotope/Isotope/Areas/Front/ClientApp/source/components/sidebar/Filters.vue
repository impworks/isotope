<script lang="ts">
import { Component, Mixins, Watch } from "vue-property-decorator";
import { HasLifetime } from "../mixins/HasLifetime";
import { HasAsyncState } from "../mixins/HasAsyncState";
import { Dep } from "../../utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { FilterStateService, IFilterState } from "../../services/FilterStateService";
import { SearchMode } from "../../vms/SearchMode";
import { Tag } from "../../vms/Tag";
import TransitionExpand from '../utils/TransitionExpand.vue';

@Component({
    components: { 
        TransitionExpand
    }
})
export default class Filters extends Mixins(HasAsyncState(), HasLifetime) {
    @Dep('$api') $api: ApiService;
    @Dep('$filter') $filter: FilterStateService;
    
    isOpen: boolean = false;
    filter: Partial<IFilterState> = { searchMode: SearchMode.CurrentFolderAndSubfolders };
    tags: Tag[] = null;
    
    async mounted() {
        this.observe(this.$filter.onStateChanged, x => this.updateFromState(x));
        this.updateFromState(this.$filter.state);
        
        try {
            await this.showLoading(async () => {
                this.tags = await this.$api.getTags();
            });
        } catch (e) {
            console.log('Failed to load tags', e);
        }
    }
    
    @Watch('filter', { deep: true })
    onFilterChanged() {
        const notEmpty = this.isNotEmpty(this.filter);
        this.$filter.update('filters', { ...this.filter, searchMode: notEmpty ? this.filter.searchMode : null });
    }

    updateFromState(state: IFilterState) {
        this.filter = {
            tags: state.tags,
            searchMode: state.searchMode || SearchMode.CurrentFolderAndSubfolders,
            dateFrom: state.dateFrom,
            dateTo: state.dateTo
        };

        if(!this.isOpen && this.isNotEmpty(this.filter))
            this.isOpen = true;
    }
    
    toggleOpen() {
        if(this.isOpen)
            this.filter = { tags: null, dateFrom: null, dateTo: null, searchMode: null };
        this.isOpen = !this.isOpen;
    }
    
    isNotEmpty(s: Partial<IFilterState>) {
        return s.tags?.length || s.dateTo || s.dateFrom;
    }
}
</script>

<template>
    <div class="filters">
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
            <div v-if="isOpen">
                <div class="filters__filter">
                    <h6>Search in</h6>
                    <div class="custom-control custom-radio">
                        <input 
                            type="radio"
                            id="sm-current-folder"
                            class="custom-control-input" 
                            v-model="filter.searchMode" 
                            :value="1"
                        >
                        <label 
                            class="custom-control-label" 
                            for="sm-current-folder"
                        >
                            Current folder
                        </label>
                    </div>
                    <div class="custom-control custom-radio">
                        <input 
                            type="radio"
                            id="sm-current-folder-nested"
                            class="custom-control-input"
                            v-model="filter.searchMode" 
                            :value="2"
                        >
                        <label 
                            class="custom-control-label" 
                            for="sm-current-folder-nested"
                        >
                            Current folder and subfolders
                        </label>
                    </div>
                    <div class="custom-control custom-radio">
                        <input 
                            id="sm-everywhere" 
                            type="radio"  
                            class="custom-control-input" 
                            v-model="filter.searchMode" 
                            :value="3"
                        >
                        <label 
                            class="custom-control-label" 
                            for="sm-everywhere"
                        >
                            Everywhere
                        </label>
                    </div>
                </div>
                <div class="filters__filter">
                    <h6>Tags</h6>
                    <v-select 
                        multiple
                        v-if="tags"
                        v-model="filter.tags" 
                        :options="tags" 
                        :reduce="x => x.id"
                        label="caption"
                    >
                        <template #open-indicator="{ attributes }">
                            <i 
                                class="icon icon-arrow"
                                v-bind="attributes"
                            ></i>
                        </template>
                    </v-select>
                </div>
                <div class="filters__filter">
                    <h6>Date range</h6>
                    <div class="d-flex align-items-center justify-content-between">
                        <div>
                            <datepicker 
                                v-model="filter.dateFrom"
                                :typeable="true" 
                                :bootstrap-styling="true" 
                            />
                        </div>
                        <div class="px-1">—</div>
                        <div>
                            <datepicker 
                                v-model="filter.dateTo"
                                :typeable="true" 
                                :bootstrap-styling="true" 
                            />
                        </div>
                    </div>
                </div>
            </div>
        </transition-expand>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";

    .filters {
        flex: 0 0 auto;
        position: relative;
        z-index: 3;

        .filter-icon {
            background-image: url(../../../images/filter.svg);
        }

        &__filter {
            padding: 0 1rem 1rem;

            &:first-child {
                padding-top: 1rem;
            }

            &:last-child {
                border-bottom: 1px solid $gray-300;
            }

            h6 {
                color: $gray-900;
                margin-bottom: 0.5rem;
            }
        }
    }
</style>