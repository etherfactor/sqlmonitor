import { Guid } from "../../types/guid/guid";
import { EntitySet, o } from "./odata.util";

interface Model {
  id: Guid;
  quantity: number;
  isActive: boolean;
  name: string;
  description?: string;
  values: SubModel[];
}

interface SubModel {
  key: string;
  value?: string;
}

describe('EntitySet', () => {
  let set: EntitySet<Model>;

  beforeEach(() => {
    set = new EntitySet<Model>();
  });

  it('should1', () => {
    const filtered = set.filter(e =>
      o.eq(
        e.prop('id'),
        o.guid('00000000-0000-0000-0000-000000000000' as Guid)
      )
    );

    const params = filtered.getParams();
    expect(params.filter).toBe("id eq 00000000-0000-0000-0000-000000000000");
  });

  it('should2', () => {
    const filtered = set.filter(e =>
      o.and(
        o.eq(
          e.prop('quantity'),
          o.int(1)
        ),
        o.eq(
          e.prop('isActive'),
          o.bool(true)
        )
      )
    );

    const params = filtered.getParams();
    expect(params.filter).toBe("(quantity eq 1 and isActive eq true)");
  });

  it('should3', () => {
    const filtered = set.filter(e =>
      o.or(
        o.and(
          o.eq(
            e.prop('quantity'),
            o.int(1)
          ),
          o.eq(
            e.prop('isActive'),
            o.bool(true)
          )
        ),
        o.and(
          o.eq(
            e.prop('quantity'),
            o.int(0)
          ),
          o.eq(
            e.prop('isActive'),
            o.bool(false)
          )
        )
      )
    );

    const params = filtered.getParams();
    expect(params.filter).toBe("((quantity eq 1 and isActive eq true) or (quantity eq 0 and isActive eq false))");
  });

  it('should4', () => {
    const filtered = set.filter(e =>
      e.any('values', a =>
        o.and(
          o.eq(
            a.prop('key'),
            o.string('key')
          ),
          o.eq(
            a.prop('value'),
            o.string('value')
          )
        )
      )
    );

    const params = filtered.getParams();
    expect(params.filter).toBe("values/any(e0: (e0/key eq 'key' and e0/value eq 'value'))")
  });
});
