import {
  CreateLoanCommandHandler,
  DisburseLoanFundsCommandHandler,
  TakePaymentCommandHandler,
} from "@cod/domain";
import { PostgresqlRepository } from "@cod/pg-data-adapter";
import { Server, withLogging } from "@cod/web-comms-adapter";

const connectionString =
  process.env["DATABASE_URL"] ??
  "postgresql://postgres:mysecretpassword@cod-database:5432/postgres";

const repository = new PostgresqlRepository(connectionString);

const createLoanHandler = withLogging(
  new CreateLoanCommandHandler(repository),
);
const disburseLoanFundsHandler = withLogging(
  new DisburseLoanFundsCommandHandler(repository),
);
const takePaymentHandler = withLogging(
  new TakePaymentCommandHandler(repository),
);

async function waitForDatabase(
  maxRetries = 10,
  delayMs = 2000,
): Promise<void> {
  for (let attempt = 1; attempt <= maxRetries; attempt++) {
    if (await repository.healthCheck()) {
      console.log(`Database ready (attempt ${attempt})`);
      return;
    }
    console.log(
      `Database not ready (attempt ${attempt}/${maxRetries}), retrying in ${delayMs}ms...`,
    );
    await new Promise((resolve) => setTimeout(resolve, delayMs));
  }
  throw new Error("Database not available after maximum retries");
}

async function main() {
  await waitForDatabase();

  const server = new Server(
    () => repository.healthCheck(),
    createLoanHandler,
    disburseLoanFundsHandler,
    takePaymentHandler,
  );
  server.serve();

  async function shutdown() {
    console.log("Shutting down...");
    await server.close();
    await repository.close();
    console.log("Shut down.");
    process.exit(0);
  }

  process.on("SIGTERM", shutdown);
  process.on("SIGINT", shutdown);
}

main().catch((err) => {
  console.error("Failed to start:", err);
  process.exit(1);
});
