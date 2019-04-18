import {createStore, applyMiddleware, compose} from 'redux';
import {rootReducer } from './reducers/root-reducer';
import Course from '../components/courses/course';
import reduxImmutaleStateInvariant from 'redux-immutable-state-invariant';

export interface Store {
    courses: Course[]
}

export default function configureStore() {
    const composeEnhancers: any = (window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
    return createStore(rootReducer, composeEnhancers(applyMiddleware(reduxImmutaleStateInvariant())));
}