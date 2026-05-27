import { DisburseLoanFundsCommand } from "@cod/domain";

export function toDisburseLoanFundsCommand(
  loanId: string,
): DisburseLoanFundsCommand {
  return DisburseLoanFundsCommand.create({
    commandId: crypto.randomUUID(),
    aggregateId: loanId,
    transactionDate: new Date(),
  });
}
