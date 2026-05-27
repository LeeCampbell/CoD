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
  process.exit(0);
}

process.on("SIGTERM", shutdown);
process.on("SIGINT", shutdown);
