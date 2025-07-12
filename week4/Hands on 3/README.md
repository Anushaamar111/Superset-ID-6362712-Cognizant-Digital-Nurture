# Hands on 3 - Custom Model Class, Authorization Filter, and Exception Filter

## Summary of Implementation

This folder contains the implementation of advanced Web API features including:

### 1. Custom Model Class
- **Employee**: Custom model with complex properties including Department and Skills
- **Department**: Related entity with Id, Name, and Description
- **Skill**: Related entity with Id, Name, Level, and YearsOfExperience

### 2. Custom Authorization Filter
- **CustomAuthFilter**: Validates Authorization header and Bearer token
- Applied to EmployeeController to secure all endpoints
- Returns BadRequest for missing or invalid authorization

### 3. Custom Exception Filter
- **CustomExceptionFilter**: Catches and logs exceptions globally
- Writes exception details to log files in the Logs folder
- Returns structured error responses

### 4. Enhanced Controller Features
- **ProducesResponseType**: Proper response type documentation
- **GetStandardEmployeeList**: Private method to generate test data
- **Exception Testing**: Built-in exception trigger for testing

## Testing Instructions

1. **Access Swagger UI**: http://localhost:5227/swagger
2. **Test Authorization**: Try requests with/without proper Bearer token
3. **Test Exception Handling**: Add `?throwException=true` to GET requests
4. **Check Logs**: Exception details are logged to the `Logs` folder

## API Endpoints

- `GET /api/employee` - Get all employees (requires Authorization)
- `GET /api/employee/standard` - Get standard employee list (requires Authorization)
- `GET /api/employee/{id}` - Get employee by ID (requires Authorization)
- `POST /api/employee` - Create new employee (requires Authorization)
- `PUT /api/employee/{id}` - Update employee (requires Authorization)
- `DELETE /api/employee/{id}` - Delete employee (requires Authorization)

## Authorization Header Format
```
Authorization: Bearer your-token-here
```

## Exception Testing
Add `?throwException=true` query parameter to any GET request to test exception handling.
