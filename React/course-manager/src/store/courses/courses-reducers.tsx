import {
  Courses,
  CREATE_COURSE,
  CoursesActionTypes,
  DELETE_COURSE
} from "./courses-types";

const initialState: Courses = {};

export function coursesReducer(
  state: Courses = initialState,
  action: CoursesActionTypes
): Courses {
  switch (action.type) {
    case CREATE_COURSE: {
      return { ...state, [action.payload.id]: action.payload };
    }
    case DELETE_COURSE: {
      const newState = { ...state };
      delete newState[action.payload.id];
      return newState;
    }
    default: {
      return state;
    }
  }
}
