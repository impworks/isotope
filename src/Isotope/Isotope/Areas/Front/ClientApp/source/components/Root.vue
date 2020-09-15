<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";

    @Component
    export default class Root extends Vue {
    }
</script>

<template>
    <div>
        <router-view />
    </div>
</template>