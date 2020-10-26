<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";
    import { GalleryInfo } from "../../vms/GalleryInfo";
    import { Dep } from "../../utils/VueInjectDecorator";
    import { ApiService } from "../../services/ApiService";
    import { AuthService } from "../../services/AuthService";

    @Component
    export default class UserDropdown extends Vue {
        @Dep('$api') $api: ApiService;
        @Dep('$auth') $auth: AuthService;
        
        avatar: string = null;
        isOpen: boolean = false;
        
        info: GalleryInfo = null;

        async mounted() {
            try {
                this.info = await this.$api.getInfo();
            } catch {
            }
            
            window.addEventListener("click", this.clickHandler);
        }

        beforeDestroy() {
            window.removeEventListener('click', this.clickHandler);
        }

        clickHandler(e: any) {
            if (this.isOpen && !this.$el.contains(e.target)) {
                this.isOpen = false;
            }
        }
        
        logout() {
            this.$auth.user = null;
        }
    }
</script>

<template>
    <div class="user-dropdown" 
         v-if="info && info.isAuthorized && info.isLinkValid === null">
        <button 
            class="btn-header"
            @click="isOpen = !isOpen"
        >
            <div class="btn-header__content">
                <i 
                    class="icon icon-user"
                    v-if="!avatar" 
                ></i>
                <div 
                    class="avatar"
                    v-else 
                    style="background-image: url('../../../images/avatar.jpg')"
                ></div>
            </div>
        </button>
        <transition name="user-dropdown__transition">
            <div 
                class="user-dropdown__content" 
                v-if="isOpen"
            >
                <ul>
                    <li><a href="/@admin" v-if="info.isAdmin">Admin panel</a></li>
                    <li><a class="clickable" @click.prevent="logout">Log out</a></li>
                </ul>
            </div>
        </transition>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";
    
    .user-dropdown {
        position: relative;

        &__transition {
            
            &-enter-active, 
            &-leave-active {
                transition: opacity 200ms ease,
                            transform 200ms ease;
            }

            &-enter, &-leave-to {
                opacity: 0;
                transform: translateY(-0.5rem);
            }
        }

        &__content {
            width: 11rem;
            top: calc(100% - 0.5rem);
            right: 0.5rem;
            position: absolute;
            z-index: $zindex-dropdown;

            @include media-breakpoint-down(sm) {
                right: 0;
            }

            ul {
                box-shadow: $box-shadow;
                background-color: $white;
                border: 1px solid $gray-400;
                border-radius: $border-radius;
                list-style: none;
                padding: 0;
                color: $gray-800;
                text-align: left;
                position: relative;
                margin-top: -1px;

                li {

                    &:first-child {

                        a {
                            border-radius: 0.25rem 0.25rem 0 0;

                            &:after, 
                            &:before {
                                bottom: 100%;
                                right: 1.5rem;
                                border: solid transparent;
                                content: "";
                                height: 0;
                                width: 0;
                                position: absolute;
                                pointer-events: none;
                            }

                            &:after {
                                border-color: rgba(0, 0, 0, 0);
                                border-bottom-color: $white;
                                border-width: 0.5rem;
                                margin-right: -0.5rem;
                            }

                            &:before {
                                border-color: rgba(0, 0, 0, 0);
                                border-bottom-color: $gray-400;
                                border-width: calc(0.5rem + 1px);
                                margin-right: calc(-0.5rem - 1px);
                            }

                            &:hover:after {
                                border-bottom-color: $gray-200;
                            }
                        }
                    }

                    &:last-child a {
                        border-radius: 0 0 0.25rem 0.25rem;
                    }

                    &:not(:last-child) a {
                        border-bottom: 1px solid $gray-300;
                    }

                    a {
                        color: $gray-800;
                        display: block;
                        padding: 0.6rem 1rem;

                        &:hover {
                            color: $gray-800;
                            text-decoration: none;
                            background-color: $gray-200;
                        }
                    }
                }
            }
        }
    }
</style>