import { type Command, type Handler, type Receipt } from "@cod/domain";

export function withLogging<TCommand extends Command, TReceipt extends Receipt>(
  handler: Handler<TCommand, TReceipt>,
): Handler<TCommand, TReceipt> {
  return {
    async handle(command: TCommand): Promise<TReceipt> {
      try {
        console.log(`Handling command ${command}...`);
        const receipt = await handler.handle(command);
        console.log(
          `Handled command ${command.commandId} for aggregate ${receipt.aggregateId} version ${receipt.version}.`,
        );
        return receipt;
      } catch (e) {
        console.warn(`Failed to handle command ${command}`);
        throw e;
      }
    },
  };
}
