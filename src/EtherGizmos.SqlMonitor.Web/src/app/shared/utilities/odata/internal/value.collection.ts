import { Value } from "../odata.util";

abstract class CollectionValue extends Value<boolean> {

  private readonly property: string;
  private readonly operand: string;
  private readonly path: string;
  private readonly value: Value<boolean>;

  constructor(property: string, operand: string, path: string, value: Value<boolean>) {
    super();
    this.property = property;
    this.operand = operand;
    this.path = path;
    this.value = value;
  }

  override toString(): string {
    return `${this.property}/${this.operand}(${this.path}: ${this.value.toString()})`;
  }
}

class AllCollectionValue extends CollectionValue {

  constructor(property: string, path: string, value: Value<boolean>) {
    super(property, 'all', path, value);
  }
}

class AnyCollectionValue extends CollectionValue {

  constructor(property: string, path: string, value: Value<boolean>) {
    super(property, 'any', path, value);
  }
}

export const ɵCollection = {
  AllCollectionValue,
  AnyCollectionValue,
};
