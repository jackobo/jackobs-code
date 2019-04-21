import React from "react";
import { Course } from "../../store/courses/courses-types";
import { connect } from "react-redux";
import { AppState } from "../../store/app-state";
import {
  createCourse,
  deleteCourse,
  loadCourses
} from "../../store/courses/courses-actions";

interface CoursesComponentStateToProps {
  courses: Course[];
}

interface CoursesActions {
  createCourse: (course: Course) => void;
  deleteCourse: (course: Course) => void;
  loadCourses: () => void;
}

type CoursesComponentProps = CoursesComponentStateToProps & CoursesActions;

class CoursesComponent extends React.Component<CoursesComponentProps> {
  componentDidMount() {
    this.props.loadCourses();
  }
  render() {
    return (
      <>
        <h1>Courses</h1>
        {this.props.courses.map(course => (
          <div key={course.id}>
            <button onClick={() => this.props.deleteCourse(course)}>
              Delete
            </button>{" "}
            {course.title}
          </div>
        ))}
      </>
    );
  }
}

function mapStateToProps(appState: AppState): CoursesComponentStateToProps {
  return {
    courses: Object.values(appState.courses)
  };
}

const mapDispatchToProps: CoursesActions = {
  createCourse,
  deleteCourse,
  loadCourses
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(CoursesComponent);
