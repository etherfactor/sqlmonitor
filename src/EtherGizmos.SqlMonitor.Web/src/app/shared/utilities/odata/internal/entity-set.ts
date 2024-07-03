import { Observable } from "rxjs";
import { EntitySet, OrderedEntitySet, Value } from "../odata.util";
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

class Implementation<TEntity> implements EntitySet<TEntity>, OrderedEntitySet<TEntity> {

  private readonly filterValue?: Value<boolean>[];
  private readonly orderByValue?: OrderBy[];
  private readonly selectValue?: string[];
  private readonly skipValue?: number;
  private readonly topValue?: number;

  constructor(options?: ODataOptions) {
    this.filterValue = options?.filter;
    this.orderByValue = options?.orderBy;
    this.selectValue = options?.select;
    this.skipValue = options?.skip;
    this.topValue = options?.top;
  }

  //expand - TODO complicated

  filter(builder: (entity: InstanceType<typeof ɵEntityAccessor.Implementation<TEntity>>) => Value<boolean>): EntitySet < TEntity > {
    const generator = new ɵPrefixGenerator.Implementation();
    const accessor = new ɵEntityAccessor.Implementation<TEntity>(generator);

    const filter = builder(accessor);
    const newFilters = [...(this.filterValue ?? []), filter];

    const options = this.getOptions();
    options.filter = newFilters;

    return new Implementation<TEntity>(options);
  }

  orderBy(property: keyof TEntity & string, direction?: 'asc' | 'desc'): OrderedEntitySet<TEntity> {
    const options = this.getOptions();
    options.orderBy = [{ property, direction: direction ?? 'asc' }];

    return new Implementation<TEntity>(options);
  }

  thenBy(property: keyof TEntity & string, direction?: 'asc' | 'desc'): OrderedEntitySet<TEntity> {
    const options = this.getOptions();
    options.orderBy?.push({ property, direction: direction ?? 'asc' });

    return new Implementation<TEntity>(options);
  }

  select<TSelected extends keyof TEntity & string>(...properties: TSelected[]): EntitySet<Pick<TEntity, TSelected>> {
    const options = this.getOptions();
    options.select ??= [];
    options.select = [...options.select, ...properties];

    return new Implementation<Pick<TEntity, TSelected>>(options);
  }

  skip(count: number): EntitySet<TEntity> {
    const options = this.getOptions();
    options.skip = count;

    return new Implementation<TEntity>(options);
  }

  top(count: number): EntitySet<TEntity> {
    const options = this.getOptions();
    options.top = count;

    return new Implementation<TEntity>(options);
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

  execute(): Observable<TEntity> {
    throw new Error('Not implemented');
  }

  //getParams(): ODataQueryOptions {
  //  const params: ODataQueryOptions = {};
  //  if (this.filterValue) {
  //    let useValue: Value<boolean>;
  //    if (this.filterValue.length > 1) {
  //      useValue = o.and(...this.filterValue);
  //    } else {
  //      useValue = this.filterValue[0];
  //    }

  //    params.filter = useValue.toString();
  //  }

  //  return params;
  //}
}

export const ɵEntitySet = {
  Implementation,
};
