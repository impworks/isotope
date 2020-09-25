<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { Dep } from "../utils/VueInjectDecorator";
import { GalleryInfo } from "../vms/GalleryInfo";
import { HasAsyncState } from "./mixins/HasAsyncState";
import { HasLifetime } from "./mixins/HasLifetime";
import LoginForm from "./LoginForm.vue";
import { ApiService } from "../services/ApiService";
import FilterStateService from "../services/FilterStateService";
import { AuthService } from "../services/AuthService";

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
            class="m-auto p-3" 
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
        <router-view v-else />
    </loading>
</template>

<style lang="scss">
    .root {
        height: 100%;
        display: flex;
        flex-direction: column;
    }
</style>