import { Duration } from "./duration.js";
import { Event } from "./event.js";
import { PaymentPlan } from "./payment-plan.js";

export class LoanCreatedEvent extends Event {
  readonly eventType = "LoanCreatedEvent" as const;
  readonly createdOn: Date;
  readonly amount: number;
  readonly term: Duration;
  readonly paymentPlan: PaymentPlan;

  constructor(
    createdOn: Date,
    amount: number,
    term: Duration,
    paymentPlan: PaymentPlan,
  ) {
    super();
    this.createdOn = createdOn;
    this.amount = amount;
    this.term = term;
    this.paymentPlan = paymentPlan;
  }
}
