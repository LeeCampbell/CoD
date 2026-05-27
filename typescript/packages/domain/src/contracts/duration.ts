import { InvalidDurationError } from "../errors.js";
import { DurationUnit } from "./duration-unit.js";

export class Duration {
  readonly #brand!: undefined;
  readonly length: number;
  readonly unit: DurationUnit;

  private constructor(length: number, unit: DurationUnit) {
    this.length = length;
    this.unit = unit;
  }

  static create(length: number, unit: DurationUnit): Duration {
    if (!Number.isInteger(length) || length <= 0)
      throw new InvalidDurationError("Duration length must be a positive integer");
    const validUnits: string[] = Object.values(DurationUnit);
    if (!validUnits.includes(unit))
      throw new InvalidDurationError("Duration unit is not valid");

    return new Duration(length, unit);
  }

  toString(): string {
    return `${this.length} ${this.unit}(s)`;
  }
}
