# JWT Authentication Demo - ASP.NET Core

A comprehensive JWT (JSON Web Token) authentication implementation in ASP.NET Core demonstrating secure API endpoints, user authentication, and authorization.

## 🌟 Features

- **JWT Token Generation**: Secure token creation with configurable expiration
- **User Authentication**: Login/Register endpoints with validation
- **Protected Routes**: Secure API endpoints requiring valid JWT tokens
- **Role-Based Access**: Support for different user roles (Admin, User)
- **Swagger Integration**: Interactive API documentation with JWT support
- **Comprehensive Testing**: HTTP requests for all endpoints

## 🏗️ Project Structure

```
JwtAuthDemo/
├── Controllers/
│   ├── AuthController.cs       # Authentication endpoints (login/register)
│   └── SecureController.cs     # Protected endpoints requiring JWT
├── Models/
│   └── LoginModel.cs          # Data models for authentication
├── appsettings.json           # JWT configuration
├── Program.cs                 # Application configuration
├── JwtAuthDemo.csproj        # Project dependencies
└── JwtAuthDemo.http          # HTTP test requests
```

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- Basic understanding of JWT concepts

### Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Access Swagger UI**:
   Open browser to `https://localhost:7043/swagger`

## 🔐 JWT Configuration

The JWT settings are configured in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "ThisIsASecretKeyForJwtTokenGeneration2024",
    "Issuer": "JwtAuthDemo",
    "Audience": "JwtAuthDemoUsers",
    "DurationInMinutes": 60
  }
}
```

### Security Settings Explained:

- **SecretKey**: Used to sign and verify JWT tokens (should be complex in production)
- **Issuer**: Who created and signed the token
- **Audience**: Who the token is intended for
- **DurationInMinutes**: Token expiration time

## 📋 API Endpoints

### Authentication Endpoints (Public)

#### POST `/api/auth/login`
Authenticate user and receive JWT token.

**Request Body**:
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "expiresAt": "2024-01-20T15:30:00Z"
}
```

#### POST `/api/auth/register`
Register a new user account.

**Request Body**:
```json
{
  "username": "newuser",
  "password": "newpass123"
}
```

#### GET `/api/auth/test-users`
Get list of available test users.

**Response**:
```json
[
  { "username": "admin", "role": "Admin" },
  { "username": "user", "role": "User" },
  { "username": "demo", "role": "User" }
]
```

### Protected Endpoints (Require JWT)

#### GET `/api/secure/data`
Access protected data (requires valid JWT token).

**Headers**:
```
Authorization: Bearer <your-jwt-token>
```

**Response**:
```json
"🔒 This is protected data."
```

#### GET `/api/secure/user-info`
Get current user information from JWT claims.

**Headers**:
```
Authorization: Bearer <your-jwt-token>
```

**Response**:
```json
{
  "message": "User information retrieved successfully",
  "userId": "admin",
  "username": "admin",
  "claims": [
    { "type": "userId", "value": "admin" },
    { "type": "username", "value": "admin" },
    { "type": "role", "value": "Admin" }
  ]
}
```

#### GET `/api/secure/public`
Public endpoint (no authentication required).

**Response**:
```json
"🌐 This is public data - no authentication required."
```

## 🧪 Testing the API

### Using Swagger UI

1. Navigate to `https://localhost:7043/swagger`
2. Click on `/api/auth/login` endpoint
3. Click "Try it out"
4. Enter credentials:
   ```json
   {
     "username": "admin",
     "password": "admin123"
   }
   ```
5. Click "Execute"
6. Copy the token from the response
7. Click the "Authorize" button at the top
8. Enter: `Bearer <your-copied-token>`
9. Now you can access protected endpoints

### Using HTTP File

The `JwtAuthDemo.http` file contains pre-configured requests:

1. First, login to get a token:
   ```http
   POST https://localhost:7043/api/auth/login
   Content-Type: application/json
   
   {
     "username": "admin",
     "password": "admin123"
   }
   ```

2. Copy the token and replace `YOUR_JWT_TOKEN` in subsequent requests:
   ```http
   GET https://localhost:7043/api/secure/data
   Authorization: Bearer YOUR_JWT_TOKEN
   ```

