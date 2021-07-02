import { DateTime } from "luxon";

export class DateHelper {
    
    /**
     * Returns the default formatting for the date.
     */
    static format(d: Date): string {
        return d ? JSON.stringify(d).substr(1, 10) : null;
    }

    /**
     * Returns the full readable date.
     */
    static formatFull(d: string | Date) {
        const dt = typeof d === "string" ? DateTime.fromISO(d) : DateTime.fromJSDate(d);
        return dt.setLocale('en').toFormat("yyyy-MM-dd TT");
    }
}