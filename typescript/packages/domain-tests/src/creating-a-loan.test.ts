import { describe, it, before } from "node:test";
import assert from "node:assert/strict";
import {
  BankAccount,
  CreateLoanCommand,
  CreateLoanCommandHandler,
  CustomerContact,
  Duration,
  DurationUnit,
  type Event,
  type Handler,
  Loan,
  LoanAlreadyCreatedError,
  LoanBankAccountChangedEvent,
  LoanCreatedEvent,
  LoanCustomerContactChangedEvent,
  PaymentPlan,
  Receipt,
  UnsupportedLoanAmountError,
  UnsupportedLoanTermError,
} from "@cod/domain";
import { Specification } from "./specification.js";

class CreatingALoan extends Specification<Loan, CreateLoanCommand, Receipt> {
  readonly command: CreateLoanCommand;

  constructor() {
    super();
    const customerContact = CustomerContact.create(
      "Jane Doe",
      "0412341234",
      "0856785678",
      "10 St Georges Terrace, Perth, WA 6000",
    );
    const bankAccount = BankAccount.create("066-000", "12345678");
    this.command = CreateLoanCommand.create({
      commandId: crypto.randomUUID(),
      aggregateId: crypto.randomUUID(),
      createdOn: new Date("2001-02-03T04:05:06.000Z"),
      customerContact,
      bankAccount,
      paymentPlan: PaymentPlan.Weekly,
      amount: 1000,
      term: Duration.create(12, DurationUnit.Month),
    });
  }

  protected createAggregate(id: string): Loan {
    return new Loan(id);
  }

  protected given(): Event[] {
    return [];
  }

  protected when(): CreateLoanCommand {
    return this.command;
  }

  protected createHandler(): Handler<CreateLoanCommand, Receipt> {
    return new CreateLoanCommandHandler(this.repository);
  }
}

describe("Creating a loan", () => {
  const spec = new CreatingALoan();
  before(async () => spec.execute());

  it("raises LoanCreatedEvent", () => {
    const actual = spec.produced[0] as LoanCreatedEvent;
    assert.deepStrictEqual(actual.createdOn, spec.command.createdOn);
    assert.strictEqual(actual.amount, spec.command.amount);
    assert.strictEqual(actual.paymentPlan, spec.command.paymentPlan);
  });

  it("raises LoanCustomerContactChangedEvent", () => {
    const actual = spec.produced[1] as LoanCustomerContactChangedEvent;
    assert.strictEqual(
      actual.customerContact.name,
      spec.command.customerContact.name,
    );
    assert.strictEqual(
      actual.customerContact.preferredPhoneNumber,
      spec.command.customerContact.preferredPhoneNumber,
    );
    assert.strictEqual(
      actual.customerContact.alternatePhoneNumber,
      spec.command.customerContact.alternatePhoneNumber,
    );
    assert.strictEqual(
      actual.customerContact.postalAddress,
      spec.command.customerContact.postalAddress,
    );
  });

  it("raises LoanBankAccountChangedEvent", () => {
    const actual = spec.produced[2] as LoanBankAccountChangedEvent;
    assert.strictEqual(
      actual.bankAccount.bsb,
      spec.command.bankAccount.bsb,
    );
    assert.strictEqual(
      actual.bankAccount.accountNumber,
      spec.command.bankAccount.accountNumber,
    );
  });
});

class CreatingALoanMultipleTimes extends Specification<
  Loan,
  CreateLoanCommand,
  Receipt
> {
  protected createAggregate(id: string): Loan {
    return new Loan(id);
  }

  protected given(): Event[] {
    return [
      new LoanCreatedEvent(
        new Date("2000-01-01"),
        2000,
        Duration.create(12, DurationUnit.Month),
        PaymentPlan.Weekly,
      ),
    ];
  }

  protected when(): CreateLoanCommand {
    const customerContact = CustomerContact.create(
      "Jane Doe",
      "0412341234",
      "0856785678",
      "10 St Georges Terrace, Perth, WA 6000",
    );
    const bankAccount = BankAccount.create("066-000", "12345678");
    return CreateLoanCommand.create({
      commandId: crypto.randomUUID(),
      aggregateId: this.sut.id,
      createdOn: new Date("2001-02-03T04:05:06.000Z"),
      customerContact,
      bankAccount,
      paymentPlan: PaymentPlan.Weekly,
      amount: 1000,
      term: Duration.create(12, DurationUnit.Month),
    });
  }

  protected createHandler(): Handler<CreateLoanCommand, Receipt> {
    return new CreateLoanCommandHandler(this.repository);
  }
}

