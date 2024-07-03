import { Value } from "../odata.util";

abstract class ComparisonValue<TValue> extends Value<boolean> {

  private readonly left: Value<TValue>;
  private readonly comparator: string;
  private readonly right: Value<TValue>;

  constructor(left: Value<TValue>, comparator: string, right: Value<TValue>) {
    super();
    this.left = left;
    this.comparator = comparator;
    this.right = right;
  }

  override toString(): string {
    return `${this.left.toString()} ${this.comparator} ${this.right.toString()}`;
  }
}

class EqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'eq', right);
  }
}

class NotEqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'ne', right);
  }
}

class GreaterThanComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'gt', right);
  }
}

class GreaterThanOrEqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'ge', right);
  }
}

class LessThanComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'lt', right);
  }
}

class LessThanOrEqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'le', right);
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
