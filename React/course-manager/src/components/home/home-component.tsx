import React from "react";
import { Link } from "react-router-dom";

interface HomeComponentProps {
  title: string;
}

const HomeComponent: React.FunctionComponent<HomeComponentProps> = props => {
  return (
    <div className="jumbotron">
      <h1>Pluralsight Administration</h1>
      <p>React, Redux and React ruter for ultra-responsive app</p>
      <Link to="about" className="btn btn-primary btn-lg">
        Learn more
      </Link>
    </div>
  );
};

export default HomeComponent;
