import { CoursesState, CREATE_COURSE, CoursesActionTypes } from "./types";

const initialState: CoursesState = {
  courses: []
};

export function coursesReducer(
  state: CoursesState = initialState,
  action: CoursesActionTypes
): CoursesState {
  switch (action.type) {
    case CREATE_COURSE: {
      return { courses: [...state.courses, action.payload] };
    }
    default: {
      return state;
    }
  }
}
