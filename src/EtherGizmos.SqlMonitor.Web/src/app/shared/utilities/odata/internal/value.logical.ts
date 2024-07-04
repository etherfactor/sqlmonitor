import { Value } from "../odata.util";

abstract class LogicalValue implements Value<boolean> {

  private readonly operand: string;
  private readonly conditions: Value<boolean>[];

  constructor(operand: string, ...conditions: Value<boolean>[]) {
    this.operand = operand;
    this.conditions = conditions;
  }

  toString(): string {
    return `(${this.conditions.map(item => item.toString()).join(` ${this.operand} `)})`;
  }
}

class AndLogicalValue extends LogicalValue {

  constructor(...conditions: Value<boolean>[]) {
    super('and', ...conditions);
  }
}

class OrLogicalValue extends LogicalValue {

  constructor(...conditions: Value<boolean>[]) {
    super('or', ...conditions);
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
}

export const ɵLogical = {
  AndLogicalValue,
  OrLogicalValue,
  NotLogicalValue,
};
