<script lang="ts">

import { Component, Vue } from "vue-property-decorator";

@Component
export default class MainView extends Vue {
    get folder(): string {
        return this.$route.path;
    }
}
</script>

<template>
    <div>Current path: <b>{{folder}}</b></div>
</template>