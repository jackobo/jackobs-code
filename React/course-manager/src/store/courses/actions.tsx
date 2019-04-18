import { Course, CREATE_COURSE, CoursesActionTypes } from "./types";

export function createCourse(newCourse: Course): CoursesActionTypes {
  return {
    type: CREATE_COURSE,
    payload: newCourse
  };
}
