import {
  BankAccount,
  CustomerContact,
  Duration,
  type Event,
  LoanBankAccountChangedEvent,
  LoanCreatedEvent,
  LoanCustomerContactChangedEvent,
  LoanDisbursedFundsEvent,
  LoanOverPaidEvent,
  LoanSettledEvent,
  PaymentTakenEvent,
} from "@cod/domain";

type EventFactory = (payload: Record<string, unknown>) => Event;

export const eventTypeRegistry = new Map<string, EventFactory>([
  [
    "LoanCreatedEvent",
    (p) =>
      new LoanCreatedEvent(
        new Date(p.createdOn as string),
        p.amount as number,
        Duration.create(
          (p.term as { length: number }).length,
          (p.term as { unit: string }).unit as "Day" | "Week" | "Month",
        ),
        p.paymentPlan as "Weekly" | "Fortnightly" | "Monthly",
      ),
  ],
  [
    "LoanDisbursedFundsEvent",
    (p) =>
      new LoanDisbursedFundsEvent(
        new Date(p.transactionDate as string),
        p.amount as number,
        BankAccount.create(
          (p.disbursedTo as { bsb: string }).bsb,
          (p.disbursedTo as { accountNumber: string }).accountNumber,
        ),
      ),
  ],
  [
    "PaymentTakenEvent",
    (p) =>
      new PaymentTakenEvent(
        p.transactionId as string,
        new Date(p.transactionDateTime as string),
        p.amount as number,
      ),
  ],
  [
    "LoanCustomerContactChangedEvent",
    (p) => {
      const cc = p.customerContact as {
        name: string;
        preferredPhoneNumber: string;
        alternatePhoneNumber?: string;
        postalAddress: string;
      };
      return new LoanCustomerContactChangedEvent(
        CustomerContact.create(
          cc.name,
          cc.preferredPhoneNumber,
          cc.alternatePhoneNumber,
          cc.postalAddress,
        ),
      );
    },
  ],
  [
    "LoanBankAccountChangedEvent",
    (p) => {
      const ba = p.bankAccount as { bsb: string; accountNumber: string };
      return new LoanBankAccountChangedEvent(
        BankAccount.create(ba.bsb, ba.accountNumber),
      );
    },
  ],
  [
    "LoanSettledEvent",
    (p) => new LoanSettledEvent(new Date(p.transactionDateTime as string)),
  ],
  [
    "LoanOverPaidEvent",
    (p) =>
      new LoanOverPaidEvent(
        new Date(p.transactionDateTime as string),
        p.amount as number,
      ),
  ],
]);
