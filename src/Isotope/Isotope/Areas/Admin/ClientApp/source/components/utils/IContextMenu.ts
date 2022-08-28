export interface IContextMenu {
    open(e: Event, data: any): void;
    close(): void;
}