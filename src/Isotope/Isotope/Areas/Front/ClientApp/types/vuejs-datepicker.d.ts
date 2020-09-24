declare module "vuejs-datepicker" {
    import { VueConstructor } from "vue";
    
    export default VuejsDatepicker;
    export const VuejsDatepicker: VuejsDatepickerConstructor;
    
    interface ClassLookup {
        [key: string]: boolean;
    }

    interface Language {
        language: string;
        months: string[];
        monthsAbbr: string[];
        days: string[];
    }
    
    type ViewType = 'day' | 'month' | 'year';
    
    export interface VuejsDatepickerProps {
        value: Date | string;
        name: string;
        id: string;
        format: string | (() => string);
        fullMonthName: boolean;
        language: Language;
        disabledDates: VuejsDatepickerDisabledDates;
        highlighted: VuejsDatepickerHighlightedDates;
        placeholder: string;
        inline: boolean;
        calendarClass: string | ClassLookup;
        inputClass: string | ClassLookup;
        wrapperClass: string | ClassLookup;
        mondayFirst: boolean;
        clearButton: boolean;
        clearButtonIcon: string;
        calendarButton: boolean;
        calendarButtonIcon: string;
        calendarButtonIconContent: string;
        dayCellContent: (() => string);
        boostrapStyling: boolean;
        initialView: string;
        disabled: boolean;
        required: boolean;
        typeable: boolean;
        useUtc: boolean;
        openDate: Date | string;
        minimumView: ViewType;
        maximumView: ViewType;
        
        opened: () => void;
        closed: () => void;
        selected: (d: Date | null) => void;
        selectedDisabled: (x: object) => void;
        input: (d: Date | null) => void;
        cleared: () => void;
        changedMonth: (x: object) => void;
        changedYear: (x: object) => void;
        changedDecade: (x: object) => void;
    }
    
    interface VuejsDatepickerDatesSpecification {
        to?: Date;
        from?: Date;
        days?: number[];
        daysOfMonth?: number[];
        dates?: Date[];
        customPredictor?: (d: Date) => boolean;
    }
    
    export interface VuejsDatepickerDisabledDates extends VuejsDatepickerDatesSpecification {
        ranges?: {
            from?: Date;
            to?: Date;
        }[];
    }
    
    export interface VuejsDatepickerHighlightedDates extends VuejsDatepickerDatesSpecification {
        includeDisabled: boolean;
    }
    
    export interface VuejsDatepickerConstructor extends VueConstructor {
        props: VuejsDatepickerProps;
    }
}

declare module "vuejs-datepicker/dist/locale" {
    export interface Language {
        language: string;
        months: string[];
        monthsAbbr: string[];
        days: string[];
    }
    
    export interface LanguageConstructor {
        new (language: string, months: string[], monthsAbbr: string[], days: string[]): Language;
    }
    
    export const ar: Language;
    export const af: Language;
    export const bg: Language;
    export const bs: Language;
    export const ca: Language;
    export const cs: Language;
    export const da: Language;
    export const de: Language;
    export const ee: Language;
    export const el: Language;
    export const en: Language;
    export const es: Language;
    export const fa: Language;
    export const fi: Language;
    export const fo: Language;
    export const fr: Language;
    export const ge: Language;
    export const gl: Language;
    export const he: Language;
    export const hr: Language;
    export const hu: Language;
    export const id: Language;
    export const is: Language;
    export const it: Language;
    export const ja: Language;
    export const kk: Language;
    export const ko: Language;
    export const lb: Language;
    export const lt: Language;
    export const lv: Language;
    export const mk: Language;
    export const mn: Language;
    export const nbNO: Language;
    export const nl: Language;
    export const pl: Language;
    export const ptBR: Language;
    export const ro: Language;
    export const ru: Language;
    export const sk: Language;
    export const slSI: Language;
    export const sv: Language;
    export const sr: Language;
    export const srCyrl: Language;
    export const th: Language;
    export const tr: Language;
    export const uk: Language;
    export const ur: Language;
    export const vi: Language;
    export const zh: Language;
    export const zhHK: Language;
}