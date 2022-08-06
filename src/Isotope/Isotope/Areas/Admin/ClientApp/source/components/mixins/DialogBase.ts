import { Component } from "vue-property-decorator";
import { DialogComponent } from "vue-modal-dialogs";

@Component
export class DialogBase extends DialogComponent<boolean> {
    static modalsDepth: number = 0;
    
    created() {
        DialogBase.modalsDepth++;
        if(DialogBase.modalsDepth === 1)
            document.body.classList.add('modal-open');
    }
    
    destroyed() {
        DialogBase.modalsDepth--;
        if(DialogBase.modalsDepth === 0)
            document.body.classList.remove('modal-open');
    }
}