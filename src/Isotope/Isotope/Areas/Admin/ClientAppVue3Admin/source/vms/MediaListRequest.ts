export interface MediaListRequest {
    folder: string;
    orderBy: 'Order' | 'UploadDate' | 'Date';
    orderDesc: boolean;
    page: number;
}