<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class ConfirmationDlg extends Vue {
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
                    <div class="modal-body">
                        {{ text }}
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" @click.prevent="$close(true)">Yes</button>
                        <button type="button" class="btn btn-secondary" @click.prevent="$close(false)">No</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-backdrop show fade">
        </div>
    </div>
</template>