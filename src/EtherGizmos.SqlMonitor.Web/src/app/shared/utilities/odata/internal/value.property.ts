import { Value } from "../odata.util";

abstract class PropertyValue<TEntity, TKey extends keyof TEntity> implements Value<TEntity[TKey]> {

  private readonly path?: string;
  private readonly property: TKey;

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
}

class EntityPropertyValue<TEntity, TKey extends keyof TEntity> extends PropertyValue<TEntity, TKey> {

  constructor(path: string | undefined, property: TKey) {
    super(path, property);
  }
}

export const ɵProperty = {
  EntityPropertyValue,
};
