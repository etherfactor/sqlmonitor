import { DateTime, Interval } from "luxon";
import { Guid } from "../../types/guid/guid";
import { InferArrayType } from "../form/form.util";

interface ODataQueryOptions {
  filter?: string;
}

export class EntitySet<TEntity> {

  private readonly filters?: Value<boolean>[];

  constructor(filters?: Value<boolean>[]) {
    this.filters = filters;
  }

  filter(builder: (entity: EntityAccessor<TEntity>) => Value<boolean>): EntitySet<TEntity> {
    const generator = new PrefixGenerator();
    const accessor = new EntityAccessor<TEntity>(generator);

    const filter = builder(accessor);
    const newFilters = [...(this.filters ?? []), filter];

    return new EntitySet<TEntity>(newFilters);
  }

  getParams(): ODataQueryOptions {
    const params: ODataQueryOptions = {};
    if (this.filters) {
      let useValue: Value<boolean>;
      if (this.filters.length > 1) {
        useValue = o.and(...this.filters);
      } else {
        useValue = this.filters[0];
      }

      params.filter = useValue.toString();
    }

    return params;
  }
}

class PrefixGenerator {

  private index: number;

  constructor() {
    this.index = 0;
  }

  getPath(): string {
    const useIndex = this.index++;
    return `e${useIndex}`;
  }
}

class EntityAccessor<TEntity> {

  private readonly path?: string;

  readonly generator: PrefixGenerator;

  constructor(generator: PrefixGenerator, path?: string) {
    this.generator = generator;
    this.path = path;
  }

  prop<TKey extends keyof TEntity & string>(property: TKey): Value<TEntity[TKey]> {
    return new PropertyValue<TEntity, TKey>(this.path, property);
  }

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  any<TKey extends keyof TEntity & string>(property: TKey extends keyof TEntity ? (TEntity[TKey] extends Array<any> ? TKey : never) : never, builder: (entity: EntityAccessor<InferArrayType<TEntity[TKey]>>) => Value<boolean>): Value<boolean> {
    const generator = this.generator;
    const path = generator.getPath();
    const accessor = new EntityAccessor<InferArrayType<TEntity[TKey]>>(generator, path);

    const value = builder(accessor);
    return new AnyArrayValue(property, path, value);
  }
}

export class o {

  //Logical operators

  static and(...conditions: Value<boolean>[]): Value<boolean> {
    return new AndGroupValue(...conditions);
  }

  static or(...conditions: Value<boolean>[]): Value<boolean> {
    return new OrGroupValue(...conditions);
  }

  static not(condition: Value<boolean>): Value<boolean> {
    return new NotValue(condition);
  }

  //Comparison operators

  static eq<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new EqualsComparisonValue(left, right);
  }

  static ne<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new NotEqualsComparisonValue(left, right);
  }

  static lt<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new LessThanComparisonValue(left, right);
  }

  static le<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new LessThanOrEqualsComparisonValue(left, right);
  }

  static gt<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new GreaterThanComparisonValue(left, right);
  }

  static ge<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new GreaterThanOrEqualsComparisonValue(left, right);
  }

  //String operators

  //Arithmetic operators

  //Constant values

  static string(value: string): Value<string> {
    return new StringConstantValue(value);
  }

  static bool(value: boolean): Value<boolean> {
    return new BooleanConstantValue(value);
  }

  static int(value: number): Value<number> {
    return new IntegerConstantValue(value);
  }

  static guid(value: Guid): Value<Guid> {
    return new GuidConstantValue(value);
  }

  static date(value: Date | DateTime): Value<DateTime> {
    if (DateTime.isDateTime(value)) {
      return new DateConstantValue(value);
    } else {
      const asLuxon = DateTime.fromJSDate(value);
      return new DateConstantValue(asLuxon);
    }
  }

  static dateTime(value: Date | DateTime): Value<DateTime> {
    if (DateTime.isDateTime(value)) {
      return new DateTimeConstantValue(value);
    } else {
      const asLuxon = DateTime.fromJSDate(value);
      return new DateTimeConstantValue(asLuxon);
    }
  }

  static time(value: Interval): Value<Interval> {
    return new TimeConstantValue(value);
  }
}

