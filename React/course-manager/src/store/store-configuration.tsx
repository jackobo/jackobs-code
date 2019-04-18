import { createStore, applyMiddleware, compose } from "redux";
import { rootReducer } from "./app-state";
import reduxImmutaleStateInvariant from "redux-immutable-state-invariant";

export default function configureStore() {
  const composeEnhancers: any =
    (window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
  return createStore(
    rootReducer,
    composeEnhancers(applyMiddleware(reduxImmutaleStateInvariant()))
  );
}
