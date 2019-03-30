import React, { Component } from "react";
import { Route, Switch } from "react-router-dom";
import HomeComponent from "./components/home/home-component";
import AboutComponent from "./components/about/about-component";
import HeaderComponent from "./components/header/header-component";
import PageNotFoundComponent from "./components/page-not-found/page-not-found-component";

class App extends Component {
  render() {
    return (
      <div className="container-fluid">
        <HeaderComponent />
        <Switch>
          <Route exact path="/" component={HomeComponent} />
          <Route path="/about" component={AboutComponent} />
          <Route component={PageNotFoundComponent} />
        </Switch>
      </div>
    );
  }
}

export default App;
