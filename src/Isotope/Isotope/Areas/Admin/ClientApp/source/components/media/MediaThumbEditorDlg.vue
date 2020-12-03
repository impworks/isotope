<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { Rect } from "../../../../../Common/source/vms/Rect";
import { Media } from "../../vms/Media";

import RectEditor from "./RectEditor.vue";

@Component({
    components: { RectEditor }
})
export default class MediaThumbEditorDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: true }) mediaKey: string;

    media: Media = null;
    value: Rect = null;

    async mounted() {
        await this.showLoading(
            async () => {
                const media = await this.$api.media.get(this.mediaKey);
                await this.loadImage(media.fullPath);
                this.media = media;
                this.value = await this.$api.media.getThumb(this.mediaKey);
            },
            'Failed to load media data'
        );
    }

    async save() {
        if(this.asyncState.isSaving)
            return;
        
        await this.showSaving(
            async () => {
                await this.$api.media.updateThumb(this.mediaKey, this.value);

                this.$close(true);

                this.$toast.success('Media thumbnail updated');
            },
            'Failed to update media thumb'
        )
    }

    private loadImage(path: string): Promise<void> {
        return new Promise((resolve, reject) => {
            const img = new Image();
            img.addEventListener('load', () => resolve());
            img.addEventListener('error', err => reject(err));
            img.src = path;
        })
    }
}
</script>

<template>
    <div>
        <div class="modal fade show">
            <form @submit.prevent="save()">
                <div class="modal-dialog modal-lg" v-if="value">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Update media thumbnail</h5>
                            <button type="button" class="close" @click="$close(false)">&times;</button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group">
                                <div class="media-wrapper">
                                    <img :src="media.fullPath" ref="img" />
                                    <div class="tag-wrapper">
                                        <RectEditor :rect="value" :lock-ratio="true"></RectEditor>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
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

<style lang="scss" scoped>
.media-wrapper {
    width: 100%;
    position: relative;

    img {
        width: 100%;
        height: auto;
    }

    .tag-wrapper {
        position: absolute;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
        z-index: 1000;
    }
}
</style>