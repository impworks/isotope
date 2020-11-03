<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { DialogsWrapper } from "vue-modal-dialogs";
import MainMenu from "./utils/MainMenu.vue";

@Component({
    components: { MainMenu, DialogsWrapper }
})
export default class Root extends Vue {

}
</script>

<template>
    <div>
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
        <dialogs-wrapper transition-name="fade"></dialogs-wrapper>
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