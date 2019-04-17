import React from "react";
import Course from './course';


interface CourseComponentState {
  course: Course;
}

export default class CoursesComponent extends React.Component<Course> {
 
  state: CourseComponentState = {
    course: {
      title: ''
    } 
  };

  constructor(props: Course) {
    super(props);
  }

  handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const course = {...this.state.course, title: event.target.value};
    this.setState({...this.state, course: course});
  };

  handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    alert(this.state.course.title);
  };

  render() {
    return (
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
    );
  }
}
