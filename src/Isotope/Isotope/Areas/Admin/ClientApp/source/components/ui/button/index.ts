import type { VariantProps } from "class-variance-authority"
import { cva } from "class-variance-authority"

export { default as Button } from "./Button.vue"

export const buttonVariants = cva(
  "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0",
  {
    variants: {
      variant: {
        default: "bg-primary text-primary-foreground hover:bg-primary/80 focus-visible:ring-primary/30",
        destructive:
          "bg-destructive text-destructive-foreground hover:bg-destructive/80 focus-visible:ring-destructive/30",
        outline:
          "border border-input bg-background hover:bg-foreground/5 focus-visible:border-gray-400 focus-visible:ring-gray-200",
        secondary:
          "border border-input bg-background text-foreground hover:bg-foreground/5 focus-visible:border-gray-400 focus-visible:ring-gray-200",
        ghost: "hover:bg-foreground/5 focus-visible:ring-gray-200",
        link: "text-primary underline-offset-4 hover:underline focus-visible:ring-primary/30",
      },
      size: {
        "default": "h-8 px-3 py-1.5",
        "sm": "h-7 px-2 py-1 text-xs",
        "lg": "h-10 px-4 py-2",
        "icon": "h-8 w-8",
        "icon-sm": "size-7",
        "icon-lg": "size-10",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "default",
    },
  },
)

export type ButtonVariants = VariantProps<typeof buttonVariants>
