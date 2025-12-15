export interface MassMediaAction {
    keys: string[];
}

export interface MassMediaMove extends MassMediaAction {
    folderKey: string;
}

export interface MassMediaUpdate extends MassMediaAction {
    description: Option<string>;
    date: Option<string>;
}

export interface Option<T> {
    isSet: boolean;
    value: T;
}