import { Author } from "../store/authors/authors-types";
const baseUrl = "http://localhost:3001/authors/";

export function getAuthors(): Promise<Author[]> {
  return fetch(baseUrl).then(response => response.json());
}
