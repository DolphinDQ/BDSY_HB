interface Array<T> {
    find(query: (a: T) => boolean): T;
}
interface External {
    On(eventName: string, arg: any);
}