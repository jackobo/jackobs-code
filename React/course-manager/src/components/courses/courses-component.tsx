import React from "react";

interface Course {
  title: string;
}

export default class CoursesComponent extends React.Component<Course> {
  state: Course;

  constructor(props: Course) {
    super(props);
    this.state = { ...props, title: "" };
  }

  handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    this.setState({ ...this.state, title: event.target.value });
  };

  handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
  };

  render() {
    return (
      <form onSubmit={this.handleSubmit}>
        <h2>Courses</h2>
        <h3>Add course</h3>
        <input
          type="text"
          onChange={this.handleChange.bind(this)}
          value={this.state.title}
        />

        <input type="submit" value="Save" />
      </form>
    );
  }
}
