interface Array<T> {
    first(query: (a: T) => boolean): T;
}
interface External {
    On(eventName: string, arg: any);
}