import React from "react";
import "./App.css";
import Header from "./components/header";
import Content from "./components/content";
import Counter from "./components/counter";
import Footer from "./components/footer";
import FunctionAsChild from "./components/function-as-child";

const App: React.FC = () => {
  return (
    <div className="App">
      <Header title="My test app" />

      <Content>
        <p>
          This is my <strong>&lt;Content/&gt;</strong> component
        </p>
        <Counter />
      </Content>

      <FunctionAsChild>
        {(count: number) => <div>This is a Function as child {count}</div>}
      </FunctionAsChild>

      <Footer copyright="Sparkware Technologies 2019" />
    </div>
  );
};

export default App;
