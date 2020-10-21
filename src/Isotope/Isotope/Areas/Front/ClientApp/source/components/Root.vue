<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { Dep } from "../utils/VueInjectDecorator";
import { GalleryInfo } from "../vms/GalleryInfo";
import { HasAsyncState } from "./mixins/HasAsyncState";
import { HasLifetime } from "./mixins/HasLifetime";
import { ApiService } from "../services/ApiService";
import { FilterStateService } from "../services/FilterStateService";
import { AuthService } from "../services/AuthService";
import LoginForm from "./LoginForm.vue";

@Component({
    components: { LoginForm }
})
export default class Root extends Mixins(HasAsyncState(), HasLifetime) {
    @Dep('$api') $api: ApiService;
    @Dep('$auth') $auth: AuthService;
    @Dep('$filter') $filter: FilterStateService;
    
    info: GalleryInfo = null;
    error: string = null;
    authRequired: boolean = true;
    
    async mounted() {
        this.$filter.updateFromRoute(this.$route);
        this.observe(this.$auth.onUserChanged, x => this.authRequired = !x);
        this.observe(this.$filter.onUrlChanged, x => this.$router.replace(x));
        
        try {
            await this.showLoading(async () => {
                this.info = await this.$api.getInfo();
                document.title = this.info.caption;
                this.authRequired = !this.info.allowGuests && !this.info.isAuthorized;
                if(this.info.isLinkValid === false) {
                    this.error = 'The specified share link is invalid.';
                }
            })
        } catch(e) {
            this.error = 'Gallery is unavailable.'
        }
    }
}
</script>

<template>
    <loading 
        class="root" 
        :is-loading="asyncState.isLoading" 
        :is-full-page="true"
    >
        <div 
            class="root__centered-content" 
            v-if="error || authRequired"
        >
            <div 
                class="alert alert-danger"
                v-if="error" 
            >
                <strong>Error</strong><br />
                <span>{{error}}</span>
            </div>
            <LoginForm v-if="!error && authRequired" />
        </div>
        <router-view v-else></router-view>
        <portal-target name="overlay"></portal-target>
    </loading>
</template>

<style lang="scss">
    @import "../../styles/variables";
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