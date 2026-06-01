export class InvalidBankAccountError extends Error {
  constructor(message: string, readonly field: "bsb" | "accountNumber") {
    super(message);
    this.name = "InvalidBankAccountError";
  }
}

export class InvalidCustomerContactError extends Error {
  constructor(
    message: string,
    readonly field:
      | "name"
      | "preferredPhoneNumber"
      | "alternatePhoneNumber"
      | "postalAddress",
  ) {
    super(message);
    this.name = "InvalidCustomerContactError";
  }
}

export class InvalidDurationError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "InvalidDurationError";
  }
}

export class InvalidCommandError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "InvalidCommandError";
  }
}

export class LoanAlreadyCreatedError extends Error {
  constructor() {
    super("Loan already created.");
    this.name = "LoanAlreadyCreatedError";
  }
}

export class UnsupportedLoanAmountError extends Error {
  constructor(amount: number) {
    super(
      `Only loan amounts between $50.00 and $2000.00 are supported. Got $${amount.toFixed(2)}.`,
    );
    this.name = "UnsupportedLoanAmountError";
  }
}

export class UnsupportedLoanTermError extends Error {
  constructor() {
    super("Only loan terms up to 2 years are supported.");
    this.name = "UnsupportedLoanTermError";
  }
}

export class FundsAlreadyDisbursedError extends Error {
  constructor() {
    super("Funds are already disbursed.");
    this.name = "FundsAlreadyDisbursedError";
  }
}

export class InvalidPaymentError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "InvalidPaymentError";
  }
}
