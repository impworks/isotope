export interface GalleryInfo {
    caption: string;
    allowGuests: boolean;
    isAuthorized: boolean;
    isLinkValid?: boolean | null;
}