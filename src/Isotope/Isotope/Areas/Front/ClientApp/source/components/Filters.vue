<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component
    export default class Filters extends Vue {} 
</script>

<template>
    <div class="filters">
        filters
    </div>
</template>

<style lang="scss">
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";

    .filters {
        padding: 1rem;
    }
</style>