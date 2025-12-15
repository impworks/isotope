import { TagBinding } from "./TagBinding";
import { Rect } from "../../../../Common/source/vms/Rect";

export interface OverlayTagBinding extends TagBinding {
    location: Rect;
}