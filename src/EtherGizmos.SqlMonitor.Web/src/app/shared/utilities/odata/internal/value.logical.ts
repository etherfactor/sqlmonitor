import { Value } from "../odata.util";

abstract class LogicalValue implements Value<boolean> {

  protected readonly operand: string;
  protected readonly conditions: Value<boolean>[];

  constructor(operand: string, ...conditions: Value<boolean>[]) {
    this.operand = operand;
    this.conditions = conditions;
  }

  toString(): string {
    return `(${this.conditions.map(item => item.toString()).join(` ${this.operand} `)})`;
  }

  abstract _eval(data?: unknown): boolean;
}

class AndLogicalValue extends LogicalValue {

  constructor(...conditions: Value<boolean>[]) {
    super('and', ...conditions);
  }

  override _eval(data?: unknown): boolean {
    const result = this.conditions.reduce((prev, curr) => prev && curr._eval(data), true);
    return result;
  }
}

class OrLogicalValue extends LogicalValue {

  constructor(...conditions: Value<boolean>[]) {
    super('or', ...conditions);
  }

  override _eval(data?: unknown): boolean {
    const result = this.conditions.reduce((prev, curr) => prev || curr._eval(data), false);
    return result;
  }
}

class NotLogicalValue implements Value<boolean> {
  private readonly condition: Value<boolean>;

  constructor(condition: Value<boolean>) {
    this.condition = condition;
  }

  toString(): string {
    return `not ${this.condition.toString()}`;
  }

  _eval(data?: unknown): boolean {
    const result = !this.condition._eval(data);
    return result;
  }
}

export const ɵLogical = {
  AndLogicalValue,
  OrLogicalValue,
  NotLogicalValue,
};
