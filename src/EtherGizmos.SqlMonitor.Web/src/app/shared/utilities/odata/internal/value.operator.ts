import { Value } from "../odata.util";

abstract class OperatorValue implements Value<number> {

  private readonly left: Value<number>;
  private readonly operator: string;
  private readonly right: Value<number>;

  constructor(left: Value<number>, operator: string, right: Value<number>) {
    this.left = left;
    this.operator = operator;
    this.right = right;
  }

  toString(): string {
    return `(${this.left.toString()} ${this.operator} ${this.right.toString()})`;
  }
}

class AddOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'add', right);
  }
}

class SubtractOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'sub', right);
  }
}

class MultiplyOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'mul', right);
  }
}

class DivideOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'div', right);
  }
}

class ModuloOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'mod', right);
  }
}

export const ɵOperator = {
  AddOperatorValue,
  SubtractOperatorValue,
  MultiplyOperatorValue,
  DivideOperatorValue,
  ModuloOperatorValue,
};
