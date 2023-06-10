export interface GalleryInfo {
    caption: string;
    subcaption: string;
    allowGuests: boolean;
    isAuthorized: boolean;
    isAdmin?: boolean | null;
    isLinkValid?: boolean | null;
}