import { fromCamelCase } from "./string.util";

describe('fromCamelCase', () => {
  it('should convert "MultipleWords" from camel case', () => {
    const test = 'MultipleWords';
    const result = fromCamelCase(test);
    expect(result).toBe('Multiple Words');
  });

  it('should convert "startingWithLowerCase" from camel case', () => {
    const test = 'startingWithLowerCase';
    const result = fromCamelCase(test);
    expect(result).toBe('Starting With Lower Case');
  });
});
