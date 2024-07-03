import { DateTime, Interval } from "luxon";
import { Guid } from "../../../types/guid/guid.js";
import { Value } from "../odata.util.js";

abstract class ConstantValue<TValue> extends Value<TValue> {

  constructor() {
    super();
  }
}

class BooleanConstantValue extends ConstantValue<boolean> {

  private readonly value: boolean;

  constructor(value: boolean) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toString();
  }
}

class DateConstantValue extends ConstantValue<DateTime> {

  private readonly value: DateTime;

  constructor(value: DateTime) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toISODate()!;
  }
}

class DateTimeConstantValue extends ConstantValue<DateTime> {

  private readonly value: DateTime;

  constructor(value: DateTime) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toISO()!;
  }
}

class GuidConstantValue extends ConstantValue<Guid> {

  private readonly value: Guid;

  constructor(value: Guid) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value;
  }
}

class IntegerConstantValue extends ConstantValue<number> {

  private readonly value: number;

  constructor(value: number) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toString();
  }
}

class StringConstantValue extends ConstantValue<string> {

  private readonly value: string;

  constructor(value: string) {
    super();
    this.value = value;
  }

  override toString() {
    return `'${this.value.replace("'", "''")}'`;
  }
}

class TimeConstantValue extends ConstantValue<Interval> {

  private readonly value: Interval;

  constructor(value: Interval) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toISOTime()!;
  }
}


export const ɵConstant = {
  BooleanConstantValue,
  DateConstantValue,
  DateTimeConstantValue,
  GuidConstantValue,
  IntegerConstantValue,
  StringConstantValue,
  TimeConstantValue,
};