describe("Creating a loan multiple times", () => {
  const spec = new CreatingALoanMultipleTimes();
  before(async () => spec.execute());

  it("throws LoanAlreadyCreatedError", () => {
    assert.ok(spec.caught instanceof LoanAlreadyCreatedError);
    assert.strictEqual(spec.caught!.message, "Loan already created.");
  });
});

class CreatingALoanWithInvalidAmount extends Specification<
  Loan,
  CreateLoanCommand,
  Receipt
> {
  readonly #amount: number;
  constructor(amount: number) {
    super();
    this.#amount = amount;
  }

  protected createAggregate(id: string): Loan {
    return new Loan(id);
  }

  protected given(): Event[] {
    return [];
  }

  protected when(): CreateLoanCommand {
    const customerContact = CustomerContact.create(
      "Jane Doe",
      "0412341234",
      "0856785678",
      "10 St Georges Terrace, Perth, WA 6000",
    );
    const bankAccount = BankAccount.create("066-000", "12345678");
    return CreateLoanCommand.create({
      commandId: crypto.randomUUID(),
      aggregateId: this.sut.id,
      createdOn: new Date("2001-02-03T04:05:06.000Z"),
      customerContact,
      bankAccount,
      paymentPlan: PaymentPlan.Weekly,
      amount: this.#amount,
      term: Duration.create(12, DurationUnit.Month),
    });
  }

  protected createHandler(): Handler<CreateLoanCommand, Receipt> {
    return new CreateLoanCommandHandler(this.repository);
  }
}

describe("Creating a loan over $2000", () => {
  const spec = new CreatingALoanWithInvalidAmount(2001);
  before(async () => spec.execute());

  it("throws UnsupportedLoanAmountError", () => {
    assert.ok(spec.caught instanceof UnsupportedLoanAmountError);
    assert.strictEqual(
      spec.caught!.message,
      "Only loan amounts between $50.00 and $2000.00 are supported.",
    );
  });
});

describe("Creating a loan under $50", () => {
  const spec = new CreatingALoanWithInvalidAmount(49);
  before(async () => spec.execute());

  it("throws UnsupportedLoanAmountError", () => {
    assert.ok(spec.caught instanceof UnsupportedLoanAmountError);
    assert.strictEqual(
      spec.caught!.message,
      "Only loan amounts between $50.00 and $2000.00 are supported.",
    );
  });
});

class CreatingALoanWithInvalidTerm extends Specification<
  Loan,
  CreateLoanCommand,
  Receipt
> {
  readonly #term: Duration;
  constructor(term: Duration) {
    super();
    this.#term = term;
  }

  protected createAggregate(id: string): Loan {
    return new Loan(id);
  }

  protected given(): Event[] {
    return [];
  }

  protected when(): CreateLoanCommand {
    const customerContact = CustomerContact.create(
      "Jane Doe",
      "0412341234",
      "0856785678",
      "10 St Georges Terrace, Perth, WA 6000",
    );
    const bankAccount = BankAccount.create("066-000", "12345678");
    return CreateLoanCommand.create({
      commandId: crypto.randomUUID(),
      aggregateId: this.sut.id,
      createdOn: new Date("2001-02-03T04:05:06.000Z"),
      customerContact,
      bankAccount,
      paymentPlan: PaymentPlan.Weekly,
      amount: 1500,
      term: this.#term,
    });
  }

  protected createHandler(): Handler<CreateLoanCommand, Receipt> {
    return new CreateLoanCommandHandler(this.repository);
  }
}

describe("Creating a loan over 24 months", () => {
  const spec = new CreatingALoanWithInvalidTerm(
    Duration.create(25, DurationUnit.Month),
  );
  before(async () => spec.execute());

  it("throws UnsupportedLoanTermError", () => {
    assert.ok(spec.caught instanceof UnsupportedLoanTermError);
    assert.strictEqual(
      spec.caught!.message,
      "Only loan terms up to 2 years are supported.",
    );
  });
});

describe("Creating a loan over 104 weeks", () => {
  const spec = new CreatingALoanWithInvalidTerm(
    Duration.create(105, DurationUnit.Week),
  );
  before(async () => spec.execute());

  it("throws UnsupportedLoanTermError", () => {
    assert.ok(spec.caught instanceof UnsupportedLoanTermError);
    assert.strictEqual(
      spec.caught!.message,
      "Only loan terms up to 2 years are supported.",
    );
  });
});
