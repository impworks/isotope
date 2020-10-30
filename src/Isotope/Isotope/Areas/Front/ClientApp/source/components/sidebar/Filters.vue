<script lang="ts">
import { Component, Mixins, Watch } from "vue-property-decorator";
import { HasLifetime } from "../mixins/HasLifetime";
import { HasAsyncState } from "../mixins/HasAsyncState";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { FilterStateService, IFilterState } from "../../services/FilterStateService";
import { Tag } from "../../vms/Tag";
import { SearchScope } from "../../../../../Common/source/vms/SearchScope";

@Component
export default class Filters extends Mixins(HasLifetime, HasAsyncState()) {
    @Dep('$api') $api: ApiService;
    @Dep('$filter') $filter: FilterStateService;

    filter: Partial<IFilterState> = { scope: SearchScope.CurrentFolderAndSubfolders };
    tags: Tag[] = [];

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
        this.$filter.update('filters', { ...this.filter });
    }

    updateFromState(state: IFilterState) {
        this.filter = {
            tags: state.tags,
            scope: state.scope || SearchScope.CurrentFolderAndSubfolders,
            from: state.from,
            to: state.to
        };
    }
}
</script>

<template>
    <fragment>
        <div class="filter">
            <h6>Tags</h6>
            <v-select
                multiple
                v-model="filter.tags"
                :disabled="asyncState.isLoading"
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
        <div class="filter hide-mobile">
            <h6>Date range</h6>
            <div class="d-flex align-items-center justify-content-between">
                <div>
                    <datepicker
                        v-model="filter.from"
                        :typeable="true"
                        :bootstrap-styling="true"
                    />
                </div>
                <div class="px-1">—</div>
                <div>
                    <datepicker
                        v-model="filter.to"
                        :typeable="true"
                        :bootstrap-styling="true"
                    />
                </div>
            </div>
        </div>
        <div class="filter hide-desktop">
            <h6>Date from</h6>
            <input 
                type="date" 
                class="form-control mb-3" 
                v-model="filter.dateFrom"
            />
            <h6>Date to</h6>
            <input 
                type="date" 
                class="form-control" 
                v-model="filter.dateTo"
            />
        </div>
        <div class="filter">
            <h6>Search in</h6>
            <div class="custom-control custom-radio">
                <input
                    type="radio"
                    id="sm-current-folder"
                    class="custom-control-input"
                    v-model="filter.scope"
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
                    v-model="filter.scope"
                    :value="2"
                >
                <label
                    class="custom-control-label"
                    for="sm-current-folder-nested"
                >
                    Current and subfolders
                </label>
            </div>
            <div class="custom-control custom-radio">
                <input
                    id="sm-everywhere"
                    type="radio"
                    class="custom-control-input"
                    v-model="filter.scope"
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
    </fragment>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    
    .filter {
        padding-bottom: 1rem;
    
        h6 {
            color: $gray-900;
            margin-bottom: 0.5rem;
        }
    }
</style>