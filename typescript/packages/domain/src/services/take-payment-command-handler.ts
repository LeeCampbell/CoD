import { TakePaymentCommand } from "../contracts/take-payment-command.js";
import { TransactionReceipt } from "../contracts/transaction-receipt.js";
import { PaymentTakenEvent } from "../contracts/payment-taken-event.js";
import { Loan } from "../model/loan.js";
import { Handler } from "./handler.js";
import { Repository } from "./repository.js";

export class TakePaymentCommandHandler
  implements Handler<TakePaymentCommand, TransactionReceipt>
{
  readonly #repository: Repository<Loan>;

  constructor(repository: Repository<Loan>) {
    this.#repository = repository;
  }

  async handle(command: TakePaymentCommand): Promise<TransactionReceipt> {
    const loan = await this.#repository.get(command.aggregateId);
    loan.takePayment(command);

    const paymentEvent = loan
      .getUncommittedEvents()
      .find(
        (e): e is PaymentTakenEvent => e.eventType === "PaymentTakenEvent",
      );
    if (!paymentEvent) {
      throw new Error("PaymentTakenEvent not found after takePayment");
    }
    const receipt = new TransactionReceipt(
      loan.id,
      loan.version,
      paymentEvent.transactionId,
    );

    await this.#repository.save(loan);
    return receipt;
  }
}
