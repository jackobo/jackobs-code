export interface Course {
  id: string;
  title: string;
}

export interface Courses {
  [key: string]: Course;
}

export const CREATE_COURSE = "CREATE_COURSE";
export const DELETE_COURSE = "DELETE_COURSE";
export const LOAD_COURSES_SUCCESS = "LOAD_COURSES_SUCCESS";

export interface CreateCourseAction {
  type: typeof CREATE_COURSE;
  payload: Course;
}

export interface DeleteCourseAction {
  type: typeof DELETE_COURSE;
  payload: Course;
}

export interface LoadCoursesSuccessAction {
  type: typeof LOAD_COURSES_SUCCESS;
  payload: Course[];
}
export type CoursesActionTypes =
  | CreateCourseAction
  | DeleteCourseAction
  | LoadCoursesSuccessAction;
