import { DateTime, Interval } from "luxon";
import { Guid } from "../../../types/guid/guid";
import { Value } from "../odata.util";

abstract class ConstantValue<TValue> implements Value<TValue> {

  constructor() {
  }

  abstract _eval(): TValue;
}

class BooleanConstantValue extends ConstantValue<boolean> {

  protected readonly value: boolean;

  constructor(value: boolean) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toString();
  }

  override _eval(): boolean {
    return this.value;
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

  override _eval(): DateTime {
    return this.value;
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

  override _eval(): DateTime {
    return this.value;
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

  override _eval(): Guid {
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

  override _eval(): number {
    return this.value;
  }
}

class NullConstantValue extends ConstantValue<any> {

  override toString() {
    return 'null';
  }

  override _eval(): any {
    return null;
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

  override _eval(): string {
    return this.value;
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

  override _eval(): Interval {
    return this.value;
  }
}


export const ɵConstant = {
  BooleanConstantValue,
  DateConstantValue,
  DateTimeConstantValue,
  GuidConstantValue,
  IntegerConstantValue,
  NullConstantValue,
  StringConstantValue,
  TimeConstantValue,
};
