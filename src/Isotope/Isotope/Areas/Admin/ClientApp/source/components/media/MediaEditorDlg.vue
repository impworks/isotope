<script lang="ts">
import { Component, Mixins, Prop } from "vue-property-decorator";
import { DialogBase, HasAsyncState } from "../mixins";
import { Dep } from "../../../../../Common/source/utils/VueInjectDecorator";
import { ApiService } from "../../services/ApiService";
import { MediaThumbnail } from "../../vms/MediaThumbnail";
import { Media } from "../../vms/Media";

@Component
export default class MediaEditorDlg extends Mixins(DialogBase, HasAsyncState()) {
    
    // -----------------------------------
    // Dependencies
    // -----------------------------------
    
    @Dep('$api') $api: ApiService;
    
    @Prop({ required: true }) otherMedia: MediaThumbnail[];
    @Prop({ required: true }) mediaKey: string;

    // -----------------------------------
    // Properties
    // -----------------------------------
    
    media: Media = null;
    currKey: string = null;
    currIndex: number = null;
    
    tabs: MediaEditorDlgTab[] = [
        { key: 'props', caption: 'Properties' },
        { key: 'tags', caption: 'Tags' },
        { key: 'thumb', caption: 'Thumbnail' }
    ];
    tab: MediaEditorDlgTab = null;
    
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
        this.tab = this.tabs[0];
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
            },
            'Failed to load media!'
        );
    }
    
    async save() {
        await this.showSaving(
            async () => {
                await this.$api.media.update(this.currKey, this.media);
                if(this.editNextOnSave)
                    await this.next();
            },
            'Failed to save media!'
        );
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
}

interface MediaEditorDlgTab {
    key: TabKey;
    caption: string;
}

type TabKey = 'props' | 'tags' | 'thumb';
</script>

<template>
    <div>
        <div class="modal fade show">
            <form @submit.prevent="save()">
                <div class="modal-dialog" v-if="media">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Update media</h5>
                            <button type="button" class="close" @click="$close(false)">&times;</button>
                        </div>
                        <div class="modal-body">
                            <ul class="nav nav-pills nav-fill">
                                <li class="nav-item" v-for="t in tabs" :key="t.key">
                                    <a class="nav-link"
                                       :class="{'active': t === tab}"
                                       @click.prevent="tab = t"
                                       >{{t.caption}}</a>
                                </li>
                            </ul>
                            <div v-if="tab && tab.key === 'props'">
                                Props
                            </div>
                            <div v-if="tab && tab.key === 'tags'">
                                Tags
                            </div>
                            <div v-if="tab && tab.key === 'thumb'">
                                Thumb
                            </div>
                            <span>{{currKey}}</span>
                        </div>
                        <div class="modal-footer">
                            <div class="mr-auto">
                                <div class="btn-group-toggle d-inline-block mr-2">
                                    <label class="btn btn-sm btn-outline-secondary" title="Keep the dialog open and edit next media in folder after saving">
                                        <span class="fa fa-fw" :class="editNextOnSave ? 'fa-check-square-o' : 'fa-square-o'"></span>
                                        <input type="checkbox" v-model="editNextOnSave" /> Edit next
                                    </label>
                                </div>
                                <div class="btn-group btn-group-sm">
                                    <button type="button"
                                            class="btn btn-outline-secondary"
                                            title="Go to previous media in folder"
                                            @click.prevent="prev()"
                                            :disabled="!hasPrev">
                                        &larr;
                                    </button>
                                    <button type="button"
                                            class="btn btn-outline-secondary"
                                            title="Go to next media in folder"
                                            @click.prevent="next()"
                                            :disabled="!hasNext">
                                        &rarr;
                                    </button>
                                </div>
                            </div>
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
        <GlobalEvents @keydown.ctrl.83.stop.prevent="save()"
                      @keydown.ctrl.49.stop.prevent="tab = tabs[0]"
                      @keydown.ctrl.50.stop.prevent="tab = tabs[1]"
                      @keydown.ctrl.51.stop.prevent="tab = tabs[2]"
                      @keydown.ctrl.left="prev()"
                      @keydown.ctrl.right="next()">            
        </GlobalEvents>
    </div>
</template>