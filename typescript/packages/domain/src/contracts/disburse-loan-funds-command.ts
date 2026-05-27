import { InvalidCommandError } from "../errors.js";
import { Command } from "./command.js";

export class DisburseLoanFundsCommand extends Command {
  readonly #brand!: undefined;
  readonly transactionDate: Date;

  private constructor(
    commandId: string,
    aggregateId: string,
    transactionDate: Date,
  ) {
    super(commandId, aggregateId);
    this.transactionDate = transactionDate;
  }

  static create(input: {
    commandId: string;
    aggregateId: string;
    transactionDate: Date;
  }): DisburseLoanFundsCommand {
    if (
      !(input.transactionDate instanceof Date) ||
      isNaN(input.transactionDate.getTime())
    )
      throw new InvalidCommandError("TransactionDate must be a valid date");

    return new DisburseLoanFundsCommand(
      input.commandId,
      input.aggregateId,
      input.transactionDate,
    );
  }

  toString(): string {
    return `DisburseLoanFundsCommand{ AggregateId:'${this.aggregateId}', TransactionDate:'${this.transactionDate.toISOString()}'}`;
  }
}
