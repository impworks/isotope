<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { HasAsyncState } from "./mixins";
import { Dep } from "../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../services/ApiService";
import { HasLifetime } from "../../../../Front/ClientApp/source/components/mixins/HasLifetime";
import { AuthService } from "../../../../Common/source/services/AuthService";

import LoginForm from "./LoginForm.vue";
import MainView from "./MainView.vue";

@Component({
    components: { MainView, LoginForm }
})
export default class Root extends Mixins(HasAsyncState(), HasLifetime) {
    @Dep('$api') $api: ApiService;
    @Dep('$auth') $auth: AuthService;
    
    isLoaded: boolean = false;
    isAuthorized: boolean = false;
    
    async mounted() {
        try {
            this.asyncState.isLoading = true;
            const info = await this.$api.front.getInfo();
            this.isAuthorized = info.isAdmin;
            this.asyncState.isLoading = false;
        } catch (e) {
            console.log('Failed to get gallery info', e);
        } finally {
            this.isLoaded = true;
        }
        
        this.observe(this.$auth.onUserChanged, x => this.isAuthorized = !!x);
    }
}
</script>

<template>
    <loading
        :is-loading="asyncState.isLoading"
        :is-full-page="true"
    >
        <div class="root">
            <MainView v-if="isAuthorized"></MainView>
            <div class="root__centered-content" v-else>
                <LoginForm></LoginForm>
            </div>
        </div>
    </loading>
</template>

<style lang="scss">
@import "./node_modules/bootstrap/scss/functions";
@import "./node_modules/bootstrap/scss/variables";
@import "./node_modules/bootstrap/scss/mixins";

.root {
    height: 100%;
    display: flex;
    flex-direction: column;

    &__centered-content {
        margin: auto 0;
        padding: 1rem;
        min-width: 300px;

        @include media-breakpoint-up(sm) {
            width: 23rem;
            margin: auto;
        }
    }
}
</style>