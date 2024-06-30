import { Guid } from "../../types/guid/guid";
import { InferArrayType } from "../form/form.util";

export class EntitySet<TEntity> {

  filter(builder: (entity: EntityAccessor<TEntity>) => Value<boolean>): this {
    builder(new EntityAccessor<TEntity>());
    return this;
  }
}

export class EntityAccessor<TEntity> {

  prop<TKey extends keyof TEntity>(property: TKey): Value<TEntity[TKey]> {
    return new PropertyValue<TEntity, TKey>(property);
  }

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  any<TKey extends keyof TEntity>(property: TKey extends keyof TEntity ? (TEntity[TKey] extends Array<any> ? TKey : never) : never, builder: (entity: EntityAccessor<InferArrayType<TEntity[TKey]>>) => Value<boolean>): Value<boolean> {
    return builder(new EntityAccessor<InferArrayType<TEntity[TKey]>>());
  }
}

export class o {

  static and(...conditions: Value<boolean>[]): Value<boolean> {
    return new AndGroupValue(...conditions);
  }

  static eq<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new EqualsComparisonValue(left, right);
  }

  static guid(value: Guid): Value<Guid> {
    return new GuidConstantValue(value);
  }

  static int(value: number): Value<number> {
    return new IntegerConstantValue(value);
  }

  static ne<TValue>(left: Value<TValue>, right: Value<TValue>): Value<boolean> {
    return new NotEqualsComparisonValue(left, right);
  }

  static string(value: string): Value<string> {
    return new StringConstantValue(value);
  }
}

abstract class Value<TValue> {

  _: TValue = undefined!;

  abstract toString(): string;
}

abstract class ConstantValue<TValue> extends Value<TValue> {

  constructor() {
    super();
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

class PropertyValue<TEntity, TKey extends keyof TEntity> extends Value<TEntity[TKey]> {

  private readonly property: TKey;

  constructor(property: TKey) {
    super();
    this.property = property;
  }

  override toString(): string {
    return this.property.toString();
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

abstract class GroupValue extends Value<boolean> {

  private readonly operand: string;
  private readonly conditions: Value<boolean>[];

  constructor(operand: string, ...conditions: Value<boolean>[]) {
    super();
    this.operand = operand;
    this.conditions = conditions;
  }

  override toString(): string {
    return `(${this.conditions.map(item => item.toString()).join(' and ')})`;
  }
}

class AndGroupValue extends GroupValue {

  constructor(...conditions: Value<boolean>[]) {
    super('and', ...conditions);
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
