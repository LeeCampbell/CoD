import type { Pool } from "pg";
import type { Event } from "@cod/domain";
import { eventTypeRegistry } from "./event-type-registry.js";

const SELECT_SQL =
  "SELECT event_type, payload FROM event_store.events " +
  "WHERE stream_type = $1 AND stream_id = $2 ORDER BY version ASC";

export class EventReader {
  readonly #pool: Pool;

  constructor(pool: Pool) {
    this.#pool = pool;
  }

  async readStreamInstance(
    streamType: string,
    streamId: string,
  ): Promise<Event[]> {
    const result = await this.#pool.query(SELECT_SQL, [streamType, streamId]);
    return result.rows.map((row) => {
      const factory = eventTypeRegistry.get(row.event_type);
      if (!factory) {
        throw new Error(`Unknown event type: ${row.event_type}`);
      }
      return factory(row.payload);
    });
  }
}
