import { combineReducers } from "redux";
import { coursesReducer } from "./courses/courses-reducers";
//https://redux.js.org/recipes/usage-with-typescript
export const rootReducer = combineReducers({
  courses: coursesReducer
});

export type AppState = ReturnType<typeof rootReducer>;
