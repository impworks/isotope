<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { DialogsWrapper } from "vue-modal-dialogs";
import { HasAsyncState } from "./mixins";

import MainMenu from "./utils/MainMenu.vue";
import { Dep } from "../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../services/ApiService";
import { AuthService } from "../../../../Common/source/services/AuthService";

@Component({
    components: { MainMenu, DialogsWrapper }
})
export default class Root extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;
    @Dep('$auth') $auth: AuthService;
    
    async mounted() {
        try {
            this.asyncState.isLoading = true;
            const info = await this.$api.info.getInfo();
            if(!info.isAdmin) {
                this.gotoAuth();
            }
            this.asyncState.isLoading = false;
        } catch (e) {
            this.gotoAuth();
        }
    }
    
    logout() {
        this.$auth.user = null;
        this.gotoAuth();
    }
    
    gotoAuth() {
        window.location.href = '/';
    }
}
</script>

<template>
    <div>
        <nav class="navbar navbar-dark bg-dark mb-4">
            <div class="container px-4">
                <router-link class="navbar-brand" :to="'/'">Isotope</router-link>
                <div class="navbar-nav mr-auto"></div>
                <div class="my-2 my-lg-0">
                    <ul class="navbar-nav">
                        <li class="nav-item">
                            <a class="nav-link clickable" @click.prevent="logout()">
                                <span class="fa fa-sign-out"></span> Exit
                            </a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
        <loading :is-loading="asyncState.isLoading" :is-full-page="true">
            <div class="container container-body">
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
        </loading>
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