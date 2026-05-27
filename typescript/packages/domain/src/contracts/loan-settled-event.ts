import { Event } from "./event.js";

export class LoanSettledEvent extends Event {
  readonly eventType = "LoanSettledEvent" as const;
  readonly transactionDateTime: Date;

  constructor(transactionDateTime: Date) {
    super();
    this.transactionDateTime = transactionDateTime;
  }
}
