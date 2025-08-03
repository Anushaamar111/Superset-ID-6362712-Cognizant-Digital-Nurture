import React, { Component } from 'react';

class Getuser extends Component {
  state = {
    person: null,
    loading: true,
  };

  async componentDidMount() {
    const url = "https://api.randomuser.me/";
    try {
      const response = await fetch(url);
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      const data = await response.json();
      this.setState({ person: data.results[0], loading: false });
      console.log(data.results[0]);
    } catch (error) {
      console.error("Failed to fetch user:", error);
      this.setState({ loading: false });
    }
  }

  render() {
    if (this.state.loading) {
      return <div>Loading...</div>;
    }

    if (!this.state.person) {
      return <div>Failed to load user.</div>;
    }

    const { title, first, last } = this.state.person.name;
    const { large } = this.state.person.picture;

    return (
      <div>
        <h1>{`${title} ${first} ${last}`}</h1>
        <img src={large} alt={`${title} ${first} ${last}`} />
      </div>
    );
  }
}

export default Getuser;