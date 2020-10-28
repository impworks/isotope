import { TagBinding } from "./TagBinding";
import { Rect } from "./Rect";

export interface OverlayTagBinding extends TagBinding {
    location: Rect;
}