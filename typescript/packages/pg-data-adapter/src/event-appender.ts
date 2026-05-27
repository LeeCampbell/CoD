import type { Pool } from "pg";
import type { Event } from "@cod/domain";

const INSERT_SQL =
  "INSERT INTO event_store.events " +
  "(stream_type, stream_id, version, event_type, payload) " +
  "VALUES ($1, $2, $3, $4, $5)";

const SELECT_MAX_VERSION =
  "SELECT coalesce(MAX(version), 0) FROM event_store.events " +
  "WHERE stream_type = $1 AND stream_id = $2";

export class WrongExpectedVersionError extends Error {
  constructor(
    streamType: string,
    streamId: string,
    currentVersion: number,
    expectedVersion: number,
  ) {
    super(
      `Wrong expected version for ${streamType}/${streamId}. ` +
        `Current: ${currentVersion}, Expected: ${expectedVersion}`,
    );
    this.name = "WrongExpectedVersionError";
  }
}

export class EventAppender {
  readonly #pool: Pool;

  constructor(pool: Pool) {
    this.#pool = pool;
  }

  async append(
    streamType: string,
    streamId: string,
    expectedVersion: number,
    events: Event[],
  ): Promise<void> {
    const client = await this.#pool.connect();
    try {
      await client.query("BEGIN");

      const versionResult = await client.query(SELECT_MAX_VERSION, [
        streamType,
        streamId,
      ]);
      const currentVersion = versionResult.rows[0].coalesce as number;
      if (currentVersion !== expectedVersion) {
        throw new WrongExpectedVersionError(
          streamType,
          streamId,
          currentVersion,
          expectedVersion,
        );
      }

      let version = expectedVersion + 1;
      for (const event of events) {
        await client.query(INSERT_SQL, [
          streamType,
          streamId,
          version,
          event.eventType,
          JSON.stringify(event),
        ]);
        version++;
      }

      await client.query("COMMIT");
    } catch (e) {
      await client.query("ROLLBACK");
      throw e;
    } finally {
      client.release();
    }
  }
}
