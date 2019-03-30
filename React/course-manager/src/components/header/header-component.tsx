import React from "react";
import { NavLink } from "react-router-dom";

const HeaderComponent: React.FunctionComponent = () => {
  const activeStyle = { color: "#F15B2A" };
  return (
    <nav>
      <NavLink to="/" exact activeStyle={activeStyle}>
        Home
      </NavLink>
      {" | "}
      <NavLink to="/about" activeStyle={activeStyle}>
        About
      </NavLink>
    </nav>
  );
};

export default HeaderComponent;
