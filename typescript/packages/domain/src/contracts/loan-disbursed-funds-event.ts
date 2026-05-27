import { BankAccount } from "./bank-account.js";
import { Event } from "./event.js";

export class LoanDisbursedFundsEvent extends Event {
  readonly eventType = "LoanDisbursedFundsEvent" as const;
  readonly transactionDate: Date;
  readonly amount: number;
  readonly disbursedTo: BankAccount;

  constructor(transactionDate: Date, amount: number, disbursedTo: BankAccount) {
    super();
    this.transactionDate = transactionDate;
    this.amount = amount;
    this.disbursedTo = disbursedTo;
  }
}
