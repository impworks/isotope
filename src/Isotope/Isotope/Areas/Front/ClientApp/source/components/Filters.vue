<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";
    import TransitionExpand from './TransitionExpand.vue';

    @Component({
        components: { 
            TransitionExpand
        }
    })
    export default class Filters extends Vue {
        isOpened: boolean = true;

        selectedTags: string[] = null;
        tags: string[] = ['tag 1', 'tag 2', 'tag 3', 'tag 4', 'tag 5', 'tag 6'];
    } 
</script>

<template>
    <div class="filters">
        <a 
            href="#"
            class="sidebar-button"
            :class="{ 'sidebar-button_opened': isOpened }"
            @click="isOpened = !isOpened"
        >
            <div class="sidebar-button__icon">
                <div class="filter-icon"></div>
            </div>
            <div class="sidebar-button__text">
                Filters
            </div>
            <div class="sidebar-button__arrow">
                <i class="icon icon-arrow-down"></i>
            </div>
        </a>
        <transition-expand>
            <div v-if="isOpened">
                <div class="filters__filter">
                    <h6>Search in</h6>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="exampleRadios" id="exampleRadios1" value="option1" checked>
                        <label class="form-check-label" for="exampleRadios1">
                            Current folder
                        </label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="exampleRadios" id="exampleRadios2" value="option2">
                        <label class="form-check-label" for="exampleRadios2">
                            Current folder and subfolders
                        </label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="exampleRadios" id="exampleRadios3" value="option3">
                        <label class="form-check-label" for="exampleRadios3">
                            Everywhere
                        </label>
                    </div>
                </div>
                <div class="filters__filter">
                    <h6>Tags</h6>
                    <v-select multiple v-model="selectedTags" :options="tags" />
                </div>
                <div class="filters__filter">
                    <h6>Date range</h6>
                    <div class="d-flex align-items-center justify-content-between">
                        <div>
                            <datepicker class="date"></datepicker>
                        </div>
                        <div class="px-1">—</div>
                        <div>
                            <datepicker class="date"></datepicker>
                        </div>
                    </div>
                </div>
            </div>
        </transition-expand>
    </div>
</template>

<style lang="scss">
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";

    .filters {
        flex: 0 0 auto;
        position: relative;
        z-index: 3;

        .filter-icon {
            background-image: url(../../images/filter.svg);
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

            .vdp-datepicker input {
                width: 140px;
            }
        }
    }
</style>