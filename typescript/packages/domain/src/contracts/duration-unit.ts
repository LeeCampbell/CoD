export const DurationUnit = {
  Day: "Day",
  Week: "Week",
  Month: "Month",
} as const;

export type DurationUnit = (typeof DurationUnit)[keyof typeof DurationUnit];
