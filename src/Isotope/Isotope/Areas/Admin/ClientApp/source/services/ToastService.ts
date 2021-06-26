import * as toastr from "toastr";
import { isAxiosError } from "../../../../Front/ClientApp/source/utils/AxiosHelpers";

export class ToastService {
    /**
     * Displays a success message (green).
     * @param msg Message.
     */
    success(msg: string) {
        toastr.success(msg);
    }

    /**
     * Displays an exception.
     * @param msg Message.
     * @param e Optional exception.
     */
    error(msg: string, e?: any) {
        if(!this.isSilent(e))
            toastr.error(msg);

        if(e)
            console.error(e);
    }

    private isSilent(e: any) {
        return isAxiosError(e) && [404, 501].indexOf(e.response.status) != -1;
    }
}