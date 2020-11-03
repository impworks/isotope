<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

@Component
export default class MainMenu extends Vue {
}
</script>

<template>
    <div>
        <ul class="list-group mb-4">
            <router-link to="/folders" tag="li" class="list-group-item clickable">Content</router-link>
            <router-link to="/tags" tag="li" class="list-group-item clickable">Tags</router-link>
        </ul>
        <ul class="list-group mb-4">
            <router-link to="/shared-links" tag="li" class="list-group-item clickable">Shared links</router-link>
        </ul>
        <ul class="list-group">
            <router-link to="/users" tag="li" class="list-group-item clickable">Users</router-link>
            <router-link to="/config" tag="li" class="list-group-item clickable">Config</router-link>
        </ul>
    </div>
</template>