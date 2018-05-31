interface Array<T> {
    first(query: (a: T) => boolean): T;
    min(query: (a: T) => number): T;
    max(query: (a: T) => number): T;
    avg(query: (a: T) => number): T;
    select<I>(query: (a: T) => I): Array<I>;
    selectMany<I>(query: (a: T) => Array<I>): Array<I>;
}
interface External {
    On(eventName: string, arg: any);
}

interface Window {
    $: any;
}