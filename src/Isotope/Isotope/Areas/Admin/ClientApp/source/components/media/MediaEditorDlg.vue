<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { DialogBase, HasAsyncState } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { Media } from "../../vms/Media";
import { Rect } from "../../../../../Common/source/vms/Rect";

import MediaThumbEditor from "./editors/MediaThumbEditor.vue";
import MediaPropsEditor from "./editors/MediaPropsEditor.vue";
import MediaTagsEditor from "./editors/MediaTagsEditor.vue";
import { Tag } from "../../vms/Tag";

@Component({
    components: { MediaTagsEditor, MediaPropsEditor, MediaThumbEditor }
})
export default class MediaEditorDlg extends Mixins(DialogBase, HasAsyncState()) {
    
    // -----------------------------------
    // Dependencies
    // -----------------------------------
    
    @Dep('$api') $api: ApiService;
    
    @Prop({ required: true }) otherMedia: MediaThumbnail[];
    @Prop({ required: true }) mediaKey: string;
    @Prop({ required: false }) tabKey: string;

    // -----------------------------------
    // Properties
    // -----------------------------------
    
    media: Media = null;
    thumbRect: Rect = null;
    currKey: string = null;
    currIndex: number = null;
    
    tabs: MediaEditorDlgTabInfo[] = [
        { key: 'props', caption: 'Properties', tooltip: 'Description and date (Ctrl + 1)' },
        { key: 'tags', caption: 'Tags', tooltip: 'Depicted people (Ctrl + 2)' },
        { key: 'thumb', caption: 'Thumbnail', tooltip: 'Preview image section (Ctrl + 3)' }
    ];
    tab: MediaEditorDlgTabInfo = null;
    
    tags: Tag[] = null;
    
    result: boolean = false;
    editNextOnSave: boolean = false;
    
    get hasPrev() {
        return this.currIndex > 0;
    }
    
    get hasNext() {
        return this.currIndex < this.otherMedia.length - 1;
    }

    // -----------------------------------
    // Hooks
    // -----------------------------------
    
    async created() {
        this.tab = (this.tabKey ? this.tabs.find(x => x.key == this.tabKey) : null) || this.tabs[0];
        await this.load(this.otherMedia.findIndex(t => t.key === this.mediaKey));
    }

    // -----------------------------------
    // Methods
    // -----------------------------------
    
    async load(idx: number) {
        await this.showLoading(
            async () => {
                this.currIndex = idx;
                this.currKey = this.otherMedia[idx].key;
                
                this.media = await this.$api.media.get(this.currKey);
                this.thumbRect = await this.$api.media.getThumb(this.currKey);
                
                if(this.tags == null)
                    this.tags = await this.$api.tags.getList();
                
                await this.loadImage(this.media.fullPath);
            },
            'Failed to load media!'
        );
    }
    
    async save() {
        const success = await this.showSaving(
            async () => {
                await this.$api.media.update(this.currKey, this.media);
                await this.$api.media.updateThumb(this.currKey, this.thumbRect);
                
                this.result = true;
            },
            'Failed to save media!'
        );
        
        if(!success)
            return;

        if(this.editNextOnSave && this.hasNext) {
            await this.next();
        } else {
            this.$close(true);
            this.$toast.success('Media updated.');
        }
    }
    
    async prev() {
        if(!this.hasPrev || this.asyncState.isLoading || this.asyncState.isSaving)
            return;

        this.currIndex--;
        await this.load(this.currIndex);
    }
    
    async next() {
        if(!this.hasNext || this.asyncState.isLoading || this.asyncState.isSaving)
            return;

        this.currIndex++;
        await this.load(this.currIndex);
    }

    // -----------------------------------
    // Private helpers
    // -----------------------------------

    private loadImage(path: string): Promise<void> {
        return new Promise((resolve, reject) => {
            const img = new Image();
            img.addEventListener('load', () => resolve());
            img.addEventListener('error', err => reject(err));
            img.src = path;
        });
    }
}

interface MediaEditorDlgTabInfo {
    key: MediaEditorDlgTab;
    caption: string;
    tooltip: string;
}
</script>

