<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import { Media } from "../../../vms/Media";

@Component
export default class MediaPropsEditor extends Vue {
    @Prop({ required: true }) media: Media;
    
    date: Date = null;
    
    created() {
        this.date = this.media.date ? new Date(this.media.date) : null;
    }
    
    @Watch('date')
    onDateChanged() {
        this.media.date = this.date ? this.date.toISOString() : null;
    }
}
</script>

<template>
    <div>
        <div class="form-group row">
            <label class="col-sm-3 col-form-label">Date</label>
            <div class="col-sm-5">
                <datepicker v-model="date" :monday-first="true"></datepicker>
            </div>
        </div>
        <div class="form-group row">
            <label class="col-sm-3 col-form-label">Description</label>
            <div class="col-sm-9">
                <textarea class="form-control" v-model="media.description" rows="5"></textarea>
            </div>
        </div>
    </div>
</template>