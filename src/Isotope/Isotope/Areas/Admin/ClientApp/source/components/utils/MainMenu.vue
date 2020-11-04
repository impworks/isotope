<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

@Component
export default class MainMenu extends Vue {
}
</script>

<template>
    <div>
        <ul class="list-group mb-4">
            <router-link to="/folders" tag="li" class="list-group-item clickable">
                <span class="fa fa-fw fa-folder-open-o"></span>
                Content
            </router-link>
            <router-link to="/tags" tag="li" class="list-group-item clickable">
                <span class="fa fa-fw fa-tags"></span>
                Tags
            </router-link>
        </ul>
        <ul class="list-group mb-4">
            <router-link to="/shared-links" tag="li" class="list-group-item clickable">
                <span class="fa fa-fw fa-share-square"></span>
                Shared links
            </router-link>
        </ul>
        <ul class="list-group">
            <router-link to="/users" tag="li" class="list-group-item clickable">
                <span class="fa fa-fw fa-user"></span>
                Users
            </router-link>
            <router-link to="/config" tag="li" class="list-group-item clickable">
                <span class="fa fa-fw fa-wrench"></span>
                Config
            </router-link>
        </ul>
    </div>
</template>

<style lang="scss" scoped>
.list-group-item {
    padding-left: 0.75rem;
    
    .fa {
        margin-right: 0.25rem;
    }
}
</style>