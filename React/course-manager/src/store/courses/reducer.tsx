import { Courses, CREATE_COURSE, CoursesActionTypes } from "./types";

const initialState: Courses = {};

export function coursesReducer(
  state: Courses = initialState,
  action: CoursesActionTypes
): Courses {
  switch (action.type) {
    case CREATE_COURSE: {
      return { ...state, [action.payload.id]: action.payload };
    }
    default: {
      return state;
    }
  }
}
