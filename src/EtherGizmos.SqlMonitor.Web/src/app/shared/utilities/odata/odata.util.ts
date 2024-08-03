import { Params } from "@angular/router";
import { DateTime, Interval } from "luxon";
import { Observable } from "rxjs";
import { Guid } from "../../types/guid/guid";
import { InferArrayType } from "../form/form.util";
import { ɵEntityAccessor } from "./internal/entity-accessor";
import { ɵComparison } from "./internal/value.comparison";
import { ɵConstant } from "./internal/value.constant";
import { ɵFunction } from "./internal/value.function";
import { ɵLogical } from "./internal/value.logical";
import { ɵOperator } from "./internal/value.operator";

export interface Value<TValue> {

  readonly _?: TValue;

  toString(): string;

  _eval(data?: unknown): TValue;
}

export interface ODataOptions {
  expand?: Expand[];
  filter?: Filter[];
  orderBy?: OrderBy[];
  select?: Select[];
  skip?: Skip;
  top?: Top;
}

export interface ODataResultSet<TEntity> {
  "@odata.count"?: number;
  "@odata.context": string;
  value: TEntity[];
}

export interface Expand {
  property: string;
  value: EntityExpand<unknown>;
}

export function expandToString(expand: Expand[]): string {
  const useValue = expand.map(expand => expand.value.toString()).join(', ');
  return useValue;
}

export type Filter = Value<boolean>;

export function filterToString(filter: Filter[]): string {
  let useValue: string;
  if (filter.length > 1) {
    useValue = o.and(...filter).toString();
  } else {
    useValue = filter[0].toString();
  }

  return useValue;
}

export type Direction = 'asc' | 'desc';

export interface OrderBy {
  property: string;
  direction: Direction;
}

export function orderByToString(orderBy: OrderBy[]): string {
  const useValue = orderBy.map(orderBy => `${orderBy.property} ${orderBy.direction}`).join(', ');
  return useValue;
}

export type Select = string;

export function selectToString(select: Select[]): string {
  const useValue = select.join(', ');
  return useValue;
}

export type Skip = number;

export function skipToString(skip: Skip): string {
  const useValue = skip.toFixed(0);
  return useValue;
}

export type Top = number;

