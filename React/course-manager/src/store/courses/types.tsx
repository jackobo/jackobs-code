export interface Course {
  title: string;
}

export interface CoursesState {
  courses: Course[];
}

export const CREATE_COURSE = "CREATE_COURSE";
export const DELETE_COURSE = "DELETE_COURSE";

export interface CreateCourseAction {
  type: typeof CREATE_COURSE;
  payload: Course;
}

export interface DeleteCourseAction {
  type: typeof DELETE_COURSE;
  payload: Course;
}

export type CoursesActionTypes = CreateCourseAction | DeleteCourseAction;
