import {
  Course,
  CREATE_COURSE,
  CreateCourseAction,
  DeleteCourseAction,
  DELETE_COURSE,
  LoadCoursesSuccessAction,
  LOAD_COURSES_SUCCESS
} from "./courses-types";

import * as coursesApi from "../../api/courses-api";

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

function loadCoursesSuccess(courses: Course[]): LoadCoursesSuccessAction {
  return {
    type: LOAD_COURSES_SUCCESS,
    payload: courses
  };
}

export function loadCourses(): Function {
  return function(dispatch: Function) {
    return coursesApi.getCourses().then(courses => {
      dispatch(loadCoursesSuccess(courses));
    });
  };
}
