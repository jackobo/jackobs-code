import { Course } from "../store/courses/courses-types";
const baseUrl = "http://localhost:3001/courses/";

export function getCourses(): Promise<Course[]> {
  return fetch(baseUrl).then(response => response.json());
}
