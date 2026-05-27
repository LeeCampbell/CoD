import {
  BankAccount,
  CreateLoanCommand,
  CustomerContact,
  Duration,
  DurationUnit,
  PaymentPlan,
} from "@cod/domain";

export interface CreateLoanModelInput {
  customerName: string;
  preferredPhoneNumber: string;
  alternatePhoneNumber?: string;
  postalAddress: string;
  bankBsb: string;
  bankAccount: string;
  loanAmount: number;
}

export function toCreateLoanCommand(
  model: CreateLoanModelInput,
): CreateLoanCommand {
  const customerContact = CustomerContact.create(
    model.customerName,
    model.preferredPhoneNumber,
    model.alternatePhoneNumber,
    model.postalAddress,
  );
  const bankAccount = BankAccount.create(model.bankBsb, model.bankAccount);
  const duration = Duration.create(12, DurationUnit.Month);

  return CreateLoanCommand.create({
    commandId: crypto.randomUUID(),
    aggregateId: crypto.randomUUID(),
    createdOn: new Date(),
    customerContact,
    bankAccount,
    paymentPlan: PaymentPlan.Weekly,
    amount: model.loanAmount,
    term: duration,
  });
}