### Test Users

The application comes with pre-configured test users:

| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Admin |
| user | user123 | User |
| demo | demo123 | User |

## 🔧 JWT Token Structure

A typical JWT token contains three parts separated by dots:

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJhZG1pbiIsInVzZXJuYW1lIjoiYWRtaW4iLCJyb2xlIjoiQWRtaW4iLCJleHAiOjE2NDA5ODc0MDB9.signature
```

1. **Header** (Algorithm & Token Type):
   ```json
   {
     "alg": "HS256",
     "typ": "JWT"
   }
   ```

2. **Payload** (Claims):
   ```json
   {
     "userId": "admin",
     "username": "admin",
     "role": "Admin",
     "exp": 1640987400
   }
   ```

3. **Signature** (Verification):
   ```
   HMACSHA256(
     base64UrlEncode(header) + "." +
     base64UrlEncode(payload),
     secret
   )
   ```

## 🛡️ Security Features

### Token Validation

The application validates JWT tokens on every protected request:

- **Signature Verification**: Ensures token hasn't been tampered with
- **Expiration Check**: Rejects expired tokens
- **Issuer Validation**: Verifies token came from trusted source
- **Audience Validation**: Confirms token is intended for this API

### Best Practices Implemented

1. **Secure Secret Key**: Use a complex, environment-specific secret
2. **Token Expiration**: Tokens expire after 1 hour by default
3. **Claims-Based Authorization**: User information stored in token claims
4. **HTTPS Only**: Tokens should only be transmitted over HTTPS
5. **Input Validation**: Request models include validation attributes

## 🔍 Troubleshooting

### Common Issues

1. **401 Unauthorized**:
   - Check if token is included in Authorization header
   - Verify token format: `Bearer <token>`
   - Ensure token hasn't expired

2. **Token Validation Failed**:
   - Check JWT settings in appsettings.json
   - Verify secret key matches between token generation and validation
   - Confirm issuer and audience settings

3. **Invalid Credentials**:
   - Use correct test user credentials
   - Check username/password formatting

### Debug Tips

1. **Decode JWT Token**: Use [jwt.io](https://jwt.io) to inspect token contents
2. **Check Console Logs**: Look for authentication/authorization errors
3. **Swagger Testing**: Use Swagger UI for interactive testing
4. **Network Tab**: Monitor HTTP requests/responses in browser dev tools

## 📈 Production Considerations

### Security Enhancements

1. **Password Hashing**: Use BCrypt or similar for password storage
2. **Database Integration**: Replace in-memory user store with database
3. **Refresh Tokens**: Implement token refresh mechanism
4. **Rate Limiting**: Add rate limiting to authentication endpoints
5. **CORS Configuration**: Configure CORS for cross-origin requests

### Environment Configuration

```json
{
  "JwtSettings": {
    "SecretKey": "${JWT_SECRET_KEY}",  // Use environment variable
    "Issuer": "${JWT_ISSUER}",
    "Audience": "${JWT_AUDIENCE}",
    "DurationInMinutes": 15            // Shorter expiration in production
  }
}
```

### Monitoring and Logging

- Log authentication attempts
- Monitor token usage patterns
- Track failed authorization attempts
- Set up alerts for security events

## 📚 Learning Outcomes

This demo teaches:

- **JWT Fundamentals**: Token structure, claims, and validation
- **ASP.NET Core Security**: Authentication and authorization middleware
- **API Security**: Protecting endpoints with bearer tokens
- **Swagger Integration**: Documenting and testing secure APIs
- **Configuration Management**: Using appsettings for security settings

## 🎯 Next Steps

To extend this demo:

1. **Add Role-Based Authorization**: Implement `[Authorize(Roles = "Admin")]`
2. **Database Integration**: Connect to SQL Server or other database
3. **Password Security**: Implement proper password hashing
4. **Refresh Tokens**: Add token refresh functionality
5. **Email Verification**: Add email confirmation for registration
6. **Two-Factor Authentication**: Implement 2FA for enhanced security

---

**Happy Coding! 🔐**
