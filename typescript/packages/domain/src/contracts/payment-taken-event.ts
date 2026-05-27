import { Event } from "./event.js";

export class PaymentTakenEvent extends Event {
  readonly eventType = "PaymentTakenEvent" as const;
  readonly transactionId: string;
  readonly transactionDateTime: Date;
  readonly amount: number;

  constructor(
    transactionId: string,
    transactionDateTime: Date,
    amount: number,
  ) {
    super();
    this.transactionId = transactionId;
    this.transactionDateTime = transactionDateTime;
    this.amount = amount;
  }
}
