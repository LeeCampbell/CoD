import { TakePaymentCommand } from "@cod/domain";

export interface LoanPaymentModelInput {
  amount: number;
}

export function toTakePaymentCommand(
  loanId: string,
  model: LoanPaymentModelInput,
): TakePaymentCommand {
  return TakePaymentCommand.create({
    commandId: crypto.randomUUID(),
    aggregateId: loanId,
    transactionDateTime: new Date(),
    amount: model.amount,
  });
}