abstract class Value<TValue> {

  readonly _: TValue = undefined!;

  abstract toString(): string;
}

abstract class ConstantValue<TValue> extends Value<TValue> {

  constructor() {
    super();
  }
}

class BooleanConstantValue extends ConstantValue<boolean> {

  private readonly value: boolean;

  constructor(value: boolean) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toString();
  }
}

class DateConstantValue extends ConstantValue<DateTime> {

  private readonly value: DateTime;

  constructor(value: DateTime) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toISODate()!;
  }
}

class DateTimeConstantValue extends ConstantValue<DateTime> {

  private readonly value: DateTime;

  constructor(value: DateTime) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toISO()!;
  }
}

class GuidConstantValue extends ConstantValue<Guid> {

  private readonly value: Guid;

  constructor(value: Guid) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value;
  }
}

class IntegerConstantValue extends ConstantValue<number> {

  private readonly value: number;

  constructor(value: number) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toString();
  }
}

class StringConstantValue extends ConstantValue<string> {

  private readonly value: string;

  constructor(value: string) {
    super();
    this.value = value;
  }

  override toString() {
    return `'${this.value.replace("'", "''")}'`;
  }
}

class TimeConstantValue extends ConstantValue<Interval> {

  private readonly value: Interval;

  constructor(value: Interval) {
    super();
    this.value = value;
  }

  override toString(): string {
    return this.value.toISOTime()!;
  }
}

class PropertyValue<TEntity, TKey extends keyof TEntity> extends Value<TEntity[TKey]> {

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

abstract class ComparisonValue<TValue> extends Value<boolean> {

  private readonly left: Value<TValue>;
  private readonly comparator: string;
  private readonly right: Value<TValue>;

  constructor(left: Value<TValue>, comparator: string, right: Value<TValue>) {
    super();
    this.left = left;
    this.comparator = comparator;
    this.right = right;
  }

  override toString(): string {
    return `${this.left.toString()} ${this.comparator} ${this.right.toString()}`;
  }
}

class EqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'eq', right);
  }
}

class NotEqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'ne', right);
  }
}

class GreaterThanComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'gt', right);
  }
}

class GreaterThanOrEqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'ge', right);
  }
}

class LessThanComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'lt', right);
  }
}

class LessThanOrEqualsComparisonValue<TValue> extends ComparisonValue<TValue> {

  constructor(left: Value<TValue>, right: Value<TValue>) {
    super(left, 'le', right);
  }
}

abstract class GroupValue extends Value<boolean> {

  private readonly operand: string;
  private readonly conditions: Value<boolean>[];

  constructor(operand: string, ...conditions: Value<boolean>[]) {
    super();
    this.operand = operand;
    this.conditions = conditions;
  }

  override toString(): string {
    return `(${this.conditions.map(item => item.toString()).join(` ${this.operand} `)})`;
  }
}

class AndGroupValue extends GroupValue {

  constructor(...conditions: Value<boolean>[]) {
    super('and', ...conditions);
  }
}

class OrGroupValue extends GroupValue {

  constructor(...conditions: Value<boolean>[]) {
    super('or', ...conditions);
  }
}

class NotValue extends Value<boolean> {
  private readonly condition: Value<boolean>;

  constructor(condition: Value<boolean>) {
    super();
    this.condition = condition;
  }

  override toString(): string {
    return `not ${this.condition.toString()}`;
  }
}

abstract class ArrayValue extends Value<boolean> {

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

class AnyArrayValue extends ArrayValue {

  constructor(property: string, path: string, value: Value<boolean>) {
    super(property, 'any', path, value);
  }
}

interface Model {
  id: Guid;
  identity: number;
  name: string;
  description?: string;
  values: SubModel[];
}

interface SubModel {
  key: string;
  value?: string;
}

const set = new EntitySet<Model>();

set.filter(e =>
  o.and(
    o.eq(
      e.prop('id'),
      o.guid('00000000-0000-0000-0000-000000000000' as Guid)
    ),
    o.eq(
      e.prop('identity'),
      o.int(1)
    ),
    e.any('values', e2 =>
      o.ne(
        e2.prop('value'),
        o.string('hello')
      )
    )
  )
);
