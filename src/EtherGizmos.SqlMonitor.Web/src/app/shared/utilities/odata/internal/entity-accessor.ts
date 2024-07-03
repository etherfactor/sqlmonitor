import { InferArrayType } from "../../form/form.util";
import { Value } from "../odata.util";
import { ɵPrefixGenerator } from "./prefix-generator";
import { ɵCollection } from "./value.collection";
import { ɵProperty } from "./value.property";

class Implementation<TEntity> {

  private readonly path?: string;

  readonly generator: InstanceType<typeof ɵPrefixGenerator.Implementation>;

  constructor(generator: InstanceType<typeof ɵPrefixGenerator.Implementation>, path?: string) {
    this.generator = generator;
    this.path = path;
  }

  prop<TKey extends keyof TEntity & string>(property: TKey): Value<TEntity[TKey]> {
    return new ɵProperty.EntityPropertyValue<TEntity, TKey>(this.path, property);
  }

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  all<TKey extends keyof TEntity & string>(property: TKey extends keyof TEntity ? (TEntity[TKey] extends Array<any> ? TKey : never) : never, builder: (entity: Implementation<InferArrayType<TEntity[TKey]>>) => Value<boolean>): Value < boolean > {
    const generator = this.generator;
    const path = generator.getPath();
    const accessor = new Implementation<InferArrayType<TEntity[TKey]>>(generator, path);

    const value = builder(accessor);
    return new ɵCollection.AllCollectionValue(property, path, value);
  }

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  any<TKey extends keyof TEntity & string>(property: TKey extends keyof TEntity ? (TEntity[TKey] extends Array<any> ? TKey : never) : never, builder: (entity: Implementation<InferArrayType<TEntity[TKey]>>) => Value<boolean>): Value<boolean> {
    const generator = this.generator;
    const path = generator.getPath();
    const accessor = new Implementation<InferArrayType<TEntity[TKey]>>(generator, path);

    const value = builder(accessor);
    return new ɵCollection.AnyCollectionValue(property, path, value);
  }
}

export const ɵEntityAccessor = {
  Implementation,
};
