import { describe, it, before } from "node:test";
import assert from "node:assert/strict";
import {
  BankAccount,
  CustomerContact,
  Duration,
  DurationUnit,
  type Event,
  type Handler,
  Loan,
  LoanBankAccountChangedEvent,
  LoanCreatedEvent,
  LoanCustomerContactChangedEvent,
  LoanDisbursedFundsEvent,
  LoanOverPaidEvent,
  LoanSettledEvent,
  PaymentPlan,
  PaymentTakenEvent,
  TakePaymentCommand,
  InvalidPaymentError,
  TakePaymentCommandHandler,
  TransactionReceipt,
} from "@cod/domain";
import { Specification } from "./specification.js";

class TakingPaymentBase extends Specification<
  Loan,
  TakePaymentCommand,
  TransactionReceipt
> {
  readonly createdDate: Date;
  readonly loanAmount: number;
  readonly transactionDate: Date;
  readonly transactionAmount: number;

  constructor(
    createdDate: Date,
    loanAmount: number,
    transactionDate: Date,
    transactionAmount: number,
  ) {
    super();
    this.createdDate = createdDate;
    this.loanAmount = loanAmount;
    this.transactionDate = transactionDate;
    this.transactionAmount = transactionAmount;
  }

  protected createAggregate(id: string): Loan {
    return new Loan(id);
  }

  protected given(): Event[] {
    const contact = CustomerContact.create(
      "bob",
      "0444444444",
      "0812341234",
      "10 Random Street",
    );
    return [
      new LoanCreatedEvent(
        this.createdDate,
        this.loanAmount,
        Duration.create(12, DurationUnit.Month),
        PaymentPlan.Weekly,
      ),
      new LoanCustomerContactChangedEvent(contact),
      new LoanBankAccountChangedEvent(
        BankAccount.create("066-000", "12345678"),
      ),
      new LoanDisbursedFundsEvent(
        new Date(this.createdDate.getTime() + 3600_000),
        -this.loanAmount,
        BankAccount.create("066-000", "12345678"),
      ),
    ];
  }

  protected when(): TakePaymentCommand {
    return TakePaymentCommand.create({
      commandId: crypto.randomUUID(),
      aggregateId: this.sut.id,
      transactionDateTime: this.transactionDate,
      amount: this.transactionAmount,
    });
  }

  protected createHandler(): Handler<TakePaymentCommand, TransactionReceipt> {
    return new TakePaymentCommandHandler(this.repository);
  }
}

describe("Take payment prior to creation", () => {
  const spec = new TakingPaymentBase(
    new Date("2017-06-05"),
    2000,
    new Date("2017-06-04"),
    123.45,
  );
  before(async () => spec.execute());

  it("throws InvalidPaymentError", () => {
    assert.ok(spec.caught instanceof InvalidPaymentError);
    assert.strictEqual(
      spec.caught!.message,
      "Transaction date can not be prior to loan creation",
    );
  });
});

describe("Take payment for zero amount", () => {
  const spec = new TakingPaymentBase(
    new Date("2000-01-01"),
    2000,
    new Date("2000-01-02"),
    0,
  );
  before(async () => spec.execute());

  it("throws InvalidPaymentError", () => {
    assert.ok(spec.caught instanceof InvalidPaymentError);
    assert.strictEqual(
      spec.caught!.message,
      "Transaction amount must be positive",
    );
  });
});

describe("Take payment for negative amount", () => {
  const spec = new TakingPaymentBase(
    new Date("2000-01-01"),
    2000,
    new Date("2000-01-02"),
    -1,
  );
  before(async () => spec.execute());

  it("throws InvalidPaymentError", () => {
    assert.ok(spec.caught instanceof InvalidPaymentError);
    assert.strictEqual(
      spec.caught!.message,
      "Transaction amount must be positive",
    );
  });
});

describe("Taking a successful payment", () => {
  const spec = new TakingPaymentBase(
    new Date("2017-06-04T18:30:20.000Z"),
    2000,
    new Date("2017-06-12T00:00:00.000Z"),
    123.45,
  );
  before(async () => spec.execute());

  it("raises PaymentTakenEvent", () => {
    const actual = spec.produced[0] as PaymentTakenEvent;
    assert.deepStrictEqual(actual.transactionDateTime, spec.transactionDate);
    assert.strictEqual(actual.amount, spec.transactionAmount);
  });
});

describe("Loan settlement", () => {
  const spec = new TakingPaymentBase(
    new Date("2000-01-01"),
    2000,
    new Date("2000-01-02"),
    2000,
  );
  before(async () => spec.execute());

  it("raises PaymentTakenEvent", () => {
    const actual = spec.produced[0] as PaymentTakenEvent;
    assert.deepStrictEqual(actual.transactionDateTime, spec.transactionDate);
    assert.strictEqual(actual.amount, spec.transactionAmount);
  });

  it("raises LoanSettledEvent", () => {
    const actual = spec.produced[1] as LoanSettledEvent;
    assert.deepStrictEqual(
      actual.transactionDateTime,
      spec.transactionDate,
    );
  });
});

describe("Loan over-payment", () => {
  const spec = new TakingPaymentBase(
    new Date("2000-01-01"),
    2000,
    new Date("2000-01-02"),
    2001,
  );
  before(async () => spec.execute());

  it("raises PaymentTakenEvent", () => {
    const actual = spec.produced[0] as PaymentTakenEvent;
    assert.deepStrictEqual(actual.transactionDateTime, spec.transactionDate);
    assert.strictEqual(actual.amount, spec.transactionAmount);
  });

  it("raises LoanSettledEvent", () => {
    const actual = spec.produced[1] as LoanSettledEvent;
    assert.deepStrictEqual(
      actual.transactionDateTime,
      spec.transactionDate,
    );
  });

  it("raises LoanOverPaidEvent", () => {
    const actual = spec.produced[2] as LoanOverPaidEvent;
    assert.deepStrictEqual(
      actual.transactionDateTime,
      spec.transactionDate,
    );
    assert.strictEqual(
      actual.amount,
      spec.transactionAmount - spec.loanAmount,
    );
  });
});
