<script lang="ts">
import { Vue, Component, Prop } from "vue-property-decorator";

@Component
export default class FilePicker extends Vue {
    @Prop({ required: false, default: false, type: Boolean }) multiple: boolean;
    @Prop({ required: false, default: false, type: Boolean }) disabled: boolean;

    onClick() {
        (this.$refs.input as HTMLInputElement).click();
    }

    onDrop(e: DragEvent) {
        const files = e.dataTransfer && e.dataTransfer.files;
        if(files && files.length)
            this.onChange(files);
    }

    onChange(files: File[]) {
        const payload = this.multiple == false && files && files.length ? files[0] : files;
        this.$emit('change', Array.from(payload));
        setTimeout(() => {
                const input = this.$refs.input as HTMLInputElement;
                if(input) input.value = '';
            },
            0
        );
    }
}
</script>

<template>
    <div class="file-picker btn btn-outline-primary">
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