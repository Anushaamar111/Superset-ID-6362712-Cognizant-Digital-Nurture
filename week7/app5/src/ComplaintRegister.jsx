import React, { Component } from 'react';

class ComplaintRegister extends Component {
  constructor(props) {
    super(props);
    this.state = {
      ename: '',
      complaint: '',
      NumberHolder: 1
    };
  }

  handleChange = (event) => {
    this.setState({ [event.target.name]: event.target.value });
  }

  handleSubmit = (event) => {
    event.preventDefault();
    const randomNumber = Math.floor(Math.random() * 100) + 1;
    this.setState({ NumberHolder: randomNumber }, () => {
        var msg = `Thanks ${this.state.ename}\nYour Complaint was Submitted.\nTransaction ID is: ${this.state.NumberHolder}`;
        alert(msg);
    });
  }

  render() {
    return (
      <div>
        <h2>Register your complaints here!!!</h2>
        <form onSubmit={this.handleSubmit}>
          <div>
            <label>Name:</label>
            <input
              type="text"
              name="ename"
              value={this.state.ename}
              onChange={this.handleChange}
            />
          </div>
          <div>
            <label>Complaint:</label>
            <textarea
              name="complaint"
              value={this.state.complaint}
              onChange={this.handleChange}
            />
          </div>
          <button type="submit">Submit</button>
        </form>
      </div>
    );
  }
}

export default ComplaintRegister;