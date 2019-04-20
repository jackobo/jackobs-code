import React from "react";
import { Course } from "../../store/courses/types";
import { connect } from "react-redux";
import { AppState } from "../../store/app-state";
import { createCourse } from "../../store/courses/actions";

interface CourseComponentState {
  course: Course;
}

interface CoursesComponentStateToProps {
  courses: Course[];
}

interface CoursesActions {
  createCourse: (course: Course) => void;
}

type CoursesComponentProps = CoursesComponentStateToProps & CoursesActions;

class CoursesComponent extends React.Component<CoursesComponentProps> {
  state: CourseComponentState = {
    course: {
      id: "",
      title: ""
    }
  };

  constructor(props: CoursesComponentProps) {
    super(props);
  }

  handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const course = { ...this.state.course, title: event.target.value };
    this.setState({ ...this.state, course: course });
  };

  handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    this.props.createCourse({
      id: this.state.course.title,
      title: this.state.course.title
    });
  };

  render() {
    return (
      <div>
        <form onSubmit={this.handleSubmit}>
          <h2>Courses</h2>
          <h3>Add course</h3>
          <input
            type="text"
            onChange={this.handleChange}
            value={this.state.course.title}
          />

          <input type="submit" value="Save" />
        </form>

        {this.props.courses.map(course => (
          <div key={course.title}>{course.title}</div>
        ))}
      </div>
    );
  }
}

function mapStateToProps(appState: AppState): CoursesComponentStateToProps {
  return {
    courses: Object.values(appState.courses)
  };
}

const mapDispatchToProps: CoursesActions = {
  createCourse
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(CoursesComponent);
