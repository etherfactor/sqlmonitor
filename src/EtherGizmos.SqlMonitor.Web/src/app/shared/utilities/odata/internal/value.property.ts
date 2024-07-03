import { Value } from "../odata.util";

abstract class PropertyValue<TEntity, TKey extends keyof TEntity> extends Value<TEntity[TKey]> {

  private readonly path?: string;
  private readonly property: TKey;

  constructor(path: string | undefined, property: TKey) {
    super();
    this.path = path;
    this.property = property;
  }

  override toString(): string {
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
