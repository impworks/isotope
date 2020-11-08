<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { FolderTitle } from "../../vms/FolderTitle";
import { Action } from "../../../../../Common/source/utils/Interfaces";

@Component
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
            <td>
                <router-link :to="'/media/' + folder.key"
                             :style="{marginLeft: depth + 'rem'}">
                    {{ folder.caption }}
                </router-link>
            </td>
            <td>
                {{ folder.mediaCount }}
            </td>
            <td>
                <a class="hover-action" href @click.prevent="create(folder)" title="Create subfolder">
                    <span class="fa fa-fw fa-plus"></span>
                </a>
                <a class="hover-action" href @click.prevent="remove(folder)" title="Remove">
                    <span class="fa fa-fw fa-remove"></span>
                </a>
                <a class="hover-action" href @click.prevent="edit(folder)" title="Edit profile">
                    <span class="fa fa-fw fa-edit"></span>
                </a>
            </td>
        </tr>
        <FolderRow v-for="s in folder.subfolders" :folder="s" :depth="depth + 1" :create="create" :edit="edit" :remove="remove"></FolderRow>
    </fragment>
</template>