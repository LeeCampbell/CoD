import { Pool } from "pg";
import { Loan, type Repository } from "@cod/domain";
import { EventReader } from "./event-reader.js";
import { EventAppender } from "./event-appender.js";

export class PostgresqlRepository implements Repository<Loan> {
  readonly #pool: Pool;
  readonly #reader: EventReader;
  readonly #writer: EventAppender;

  constructor(connectionString: string) {
    this.#pool = new Pool({ connectionString });
    this.#reader = new EventReader(this.#pool);
    this.#writer = new EventAppender(this.#pool);
  }

  async get(id: string): Promise<Loan> {
    const events = await this.#reader.readStreamInstance("loan", id);
    const loan = new Loan(id);
    for (const event of events) {
      loan.applyEvent(event);
    }
    return loan;
  }

  async save(loan: Loan): Promise<void> {
    const uncommittedEvents = loan.getUncommittedEvents();
    const expectedVersion = loan.version - uncommittedEvents.length;
    await this.#writer.append(
      "loan",
      loan.id,
      expectedVersion,
      uncommittedEvents,
    );
    loan.clearUncommittedEvents();
  }

  async healthCheck(): Promise<boolean> {
    try {
      await this.#pool.query("SELECT 1");
      return true;
    } catch {
      return false;
    }
  }

  async close(): Promise<void> {
    await this.#pool.end();
  }
}
