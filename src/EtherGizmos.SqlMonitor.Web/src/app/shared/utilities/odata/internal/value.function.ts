import { Value } from "../odata.util";

abstract class FunctionValue<TValue> implements Value<TValue> {

  private readonly name: string;
  private readonly args: Value<unknown>[];

  constructor(name: string, ...args: Value<unknown>[]) {
    this.name = name;
    this.args = args;
  }

  toString(): string {
    return `${this.name}(${this.args.map(item => item.toString()).join(', ')})`;
  }
}

class ContainsFunctionValue extends FunctionValue<boolean> {

  constructor(string: Value<string>, contains: Value<string>) {
    super('contains', string, contains);
  }
}

class StartsWithFunctionValue extends FunctionValue<boolean> {

  constructor(string: Value<string>, startsWith: Value<string>) {
    super('startswith', string, startsWith);
  }
}

class EndsWithFunctionValue extends FunctionValue<boolean> {

  constructor(string: Value<string>, endsWith: Value<string>) {
    super('endswith', string, endsWith);
  }
}

class ConcatFunctionValue extends FunctionValue<string> {

  constructor(left: Value<string>, right: Value<string>) {
    super('concat', left, right);
  }
}

class IndexOfFunctionValue extends FunctionValue<number> {

  constructor(string: Value<string>, indexOf: Value<string>) {
    super('indexof', string, indexOf);
  }
}

class LengthFunctionValue extends FunctionValue<number> {

  constructor(string: Value<string>) {
    super('length', string);
  }
}

class SubstringFunctionValue extends FunctionValue<string> {

  constructor(value: Value<string>, start: Value<number>, finish?: Value<number>) {
    if (finish) {
      super('substring', value, start, finish);
    } else {
      super('substring', value, start);
    }
  }
}

class ToLowerFunctionValue extends FunctionValue<string> {

  constructor(value: Value<string>) {
    super('tolower', value);
  }
}

class ToUpperFunctionValue extends FunctionValue<string> {

  constructor(value: Value<string>) {
    super('toupper', value);
  }
}

class TrimFunctionValue extends FunctionValue<string> {

  constructor(value: Value<string>) {
    super('trim', value);
  }
}

class CeilingFunctionValue extends FunctionValue<number> {

  constructor(value: Value<number>) {
    super('ceiling', value);
  }
}

class FloorFunctionValue extends FunctionValue<number> {

  constructor(value: Value<number>) {
    super('floor', value);
  }
}

class RoundFunctionValue extends FunctionValue<number> {

  constructor(value: Value<number>) {
    super('round', value);
  }
}

export const ɵFunction = {
  ContainsFunctionValue,
  StartsWithFunctionValue,
  EndsWithFunctionValue,
  ConcatFunctionValue,
  IndexOfFunctionValue,
  LengthFunctionValue,
  SubstringFunctionValue,
  ToLowerFunctionValue,
  ToUpperFunctionValue,
  TrimFunctionValue,
  CeilingFunctionValue,
  FloorFunctionValue,
  RoundFunctionValue,
};
