import { InvalidCommandError } from "../errors.js";

const UUID_REGEX =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export abstract class Command {
  readonly commandId: string;
  readonly aggregateId: string;

  protected constructor(commandId: string, aggregateId: string) {
    if (!commandId || !UUID_REGEX.test(commandId))
      throw new InvalidCommandError("commandId must be a valid UUID");
    if (!aggregateId || !UUID_REGEX.test(aggregateId))
      throw new InvalidCommandError("aggregateId must be a valid UUID");

    this.commandId = commandId;
    this.aggregateId = aggregateId;
  }
}
