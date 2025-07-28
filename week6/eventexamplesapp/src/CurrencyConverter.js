import React, { Component } from 'react';

class CurrencyConvertor extends Component {
  constructor(props) {
    super(props);
    this.state = {
      amount: '',
      currency: '',
      converted: ''
    };
  }

  handleChange = (e) => {
    this.setState({ [e.target.name]: e.target.value });
  }

  handleSubmit = (e) => {
    e.preventDefault();
    const { amount, currency } = this.state;

    if (currency.toLowerCase() === 'euro') {
      const converted = (parseFloat(amount) / 90).toFixed(2);
      alert(`Converting ${amount} rupees to ${converted} euro`);
    } else {
      alert("Only Euro supported currently.");
    }
  }

  render() {
    return (
      <div>
        <h2 style={{ color: 'green' }}>Currency Convertor!!!</h2>
        <form onSubmit={this.handleSubmit}>
          <label>
            Amount: 
            <input
              type="text"
              name="amount"
              value={this.state.amount}
              onChange={this.handleChange}
            />
          </label>
          <br />
          <label>
            Currency: 
            <input
              type="text"
              name="currency"
              value={this.state.currency}
              onChange={this.handleChange}
            />
          </label>
          <br />
          <button type="submit">Submit</button>
        </form>
      </div>
    );
  }
}

export default CurrencyConvertor;
