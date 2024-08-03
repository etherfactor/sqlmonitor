import { Value } from "../odata.util";

abstract class OperatorValue implements Value<number> {

  protected readonly left: Value<number>;
  protected readonly operator: string;
  protected readonly right: Value<number>;

  constructor(left: Value<number>, operator: string, right: Value<number>) {
    this.left = left;
    this.operator = operator;
    this.right = right;
  }

  toString(): string {
    return `(${this.left.toString()} ${this.operator} ${this.right.toString()})`;
  }

  abstract _eval(data?: unknown): number;
}

class AddOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'add', right);
  }

  _eval(data?: unknown): number {
    const left = this.left._eval(data);
    const right = this.right._eval(data);
    return left + right;
  }
}

class SubtractOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'sub', right);
  }

  _eval(data?: unknown): number {
    const left = this.left._eval(data);
    const right = this.right._eval(data);
    return left - right;
  }
}

class MultiplyOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'mul', right);
  }

  _eval(data?: unknown): number {
    const left = this.left._eval(data);
    const right = this.right._eval(data);
    return left * right;
  }
}

class DivideOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'div', right);
  }

  _eval(data?: unknown): number {
    const left = this.left._eval(data);
    const right = this.right._eval(data);
    return left / right;
  }
}

class ModuloOperatorValue extends OperatorValue {

  constructor(left: Value<number>, right: Value<number>) {
    super(left, 'mod', right);
  }

  _eval(data?: unknown): number {
    const left = this.left._eval(data);
    const right = this.right._eval(data);
    return left % right;
  }
}

export const ɵOperator = {
  AddOperatorValue,
  SubtractOperatorValue,
  MultiplyOperatorValue,
  DivideOperatorValue,
  ModuloOperatorValue,
};
