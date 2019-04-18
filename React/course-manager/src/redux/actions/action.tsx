export default interface Action<TPayload> {
    type: string,
    payload: TPayload
}

