import { InvalidCustomerContactError } from "../errors.js";

export class CustomerContact {
  static readonly #PHONE_REGEX = /^0\d{9}$/;

  readonly #brand!: undefined;
  readonly name: string;
  readonly preferredPhoneNumber: string;
  readonly alternatePhoneNumber: string | undefined;
  readonly postalAddress: string;

  private constructor(
    name: string,
    preferredPhoneNumber: string,
    alternatePhoneNumber: string | undefined,
    postalAddress: string,
  ) {
    this.name = name;
    this.preferredPhoneNumber = preferredPhoneNumber;
    this.alternatePhoneNumber = alternatePhoneNumber;
    this.postalAddress = postalAddress;
  }

  static create(
    name: string,
    preferredPhoneNumber: string,
    alternatePhoneNumber: string | undefined,
    postalAddress: string,
  ): CustomerContact {
    if (!name?.trim())
      throw new InvalidCustomerContactError("Name is required", "name");
    if (!preferredPhoneNumber?.trim())
      throw new InvalidCustomerContactError(
        "Preferred phone number is required",
        "preferredPhoneNumber",
      );
    if (!CustomerContact.#PHONE_REGEX.test(preferredPhoneNumber))
      throw new InvalidCustomerContactError(
        "Preferred phone number is not valid",
        "preferredPhoneNumber",
      );
    if (
      alternatePhoneNumber !== undefined &&
      !CustomerContact.#PHONE_REGEX.test(alternatePhoneNumber)
    )
      throw new InvalidCustomerContactError(
        "Alternate phone number is not valid",
        "alternatePhoneNumber",
      );
    if (!postalAddress?.trim())
      throw new InvalidCustomerContactError(
        "Postal address is required",
        "postalAddress",
      );

    return new CustomerContact(
      name,
      preferredPhoneNumber,
      alternatePhoneNumber,
      postalAddress,
    );
  }
}
