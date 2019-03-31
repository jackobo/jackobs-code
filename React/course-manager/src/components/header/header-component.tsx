import React from "react";
import Link from "./header-link-component";
import Separator from "./nav-link-separator-component";

const HeaderComponent: React.FunctionComponent = () => {
  const activeStyle = { color: "#F15B2A" };
  return (
    <nav>
      <Link to="/" exact={true} text="Home" />
      <Separator />
      <Link to="/courses" text="Courses" />
      <Separator />
      <Link to="/about" text="About" />
    </nav>
  );
};

export default HeaderComponent;
