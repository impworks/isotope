<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component
    export default class Folders extends Vue {
        opened: Folder = null;
        currentFolder = 1;
        folderId = 1;
        folders: Folder[] = [];
        things: string[] = ['Rock', 'Paper', 'Scissor'];
        animation: string = null;

        mounted() {
            this.folders = this.getFolders();
        }

        getFolderId(): number {
            return this.folderId++;
        }

        getFolders(): Folder[] {
            let result = [];

            for (let i = 0; i < (Math.random() * (100 - 50) + 50); i++) {
                result.push({
                    id: this.getFolderId(),
                    name: this.things[Math.floor(Math.random() * this.things.length)]
                });
            }

            return result;
        }

        openFolder(i : number, folder: Folder) {
            this.opened = folder;
            this.animation = 'slide-forward';

            setTimeout(() => {
                if (i != 0 && i % 5 == 0) {
                    this.currentFolder++;
                    this.folders = this.getFolders();
                }
            }, 1);
        }

        back() {
            this.animation = 'slide-backward';
            this.currentFolder--;
            this.folders = this.getFolders();
        }
    } 

    interface Folder {
        id: number;
        name: string;
    }

</script>

<template>
    <div class="folders">
        <transition name="slide-down">
            <div class="folders-actions">
                <button 
                    class="btn btn-block btn-dark"
                    :disabled="currentFolder == 1"  
                    @click="back()"
                >
                    Go back
                </button>
            </div>
        </transition>
        <div class="folders-content">
            <transition :name="animation">
                <perfect-scrollbar class="scrollable" :key="currentFolder">
                        <a 
                            href="#"
                            class="folder"
                            :class="{ 'opened': opened == folder }"
                            v-for="(folder, i) in folders" 
                            :key="i"
                            @click="openFolder(i, folder)"
                        >
                            <div class="folder-icon"></div>
                            <div v-if="i != 0 && i % 5 == 0" class="folder-name">Folder with many other folders</div>
                            <div v-else class="folder-name">{{ folder.name }} folder</div>
                        </a>
                </perfect-scrollbar>
            </transition>
        </div>
    </div>
</template>

<style lang="scss">
    @import "../../styles/variables";
    @import "./node_modules/bootstrap/scss/functions";
    @import "./node_modules/bootstrap/scss/variables";

    .folders {
        flex: 1 1 auto;
        display: flex;
        flex-direction: column;
        overflow: hidden;
        min-height: 0;

        &-actions {

            .btn-dark {
                border-radius: 0;
            }
        }

        &-content {
            flex: 1 1 auto;
            display: flex;
            flex-direction: column;
            overflow: hidden;
            position: relative;
            min-height: 0;
            
            .scrollable {
                overflow: auto;
                min-height: 0;
                flex: 1 1 auto;
                box-shadow: $box-shadow;
                background: $white;
            }

            .folder {
                display: flex;
                flex-direction: row;
                padding: 0.5em 1em;
                line-height: 1.5;
                color: $gray-800;
                border-top: 1px solid $gray-200;

                &:first-of-type {
                    border-top-color: rgba(0,0,0,0);
                }

                &:hover {
                    text-decoration: none;
                    background-color: $gray-200;
                }

                &-icon,
                &-name {
                    flex: 0 0 auto;
                }

                &-icon {
                    $size: 1.5em;

                    width: $size;
                    height: $size;
                    background-image: url(../../images/folder.svg);
                    background-position: 0 0;
                    background-size: auto 200%;
                    background-repeat: no-repeat;
                }

                &-name {
                    flex-grow: 1;
                    padding: 0 1em;
                    flex: 0 1 auto;
                }

                &.opened {
                    color: $white;
                    background-color: $primary;
                    border-color: $primary;

                    & + .folder {
                        border-top-color: rgba(0,0,0,0);
                    }

                    .folder-icon {
                        background-position: 0 100%;
                    }
                }
            }
        }
    }
</style>