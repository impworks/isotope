<script lang="ts">
import { Vue, Component, Model, Watch } from "vue-property-decorator";
import ModalWindow from "../utils/ModalWindow.vue";

@Component({
    components: { ModalWindow }
})
export default class ShareLink extends Vue {
    @Model() model: boolean;

    isVisible: boolean = false;

    mounted() {
        this.isVisible = this.model;
    }

    close() {
        this.isVisible = false;
    }

    @Watch('model')
    onModelChanged() {
        this.isVisible = this.model;
    }

    @Watch('isVisible')
    onVisibilityChanged(value: boolean) {
        this.$emit('model', value);
    }
}

</script>

<template>
    <modal-window v-model="isVisible" >
        <template v-slot:header>
            <div class="modal-window__header__caption">
                Link sharing
            </div>
            <div class="modal-window__header__actions">
                <button
                    href="#" 
                    class="btn-header" 
                    @click.prevent="isVisible = !isVisible"
                >
                    <div class="btn-header__content">
                        <i class="icon icon-cross"></i>
                    </div>
                </button>
            </div>
        </template>
        <template v-slot:content>
            <p>You are creating a public link with the following properties:</p>
            <p>Scrollable content with max-height 100%</p>
        </template>
        <template v-slot:footer>
            <button 
                type="button" 
                class="btn btn-block btn-primary"
            >
                Create
            </button>
        </template>
    </modal-window>
</template>