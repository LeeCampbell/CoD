// Contracts - Value Objects
export { BankAccount } from "./contracts/bank-account.js";
export { CustomerContact } from "./contracts/customer-contact.js";
export { Duration } from "./contracts/duration.js";
export { DurationUnit } from "./contracts/duration-unit.js";
export { PaymentPlan } from "./contracts/payment-plan.js";

// Contracts - Base Types
export { Command } from "./contracts/command.js";
export { Event } from "./contracts/event.js";
export { Receipt } from "./contracts/receipt.js";
export { TransactionReceipt } from "./contracts/transaction-receipt.js";

// Contracts - Commands
export { CreateLoanCommand } from "./contracts/create-loan-command.js";
export { DisburseLoanFundsCommand } from "./contracts/disburse-loan-funds-command.js";
export { TakePaymentCommand } from "./contracts/take-payment-command.js";

// Contracts - Events
export { LoanCreatedEvent } from "./contracts/loan-created-event.js";
export { LoanDisbursedFundsEvent } from "./contracts/loan-disbursed-funds-event.js";
export { PaymentTakenEvent } from "./contracts/payment-taken-event.js";
export { LoanCustomerContactChangedEvent } from "./contracts/loan-customer-contact-changed-event.js";
export { LoanBankAccountChangedEvent } from "./contracts/loan-bank-account-changed-event.js";
export { LoanSettledEvent } from "./contracts/loan-settled-event.js";
export { LoanOverPaidEvent } from "./contracts/loan-over-paid-event.js";

// Errors
export {
  InvalidBankAccountError,
  InvalidCustomerContactError,
  InvalidDurationError,
  InvalidCommandError,
  LoanAlreadyCreatedError,
  UnsupportedLoanAmountError,
  UnsupportedLoanTermError,
  FundsAlreadyDisbursedError,
  InvalidPaymentError,
} from "./errors.js";
