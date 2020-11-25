import { Component } from "vue-property-decorator";
import { DialogComponent } from "vue-modal-dialogs";

@Component
export class DialogBase extends DialogComponent<boolean> {
    created() {
        document.body.classList.add('modal-open');
    }
    
    destroyed() {
        document.body.classList.remove('modal-open');
    }
}