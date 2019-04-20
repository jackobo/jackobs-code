import {
  Course,
  CREATE_COURSE,
  CreateCourseAction,
  DeleteCourseAction,
  DELETE_COURSE
} from "./courses-types";

export function createCourse(newCourse: Course): CreateCourseAction {
  return {
    type: CREATE_COURSE,
    payload: newCourse
  };
}

export function deleteCourse(course: Course): DeleteCourseAction {
  return {
    type: DELETE_COURSE,
    payload: course
  };
}
