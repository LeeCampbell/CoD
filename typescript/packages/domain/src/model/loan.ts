import { BankAccount } from "../contracts/bank-account.js";
import { CreateLoanCommand } from "../contracts/create-loan-command.js";
import { DisburseLoanFundsCommand } from "../contracts/disburse-loan-funds-command.js";
import { Duration } from "../contracts/duration.js";
import { DurationUnit } from "../contracts/duration-unit.js";
import { LoanBankAccountChangedEvent } from "../contracts/loan-bank-account-changed-event.js";
import { LoanCreatedEvent } from "../contracts/loan-created-event.js";
import { LoanCustomerContactChangedEvent } from "../contracts/loan-customer-contact-changed-event.js";
import { LoanDisbursedFundsEvent } from "../contracts/loan-disbursed-funds-event.js";
import { LoanOverPaidEvent } from "../contracts/loan-over-paid-event.js";
import { LoanSettledEvent } from "../contracts/loan-settled-event.js";
import { PaymentTakenEvent } from "../contracts/payment-taken-event.js";
import { TakePaymentCommand } from "../contracts/take-payment-command.js";
import {
  FundsAlreadyDisbursedError,
  InvalidPaymentError,
  LoanAlreadyCreatedError,
  UnsupportedLoanAmountError,
  UnsupportedLoanTermError,
} from "../errors.js";
import { AggregateRoot } from "./aggregate-root.js";

export class Loan extends AggregateRoot {
  #bankAccount?: BankAccount;
  #createdOn?: Date;
  #disbursedOn?: Date;
  #loanAmount = 0;
  #balance = 0;

  constructor(loanId: string) {
    super(loanId);
    this.registerHandler<LoanCreatedEvent>(
      "LoanCreatedEvent",
      (e) => this.#handleLoanCreated(e),
    );
    this.registerHandler<LoanDisbursedFundsEvent>(
      "LoanDisbursedFundsEvent",
      (e) => this.#handleLoanDisbursedFunds(e),
    );
    this.registerHandler<LoanCustomerContactChangedEvent>(
      "LoanCustomerContactChangedEvent",
      () => {},
    );
    this.registerHandler<LoanBankAccountChangedEvent>(
      "LoanBankAccountChangedEvent",
      (e) => this.#handleLoanBankAccountChanged(e),
    );
    this.registerHandler<PaymentTakenEvent>(
      "PaymentTakenEvent",
      (e) => this.#handlePaymentTaken(e),
    );
    this.registerHandler<LoanSettledEvent>("LoanSettledEvent", () => {});
    this.registerHandler<LoanOverPaidEvent>("LoanOverPaidEvent", () => {});
  }

  create(command: CreateLoanCommand): void {
    if (this.version > 0) throw new LoanAlreadyCreatedError();
    if (command.amount < 50 || command.amount > 2000)
      throw new UnsupportedLoanAmountError(command.amount);
    if (!Loan.#isUnder2Years(command.term))
      throw new UnsupportedLoanTermError();

    this.addEvent(
      new LoanCreatedEvent(
        command.createdOn,
        command.amount,
        command.term,
        command.paymentPlan,
      ),
    );
    this.addEvent(
      new LoanCustomerContactChangedEvent(command.customerContact),
    );
    this.addEvent(new LoanBankAccountChangedEvent(command.bankAccount));
  }

  disburseFunds(command: DisburseLoanFundsCommand): void {
    if (this.#disbursedOn) throw new FundsAlreadyDisbursedError();

    this.addEvent(
      new LoanDisbursedFundsEvent(
        command.transactionDate,
        -this.#loanAmount,
        BankAccount.create(
          this.#bankAccount!.bsb,
          this.#bankAccount!.accountNumber,
        ),
      ),
    );
  }

  takePayment(command: TakePaymentCommand): void {
    if (command.transactionDateTime < this.#createdOn!)
      throw new InvalidPaymentError(
        "Transaction date can not be prior to loan creation",
      );
    if (command.amount <= 0)
      throw new InvalidPaymentError("Transaction amount must be positive");

    this.addEvent(
      new PaymentTakenEvent(
        crypto.randomUUID(),
        command.transactionDateTime,
        command.amount,
      ),
    );
    if (this.#balance >= 0) {
      this.addEvent(new LoanSettledEvent(command.transactionDateTime));
    }
    if (this.#balance > 0) {
      this.addEvent(
        new LoanOverPaidEvent(command.transactionDateTime, this.#balance),
      );
    }
  }

  #handleLoanCreated(e: LoanCreatedEvent): void {
    this.#createdOn = e.createdOn;
    this.#loanAmount = e.amount;
    this.#balance = 0;
  }

  #handleLoanDisbursedFunds(e: LoanDisbursedFundsEvent): void {
    this.#disbursedOn = e.transactionDate;
    this.#balance += e.amount;
  }

  #handleLoanBankAccountChanged(e: LoanBankAccountChangedEvent): void {
    this.#bankAccount = BankAccount.create(
      e.bankAccount.bsb,
      e.bankAccount.accountNumber,
    );
  }

  #handlePaymentTaken(e: PaymentTakenEvent): void {
    this.#balance += e.amount;
  }

  static #isUnder2Years(duration: Duration): boolean {
    switch (duration.unit) {
      case DurationUnit.Day:
        return duration.length < 365 * 2;
      case DurationUnit.Week:
        return duration.length < 52 * 2;
      case DurationUnit.Month:
        return duration.length < 12 * 2;
      default:
        throw new Error("Unknown duration unit");
    }
  }
}
