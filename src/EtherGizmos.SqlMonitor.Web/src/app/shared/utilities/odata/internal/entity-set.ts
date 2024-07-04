import { Params } from "@angular/router";
import { Observable } from "rxjs";
import { InferArrayType } from "../../form/form.util";
import { EntityExpand, EntitySet, Expand, Filter, ODataOptions, OrderBy, OrderedEntitySet, Select, Skip, Top, Value, expandToString, filterToString, orderByToString, selectToString, skipToString, topToString } from "../odata.util";
import { ɵEntityAccessor } from "./entity-accessor";
import { ɵEntityExpand } from "./entity-expand";
import { ɵPrefixGenerator } from "./prefix-generator";

class Implementation<TEntity> implements EntitySet<TEntity>, OrderedEntitySet<TEntity> {

  private readonly expandValue?: Expand[];
  private readonly filterValue?: Filter[];
  private readonly orderByValue?: OrderBy[];
  private readonly selectValue?: Select[];
  private readonly skipValue?: Skip;
  private readonly topValue?: Top;

  constructor(options?: ODataOptions) {
    this.expandValue = options?.expand;
    this.filterValue = options?.filter;
    this.orderByValue = options?.orderBy;
    this.selectValue = options?.select;
    this.skipValue = options?.skip;
    this.topValue = options?.top;
  }

  expand<TExpanded extends keyof TEntity & string>(property: TExpanded, builder?: (expand: EntityExpand<InferArrayType<TEntity[TExpanded]>>) => EntityExpand<InferArrayType<TEntity[TExpanded]>>): EntitySet<TEntity> {
    let expander: EntityExpand<InferArrayType<TEntity[TExpanded]>> = new ɵEntityExpand.Implementation<InferArrayType<TEntity[TExpanded]>>(property);
    if (builder) {
      expander = builder(expander);
    }

    const expand: Expand = { property, value: expander };
    const newExpand = [...(this.expandValue ?? []), expand];

    const options = this.getOptions();
    options.expand = newExpand;

    return new Implementation<TEntity>(options);
  }

  filter(builder: (entity: InstanceType<typeof ɵEntityAccessor.Implementation<TEntity>>) => Value<boolean>): EntitySet<TEntity> {
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
      expand: this.expandValue,
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

  getParams(): Params {
    const params: Params = {};

    if (this.expandValue) {
      params['$expand'] = expandToString(this.expandValue);
    }

    if (this.filterValue) {
      params['$filter'] = filterToString(this.filterValue);
    }

    if (this.orderByValue) {
      params['$orderby'] = orderByToString(this.orderByValue);
    }

    if (this.selectValue) {
      params['$select'] = selectToString(this.selectValue);
    }

    if (this.skipValue) {
      params['$skip'] = skipToString(this.skipValue);
    }

    if (this.topValue) {
      params['$top'] = topToString(this.topValue);
    }

    return params;
  }
}

export const ɵEntitySet = {
  Implementation,
};
