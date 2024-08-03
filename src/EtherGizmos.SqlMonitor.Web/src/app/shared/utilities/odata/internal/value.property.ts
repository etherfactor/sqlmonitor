import { Value } from "../odata.util";

abstract class PropertyValue<TEntity, TKey extends keyof TEntity> implements Value<TEntity[TKey]> {

  protected readonly path?: string;
  protected readonly property: TKey;

  constructor(path: string | undefined, property: TKey) {
    this.path = path;
    this.property = property;
  }

  toString(): string {
    if (this.path) {
      return `${this.path}/${this.property.toString()}`;
    } else {
      return this.property.toString();
    }
  }

  abstract _eval(data?: unknown): TEntity[TKey];
}

class EntityPropertyValue<TEntity, TKey extends keyof TEntity> extends PropertyValue<TEntity, TKey> {

  constructor(path: string | undefined, property: TKey) {
    super(path, property);
  }

  _eval(data?: unknown): TEntity[TKey] {
    if (!data)
      return undefined as TEntity[TKey];

    if (typeof data !== 'object')
      return undefined as TEntity[TKey];

    const property = data[this.property as keyof typeof data];
    return property;
  }
}

export const ɵProperty = {
  EntityPropertyValue,
};
