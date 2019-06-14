import React from "react";

interface FooterProps {
  copyright: string;
}

const Footer: React.FC<FooterProps> = props => {
  return <p>&copy; {props.copyright}</p>;
};

export default Footer;
