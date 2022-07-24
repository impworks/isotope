<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { Dep } from "../../../../Common/source/utils/VueInjectDecorator";
import { HasAsyncState } from "./mixins/HasAsyncState";
import { HasLifetime } from "./mixins/HasLifetime";
import { ApiService } from "../services/ApiService";
import { FilterStateService } from "../services/FilterStateService";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { DeviceHelper } from "../utils/DeviceHelper";

import LoginForm from "./LoginForm.vue";

@Component({
    components: { LoginForm }
})
export default class Root extends Mixins(HasAsyncState(), HasLifetime) {
    @Dep('$api') $api: ApiService;
    @Dep('$auth') $auth: AuthService;
    @Dep('$filter') $filter: FilterStateService;
    
    error: string = null;
    authRequired: boolean = true;
    loaded: boolean = false;
    isTouchDevice: boolean = false;

    async created() {
        this.$filter.updateFromRoute(this.$route);
        this.observe(this.$filter.onUrlChanged, x => this.$router.replace(x));

        this.isTouchDevice = DeviceHelper.isTouch();
        document.addEventListener("touchstart", function() {}, false); // enables :active pseudo for iOS Safari
        
        try {
            await this.showLoading(async () => {
                const info = await this.$api.getInfo();
                document.title = info.caption;
                if(info.allowGuests) {
                    this.authRequired = false;
                } else {
                    this.authRequired = !info.isAuthorized && info.isLinkValid === null;
                    this.observe(this.$auth.onUserChanged, x => this.authRequired = !x);
                }
                if(info.isLinkValid === false) {
                    this.error = 'The specified share link is invalid.';
                }
            });
        } catch(e) {
            this.error = 'Gallery is unavailable.';
        } finally {
            this.loaded = true;
        }
    }
}
</script>

<template>
    <loading 
        :is-loading="asyncState.isLoading" 
        :is-full-page="true"
    >
        <div 
            class="root"
            :class="{
                'touch': isTouchDevice,
                'no-touch': !isTouchDevice
            }"
            v-if="loaded"
        >
            <div 
                class="root__centered-content" 
                v-if="error || authRequired"
            >
                <div v-if="error" class="gallery__error">
                    <div class="gallery__error__content">
                        <div class="gallery__error__image"></div>
                        <h3>Error</h3>
                        <p>{{error}}</p>
                    </div>
                </div>
                <LoginForm v-else></LoginForm>
            </div>
            <router-view v-else></router-view>
            <portal-target name="overlay"></portal-target>
        </div>
    </loading>
</template>

<style lang="scss">
    @import "../../../../Common/styles/variables";
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