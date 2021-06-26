<script lang="ts">
import { Component, Mixins } from "vue-property-decorator";
import { DialogBase, HasAsyncState } from "../mixins";

@Component
export default class MediaEditorDlg extends Mixins(DialogBase, HasAsyncState()) {
    
}
</script>

<template>
    <div>
        <div class="modal fade show">
            <form @submit.prevent="save()">
                <div class="modal-dialog" v-if="value">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Update media</h5>
                            <button type="button" class="close" @click="$close(false)">&times;</button>
                        </div>
                        <div class="modal-body">
                            
                        </div>
                        <div class="modal-footer">
                            <label class="mr-auto" title="Keep the dialog open and edit next media in folder after saving">
                                <input type="checkbox" v-model="editMore" /> Edit next
                            </label>
                            <button type="button" class="btn btn-outline-secondary" :disabled="">
                                
                            </button>
                            <button type="submit" class="btn btn-primary" :disabled="asyncState.isSaving" title="Ctrl + S">
                                <span v-if="asyncState.isSaving">Saving...</span>
                                <span v-else>Update</span>
                            </button>
                            <button type="button" class="btn btn-secondary" @click.prevent="$close(false)">Cancel</button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <div class="modal-backdrop show fade">
        </div>
        <GlobalEvents @keydown.ctrl.83.stop.prevent="save()"></GlobalEvents>
    </div>
</template>