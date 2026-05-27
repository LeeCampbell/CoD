import { Event } from "./event.js";

export class LoanOverPaidEvent extends Event {
  readonly eventType = "LoanOverPaidEvent" as const;
  readonly transactionDateTime: Date;
  readonly amount: number;

  constructor(transactionDateTime: Date, amount: number) {
    super();
    this.transactionDateTime = transactionDateTime;
    this.amount = amount;
  }
}
