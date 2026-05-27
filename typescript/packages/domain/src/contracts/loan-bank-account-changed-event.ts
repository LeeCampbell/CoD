import { BankAccount } from "./bank-account.js";
import { Event } from "./event.js";

export class LoanBankAccountChangedEvent extends Event {
  readonly eventType = "LoanBankAccountChangedEvent" as const;
  readonly bankAccount: BankAccount;

  constructor(bankAccount: BankAccount) {
    super();
    this.bankAccount = bankAccount;
  }
}
