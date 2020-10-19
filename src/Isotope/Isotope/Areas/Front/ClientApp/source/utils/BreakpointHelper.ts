export enum Breakpoints {
    xs = 1,
    sm = 2,
    md = 3,
    lg = 4,
    xl = 5
}

export class BreakpointHelper {

    static breakpoints: { [key in Breakpoints]: number } = {
        [Breakpoints.xs]: 0,
        [Breakpoints.sm]: 576,
        [Breakpoints.md]: 768,
        [Breakpoints.lg]: 992,
        [Breakpoints.xl]: 1200
    };
    
    static down(breakpoint: Breakpoints): boolean {
        return window.innerWidth < this.breakpoints[breakpoint];
    }

    static up(breakpoint: Breakpoints): boolean {
        return window.innerWidth >= this.breakpoints[breakpoint];
    }
}

