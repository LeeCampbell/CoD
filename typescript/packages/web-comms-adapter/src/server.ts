import * as http from "node:http";
import {
  type CreateLoanCommand,
  type DisburseLoanFundsCommand,
  type Handler,
  type Receipt,
  type TakePaymentCommand,
  type TransactionReceipt,
  InvalidBankAccountError,
  InvalidCommandError,
  InvalidCustomerContactError,
  InvalidDurationError,
  InvalidPaymentError,
  LoanAlreadyCreatedError,
  UnsupportedLoanAmountError,
  UnsupportedLoanTermError,
  FundsAlreadyDisbursedError,
} from "@cod/domain";
import { Router, json, text, jsonBody } from "./router.js";
import { toCreateLoanCommand } from "./create-loan-model.js";
import type { CreateLoanModelInput } from "./create-loan-model.js";
import type { LoanCreatedModel } from "./loan-created-model.js";
import { toDisburseLoanFundsCommand } from "./disburse-loan-model.js";
import type { DisbursedModel } from "./disbursed-model.js";
import { toTakePaymentCommand } from "./loan-payment-model.js";
import type { LoanPaymentModelInput } from "./loan-payment-model.js";
import type { PaymentTakenModel } from "./payment-taken-model.js";

const DOMAIN_ERRORS = [
  InvalidBankAccountError,
  InvalidCommandError,
  InvalidCustomerContactError,
  InvalidDurationError,
  InvalidPaymentError,
  LoanAlreadyCreatedError,
  UnsupportedLoanAmountError,
  UnsupportedLoanTermError,
  FundsAlreadyDisbursedError,
];

export type HealthCheck = () => Promise<boolean>;

export class Server {
  readonly #router: Router;
  readonly #port: number;
  #server?: http.Server;

  constructor(
    healthCheck: HealthCheck,
    createLoanHandler: Handler<CreateLoanCommand, Receipt>,
    disburseLoanFundsHandler: Handler<DisburseLoanFundsCommand, Receipt>,
    takePaymentHandler: Handler<TakePaymentCommand, TransactionReceipt>,
    port = 4567,
  ) {
    this.#port = port;
    this.#router = new Router();

    this.#router.get("/", () => text(200, "YOW 2017 - Cost Of a Dependency"));

    this.#router.get("/health", async () => {
      const healthy = await healthCheck();
      return text(healthy ? 200 : 503, healthy ? "Healthy" : "Unhealthy");
    });

    this.#router.post("/Loan", async (req) => {
      const model = await jsonBody<CreateLoanModelInput>(req);
      const command = toCreateLoanCommand(model);
      const receipt = await createLoanHandler.handle(command);
      return json<LoanCreatedModel>(200, { loanId: receipt.aggregateId });
    });

    this.#router.post("/Loan/:id/disburse", async (req) => {
      const command = toDisburseLoanFundsCommand(req.params.id);
      const receipt = await disburseLoanFundsHandler.handle(command);
      return json<DisbursedModel>(200, {
        aggregateId: receipt.aggregateId,
        version: receipt.version,
      });
    });

    this.#router.post("/Loan/:id", async (req) => {
      const model = await jsonBody<LoanPaymentModelInput>(req);
      const command = toTakePaymentCommand(req.params.id, model);
      const receipt = await takePaymentHandler.handle(command);
      return json<PaymentTakenModel>(200, {
        transactionId: receipt.transactionId,
      });
    });
  }

  serve(): void {
    this.#server = http.createServer(async (req, res) => {
      try {
        await this.#router.handle(req, res);
      } catch (e) {
        console.error("Request error:", e);
        if (isDomainError(e)) {
          res.writeHead(400, { "Content-Type": "application/json" });
          res.end(JSON.stringify({ error: (e as Error).message }));
        } else {
          res.writeHead(500, { "Content-Type": "application/json" });
          res.end(JSON.stringify({ error: "Internal Server Error" }));
        }
      }
    });

    this.#server.listen(this.#port, () => {
      console.log(`Server listening on port ${this.#port}`);
    });
  }

  close(): Promise<void> {
    return new Promise((resolve, reject) => {
      if (!this.#server) {
        resolve();
        return;
      }
      this.#server.close((err) => (err ? reject(err) : resolve()));
    });
  }
}

function isDomainError(e: unknown): boolean {
  return DOMAIN_ERRORS.some((errorClass) => e instanceof errorClass);
}
