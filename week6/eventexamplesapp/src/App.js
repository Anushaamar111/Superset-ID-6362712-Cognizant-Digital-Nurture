import React, { Component } from 'react';
import CurrencyConvertor from './CurrencyConverter';

class App extends Component {
  constructor(props) {
    super(props);
    this.state = {
      counter: 0
    };
  }

  increment = () => {
    this.setState({ counter: this.state.counter + 1 });
    alert("Counter incremented");
  }

  decrement = () => {
    this.setState({ counter: this.state.counter - 1 });
  }

  sayWelcome = (msg) => {
    alert(msg);
  }

  onPress = () => {
    alert("I was clicked");
  }

  render() {
    return (
      <div style={{ padding: '20px' }}>
        <h2>React Event Examples</h2>
        <p>Counter: {this.state.counter}</p>
        <button onClick={this.increment}>Increment</button>
        <button onClick={this.decrement}>Decrement</button>
        <br /><br />
        <button onClick={() => this.sayWelcome("welcome")}>Say Welcome</button>
        <br /><br />
        <button onClick={this.onPress}>Click on me</button>

        <hr />
        <CurrencyConvertor />
      </div>
    );
  }
}

export default App;
