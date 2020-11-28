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
    
    showMenu(e: MouseEvent, f: FolderTitle) {
        this.$root.$emit('menu-requested', { event: e, folder: f });
    }
}
</script>

<template>
    <fragment>
        <tr v-action-row class="hover-actions" @contextmenu.prevent="showMenu($event, folder)">
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
                <a class="hover-action" @click.stop="showMenu($event, folder)" title="Actions">
                    <span class="fa fa-fw fa-ellipsis-v"></span>
                </a>
            </td>
        </tr>
        <FolderRow v-for="s in folder.subfolders" :folder="s" :depth="depth + 1"></FolderRow>
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