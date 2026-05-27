import { describe, it, before } from "node:test";
import assert from "node:assert/strict";
import {
  BankAccount,
  DisburseLoanFundsCommand,
  DisburseLoanFundsCommandHandler,
  Duration,
  DurationUnit,
  type Event,
  type Handler,
  Loan,
  LoanBankAccountChangedEvent,
  LoanCreatedEvent,
  LoanCustomerContactChangedEvent,
  LoanDisbursedFundsEvent,
  FundsAlreadyDisbursedError,
  PaymentPlan,
  Receipt,
} from "@cod/domain";
import { Specification } from "./specification.js";

const CREATED_DATE = new Date("2017-06-04T18:30:20.000Z");
const LOAN_AMOUNT = 2000;

class DisbursingFunds extends Specification<
  Loan,
  DisburseLoanFundsCommand,
  Receipt
> {
  transactionDate!: Date;

  protected createAggregate(id: string): Loan {
    return new Loan(id);
  }

  protected given(): Event[] {
    return [
      new LoanCreatedEvent(
        CREATED_DATE,
        LOAN_AMOUNT,
        Duration.create(12, DurationUnit.Month),
        PaymentPlan.Weekly,
      ),
      new LoanCustomerContactChangedEvent(
        CustomerContact_create(),
      ),
      new LoanBankAccountChangedEvent(BankAccount.create("066-000", "12345678")),
    ];
  }

  protected when(): DisburseLoanFundsCommand {
    this.transactionDate = new Date(CREATED_DATE.getTime() + 3600_000);
    return DisburseLoanFundsCommand.create({
      commandId: crypto.randomUUID(),
      aggregateId: this.sut.id,
      transactionDate: this.transactionDate,
    });
  }

  protected createHandler(): Handler<DisburseLoanFundsCommand, Receipt> {
    return new DisburseLoanFundsCommandHandler(this.repository);
  }
}

class DisbursingFundsMultipleTimes extends Specification<
  Loan,
  DisburseLoanFundsCommand,
  Receipt
> {
  protected createAggregate(id: string): Loan {
    return new Loan(id);
  }

  protected given(): Event[] {
    return [
      new LoanCreatedEvent(
        CREATED_DATE,
        LOAN_AMOUNT,
        Duration.create(12, DurationUnit.Month),
        PaymentPlan.Weekly,
      ),
      new LoanCustomerContactChangedEvent(
        CustomerContact_create(),
      ),
      new LoanBankAccountChangedEvent(BankAccount.create("066-000", "12345678")),
      new LoanDisbursedFundsEvent(
        new Date(CREATED_DATE.getTime() + 3600_000),
        LOAN_AMOUNT,
        BankAccount.create("066-000", "12345678"),
      ),
    ];
  }

  protected when(): DisburseLoanFundsCommand {
    return DisburseLoanFundsCommand.create({
      commandId: crypto.randomUUID(),
      aggregateId: this.sut.id,
      transactionDate: new Date(CREATED_DATE.getTime() + 3600_000),
    });
  }

  protected createHandler(): Handler<DisburseLoanFundsCommand, Receipt> {
    return new DisburseLoanFundsCommandHandler(this.repository);
  }
}

// Helper to avoid importing CustomerContact in every test
import { CustomerContact } from "@cod/domain";
function CustomerContact_create() {
  return CustomerContact.create(
    "bob",
    "0444444444",
    "0812341234",
    "10 Random Street",
  );
}

describe("Disbursing funds", () => {
  const spec = new DisbursingFunds();
  before(async () => spec.execute());

  it("raises LoanDisbursedFundsEvent", () => {
    assert.strictEqual(spec.produced.length, 1);
    const actual = spec.produced[0] as LoanDisbursedFundsEvent;
    assert.deepStrictEqual(actual.transactionDate, spec.transactionDate);
    assert.strictEqual(actual.amount, -LOAN_AMOUNT);
    assert.strictEqual(actual.disbursedTo.bsb, "066-000");
    assert.strictEqual(actual.disbursedTo.accountNumber, "12345678");
  });
});

describe("Disbursing funds multiple times", () => {
  const spec = new DisbursingFundsMultipleTimes();
  before(async () => spec.execute());

  it("throws FundsAlreadyDisbursedError", () => {
    assert.ok(spec.caught instanceof FundsAlreadyDisbursedError);
    assert.strictEqual(spec.caught!.message, "Funds are already disbursed.");
  });
});
