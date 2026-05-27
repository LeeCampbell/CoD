export class Receipt {
  readonly #brand!: undefined;
  readonly aggregateId: string;
  readonly version: number;

  constructor(aggregateId: string, version: number) {
    this.aggregateId = aggregateId;
    this.version = version;
  }
}
