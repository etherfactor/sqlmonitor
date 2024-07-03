import { Value } from "../odata.util";

abstract class LogicalValue extends Value<boolean> {

  private readonly operand: string;
  private readonly conditions: Value<boolean>[];

  constructor(operand: string, ...conditions: Value<boolean>[]) {
    super();
    this.operand = operand;
    this.conditions = conditions;
  }

  override toString(): string {
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

class NotLogicalValue extends Value<boolean> {
  private readonly condition: Value<boolean>;

  constructor(condition: Value<boolean>) {
    super();
    this.condition = condition;
  }

  override toString(): string {
    return `not ${this.condition.toString()}`;
  }
}

export const ɵLogical = {
  AndLogicalValue,
  OrLogicalValue,
  NotLogicalValue,
};
