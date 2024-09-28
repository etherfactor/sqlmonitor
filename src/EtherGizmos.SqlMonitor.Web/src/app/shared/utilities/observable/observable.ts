import { MonoTypeOperatorFunction, Observable, of, switchMap, tap } from "rxjs";

type Action = () => void;

export function precedeWith<TValue>(action: Action): MonoTypeOperatorFunction<TValue>;
export function precedeWith<TValue>(observable: Observable<unknown>): MonoTypeOperatorFunction<TValue>;
export function precedeWith<TValue>(input: Observable<unknown> | Action): MonoTypeOperatorFunction<TValue> {
  let useObservable: Observable<unknown>;
  if (input instanceof Observable) {
    useObservable = input;
  } else {
    useObservable = of(true).pipe(
      tap(() => input()),
    );
  }

  return source => useObservable.pipe(
    switchMap(() => source),
  );
}

export function wrapWith<TValue>(start: Action, end: Action): MonoTypeOperatorFunction<TValue> {
  return source => of(true).pipe(
    precedeWith(start),
    switchMap(() => source),
    tap(end),
  );
}
