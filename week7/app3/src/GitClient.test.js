/**
 * @jest-environment node
 */
import axios from 'axios';
import GitClient from './GitClient';

jest.mock('axios');

describe("Git Client Tests", () => {
  test("should return repository names for techiesyed", async () => {
    const dummyRepos = {
      data: [
        { name: "repo1" },
        { name: "repo2" }
      ]
    };
    axios.get.mockResolvedValue(dummyRepos);

    const response = await GitClient.getRepositories('techiesyed');

    expect(response.data.length).toBe(2);
    expect(response.data[0].name).toBe('repo1');
    expect(axios.get).toHaveBeenCalledWith('https://api.github.com/users/techiesyed/repos');
  });
});