export function maybe<TValue>(func: () => TValue): TValue | undefined {
  try {
    return func();
  } catch (ex) {
    return undefined;
  }
}
