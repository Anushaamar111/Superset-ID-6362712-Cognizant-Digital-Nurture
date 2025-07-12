using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeCrudController : ControllerBase
    {
        // Static list to maintain data across requests
        private static List<Employee> _employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                Name = "John Doe",
                Salary = 75000,
                Permanent = true,
                DateOfBirth = new DateTime(1990, 5, 15),
                Department = new Department { Id = 1, Name = "IT", Description = "Information Technology" },
                Skills = new List<Skill>
                {
                    new Skill { Id = 1, Name = "C#", Level = "Advanced", YearsOfExperience = 5 },
                    new Skill { Id = 2, Name = "ASP.NET Core", Level = "Expert", YearsOfExperience = 4 }
                }
            },
            new Employee
            {
                Id = 2,
                Name = "Jane Smith",
                Salary = 65000,
                Permanent = true,
                DateOfBirth = new DateTime(1985, 8, 22),
                Department = new Department { Id = 2, Name = "HR", Description = "Human Resources" },
                Skills = new List<Skill>
                {
                    new Skill { Id = 3, Name = "Communication", Level = "Expert", YearsOfExperience = 8 },
                    new Skill { Id = 4, Name = "Team Management", Level = "Advanced", YearsOfExperience = 6 }
                }
            },
            new Employee
            {
                Id = 3,
                Name = "Bob Johnson",
                Salary = 80000,
                Permanent = false,
                DateOfBirth = new DateTime(1988, 3, 10),
                Department = new Department { Id = 3, Name = "Finance", Description = "Financial Department" },
                Skills = new List<Skill>
                {
                    new Skill { Id = 5, Name = "Excel", Level = "Expert", YearsOfExperience = 7 },
                    new Skill { Id = 6, Name = "Financial Analysis", Level = "Advanced", YearsOfExperience = 5 }
                }
            },
            new Employee
            {
                Id = 4,
                Name = "Alice Brown",
                Salary = 70000,
                Permanent = true,
                DateOfBirth = new DateTime(1992, 12, 5),
                Department = new Department { Id = 1, Name = "IT", Description = "Information Technology" },
                Skills = new List<Skill>
                {
                    new Skill { Id = 7, Name = "JavaScript", Level = "Advanced", YearsOfExperience = 3 },
                    new Skill { Id = 8, Name = "React", Level = "Intermediate", YearsOfExperience = 2 }
                }
            }
        };

        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns>List of all employees</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Employee>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<Employee>> GetAllEmployees()
        {
            return Ok(_employees);
        }

        /// <summary>
        /// Get employee by ID
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Employee> GetEmployee(int id)
        {
            // Check if id is valid
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid employee id" });
            }

            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound(new { message = $"Employee with id {id} not found" });
            }

            return Ok(employee);
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="employee">Employee data</param>
        /// <returns>Created employee</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Employee))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Employee> CreateEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest(new { message = "Employee data is required" });
            }

            // Generate new ID
            employee.Id = _employees.Any() ? _employees.Max(e => e.Id) + 1 : 1;
            
            _employees.Add(employee);
            
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        /// <param name="id">Employee ID to update</param>
        /// <param name="employee">Updated employee data</param>
        /// <returns>Updated employee</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Employee> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            // Check if id is lesser than or equal to 0
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid employee id" });
            }

            // Check if employee data is provided
            if (employee == null)
            {
                return BadRequest(new { message = "Employee data is required" });
            }

            // Find the existing employee
            var existingEmployee = _employees.FirstOrDefault(e => e.Id == id);
            
            // If id is greater than 0 but not available in the list
            if (existingEmployee == null)
            {
                return BadRequest(new { message = "Invalid employee id" });
            }

            // Update the employee data
            existingEmployee.Name = employee.Name ?? existingEmployee.Name;
            existingEmployee.Salary = employee.Salary;
            existingEmployee.Permanent = employee.Permanent;
            existingEmployee.DateOfBirth = employee.DateOfBirth;
            
            // Update Department if provided
            if (employee.Department != null)
            {
                existingEmployee.Department = employee.Department;
            }
            
            // Update Skills if provided
            if (employee.Skills != null && employee.Skills.Any())
            {
                existingEmployee.Skills = employee.Skills;
            }

            // Filter the employee list data for the input id and return that as output
            var updatedEmployee = _employees.FirstOrDefault(e => e.Id == id);
            
            return Ok(updatedEmployee);
        }

        /// <summary>
        /// Delete an employee
        /// </summary>
        /// <param name="id">Employee ID to delete</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteEmployee(int id)
        {
            // Check if id is valid
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid employee id" });
            }

            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return BadRequest(new { message = "Invalid employee id" });
            }

            _employees.Remove(employee);
            return NoContent();
        }

        /// <summary>
        /// Reset employees to default data (for testing purposes)
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult ResetEmployees()
        {
            _employees.Clear();
            _employees.AddRange(GetDefaultEmployees());
            return Ok(new { message = "Employee data reset to default values", count = _employees.Count });
        }

        /// <summary>
        /// Get default employees for reset functionality
        /// </summary>
        /// <returns>List of default employees</returns>
        private List<Employee> GetDefaultEmployees()
        {
            return new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "John Doe",
                    Salary = 75000,
                    Permanent = true,
                    DateOfBirth = new DateTime(1990, 5, 15),
                    Department = new Department { Id = 1, Name = "IT", Description = "Information Technology" },
                    Skills = new List<Skill>
                    {
                        new Skill { Id = 1, Name = "C#", Level = "Advanced", YearsOfExperience = 5 },
                        new Skill { Id = 2, Name = "ASP.NET Core", Level = "Expert", YearsOfExperience = 4 }
                    }
                },
                new Employee
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Salary = 65000,
                    Permanent = true,
                    DateOfBirth = new DateTime(1985, 8, 22),
                    Department = new Department { Id = 2, Name = "HR", Description = "Human Resources" },
                    Skills = new List<Skill>
                    {
                        new Skill { Id = 3, Name = "Communication", Level = "Expert", YearsOfExperience = 8 },
                        new Skill { Id = 4, Name = "Team Management", Level = "Advanced", YearsOfExperience = 6 }
                    }
                },
                new Employee
                {
                    Id = 3,
                    Name = "Bob Johnson",
                    Salary = 80000,
                    Permanent = false,
                    DateOfBirth = new DateTime(1988, 3, 10),
                    Department = new Department { Id = 3, Name = "Finance", Description = "Financial Department" },
                    Skills = new List<Skill>
                    {
                        new Skill { Id = 5, Name = "Excel", Level = "Expert", YearsOfExperience = 7 },
                        new Skill { Id = 6, Name = "Financial Analysis", Level = "Advanced", YearsOfExperience = 5 }
                    }
                },
                new Employee
                {
                    Id = 4,
                    Name = "Alice Brown",
                    Salary = 70000,
                    Permanent = true,
                    DateOfBirth = new DateTime(1992, 12, 5),
                    Department = new Department { Id = 1, Name = "IT", Description = "Information Technology" },
                    Skills = new List<Skill>
                    {
                        new Skill { Id = 7, Name = "JavaScript", Level = "Advanced", YearsOfExperience = 3 },
                        new Skill { Id = 8, Name = "React", Level = "Intermediate", YearsOfExperience = 2 }
                    }
                }
            };
        }
    }
}
