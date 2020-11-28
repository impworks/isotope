<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { DialogsWrapper } from "vue-modal-dialogs";

import MainMenu from "./utils/MainMenu.vue";
import { Dep } from "../../../../Common/source/utils/VueInjectDecorator";
import { AuthService } from "../../../../Common/source/services/AuthService";

@Component({
    components: { MainMenu, DialogsWrapper }
})
export default class MainView extends Vue {
    @Dep('$auth') $auth: AuthService;
    
    logout() {
        this.$auth.user = null;
    }
}
</script>

<template>
    <div>
        <nav class="navbar navbar-dark bg-dark mb-4">
            <div class="container px-4">
                <a class="navbar-brand" href="/" title="To gallery home">
                    <span class="fa fa-fw fa-image"></span>
                    Isotope
                </a>
                <div class="navbar-nav mr-auto"></div>
                <div class="my-2 my-lg-0">
                    <ul class="navbar-nav">
                        <li class="nav-item">
                            <a class="nav-link clickable" @click.prevent="logout()">
                                <span class="fa fa-sign-out"></span> Logout
                            </a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
        <div class="container container-body">
            <div class="row">
                <div class="col-sm-2">
                    <MainMenu></MainMenu>
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
        <DialogsWrapper transition-name="fade"></DialogsWrapper>
        <portal-target name="context-menu"></portal-target>
    </div>
</template>

<style lang="scss">
.container {
    max-width: 1300px;
}

.container-body {
    min-height: 100%;
    overflow: hidden;
    display: flex;
    flex-direction: column;
}
</style>