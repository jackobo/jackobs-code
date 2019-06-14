import React from "react";

interface FunctionAsChildProps {
  children: (count: number) => React.ReactElement;
}

const FunctionAsChild: React.FC<FunctionAsChildProps> = props => {
  return props.children(100);
};

export default FunctionAsChild;
