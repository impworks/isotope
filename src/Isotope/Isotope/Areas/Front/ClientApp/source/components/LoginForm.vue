<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { HasAsyncState } from "./mixins/HasAsyncState";
import { Dep } from "../utils/VueInjectDecorator";
import { ClientApiService } from "../services/ClientApiService";
import { UserStateService } from "../services/UserStateService";

@Component
export default class LoginForm extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ClientApiService;
    @Dep('$userState') $userState: UserStateService;
    
    username: string = null;
    password: string = null;
    error: string = null;
    
    get canSignIn(): boolean {
        return !!this.username && !!this.password;
    }
    
    async signIn() {
        if(!this.canSignIn)
            return;
        
        this.error = null;
        try {
            await this.showLoading(async () => {
                const result = await this.$api.authorize({ username: this.username, password: this.password});
                if(result.success) {
                    this.$userState.user = result.user;
                } else {
                    this.error = result.errorMessage;
                }
            })
        } catch (e) {
            this.error = 'Server connection failed.';
        }
    }
}
</script>

<template>
    <form @submit.prevent="signIn()">
        <div class="card">
            <div class="card-header">Sign in required</div>
            <div class="card-body">
                <div class="form-group">
                    <input type="text" v-model="username" placeholder="Username" class="form-control" :disabled="asyncState.isLoading" />
                </div>
                <div class="form-group">
                    <input type="password" v-model="password" placeholder="Password" class="form-control" :disabled="asyncState.isLoading" />
                </div>
            </div>
            <div class="card-footer">
                <div class="float-right">
                    <button type="submit" class="btn btn-primary" :disabled="!canSignIn || asyncState.isLoading">
                        <span v-if="asyncState.isLoading">Working...</span>
                        <span v-if="!asyncState.isLoading">Log in</span>
                    </button>
                </div>
                <div class="clearfix"></div>
            </div>
        </div>
        <div class="alert alert-danger mt-2" v-if="error">{{error}}</div>
    </form>
</template>