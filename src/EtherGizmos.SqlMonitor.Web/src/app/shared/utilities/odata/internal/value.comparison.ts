import { Value } from "../odata.util";

abstract class ComparisonValue<TValue> implements Value<boolean> {

  protected readonly left: Value<TValue>;
  protected readonly comparator: string;
  protected readonly right: Value<TValue>;

  constructor(left: Value<TValue>, comparator: string, right: Value<TValue>) {
    this.left = left;
    this.comparator = comparator;
    this.right = right;
  }

  toString(): string {
    return `${this.left.toString()} ${this.comparator} ${this.right.toString()}`;
  }

  abstract _eval(data?: unknown): boolean;
}

class EqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'eq', right);
  }

  _eval(data?: unknown): boolean {
    return this.left._eval(data) === this.right._eval(data);
  }
}

class NotEqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'ne', right);
  }

  _eval(data?: unknown): boolean {
    return this.left._eval(data) !== this.right._eval(data);
  }
}

class GreaterThanComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'gt', right);
  }

  _eval(data?: unknown): boolean {
    return this.left._eval(data) > this.right._eval(data);
  }
}

class GreaterThanOrEqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'ge', right);
  }

  _eval(data?: unknown): boolean {
    return this.left._eval(data) >= this.right._eval(data);
  }
}

class LessThanComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'lt', right);
  }

  _eval(data?: unknown): boolean {
    return this.left._eval(data) < this.right._eval(data);
  }
}

class LessThanOrEqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'le', right);
  }

  _eval(data?: unknown): boolean {
    return this.left._eval(data) <= this.right._eval(data);
  }
}

export const ɵComparison = {
  EqualsComparisonValue,
  NotEqualsComparisonValue,
  GreaterThanComparisonValue,
  GreaterThanOrEqualsComparisonValue,
  LessThanComparisonValue,
  LessThanOrEqualsComparisonValue,
};
