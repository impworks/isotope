import { AxiosError } from "axios";

export function isAxiosError(e: any): e is AxiosError {
    return e && e.config && e.request && e.response;
}