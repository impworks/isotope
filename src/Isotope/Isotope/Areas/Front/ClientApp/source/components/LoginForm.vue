<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { HasAsyncState } from "./mixins/HasAsyncState";
import { Dep } from "../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../services/ApiService";
import { AuthService } from "../services/AuthService";

@Component
export default class LoginForm extends Mixins(HasAsyncState()) {
    @Dep('$api') $api: ApiService;
    @Dep('$auth') $auth: AuthService;
    
    username: string = null;
    password: string = null;
    error: string = null;
    
    caption: string = 'isotope';
    
    async mounted() {
        this.caption = (await this.$api.getInfo()).caption;
    }
    
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
                    this.$auth.user = result.user;
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
        <div class="card shadow-sm">
            <div class="card-header bg-white py-0">
                <div class="logotype">{{ caption }}</div>
            </div>
            <div class="card-body">
                <div class="form-group">
                    <input 
                        type="text" 
                        class="form-control" 
                        placeholder="Username" 
                        v-model="username" 
                        :disabled="asyncState.isLoading"
                        v-autofocus
                    />
                </div>
                <div class="form-group mb-0">
                    <input 
                        type="password" 
                        v-model="password" 
                        placeholder="Password" 
                        class="form-control" 
                        :disabled="asyncState.isLoading" 
                    />
                </div>
                <div 
                    class="alert alert-danger m-0 mt-3" 
                    v-if="error"
                >
                    {{error}}
                </div>
            </div>
            <div class="card-footer bg-white">
                <button 
                    type="submit" 
                    class="btn btn-primary btn-block" 
                    :disabled="!canSignIn || asyncState.isLoading"
                >
                    <loading 
                        :is-loading="asyncState.isLoading" 
                        :text="'Working…'"
                    >
                        Log in
                    </loading>
                </button>
            </div>
        </div>
    </form>
</template>