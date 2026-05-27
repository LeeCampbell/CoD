import {
  AggregateRoot,
  type Command,
  Event,
  type Handler,
  type Receipt,
} from "@cod/domain";
import { FakeRepository } from "./fake-repository.js";

export abstract class Specification<
  TAggregate extends AggregateRoot,
  TCommand extends Command,
  TReceipt extends Receipt,
> {
  #repository!: FakeRepository<TAggregate>;
  #sut!: TAggregate;
  #produced: Event[] = [];
  #receipt?: TReceipt;
  #caught?: Error;

  protected abstract createAggregate(id: string): TAggregate;
  protected abstract given(): Event[];
  protected abstract when(): TCommand;
  protected abstract createHandler(): Handler<TCommand, TReceipt>;

  get sut(): TAggregate {
    return this.#sut;
  }

  get produced(): Event[] {
    return this.#produced;
  }

  get receipt(): TReceipt | undefined {
    return this.#receipt;
  }

  get caught(): Error | undefined {
    return this.#caught;
  }

  get repository(): FakeRepository<TAggregate> {
    return this.#repository;
  }

  async execute(): Promise<void> {
    try {
      this.#repository = new FakeRepository<TAggregate>((id) =>
        this.createAggregate(id),
      );
      this.#sut = await this.#repository.get(crypto.randomUUID());

      for (const event of this.given()) {
        this.#sut.applyEvent(event);
      }

      const handler = this.createHandler();
      const command = this.when();
      this.#receipt = await handler.handle(command);
      this.#produced = this.#repository.committedEvents;
    } catch (e) {
      this.#caught = e as Error;
    }
  }
}
