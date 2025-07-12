# Hands on 5: JWT Authentication Implementation

## Overview
This hands-on exercise implements JWT (JSON Web Token) authentication for the .NET Core Web API project. It covers all aspects of JWT authentication including token generation, validation, expiration, and role-based authorization.

## Features Implemented

### 1. JWT Authentication Configuration
- **JWT Configuration**: Added JWT authentication middleware with proper token validation
- **Security Settings**: Configured issuer, audience, and signing key validation
- **Token Validation**: Implemented comprehensive token validation parameters
- **Swagger Integration**: Added JWT authentication to Swagger UI for easy testing

### 2. Authentication Controller (`AuthController`)
- **Login Endpoint**: `POST /api/auth/login` - Authenticate users and generate JWT tokens
- **User Management**: In-memory user store with different roles (Admin, Manager, User)
- **Token Generation**: JWT tokens with proper claims and expiration
- **Current User Info**: `GET /api/auth/me` - Get current authenticated user information
- **User Listing**: `GET /api/auth/users` - Admin-only endpoint to list all users
- **Token Refresh**: `POST /api/auth/refresh` - Refresh expired tokens

### 3. Test Authentication Controller (`TestAuthController`)
- **Short-lived Tokens**: Generates JWT tokens with 2-minute expiration for testing
- **Expiration Testing**: Allows testing of token expiration scenarios
- **Same User Base**: Uses the same user credentials as the main AuthController

### 4. Role-Based Authorization
- **Employee Controller**: Updated to use JWT authentication instead of custom auth filter
- **Role Restrictions**: 
  - GET operations: All authenticated users
  - POST/PUT operations: Admin and Manager roles only
  - DELETE operations: Admin role only
- **Test Role Controller**: Dedicated controller for testing different role combinations

### 5. JWT Token Features
- **Claims**: User ID, username, role, email, and full name
- **Expiration**: Configurable token expiration (default 60 minutes, 2 minutes for testing)
- **Security**: HMAC SHA256 signing algorithm
- **Validation**: Comprehensive token validation including lifetime, issuer, audience, and signature

## API Endpoints

### Authentication Endpoints
```
POST /api/auth/login           - Login and get JWT token
GET  /api/auth/me              - Get current user info (authenticated)
GET  /api/auth/users           - Get all users (Admin only)
POST /api/auth/refresh         - Refresh JWT token (authenticated)
```

### Test Authentication Endpoints
```
POST /api/testauth/login       - Login with 2-minute token expiration
```

### Employee Endpoints (JWT Protected)
```
GET    /api/employee           - Get all employees (authenticated)
GET    /api/employee/{id}      - Get employee by ID (authenticated)
POST   /api/employee           - Create employee (Admin, Manager)
PUT    /api/employee/{id}      - Update employee (Admin, Manager)
DELETE /api/employee/{id}      - Delete employee (Admin only)
```

### Test Role Endpoints
```
GET /api/testrole/poc-only     - POC role only (will fail with Admin token)
GET /api/testrole/admin-or-poc - Admin or POC roles (works with Admin token)
GET /api/testrole/admin-only   - Admin role only
GET /api/testrole/authenticated - Any authenticated user
GET /api/testrole/anonymous    - Anonymous access
```

## Test Users
The system includes the following test users:

| Username | Password   | Role    | Access Level |
|----------|-----------|---------|--------------|
| admin    | admin123  | Admin   | Full access to all endpoints |
| manager  | manager123| Manager | Can read, create, update employees |
| user     | user123   | User    | Can only read employees |

## Testing Scenarios

### 1. Authentication Testing
- ✅ Valid login credentials return JWT token
- ✅ Invalid login credentials return 401 Unauthorized
- ✅ Missing credentials return 400 Bad Request
- ✅ JWT token includes proper claims (user info, roles)

### 2. Authorization Testing
- ✅ Requests without JWT token return 401 Unauthorized
- ✅ Requests with invalid JWT token return 401 Unauthorized
- ✅ Requests with valid JWT token but insufficient role return 403 Forbidden
- ✅ Requests with valid JWT token and proper role return 200 OK

### 3. Token Expiration Testing
- ✅ Fresh tokens work correctly
- ✅ Expired tokens return 401 Unauthorized
- ✅ Token expiration can be configured (2 minutes for testing)

