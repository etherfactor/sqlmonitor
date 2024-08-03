import { Value } from "../odata.util";

abstract class CollectionValue implements Value<boolean> {

  protected readonly property: string;
  protected readonly operand: string;
  protected readonly path: string;
  protected readonly value: Value<boolean>;

  constructor(property: string, operand: string, path: string, value: Value<boolean>) {
    this.property = property;
    this.operand = operand;
    this.path = path;
    this.value = value;
  }

  toString(): string {
    return `${this.property}/${this.operand}(${this.path}: ${this.value.toString()})`;
  }

  abstract _eval(data?: unknown): boolean;
}

class AllCollectionValue extends CollectionValue {

  constructor(property: string, path: string, value: Value<boolean>) {
    super(property, 'all', path, value);
  }

  _eval(data?: unknown): boolean {
    if (!data)
      return false;

    if (typeof data !== 'object')
      return false;

    const array = data[this.property as keyof typeof data] as object[];
    const result = array.reduce((prev, curr) => prev && this.value._eval(curr), true);

    return result;
  }
}

class AnyCollectionValue extends CollectionValue {

  constructor(property: string, path: string, value: Value<boolean>) {
    super(property, 'any', path, value);
  }

  _eval(data?: unknown): boolean {
    if (!data)
      return false;

    if (typeof data !== 'object')
      return false;

    const array = data[this.property as keyof typeof data] as object[];
    const result = array.reduce((prev, curr) => prev || this.value._eval(curr), false);

    return result;
  }
}

export const ɵCollection = {
  AllCollectionValue,
  AnyCollectionValue,
};
