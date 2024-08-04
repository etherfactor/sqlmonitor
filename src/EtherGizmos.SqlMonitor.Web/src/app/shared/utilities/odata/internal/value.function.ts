import { Value } from "../odata.util";

abstract class FunctionValue<TValue> implements Value<TValue> {

  protected readonly name: string;
  protected readonly args: Value<unknown>[];

  constructor(name: string, ...args: Value<unknown>[]) {
    this.name = name;
    this.args = args;
  }

  toString(): string {
    return `${this.name}(${this.args.map(item => item.toString()).join(', ')})`;
  }

  abstract _eval(data?: unknown): TValue;
}

class ContainsFunctionValue extends FunctionValue<boolean> {

  constructor(string: Value<string>, contains: Value<string>) {
    super('contains', string, contains);
  }

  override _eval(data?: unknown): boolean {
    const root = (this.args[0] as Value<string>)._eval(data).toLowerCase();
    const arg1 = (this.args[1] as Value<string>)._eval(data).toLowerCase();
    return root.includes(arg1);
  }
}

class StartsWithFunctionValue extends FunctionValue<boolean> {

  constructor(string: Value<string>, startsWith: Value<string>) {
    super('startswith', string, startsWith);
  }

  override _eval(data?: unknown): boolean {
    const root = (this.args[0] as Value<string>)._eval(data).toLowerCase();
    const arg1 = (this.args[1] as Value<string>)._eval(data).toLowerCase();
    return root.startsWith(arg1);
  }
}

class EndsWithFunctionValue extends FunctionValue<boolean> {

  constructor(string: Value<string>, endsWith: Value<string>) {
    super('endswith', string, endsWith);
  }

  override _eval(data?: unknown): boolean {
    const root = (this.args[0] as Value<string>)._eval(data).toLowerCase();
    const arg1 = (this.args[1] as Value<string>)._eval(data).toLowerCase();
    return root.endsWith(arg1);
  }
}

class ConcatFunctionValue extends FunctionValue<string> {

  constructor(left: Value<string>, right: Value<string>) {
    super('concat', left, right);
  }

  override _eval(data?: unknown): string {
    const root = (this.args[0] as Value<string>)._eval(data);
    const arg1 = (this.args[1] as Value<string>)._eval(data);
    return root.concat(arg1);
  }
}

class IndexOfFunctionValue extends FunctionValue<number> {

  constructor(string: Value<string>, indexOf: Value<string>) {
    super('indexof', string, indexOf);
  }

  override _eval(data?: unknown): number {
    const root = (this.args[0] as Value<string>)._eval(data).toLowerCase();
    const arg1 = (this.args[1] as Value<string>)._eval(data).toLowerCase();
    return root.indexOf(arg1);
  }
}

class LengthFunctionValue extends FunctionValue<number> {

  constructor(string: Value<string>) {
    super('length', string);
  }

  override _eval(data?: unknown): number {
    const root = (this.args[0] as Value<string>)._eval(data);
    return root.length;
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

  override _eval(data?: unknown): string {
    const root = (this.args[0] as Value<string>)._eval(data);
    const arg1 = (this.args[1] as Value<number>)._eval(data);
    const arg2 = (this.args[2] as Value<number>)._eval(data);
    return root.substring(arg1, arg2);
  }
}

class ToLowerFunctionValue extends FunctionValue<string> {

  constructor(value: Value<string>) {
    super('tolower', value);
  }

  override _eval(data?: unknown): string {
    const root = (this.args[0] as Value<string>)._eval(data);
    return root.toLowerCase();
  }
}

class ToUpperFunctionValue extends FunctionValue<string> {

  constructor(value: Value<string>) {
    super('toupper', value);
  }

  override _eval(data?: unknown): string {
    const root = (this.args[0] as Value<string>)._eval(data);
    return root.toUpperCase();
  }
}

class TrimFunctionValue extends FunctionValue<string> {

  constructor(value: Value<string>) {
    super('trim', value);
  }

  override _eval(data?: unknown): string {
    const root = (this.args[0] as Value<string>)._eval(data);
    return root.trim();
  }
}

class CeilingFunctionValue extends FunctionValue<number> {

  constructor(value: Value<number>) {
    super('ceiling', value);
  }

  override _eval(data?: unknown): number {
    const root = (this.args[0] as Value<number>)._eval(data);
    return Math.ceil(root);
  }
}

class FloorFunctionValue extends FunctionValue<number> {

  constructor(value: Value<number>) {
    super('floor', value);
  }

  override _eval(data?: unknown): number {
    const root = (this.args[0] as Value<number>)._eval(data);
    return Math.floor(root);
  }
}

class RoundFunctionValue extends FunctionValue<number> {

  constructor(value: Value<number>) {
    super('round', value);
  }

  override _eval(data?: unknown): number {
    const root = (this.args[0] as Value<number>)._eval(data);
    return Math.round(root);
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
