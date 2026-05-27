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
import { toCreateLoanCommand } from "./create-loan-model.js";
import type { LoanCreatedModel } from "./loan-created-model.js";
import { toDisburseLoanFundsCommand } from "./disburse-loan-model.js";
import type { DisbursedModel } from "./disbursed-model.js";
import { toTakePaymentCommand } from "./loan-payment-model.js";
import type { PaymentTakenModel } from "./payment-taken-model.js";

const MAX_BODY_SIZE = 1024 * 1024; // 1 MB

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
  readonly #createLoanHandler: Handler<CreateLoanCommand, Receipt>;
  readonly #disburseLoanFundsHandler: Handler<DisburseLoanFundsCommand, Receipt>;
  readonly #takePaymentHandler: Handler<TakePaymentCommand, TransactionReceipt>;
  readonly #healthCheck: HealthCheck;
  readonly #port: number;
  #server?: http.Server;

  constructor(
    healthCheck: HealthCheck,
    createLoanHandler: Handler<CreateLoanCommand, Receipt>,
    disburseLoanFundsHandler: Handler<DisburseLoanFundsCommand, Receipt>,
    takePaymentHandler: Handler<TakePaymentCommand, TransactionReceipt>,
    port = 4567,
  ) {
    this.#healthCheck = healthCheck;
    this.#createLoanHandler = createLoanHandler;
    this.#disburseLoanFundsHandler = disburseLoanFundsHandler;
    this.#takePaymentHandler = takePaymentHandler;
    this.#port = port;
  }

  serve(): void {
    this.#server = http.createServer(async (req, res) => {
      try {
        const url = new URL(req.url ?? "/", `http://${req.headers.host}`);
        const method = req.method ?? "GET";

        if (method === "GET" && url.pathname === "/") {
          res.writeHead(200, { "Content-Type": "text/plain" });
          res.end("YOW 2017 - Cost Of a Dependency");
          return;
        }

        if (method === "GET" && url.pathname === "/health") {
          const healthy = await this.#healthCheck();
          const status = healthy ? 200 : 503;
          res.writeHead(status, { "Content-Type": "text/plain" });
          res.end(healthy ? "Healthy" : "Unhealthy");
          return;
        }

        if (method === "POST" && url.pathname === "/Loan") {
          const body = await readBody(req);
          const model = JSON.parse(body);
          const command = toCreateLoanCommand(model);
          const receipt = await this.#createLoanHandler.handle(command);
          const response: LoanCreatedModel = {
            loanId: receipt.aggregateId,
          };
          res.writeHead(200, { "Content-Type": "application/json" });
          res.end(JSON.stringify(response));
          return;
        }

        const disburseMatch = url.pathname.match(
          /^\/Loan\/([^/]+)\/disburse$/,
        );
        if (method === "POST" && disburseMatch) {
          const loanId = disburseMatch[1];
          const command = toDisburseLoanFundsCommand(loanId);
          const receipt =
            await this.#disburseLoanFundsHandler.handle(command);
          const response: DisbursedModel = {
            aggregateId: receipt.aggregateId,
            version: receipt.version,
          };
          res.writeHead(200, { "Content-Type": "application/json" });
          res.end(JSON.stringify(response));
          return;
        }

        const paymentMatch = url.pathname.match(/^\/Loan\/([^/]+)$/);
        if (method === "POST" && paymentMatch) {
          const loanId = paymentMatch[1];
          const body = await readBody(req);
          const model = JSON.parse(body);
          const command = toTakePaymentCommand(loanId, model);
          const receipt = await this.#takePaymentHandler.handle(command);
          const response: PaymentTakenModel = {
            transactionId: receipt.transactionId,
          };
          res.writeHead(200, { "Content-Type": "application/json" });
          res.end(JSON.stringify(response));
          return;
        }

        res.writeHead(404, { "Content-Type": "text/plain" });
        res.end("Not Found");
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

function readBody(req: http.IncomingMessage): Promise<string> {
  return new Promise((resolve, reject) => {
    const chunks: Buffer[] = [];
    let size = 0;
    req.on("data", (chunk: Buffer) => {
      size += chunk.length;
      if (size > MAX_BODY_SIZE) {
        req.destroy();
        reject(new Error("Request body too large"));
        return;
      }
      chunks.push(chunk);
    });
    req.on("end", () => resolve(Buffer.concat(chunks).toString()));
    req.on("error", reject);
  });
}
