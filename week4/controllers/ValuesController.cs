using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// Get all values
        /// </summary>
        /// <returns>List of values</returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2", "value3" };
        }

        /// <summary>
        /// Get value by ID
        /// </summary>
        /// <param name="id">Value ID</param>
        /// <returns>Value</returns>
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return $"value{id}";
        }

        /// <summary>
        /// Create a new value
        /// </summary>
        /// <param name="value">Value to create</param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            // Implementation for creating a value
        }

        /// <summary>
        /// Update a value
        /// </summary>
        /// <param name="id">Value ID</param>
        /// <param name="value">Updated value</param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            // Implementation for updating a value
        }

        /// <summary>
        /// Delete a value
        /// </summary>
        /// <param name="id">Value ID</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            // Implementation for deleting a value
        }
    }
}
