export interface GalleryInfo {
    caption: string;
    allowGuests: boolean;
    isAuthorized: boolean;
    isAdmin?: boolean | null;
    isLinkValid?: boolean | null;
}