import { maybe } from "./maybe.util";

describe('maybe', () => {
  it('should return a value when no error occurs', () => {
    const value = maybe(() => 5);
    expect(value).toBe(5);
  });

  it('should return undefined when an error occurs', () => {
    const value = maybe(() => { throw new Error(); });
    expect(value).toBe(undefined);
  });
});