export function topToString(top: Top): string {
  const useValue = top.toFixed(0);
  return useValue;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type IsObjectOrArray<TValue> = TValue extends object ? (TValue extends Array<any> ? (TValue[number] extends object ? TValue : never) : TValue): never;

export interface EntitySet<TEntity> {
  execute(): Observable<TEntity[]>;
  expand<TExpanded extends keyof TEntity & string>(
    property: TExpanded /*& (TEntity[TExpanded] extends Array<any> | object ? TExpanded : never)*/,
    builder?: (expand: EntityExpand<InferArrayType<TEntity[TExpanded]>>) => EntityExpand<InferArrayType<TEntity[TExpanded]>>): EntitySet<TEntity>;
  filter(builder: (entity: InstanceType<typeof ɵEntityAccessor.Implementation<TEntity>>) => Value<boolean>): EntitySet<TEntity>;
  orderBy(property: keyof TEntity & string, direction?: Direction): OrderedEntitySet<TEntity>;
  select<TSelected extends keyof TEntity & string>(...properties: TSelected[]): EntitySet<Pick<TEntity, TSelected>>;
  skip(count: number): EntitySet<TEntity>;
  top(count: number): EntitySet<TEntity>;
  getParams(): Params;
}

export interface OrderedEntitySet<TEntity> extends EntitySet<TEntity> {
  thenBy(property: keyof TEntity & string, direction?: Direction): EntitySet<TEntity>;
}

export interface EntityExpand<TEntity> {
  expand<TExpanded extends keyof TEntity & string>(
    property: TExpanded /*& (TEntity[TExpanded] extends Array<any> | object ? TExpanded : never)*/,
    builder?: (expand: EntityExpand<InferArrayType<TEntity[TExpanded]>>) => EntityExpand<InferArrayType<TEntity[TExpanded]>>): EntityExpand<TEntity>;
  filter(builder: (entity: InstanceType<typeof ɵEntityAccessor.Implementation<TEntity>>) => Value<boolean>): EntityExpand<TEntity>;
  orderBy(property: keyof TEntity & string, direction?: Direction): OrderedEntityExpand<TEntity>;
  select<TSelected extends keyof TEntity & string>(...properties: TSelected[]): EntityExpand<Pick<TEntity, TSelected>>;
  skip(count: number): EntityExpand<TEntity>;
  top(count: number): EntityExpand<TEntity>;
  toString(): string;
}

export interface OrderedEntityExpand<TEntity> extends EntityExpand<TEntity> {
  thenBy(property: keyof TEntity & string, direction?: Direction): OrderedEntityExpand<TEntity>;
}

export class o {

  //Logical operators

  //(... and ...)
  static and(...conditions: Value<boolean>[]): Value<boolean> {
    return new ɵLogical.AndLogicalValue(...conditions);
  }

  //(... or ...)
  static or(...conditions: Value<boolean>[]): Value<boolean> {
    return new ɵLogical.OrLogicalValue(...conditions);
  }

  //not (...)
  static not(condition: Value<boolean>): Value<boolean> {
    return new ɵLogical.NotLogicalValue(condition);
  }

  //Comparison operators

  //... eq ...
  static eq<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new ɵComparison.EqualsComparisonValue(left, right);
  }

  //... ne ...
  static ne<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new ɵComparison.NotEqualsComparisonValue(left, right);
  }

  //... lt ...
  static lt<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new ɵComparison.LessThanComparisonValue(left, right);
  }

  //... le ...
  static le<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new ɵComparison.LessThanOrEqualsComparisonValue(left, right);
  }

  //... gt ...
  static gt<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new ɵComparison.GreaterThanComparisonValue(left, right);
  }

  //... ge ...
  static ge<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new ɵComparison.GreaterThanOrEqualsComparisonValue(left, right);
  }

  //String operators

  //contains(..., ...)
  static contains(string: Value<string>, contains: Value<string>): Value<boolean> {
    return new ɵFunction.ContainsFunctionValue(string, contains);
  }

  //startswith(..., ...)
  static startsWith(string: Value<string>, startsWith: Value<string>): Value<boolean> {
    return new ɵFunction.StartsWithFunctionValue(string, startsWith);
  }

  //endswith(..., ...)
  static endsWith(string: Value<string>, endsWith: Value<string>): Value<boolean> {
    return new ɵFunction.EndsWithFunctionValue(string, endsWith);
  }

  //concat(..., ...)
  static concat(left: Value<string>, right: Value<string>): Value<string> {
    return new ɵFunction.ConcatFunctionValue(left, right);
  }

  //indexof(..., ...)
  static indexOf(string: Value<string>, indexOf: Value<string>): Value<number> {
    return new ɵFunction.IndexOfFunctionValue(string, indexOf);
  }

  //length(...)
  static lengthOf(value: Value<string>): Value<number> {
    return new ɵFunction.LengthFunctionValue(value);
  }

  //substring(..., ...)
  //substring(..., ..., ...)
  static substring(value: Value<string>, start: Value<number>, finish?: Value<number>) {
    return new ɵFunction.SubstringFunctionValue(value, start, finish);
  }

  //tolower(...)
  static toLower(value: Value<string>): Value<string> {
    return new ɵFunction.ToLowerFunctionValue(value);
  }

  //toupper(...)
  static toUpper(value: Value<string>): Value<string> {
    return new ɵFunction.ToUpperFunctionValue(value);
  }

  //trim(...)
  static trim(value: Value<string>): Value<string> {
    return new ɵFunction.TrimFunctionValue(value);
  }

  //Arithmetic operators

  //(... add ...)
  static add(left: Value<number>, right: Value<number>): Value<number> {
    return new ɵOperator.AddOperatorValue(left, right);
  }

  //(... sub ...)
  static subtract(left: Value<number>, right: Value<number>): Value<number> {
    return new ɵOperator.SubtractOperatorValue(left, right);
  }

  //(... mul ...)
  static multiply(left: Value<number>, right: Value<number>): Value<number> {
    return new ɵOperator.MultiplyOperatorValue(left, right);
  }

  //(... div ...)
  static divide(left: Value<number>, right: Value<number>): Value<number> {
    return new ɵOperator.DivideOperatorValue(left, right);
  }

  //(... mod ...)
  static modulo(left: Value<number>, right: Value<number>): Value<number> {
    return new ɵOperator.ModuloOperatorValue(left, right);
  }

  //ceiling(...)
  static ceiling(value: Value<number>): Value<number> {
    return new ɵFunction.CeilingFunctionValue(value);
  }

  //floor(...)
  static floor(value: Value<number>): Value<number> {
    return new ɵFunction.FloorFunctionValue(value);
  }

  //round(...)
  static round(value: Value<number>): Value<number> {
    return new ɵFunction.RoundFunctionValue(value);
  }

  //Constant values

  //null
  static null() {
    return new ɵConstant.NullConstantValue();
  }

  //'...'
  static string(value: string): Value<string> {
    return new ɵConstant.StringConstantValue(value);
  }

  //...
  static bool(value: boolean): Value<boolean> {
    return new ɵConstant.BooleanConstantValue(value);
  }

  //...
  static int(value: number): Value<number> {
    return new ɵConstant.IntegerConstantValue(value);
  }

  //...
  static guid(value: Guid): Value<Guid> {
    return new ɵConstant.GuidConstantValue(value);
  }

  //...
  static date(value: Date | DateTime): Value<DateTime> {
    if (DateTime.isDateTime(value)) {
      return new ɵConstant.DateConstantValue(value);
    } else {
      const asLuxon = DateTime.fromJSDate(value);
      return new ɵConstant.DateConstantValue(asLuxon);
    }
  }

  //...
  static dateTime(value: Date | DateTime): Value<DateTime> {
    if (DateTime.isDateTime(value)) {
      return new ɵConstant.DateTimeConstantValue(value);
    } else {
      const asLuxon = DateTime.fromJSDate(value);
      return new ɵConstant.DateTimeConstantValue(asLuxon);
    }
  }

  //...
  static time(value: Interval): Value<Interval> {
    return new ɵConstant.TimeConstantValue(value);
  }
}
