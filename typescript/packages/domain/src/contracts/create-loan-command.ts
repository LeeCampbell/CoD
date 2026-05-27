import { InvalidCommandError } from "../errors.js";
import { BankAccount } from "./bank-account.js";
import { Command } from "./command.js";
import { CustomerContact } from "./customer-contact.js";
import { Duration } from "./duration.js";
import { PaymentPlan } from "./payment-plan.js";

export class CreateLoanCommand extends Command {
  readonly #brand!: undefined;
  readonly createdOn: Date;
  readonly customerContact: CustomerContact;
  readonly bankAccount: BankAccount;
  readonly paymentPlan: PaymentPlan;
  readonly amount: number;
  readonly term: Duration;

  private constructor(
    commandId: string,
    aggregateId: string,
    createdOn: Date,
    customerContact: CustomerContact,
    bankAccount: BankAccount,
    paymentPlan: PaymentPlan,
    amount: number,
    term: Duration,
  ) {
    super(commandId, aggregateId);
    this.createdOn = createdOn;
    this.customerContact = customerContact;
    this.bankAccount = bankAccount;
    this.paymentPlan = paymentPlan;
    this.amount = amount;
    this.term = term;
  }

  static create(input: {
    commandId: string;
    aggregateId: string;
    createdOn: Date;
    customerContact: CustomerContact;
    bankAccount: BankAccount;
    paymentPlan: PaymentPlan;
    amount: number;
    term: Duration;
  }): CreateLoanCommand {
    if (!(input.createdOn instanceof Date) || isNaN(input.createdOn.getTime()))
      throw new InvalidCommandError("CreatedOn must be a valid date");
    if (!input.customerContact)
      throw new InvalidCommandError("CustomerContact must be non null");
    if (!input.bankAccount)
      throw new InvalidCommandError("BankAccount must be non null");
    if (!input.paymentPlan)
      throw new InvalidCommandError("PaymentPlan must be non default value");
    if (input.amount === 0)
      throw new InvalidCommandError("Amount must be non zero value");
    if (!input.term)
      throw new InvalidCommandError("Term must be non null");

    return new CreateLoanCommand(
      input.commandId,
      input.aggregateId,
      input.createdOn,
      input.customerContact,
      input.bankAccount,
      input.paymentPlan,
      input.amount,
      input.term,
    );
  }

  toString(): string {
    return `CreateLoanCommand{ CreatedOn:'${this.createdOn.toISOString()}', Amount:'${this.amount}', Term:${this.term}}`;
  }
}
