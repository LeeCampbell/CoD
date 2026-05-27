import { DisburseLoanFundsCommand } from "../contracts/disburse-loan-funds-command.js";
import { Receipt } from "../contracts/receipt.js";
import { Loan } from "../model/loan.js";
import { Handler } from "./handler.js";
import { Repository } from "./repository.js";

export class DisburseLoanFundsCommandHandler
  implements Handler<DisburseLoanFundsCommand, Receipt>
{
  readonly #repository: Repository<Loan>;

  constructor(repository: Repository<Loan>) {
    this.#repository = repository;
  }

  async handle(command: DisburseLoanFundsCommand): Promise<Receipt> {
    const loan = await this.#repository.get(command.aggregateId);
    loan.disburseFunds(command);
    await this.#repository.save(loan);
    return new Receipt(loan.id, loan.version);
  }
}
