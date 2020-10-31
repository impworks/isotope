<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import MainMenu from "./MainMenu.vue";
@Component({
    components: { MainMenu }
})
export default class Root extends Vue {
    
}
</script>

<template>
    <div class="main-container d-flex flex-column">
        <div class="row">
            <div class="col-sm-2">
                <main-menu></main-menu>
            </div>
            <div class="col-sm-10">
                <div class="card">
                    <div class="card-body">
                        <router-view></router-view>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="scss">
.main-container {
    margin: auto;
    padding-top: 10px;
    max-width: 1300px;
    min-height: 100%;
    overflow: hidden;
}
</style>