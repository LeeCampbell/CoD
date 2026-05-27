import { AggregateRoot } from "../model/aggregate-root.js";

export interface Repository<T extends AggregateRoot> {
  get(id: string): Promise<T>;
  save(item: T): Promise<void>;
}
