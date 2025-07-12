# Hands on 4 - Web API CRUD Operations

## Overview
This implementation focuses on enhanced CRUD operations, specifically the PUT method for updating employee data with comprehensive validation and error handling.

## Key Features Implemented

### ✅ Enhanced PUT Method Requirements:
1. **ID Validation**: Checks if ID <= 0 and returns BadRequest with "Invalid employee id"
2. **Existence Validation**: Checks if ID exists in the employee list
3. **Data Update**: Updates employee using JSON input from request body
4. **Return Type**: Returns `ActionResult<Employee>` with updated employee data
5. **Filtering**: Filters and returns the specific updated employee

### ✅ Complete CRUD Operations:
- **CREATE** (POST): Add new employees with auto-generated IDs
- **READ** (GET): Retrieve all employees or by specific ID
- **UPDATE** (PUT): Update existing employees with validation
- **DELETE** (DELETE): Remove employees with validation

### ✅ Error Handling:
- **400 Bad Request**: For invalid IDs (≤ 0 or non-existent)
- **404 Not Found**: For GET operations on non-existent employees
- **500 Internal Server Error**: For unexpected errors

### ✅ Response Types:
- Proper `ProducesResponseType` attributes for all status codes
- Structured error messages in JSON format
- Consistent response patterns

## API Endpoints

### Employee CRUD Controller (`/api/employeecrud`)

| Method | Endpoint | Description | Validation |
|--------|----------|-------------|------------|
| GET | `/api/employeecrud` | Get all employees | None |
| GET | `/api/employeecrud/{id}` | Get employee by ID | ID > 0, must exist |
| POST | `/api/employeecrud` | Create new employee | Employee data required |
| PUT | `/api/employeecrud/{id}` | Update employee | ID > 0, must exist, data required |
| DELETE | `/api/employeecrud/{id}` | Delete employee | ID > 0, must exist |
| POST | `/api/employeecrud/reset` | Reset to default data | None |

## PUT Method Implementation Details

### Validation Flow:
1. **Check ID ≤ 0**: Returns `BadRequest("Invalid employee id")`
2. **Check employee data null**: Returns `BadRequest("Employee data is required")`
3. **Check employee exists**: Returns `BadRequest("Invalid employee id")`
4. **Update data**: Updates all provided fields
5. **Return result**: Filters and returns updated employee

### Request Format:
```json
{
  "name": "Updated Name",
  "salary": 85000,
  "permanent": true,
  "dateOfBirth": "1990-05-15T00:00:00",
  "department": {
    "id": 1,
    "name": "IT",
    "description": "Information Technology"
  },
  "skills": [
    {
      "id": 1,
      "name": "C#",
      "level": "Expert",
      "yearsOfExperience": 6
    }
  ]
}
```

### Response Format:
```json
{
  "id": 1,
  "name": "Updated Name",
  "salary": 85000,
  "permanent": true,
  "dateOfBirth": "1990-05-15T00:00:00",
  "department": {
    "id": 1,
    "name": "IT",
    "description": "Information Technology"
  },
  "skills": [...]
}
```

## Testing Scenarios

### ✅ Valid Update Tests:
- Update employee with ID 1, 2, 3, 4
- Partial data updates
- Complete data updates with department and skills

### ✅ Invalid ID Tests:
- ID = 0 (should return BadRequest)
- ID = -5 (should return BadRequest)
- ID = 999 (non-existent, should return BadRequest)

### ✅ Data Validation Tests:
- Missing employee data
- Null values handling
- Complex object updates (Department, Skills)

## How to Test

1. **Start the API**: Ensure the Web API is running on `http://localhost:5227`

2. **Use Swagger UI**: Navigate to `http://localhost:5227/swagger`
   - Expand the EmployeeCrud section
   - Test the PUT method with various scenarios

3. **Use HTTP Test File**: 
   - Open `HandsOn4_Tests.http` in VS Code
   - Click "Send Request" on any test case
   - Observe the responses and status codes

4. **Use POSTMAN**:
   - Import the API endpoints
   - Test with valid and invalid scenarios
   - Verify response codes and messages

## Expected Results

### Valid PUT Request (ID = 1):
- **Status**: 200 OK
- **Response**: Updated employee object
- **Verification**: GET request returns updated data

### Invalid PUT Request (ID ≤ 0):
- **Status**: 400 Bad Request
- **Response**: `{"message": "Invalid employee id"}`

### Invalid PUT Request (ID not found):
- **Status**: 400 Bad Request  
- **Response**: `{"message": "Invalid employee id"}`

## File Structure
```
week4/
├── Hands on 4/
│   ├── README.md                    ← This documentation
│   └── HandsOn4_Tests.http         ← HTTP test cases
└── controllers/
    └── EmployeeCrudController.cs   ← Enhanced CRUD controller
```

This implementation provides a robust foundation for Web API CRUD operations with comprehensive validation, error handling, and testing capabilities.
