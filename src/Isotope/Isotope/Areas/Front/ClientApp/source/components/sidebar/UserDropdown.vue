<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";


    @Component
    export default class UserDropdown extends Vue {
        avatar: string = null;
        isOpen: boolean = false;

        mounted() {
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
    }
</script>

<template>
    <div class="user-dropdown">
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
        <div 
            class="user-dropdown__content" 
            v-if="isOpen"
        >
            <ul>
                <li><a href="#">Admin panel</a></li>
                <li><a href="#">Log out</a></li>
            </ul>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";
    @import "./node_modules/bootstrap/scss/mixins";
    
    .user-dropdown {
        position: relative;

        &__content {
            width: 11rem;
            top: calc(100% - 0.5rem);
            right: 0.5rem;
            position: absolute;
            z-index: $zindex-dropdown;

            @include media-breakpoint-down(sm) {
                right: 0;
            }

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
                overflow: hidden;

                li {
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