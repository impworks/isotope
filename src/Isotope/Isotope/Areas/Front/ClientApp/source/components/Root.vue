<script lang="ts">
    import { Component, Vue } from "vue-property-decorator";
    import { Dep } from "../utils/VueInjectDecorator";
    import { ClientApiService } from "../services/ClientApiService";

    @Component
    export default class Root extends Vue {
      @Dep('$api') $api: ClientApiService;
      
      async mounted() {
        const folders = await this.$api.getFolderTree();
        console.log(folders);
      }
    }
</script>

<template>
    <router-view />
</template>