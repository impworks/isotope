<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { Dep } from "../utils/VueInjectDecorator";
import { ClientApiService } from "../services/ClientApiService";
import { UserStateService } from "../services/UserStateService";
import { GalleryInfo } from "../vms/GalleryInfo";
import { HasAsyncState } from "./mixins/HasAsyncState";

    @Component
    export default class Root extends Mixins(HasAsyncState()) {
        @Dep('$api') $api: ClientApiService;
        @Dep('$userState') $userState: UserStateService;
        
        info: GalleryInfo = null;
        error: string = null;
        authRequired: boolean = true;
        
        async mounted() {
            this.$userState.shareId = this.$route.query['sh'];
            
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
    <loading :is-loading="asyncState.isLoading" :is-full-page="true">
        <div v-if="error || authRequired" class="row mt-5">
            <div class="col-sm-4 offset-sm-4">
                <div v-if="error" class="alert alert-danger">
                    <strong>Error</strong><br />
                    <span>{{error}}</span>
                </div>
                <div v-if="!error && authRequired">
                    <div>TODO: authorization form here</div>
                </div>
            </div>
        </div>
        <div v-else>
            <router-view />
        </div>
    </loading>
</template>s