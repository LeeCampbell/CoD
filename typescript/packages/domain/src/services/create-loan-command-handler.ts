import { CreateLoanCommand } from "../contracts/create-loan-command.js";
import { Receipt } from "../contracts/receipt.js";
import { Loan } from "../model/loan.js";
import { Handler } from "./handler.js";
import { Repository } from "./repository.js";

export class CreateLoanCommandHandler
  implements Handler<CreateLoanCommand, Receipt>
{
  readonly #repository: Repository<Loan>;

  constructor(repository: Repository<Loan>) {
    this.#repository = repository;
  }

  async handle(command: CreateLoanCommand): Promise<Receipt> {
    const loan = await this.#repository.get(command.aggregateId);
    loan.create(command);
    await this.#repository.save(loan);
    return new Receipt(loan.id, loan.version);
  }
}
