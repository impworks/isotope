export class DateHelper {
    
    /**
     * Returns the default formatting for the date.
     */
    static format(d: Date): string {
        return d ? JSON.stringify(d).substr(1, 10) : null;
    }
}