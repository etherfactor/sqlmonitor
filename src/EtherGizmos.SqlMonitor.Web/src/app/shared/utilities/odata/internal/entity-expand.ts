import { EntityExpand, OrderedEntityExpand, Value } from "../odata.util";
import { ɵEntityAccessor } from "./entity-accessor";
import { ɵPrefixGenerator } from "./prefix-generator";

interface ODataOptions {
  filter?: Value<boolean>[];
  orderBy?: OrderBy[];
  select?: string[];
  skip?: number;
  top?: number;
}

interface OrderBy {
  property: string;
  direction: 'asc' | 'desc';
}

class Implementation<TEntity> implements EntityExpand<TEntity>, OrderedEntityExpand<TEntity> {

  private readonly property: string;

  private readonly filterValue?: Value<boolean>[];
  private readonly orderByValue?: OrderBy[];
  private readonly selectValue?: string[];
  private readonly skipValue?: number;
  private readonly topValue?: number;

  constructor(property: string, options?: ODataOptions) {
    this.property = property;

    this.filterValue = options?.filter;
    this.orderByValue = options?.orderBy;
    this.selectValue = options?.select;
    this.skipValue = options?.skip;
    this.topValue = options?.top;
  }

  expand<TExpanded extends keyof TEntity & string>(property: TExpanded): EntityExpand<TEntity[TExpanded]> {
    const options = this.getOptions();

    return new Implementation<TEntity[TExpanded]>(property, options);
  }

  filter(builder: (entity: InstanceType<typeof ɵEntityAccessor.Implementation<TEntity>>) => Value<boolean>): EntityExpand<TEntity> {
    const generator = new ɵPrefixGenerator.Implementation();
    const accessor = new ɵEntityAccessor.Implementation<TEntity>(generator);

    const filter = builder(accessor);
    const newFilters = [...(this.filterValue ?? []), filter];

    const options = this.getOptions();
    options.filter = newFilters;

    return new Implementation<TEntity>(this.property, options);
  }

  orderBy(property: keyof TEntity & string, direction?: 'asc' | 'desc'): OrderedEntityExpand<TEntity> {
    const options = this.getOptions();
    options.orderBy = [{ property, direction: direction ?? 'asc' }];

    return new Implementation<TEntity>(this.property, options);
  }

  thenBy(property: keyof TEntity & string, direction?: 'asc' | 'desc'): OrderedEntityExpand<TEntity> {
    const options = this.getOptions();
    options.orderBy?.push({ property, direction: direction ?? 'asc' });

    return new Implementation<TEntity>(this.property, options);
  }

  select<TSelected extends keyof TEntity & string>(...properties: TSelected[]): EntityExpand<Pick<TEntity, TSelected>> {
    const options = this.getOptions();
    options.select ??= [];
    options.select = [...options.select, ...properties];

    return new Implementation<Pick<TEntity, TSelected>>(this.property, options);
  }

  skip(count: number): EntityExpand<TEntity> {
    const options = this.getOptions();
    options.skip = count;

    return new Implementation<TEntity>(this.property, options);
  }

  top(count: number): EntityExpand<TEntity> {
    const options = this.getOptions();
    options.top = count;

    return new Implementation<TEntity>(this.property, options);
  }

  private getOptions(): ODataOptions {
    return {
      filter: this.filterValue,
      orderBy: this.orderByValue,
      select: this.selectValue,
      skip: this.skipValue,
      top: this.topValue,
    };
  }
}

export const ɵEntityExpand = {
  Implementation,
};
