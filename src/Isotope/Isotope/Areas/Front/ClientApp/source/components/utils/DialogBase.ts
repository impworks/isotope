import { Component, Model, Vue, Watch } from "vue-property-decorator";

@Component
export class DialogBase extends Vue {
    @Model() model: boolean;
    isVisible: boolean = false;

    async mounted() {
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