import { Command } from "../contracts/command.js";
import { Receipt } from "../contracts/receipt.js";

export interface Handler<
  TCommand extends Command,
  TReceipt extends Receipt,
> {
  handle(command: TCommand): Promise<TReceipt>;
}
