import Course from '../../components/courses/course';
import Action from './action';
import ACTION_TYPES from './action-types';

export function createCourseAction(course: Course): Action<Course> {
    return {
        type: ACTION_TYPES.CREATE_COURSE,
        payload: course
    }
}