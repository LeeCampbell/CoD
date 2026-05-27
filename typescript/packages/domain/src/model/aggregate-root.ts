import { Event } from "../contracts/event.js";

export abstract class AggregateRoot {
  readonly id: string;
  #uncommittedEvents: Event[] = [];
  #handlers = new Map<string, (event: Event) => void>();
  #version = 0;

  protected constructor(id: string) {
    this.id = id;
  }

  get version(): number {
    return this.#version;
  }

  applyEvent(event: Event): void {
    const handler = this.#handlers.get(event.eventType);
    if (!handler) {
      throw new Error(
        `No handler registered for event type '${event.eventType}'`,
      );
    }
    handler(event);
    this.#version++;
  }

  getUncommittedEvents(): Event[] {
    return [...this.#uncommittedEvents];
  }

  clearUncommittedEvents(): void {
    this.#uncommittedEvents = [];
  }

  protected addEvent(event: Event): void {
    this.applyEvent(event);
    this.#uncommittedEvents.push(event);
  }

  protected registerHandler<T extends Event>(
    eventType: string,
    handler: (event: T) => void,
  ): void {
    this.#handlers.set(eventType, handler as (event: Event) => void);
  }
}
