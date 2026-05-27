import { InvalidBankAccountError } from "../errors.js";

export class BankAccount {
  static readonly #BSB_REGEX = /^(?<bank>\d{3})-?(?<branch>\d{3})$/;
  static readonly #ACCOUNT_NUMBER_REGEX = /^\d{3,12}$/;

  readonly #brand!: undefined;
  readonly bsb: string;
  readonly accountNumber: string;

  private constructor(bsb: string, accountNumber: string) {
    this.bsb = bsb;
    this.accountNumber = accountNumber;
  }

  static create(bsb: string, accountNumber: string): BankAccount {
    if (!bsb?.trim())
      throw new InvalidBankAccountError("BSB is required", "bsb");
    if (!accountNumber?.trim())
      throw new InvalidBankAccountError(
        "Account number is required",
        "accountNumber",
      );

    const match = BankAccount.#BSB_REGEX.exec(bsb);
    if (!match?.groups)
      throw new InvalidBankAccountError("BSB is not valid", "bsb");
    if (!BankAccount.#ACCOUNT_NUMBER_REGEX.test(accountNumber))
      throw new InvalidBankAccountError(
        "Account number is not valid",
        "accountNumber",
      );

    return new BankAccount(
      `${match.groups["bank"]}-${match.groups["branch"]}`,
      accountNumber,
    );
  }
}
