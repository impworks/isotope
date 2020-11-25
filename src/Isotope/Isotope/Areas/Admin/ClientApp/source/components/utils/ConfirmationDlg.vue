<script lang="ts">
import { Component, Prop } from "vue-property-decorator";
import { DialogBase } from "../mixins";

@Component
export default class ConfirmationDlg extends Mixins(DialogBase) {
    @Prop({ required: false, default: 'Confirmation' }) header: string;
    @Prop({ required: true }) text: string;
}
</script>

<template>
    <div>
        <div class="modal fade show">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">{{ header }}</h5>
                    </div>
                    <div class="modal-body" v-html="text">
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" @click.prevent="$close(true)">Yes</button>
                        <button type="button" class="btn btn-secondary" @click.prevent="$close(false)">No</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-backdrop show fade">
        </div>
    </div>
</template>