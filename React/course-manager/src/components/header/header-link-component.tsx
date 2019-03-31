import React from "react";
import { NavLink } from "react-router-dom";

interface LinkProps {
  to: string;
  text: string;
  exact?: boolean;
}
const HeaderLink: React.FunctionComponent<LinkProps> = (props: LinkProps) => {
  const activeStyle = { color: "#F15B2A" };
  return (
    <NavLink to={props.to} exact={props.exact} activeStyle={activeStyle}>
      {props.text}
    </NavLink>
  );
};

export default HeaderLink;
