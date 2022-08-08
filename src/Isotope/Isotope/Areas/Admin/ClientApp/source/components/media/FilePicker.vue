<script lang="ts">
import { Vue, Component, Prop } from "vue-property-decorator";

@Component
export default class FilePicker extends Vue {
    @Prop({ required: false, default: false, type: Boolean }) multiple: boolean;
    @Prop({ required: false, default: false, type: Boolean }) disabled: boolean;
    
    $refs: {
        input: HTMLInputElement;
    };

    onClick() {
        this.$refs.input.click();
    }

    onDrop(e: DragEvent) {
        const files = e.dataTransfer && e.dataTransfer.files;
        if(files && files.length)
            this.onChange(files);
    }

    onChange(files: FileList) {
        const payload = this.multiple == false && files?.length > 0
            ? [files[0]]
            : Array.from(files);
        this.$emit('change', payload);
        
        setTimeout(
            () => {
                if(this.$refs.input)
                    this.$refs.input.value = '';
            },
            0
        );
    }
}
</script>

<template>
    <div class="file-picker btn btn-outline-primary" :class="{disabled : disabled}">
        <span class="file-picker-label" @click="onClick()" @drop.prevent="onDrop" @dragover.prevent>
            <slot>
                <i class="fa fa-upload"></i> Upload
            </slot>
        </span>
        <input type="file" ref="input" :multiple="multiple" :disabled="disabled" @change="onChange($event.target.files)" />
    </div>
</template>

<style lang="scss" scoped>
.file-picker {
    width: 100%;
    height: 100px;
    position: relative;
    cursor: pointer;
    line-height: 90px;
    font-size: 2rem;
    text-align: center;
    
    &.disabled {
        cursor: auto;
    }
    
    .file-picker-label {
        display: block;
        width: 100%;
        height: 100%;
    }

    input {
        position: absolute;
        line-height: 0;
        display: block;
        opacity: 0;
        width: 0;
        height: 0;
    }
}
</style>