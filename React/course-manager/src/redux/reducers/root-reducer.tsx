import {combineReducers} from 'redux';
import createCourseReducer from './create-course-reducer';

const rootReducer = combineReducers({
    createCourseReducer
});


export {rootReducer};