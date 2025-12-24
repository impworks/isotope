import type { VariantProps } from "class-variance-authority"
import { cva } from "class-variance-authority"

export { default as Badge } from "./Badge.vue"

export const badgeVariants = cva(
  "inline-flex gap-1 items-center rounded-full border px-2 py-0.5 text-xs font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2",
  {
    variants: {
      variant: {
        default:
          "border-transparent bg-primary text-primary-foreground hover:bg-primary/80",
        secondary:
          "border-transparent bg-secondary text-secondary-foreground hover:bg-secondary/80",
        destructive:
          "border-transparent bg-destructive text-destructive-foreground hover:bg-destructive/80",
        outline: "text-foreground",
        blue: "border-transparent bg-blue-100 text-blue-700 hover:bg-blue-200",
        green: "border-transparent bg-green-100 text-green-700 hover:bg-green-200",
        yellow: "border-transparent bg-yellow-100 text-yellow-700 hover:bg-yellow-200",
        orange: "border-transparent bg-orange-100 text-orange-700 hover:bg-orange-200",
        red: "border-transparent bg-red-100 text-red-700 hover:bg-red-200",
      },
    },
    defaultVariants: {
      variant: "default",
    },
  },
)

export type BadgeVariants = VariantProps<typeof badgeVariants>