### 4. Role-Based Access Testing
- ✅ Admin can access all endpoints
- ✅ Manager can read, create, update but not delete
- ✅ User can only read data
- ✅ POC role testing (should fail with Admin token)

### 5. Swagger Integration Testing
- ✅ Swagger UI includes JWT authentication
- ✅ Can test all endpoints directly from Swagger
- ✅ Proper authorization headers are sent

## Configuration

### JWT Settings (appsettings.json)
```json
{
  "Jwt": {
    "Key": "MySecretKey12345678901234567890123456789012345678901234567890",
    "Issuer": "https://localhost:7000",
    "Audience": "https://localhost:7000",
    "ExpireMinutes": 60
  }
}
```

### Program.cs Configuration
- JWT authentication middleware
- Authorization middleware
- Swagger JWT authentication
- Token validation parameters

## How to Test

### Using HTTP Test Files
1. **AuthController_Tests.http**: Test basic authentication functionality
2. **EmployeeController_JWT_Tests.http**: Test employee operations with JWT
3. **RoleBasedAuth_Tests.http**: Test role-based authorization scenarios

### Using Swagger UI
1. Navigate to `/swagger` endpoint
2. Use the "Authorize" button to enter JWT token
3. Test endpoints directly from the interface

### Using Postman
1. Import the HTTP test files or create requests manually
2. Use the login endpoint to get JWT tokens
3. Add Authorization header with "Bearer {token}" format
4. Test different scenarios and roles

## Testing Steps

### Step 1: Get JWT Tokens
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

### Step 2: Test Protected Endpoints
```http
GET /api/employee
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

### Step 3: Test Role-Based Access
```http
DELETE /api/employee/1
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN_HERE
```

### Step 4: Test Token Expiration
```http
POST /api/testauth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```
Wait 2+ minutes, then test with the expired token.

### Step 5: Test Invalid Scenarios
- Test with no token
- Test with invalid token
- Test with expired token
- Test with insufficient role permissions

## Security Features

### Token Security
- **HMAC SHA256**: Secure signing algorithm
- **Claims Validation**: Proper claims structure and validation
- **Expiration**: Time-based token expiration
- **Issuer/Audience**: Prevents token misuse across different systems

### Role-Based Security
- **Hierarchical Permissions**: Different access levels for different roles
- **Endpoint Protection**: Specific endpoints require specific roles
- **Claim-Based Authorization**: Uses JWT claims for role validation

## File Structure
```
week4/
├── controllers/
│   ├── AuthController.cs           # Main authentication controller
│   ├── TestAuthController.cs       # Test controller with 2-min expiration
│   ├── EmployeeController.cs       # Updated with JWT authentication
│   └── TestRoleController.cs       # Role-based testing controller
├── Models/
│   └── User.cs                     # User and authentication models
├── Hands on 5/
│   ├── README.md                   # This file
│   ├── AuthController_Tests.http   # Authentication tests
│   ├── EmployeeController_JWT_Tests.http # Employee API tests
│   └── RoleBasedAuth_Tests.http    # Role-based authorization tests
├── Program.cs                      # JWT configuration
└── appsettings.json               # JWT settings
```

## Key Learnings

1. **JWT Structure**: Understanding JWT token structure and claims
2. **Authentication vs Authorization**: Clear distinction between authentication and authorization
3. **Role-Based Security**: Implementing different access levels for different user roles
4. **Token Expiration**: Managing token lifecycle and expiration
5. **Security Best Practices**: Proper token validation and security measures

## Next Steps

1. **Database Integration**: Replace in-memory user store with database
2. **Password Hashing**: Implement proper password hashing
3. **Refresh Tokens**: Implement refresh token mechanism
4. **Token Blacklisting**: Add token blacklisting for logout
5. **Rate Limiting**: Add rate limiting to prevent abuse

## Troubleshooting

### Common Issues
1. **401 Unauthorized**: Check if token is valid and not expired
2. **403 Forbidden**: Check if user has the required role
3. **Invalid Token**: Ensure token format is correct (Bearer {token})
4. **Expired Token**: Use TestAuthController for quick expiration testing

### Debug Tips
1. Check token claims in jwt.io
2. Verify token expiration time
3. Check user roles in the authentication response
4. Use Swagger UI for easy testing
5. Check the authorization header format

This implementation provides a comprehensive JWT authentication system that meets all the requirements specified in the hands-on exercise.
