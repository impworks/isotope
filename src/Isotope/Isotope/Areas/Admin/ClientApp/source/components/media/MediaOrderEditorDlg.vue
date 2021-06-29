<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { HasAsyncState, DialogBase } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { MediaThumbnail } from "../../vms/MediaThumbnail";

@Component
export default class MediaOrderEditorDlg extends Mixins(HasAsyncState(), DialogBase) {
    @Dep('$api') $api: ApiService;
    @Prop({ required: true }) folderKey: string;

    media: MediaThumbnail[] = [];

    async mounted() {
        await this.showLoading(
            async () => this.media = await this.$api.media.getList(this.folderKey),
            'Failed to load media list'
        );
    }

    async save() {
        if(this.asyncState.isSaving)
            return;

        await this.showSaving(
            async () => {
                await this.$api.media.reorder(this.folderKey, this.media.map(x => x.key));

                this.$close(true);

                this.$toast.success('Media order updated');
            },
            'Failed to update media order'
        )
    }
}
</script>

<template>
    <div>
        <div class="modal-backdrop show fade">
        </div>
        <div class="modal fade show">
            <form @submit.prevent="save()">
                <div class="modal-dialog modal-lg" v-if="media">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Reorder media</h5>
                            <button type="button" class="close" @click="$close(false)">&times;</button>
                        </div>
                        <div class="modal-body">
                            <Draggable v-model="media" ghost-class="media-reorder-thumb-ghost">
                                <div v-for="m in media" class="media-reorder-thumb" :style="{'background-image': 'url(' + m.thumbnailPath + ')'}"></div>
                            </Draggable>
                        </div>
                        <div class="modal-footer">
                            <button type="submit" class="btn btn-primary" :disabled="asyncState.isSaving">
                                <span v-if="asyncState.isSaving">Saving...</span>
                                <span v-else>Update</span>
                            </button>
                            <button type="button" class="btn btn-secondary" @click.prevent="$close(false)">Cancel</button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
</template>

<style lang="scss">
.media-reorder-thumb {
    background-position: center center;
    background-size: cover;
    display: inline-block;
    width: 100px;
    height: 100px;
    margin: 0 1em 1em 0;
    cursor: move;
}

.media-reorder-thumb-ghost {
    background-image: none !important;
    border: 1px #ccc solid;
    display: inline-block;
    width: 100px;
    height: 100px;
    margin: 0 1em 1em 0;
}

.modal-media-reorder {
    max-width: 715px;

    .modal-body {
        padding-right: 0;
    }
}
</style>