import { Receipt } from "./receipt.js";

export class TransactionReceipt extends Receipt {
  readonly transactionId: string;

  constructor(aggregateId: string, version: number, transactionId: string) {
    super(aggregateId, version);
    this.transactionId = transactionId;
  }
}
