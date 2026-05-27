import { CustomerContact } from "./customer-contact.js";
import { Event } from "./event.js";

export class LoanCustomerContactChangedEvent extends Event {
  readonly eventType = "LoanCustomerContactChangedEvent" as const;
  readonly customerContact: CustomerContact;

  constructor(customerContact: CustomerContact) {
    super();
    this.customerContact = customerContact;
  }
}
