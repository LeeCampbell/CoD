export const PaymentPlan = {
  Weekly: "Weekly",
  Fortnightly: "Fortnightly",
  Monthly: "Monthly",
} as const;

export type PaymentPlan = (typeof PaymentPlan)[keyof typeof PaymentPlan];
