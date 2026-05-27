import { InvalidCommandError } from "../errors.js";
import { Command } from "./command.js";

export class TakePaymentCommand extends Command {
  readonly #brand!: undefined;
  readonly transactionDateTime: Date;
  readonly amount: number;

  private constructor(
    commandId: string,
    aggregateId: string,
    transactionDateTime: Date,
    amount: number,
  ) {
    super(commandId, aggregateId);
    this.transactionDateTime = transactionDateTime;
    this.amount = amount;
  }

  static create(input: {
    commandId: string;
    aggregateId: string;
    transactionDateTime: Date;
    amount: number;
  }): TakePaymentCommand {
    if (
      !(input.transactionDateTime instanceof Date) ||
      isNaN(input.transactionDateTime.getTime())
    )
      throw new InvalidCommandError(
        "TransactionDateTime must be a valid date",
      );
    if (typeof input.amount !== "number")
      throw new InvalidCommandError("Amount must be a number");

    return new TakePaymentCommand(
      input.commandId,
      input.aggregateId,
      input.transactionDateTime,
      input.amount,
    );
  }

  toString(): string {
    return `TakePaymentCommand{ AggregateId:'${this.aggregateId}', Amount:'${this.amount}'}`;
  }
}