<template>
    <div>
        <div class="modal fade show">
            <form @submit.prevent="save()">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header pb-0">
                            <h5 class="modal-title">Update media</h5>
                            <ul class="nav nav-tabs nav-tabs-embedded ml-5">
                                <li class="nav-item" v-for="t in tabs" :key="t.key">
                                    <a class="nav-link clickable"
                                       :class="{'active': t === tab}"
                                       :title="t.tooltip"
                                       @click.prevent="tab = t">
                                        {{t.caption}}
                                    </a>
                                </li>
                            </ul>
                            <button type="button" class="close" @click="$close(result)">&times;</button>
                        </div>
                        <div class="modal-body" v-if="!asyncState.isLoading && media && thumbRect">
                            <div v-if="tab && tab.key === 'props'">
                                <MediaPropsEditor :media="media"></MediaPropsEditor>
                            </div>
                            <div v-if="tab && tab.key === 'tags'">
                                <MediaTagsEditor :media="media" :tags-lookup="tags"></MediaTagsEditor>
                            </div>
                            <div v-if="tab && tab.key === 'thumb'">
                                <MediaThumbEditor :media="media" :rect="thumbRect"></MediaThumbEditor>
                            </div>
                        </div>
                        <div class="modal-body text-center" v-else>
                            <p class="m-5">
                                <span class="fa fa-spinner spinner"></span>
                                Loading...
                            </p>
                        </div>
                        <div class="modal-footer">
                            <div class="mr-auto">
                                <div class="btn-group-toggle d-inline-block mr-2">
                                    <label class="btn btn-sm btn-outline-secondary"
                                           :class="editNextOnSave ? 'active' : ''"
                                           title="Keep the dialog open and edit next media in folder after saving">
                                        <span class="fa fa-fw" :class="editNextOnSave ? 'fa-check-square-o' : 'fa-square-o'"></span>
                                        <input type="checkbox" v-model="editNextOnSave" /> Edit next
                                    </label>
                                </div>
                                <div class="btn-group btn-group-sm">
                                    <button type="button"
                                            class="btn btn-outline-secondary"
                                            title="Go to previous media in folder without saving"
                                            @click.prevent="prev()"
                                            :disabled="!hasPrev || asyncState.isSaving || asyncState.isLoading">
                                        &larr;
                                    </button>
                                    <button type="button"
                                            class="btn btn-outline-secondary"
                                            title="Go to next media in folder without saving"
                                            @click.prevent="next()"
                                            :disabled="!hasNext || asyncState.isSaving || asyncState.isLoading">
                                        &rarr;
                                    </button>
                                </div>
                            </div>
                            <button type="submit" class="btn btn-primary" :disabled="asyncState.isSaving || asyncState.isLoading" title="Ctrl + S">
                                <span v-if="asyncState.isSaving">Saving...</span>
                                <span v-else>Update</span>
                            </button>
                            <button type="button" class="btn btn-secondary" @click.prevent="$close(result)">Cancel</button>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <div class="modal-backdrop show fade">
        </div>
        <GlobalEvents @keydown.ctrl.83.stop.prevent="save()"
                      @keydown.ctrl.49.stop.prevent="tab = tabs[0]"
                      @keydown.ctrl.50.stop.prevent="tab = tabs[1]"
                      @keydown.ctrl.51.stop.prevent="tab = tabs[2]"
                      @keydown.ctrl.left="prev()"
                      @keydown.ctrl.right="next()">            
        </GlobalEvents>
    </div>
</template>

<style lang="scss">
.media-container {
    display: flex;
    justify-content: center;
    margin-bottom: 0;

    .media-wrapper {
        position: relative;

        img {
            max-width: 100%;
            max-height: 800px;
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
}

.nav-tabs.nav-tabs-embedded {
    border-bottom: 0;
    
    .nav-link {
        border-bottom: 0;
        padding: 0.25rem 1rem;
        height: 40px;
    }
}

.spinner {
    -webkit-animation:spin 2s linear infinite;
    -moz-animation:spin 2s linear infinite;
    animation:spin 2s linear infinite;
}

@-moz-keyframes spin { 100% { -moz-transform: rotate(360deg); } }
@-webkit-keyframes spin { 100% { -webkit-transform: rotate(360deg); } }
@keyframes spin { 100% { -webkit-transform: rotate(360deg); transform:rotate(360deg); } }
</style>