import Action from "../actions/action";
import Course from "../../components/courses/course";
import ACTION_TYPES from "../actions/action-types";

export default function createCourseReducer(state: any, action: Action<Course>): any {
    if(action.type !== ACTION_TYPES.CREATE_COURSE) {
        return state;
    }

    return {...state, courses: [...state.courses, action.payload]};
}