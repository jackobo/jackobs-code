import { createStore, applyMiddleware, compose } from "redux";
import { rootReducer } from "./app-state";
import reduxImmutaleStateInvariant from "redux-immutable-state-invariant";
import thunk from "redux-thunk";

export default function configureStore() {
  const composeEnhancers: any =
    (window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
  return createStore(
    rootReducer,
    composeEnhancers(applyMiddleware(thunk, reduxImmutaleStateInvariant()))
  );
}
