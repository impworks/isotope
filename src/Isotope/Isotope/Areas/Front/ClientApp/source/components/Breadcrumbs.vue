<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component
    export default class Breadcrumbs extends Vue {} 
</script>

<template>
    <ul class="breadcrumbs">
        <li class="breadcrumbs__item">
            <a href="#">My gallery</a>
        </li>
        <li class="breadcrumbs__item">
            <a href="#">Subfolder</a>
        </li>
        <li class="breadcrumbs__item breadcrumbs__item_active">
            Active folder
        </li>
    </ul>
</template>

<style lang="scss">
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";
    
    .breadcrumbs {
        list-style-type: none;
        margin: 0;
        padding: 1rem;
        display: flex;
        flex-direction: row;
        line-height: 1.625;

        &__item {
            position: relative;
            padding: 0 2em 0 0;
            
            &:not(:last-child):after {
                top: 0;
                right: 0;
                width: 2em;
                content: '>';
                color: $gray-500;
                position: absolute;
                text-align: center;
                display: inline-block;
            }

            &_active {
                font-weight: 500;
            }
        }
    }
</style>