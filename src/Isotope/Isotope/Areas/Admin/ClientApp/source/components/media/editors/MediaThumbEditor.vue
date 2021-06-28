<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import { Media } from "../../../vms/Media";
import { Rect } from "../../../../../../Common/source/vms/Rect";

import RectEditor from "../RectEditor.vue";

@Component({
    components: { RectEditor }
})
export default class MediaThumbEditor extends Vue {
    @Prop({ required: true }) media: Media;
    @Prop({ required: true }) rect: Rect;
}
</script>

<template>
    <div class="form-group media-container">
        <div class="media-wrapper">
            <img :src="media.fullPath" />
            <div class="tag-wrapper">
                <RectEditor :rect="rect" :lock-ratio="true"></RectEditor>
            </div>
        </div>
    </div>
</template>