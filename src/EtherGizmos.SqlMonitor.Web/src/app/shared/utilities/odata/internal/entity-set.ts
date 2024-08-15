import { Params } from "@angular/router";
import { sort } from "moderndash";
import { Observable, delay, of } from "rxjs";
import { InferArrayType } from "../../form/form.util";
import { Count, EntityExpand, EntitySet, Expand, Filter, ODataOptions, ODataResultSet, OrderBy, OrderedEntitySet, Select, Skip, Top, Value, expandToString, filterToString, orderByToString, selectToString, skipToString, topToString } from "../odata.util";
import { ɵEntityAccessor } from "./entity-accessor";
import { ɵEntityExpand } from "./entity-expand";
import { ɵPrefixGenerator } from "./prefix-generator";

abstract class Base<TEntity> implements EntitySet<TEntity>, OrderedEntitySet<TEntity> {

  protected readonly worker: EntitySetWorker<TEntity>;
  protected readonly countValue?: Count;
  protected readonly expandValue?: Expand[];
  protected readonly filterValue?: Filter[];
  protected readonly orderByValue?: OrderBy[];
  protected readonly selectValue?: Select[];
  protected readonly skipValue?: Skip;
  protected readonly topValue?: Top;

  constructor(worker: EntitySetWorker<TEntity>, options?: ODataOptions) {
    this.worker = worker;
    this.countValue = options?.count;
    this.expandValue = options?.expand;
    this.filterValue = options?.filter;
    this.orderByValue = options?.orderBy;
    this.selectValue = options?.select;
    this.skipValue = options?.skip;
    this.topValue = options?.top;
  }

  protected abstract new<TNewEntity = TEntity>(worker: EntitySetWorker<TNewEntity>, options?: ODataOptions): Base<TNewEntity>;

  count(): EntitySet<TEntity> {
    const options = this.getOptions();
    options.count = true;

    return this.new<TEntity>(this.worker, options);
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

    return this.new<TEntity>(this.worker, options);
  }

  filter(builder: (entity: InstanceType<typeof ɵEntityAccessor.Implementation<TEntity>>) => Value<boolean>): EntitySet<TEntity> {
    const generator = new ɵPrefixGenerator.Implementation();
    const accessor = new ɵEntityAccessor.Implementation<TEntity>(generator);

    const filter = builder(accessor);
    const newFilters = [...(this.filterValue ?? []), filter];

    const options = this.getOptions();
    options.filter = newFilters;

    return this.new<TEntity>(this.worker, options);
  }

  orderBy(property: keyof TEntity & string, direction?: 'asc' | 'desc'): OrderedEntitySet<TEntity> {
    const options = this.getOptions();
    options.orderBy = [{ property, direction: direction ?? 'asc' }];

    return this.new<TEntity>(this.worker, options);
  }

  thenBy(property: keyof TEntity & string, direction?: 'asc' | 'desc'): OrderedEntitySet<TEntity> {
    const options = this.getOptions();
    options.orderBy?.push({ property, direction: direction ?? 'asc' });

    return this.new<TEntity>(this.worker, options);
  }

  select<TSelected extends keyof TEntity & string>(...properties: TSelected[]): EntitySet<Pick<TEntity, TSelected>> {
    const options = this.getOptions();
    options.select ??= [];
    options.select = [...options.select, ...properties];

    return this.new<Pick<TEntity, TSelected>>(this.worker, options);
  }

  skip(count: number): EntitySet<TEntity> {
    const options = this.getOptions();
    options.skip = count;

    return this.new(this.worker, options);
  }

  top(count: number): EntitySet<TEntity> {
    const options = this.getOptions();
    options.top = count;

    return this.new<TEntity>(this.worker, options);
  }

  private getOptions(): ODataOptions {
    return {
      count: this.countValue,
      expand: this.expandValue,
      filter: this.filterValue,
      orderBy: this.orderByValue,
      select: this.selectValue,
      skip: this.skipValue,
      top: this.topValue,
    };
  }

  execute(): Observable<ODataResultSet<TEntity>> {
    return this.worker.execute(this.getOptions());
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

class Implementation<TEntity> extends Base<TEntity> {

  constructor(options?: ODataOptions) {
    super(new ConcreteEntitySetWorker(), options);
  }

  protected override new<TNewEntity = TEntity>(worker: EntitySetWorker<TNewEntity>, options?: ODataOptions): Base<TNewEntity> {
    return new Implementation(options);
  }
}

class MockImplementation<TEntity> extends Base<TEntity> {

  private getData: () => TEntity[];

  constructor(getData: () => TEntity[], worker?: EntitySetWorker<TEntity>, options?: ODataOptions) {
    super(worker ?? new MockEntitySetWorker(getData), options);
    this.getData = getData;
  }

  protected override new<TNewEntity = TEntity>(worker: EntitySetWorker<TNewEntity>, options?: ODataOptions): Base<TNewEntity> {
    return new MockImplementation<TNewEntity>(this.getData as unknown as () => TNewEntity[], worker, options);
  }
}

abstract class EntitySetWorker<TEntity> {
  abstract execute(options: ODataOptions): Observable<ODataResultSet<TEntity>>;
}

class MockEntitySetWorker<TEntity> extends EntitySetWorker<TEntity> {

  private getData: () => TEntity[];

  constructor(
    getData: () => TEntity[],
  ) {
    super();
    this.getData = getData;
  }

  override execute(options: ODataOptions): Observable<ODataResultSet<TEntity>> {
    const data: ODataResultSet<TEntity> = { value: this.getData() };
    data.value = this.applyFilters(data.value, options.filter ?? []);
    if (options.count) {
      data["@odata.count"] = data.value.length;
    }
    data.value = this.applyOrderBy(data.value, options.orderBy ?? []);
    data.value = this.applySkipTop(data.value, options.skip ?? 0, options.top ?? 100);
    data.value = this.applySelect(data.value, options.select ?? []);

    return of(data).pipe(
      delay(1000),
    );
  }

  private applyFilters(data: TEntity[], filters: Filter[]): TEntity[] {
    let finalData = data;
    for (const filter of filters) {
      finalData = finalData.filter(datum => filter._eval(datum));
    }

    return finalData;
  }
  
  private applyOrderBy(data: TEntity[], orderBy: OrderBy[]): TEntity[] {
    const rules = orderBy.map(order => ({ order: order.direction, by: (entity: TEntity) => entity[order.property as keyof TEntity] as string }));
    const finalData = sort(data, ...rules);

    return finalData;
  }

  private applySkipTop(data: TEntity[], skip: Skip, top: Top) {
    const startAt = skip;
    const endAt = startAt + top;
    const finalData = data.slice(startAt, endAt);

    return finalData;
  }

  private applySelect(data: TEntity[], select: Select[]) {
    if (select.length === 0)
      return data;

    const finalData = data.map(datum => {
      const newObj: TEntity = {} as TEntity;
      for (const property of select) {
        newObj[property as keyof TEntity] = datum[property as keyof TEntity];
      }

      return newObj;
    });

    return finalData;
  }
}

class ConcreteEntitySetWorker<TEntity> extends EntitySetWorker<TEntity> {

  override execute(options: ODataOptions): Observable<ODataResultSet<TEntity>> {
    throw new Error("Method not implemented.");
  }
}

export const ɵEntitySet = {
  Implementation,
  MockImplementation,
};
