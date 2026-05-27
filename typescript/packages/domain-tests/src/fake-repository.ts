import {
  AggregateRoot,
  Event,
  type Repository,
} from "@cod/domain";

export class FakeRepository<T extends AggregateRoot>
  implements Repository<T>
{
  readonly #items = new Map<string, T>();
  readonly #factory: (id: string) => T;
  readonly committedEvents: Event[] = [];

  constructor(factory: (id: string) => T) {
    this.#factory = factory;
  }

  async get(id: string): Promise<T> {
    let item = this.#items.get(id);
    if (!item) {
      item = this.#factory(id);
      this.#items.set(id, item);
    }
    return item;
  }

  async save(item: T): Promise<void> {
    this.committedEvents.push(...item.getUncommittedEvents());
    item.clearUncommittedEvents();
  }
}
