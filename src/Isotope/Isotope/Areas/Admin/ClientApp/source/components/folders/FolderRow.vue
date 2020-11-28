<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { FolderTitle } from "../../vms/FolderTitle";
import { Action } from "../../../../../Common/source/utils/Interfaces";
import SharedLinksPage from "../shared-links/SharedLinksPage.vue";

@Component({
    name: 'FolderRow',
    components: { SharedLinksPage, FolderRow }
})
export default class FolderRow extends Vue {
    @Prop({ required: true }) folder: FolderTitle;
    @Prop({ required: true}) depth: number;
    
    @Prop({ required: true }) create: Action<FolderTitle>;
    @Prop({ required: true }) remove: Action<FolderTitle>;
    @Prop({ required: true }) edit: Action<FolderTitle>;
}
</script>

<template>
    <fragment>
        <tr v-action-row class="hover-actions">
            <td :style="{'padding-left': (depth + 1) + 'rem'}">
                <div v-if="folder.thumbnailPath" class="folder-thumb" :style="{'background-image': 'url(' + folder.thumbnailPath + ')'}"></div>
                <span v-else class="fa fa-fw fa-folder-o"></span>
                <router-link :to="'/folders/' + folder.key">
                    {{ folder.caption }}
                </router-link>
            </td>
            <td>
                <span v-if="folder.mediaCount > 0">{{ folder.mediaCount }}</span>
                <span v-else title="Empty folder">&mdash;</span>
            </td>
            <td>
                <a class="hover-action" @click.prevent="create(folder)" title="Create subfolder">
                    <span class="fa fa-fw fa-plus"></span>
                </a>
                <a class="hover-action" @click.prevent="remove(folder)" title="Remove">
                    <span class="fa fa-fw fa-remove"></span>
                </a>
                <a class="hover-action" @click.prevent="edit(folder)" title="Edit">
                    <span class="fa fa-fw fa-edit"></span>
                </a>
            </td>
        </tr>
        <FolderRow v-for="s in folder.subfolders" :folder="s" :depth="depth + 1" :create="create" :edit="edit" :remove="remove"></FolderRow>
    </fragment>
</template>

<style scoped lang="scss">
.folder-thumb {
    display: inline-block;
    width: 32px;
    height: 32px;
    background-position: center center;
    background-size: cover;
    vertical-align: middle;
    margin: -5px;
    margin-right: 8px;
}
</style>