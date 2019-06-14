import {
  Courses,
  CREATE_COURSE,
  CoursesActionTypes,
  DELETE_COURSE,
  LOAD_COURSES_SUCCESS,
  Course
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
    case LOAD_COURSES_SUCCESS: {
      return action.payload.reduce((courses: Courses, course: Course) => {
        courses[course.id] = course;
        return courses;
      }, {});

      /*
      let newState: Courses = {};
      action.payload.forEach(course => {
        newState[course.id] = course;
      });
      return newState;
      */
    }
    default: {
      return state;
    }
  }
}
