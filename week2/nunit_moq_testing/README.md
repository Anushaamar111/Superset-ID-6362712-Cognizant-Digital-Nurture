# Mocking in Unit Testing with Moq - Complete Guide

## Overview
This project demonstrates how mocking enhances Test-Driven Development (TDD) by allowing you to isolate units under test from their dependencies. It includes practical examples of mocking mail services, database operations, and file system operations.

## Key Concepts

### What is Mocking?
Mocking is a technique in unit testing where you create fake objects that simulate the behavior of real dependencies. This allows you to:

1. **Isolate the unit under test** - Test only specific functionality without external dependencies
2. **Control behavior** - Define exactly how dependencies should behave during tests
3. **Verify interactions** - Check if methods were called with expected parameters
4. **Speed up tests** - Avoid slow operations like database calls or network requests
5. **Make tests reliable** - Not affected by external system states

### Test Doubles: Mock vs Fake vs Stub

- **Mock**: Objects that verify interactions (method calls, parameters) and can simulate behavior
- **Stub**: Objects that provide predefined responses to method calls
- **Fake**: Objects with working implementations but simplified (e.g., in-memory database)

### Dependency Injection (DI)
DI is a design pattern where dependencies are provided to a class rather than created within it:

- **Constructor Injection**: Dependencies passed through constructor (most common)
- **Method Injection**: Dependencies passed as method parameters
- **Property Injection**: Dependencies set through properties

## Project Structure

### CustomerCommLib (Class Library)
Contains the business logic classes with dependency injection:

1. **MailSender.cs**
   - `IMailSender` interface for mail operations
   - `MailSender` class with real SMTP implementation
   - `CustomerComm` class that uses dependency injection

2. **CustomerService.cs**
   - `ICustomerRepository` interface for database operations
   - `CustomerRepository` class with real database implementation
   - `CustomerService` class demonstrating database-dependent business logic

3. **FileServices.cs**
   - `IFileService` interface for file system operations
   - `FileService` class with real file system implementation
   - `LogService` and `ConfigurationService` classes demonstrating file-dependent operations

### CustomerComm.Tests (Test Project)
Contains comprehensive unit tests demonstrating different mocking scenarios:

1. **CustomerCommTests.cs** - Basic mail service mocking
2. **CustomerServiceTests.cs** - Database operation mocking
3. **FileServiceTests.cs** - File system operation mocking

## Key Benefits of Mocking in TDD

### 1. Fast Test Execution
- No need to set up real databases, file systems, or network connections
- Tests run in milliseconds rather than seconds or minutes

### 2. Reliable Tests
- Tests are not affected by external system states
- No flaky tests due to network issues or file system permissions

### 3. Isolated Testing
- Each unit is tested independently
- Failures clearly indicate which component has issues

### 4. Better Design
- Forces loose coupling between components
- Promotes dependency injection and interface-based design

### 5. Comprehensive Coverage
- Can test error scenarios that are hard to reproduce with real systems
- Can simulate various states and conditions

## Moq Framework Features Demonstrated

### Basic Setup and Verification
```csharp
// Create mock
var mockMailSender = new Mock<IMailSender>();

// Configure behavior
mockMailSender.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>()))
              .Returns(true);

// Verify calls
mockMailSender.Verify(x => x.SendMail("test@test.com", "message"), Times.Once);
```

### Parameter Matching
- `It.IsAny<T>()` - Matches any value of type T
- `It.Is<T>(predicate)` - Matches values that satisfy a condition
- Specific values - Exact parameter matching

### Return Value Configuration
- `Returns(value)` - Return specific value
- `Returns(func)` - Return value from function
- `Throws(exception)` - Throw exception

### Verification Options
- `Times.Once` - Called exactly once
- `Times.Never` - Never called
- `Times.AtLeast(n)` - Called at least n times
- `Times.Between(min, max, Range.Inclusive)` - Called within range

### Callbacks
```csharp
mockService.Setup(x => x.Method(It.IsAny<string>()))
           .Callback<string>(param => capturedValue = param)
           .Returns(true);
```

## Test Examples Included

### 1. Mail Service Mocking (`CustomerCommTests.cs`)
- Basic mock setup and verification
- Parameter validation
- Failure scenario testing
- Parameterized tests with TestCase attributes

### 2. Database Mocking (`CustomerServiceTests.cs`)
- Mocking entity retrieval operations
- Mocking save/create operations
- Testing null return scenarios
- Exception handling in database operations
- Complex business logic testing

### 3. File System Mocking (`FileServiceTests.cs`)
- File creation and writing operations
- File reading and parsing operations
- File existence checking
- File deletion operations
- Configuration file management

## Best Practices Demonstrated

### 1. Test Structure (Arrange-Act-Assert)
```csharp
[Test]
public void Method_Scenario_ExpectedResult()
{
    // Arrange - Set up mocks and test data
    mockService.Setup(x => x.Method()).Returns(expectedValue);
    
    // Act - Call the method under test
    var result = systemUnderTest.Method();
    
    // Assert - Verify results and interactions
    Assert.That(result, Is.EqualTo(expectedValue));
    mockService.Verify(x => x.Method(), Times.Once);
}
```

### 2. Mock Reset Between Tests
```csharp
[SetUp]
public void SetUp()
{
    mockService.Reset(); // Ensures clean state for each test
}
```

### 3. Descriptive Test Names
Test names follow the pattern: `MethodName_Scenario_ExpectedBehavior`

### 4. Comprehensive Test Coverage
- Happy path scenarios
- Error conditions
- Edge cases
- Boundary conditions

## Running the Tests

1. **Build the solution:**
   ```
   dotnet build
   ```

2. **Run all tests:**
   ```
   dotnet test
   ```

3. **Run specific test class:**
   ```
   dotnet test --filter "CustomerCommTests"
   ```

## Test Results Summary
- **Total Tests**: 23
- **Passed**: 23
- **Failed**: 0
- **Execution Time**: ~6.5 seconds

All tests demonstrate successful mocking of external dependencies without requiring actual mail servers, databases, or file systems.

## Key Takeaways

1. **Mocking enables true unit testing** by isolating the code under test
2. **Dependency injection is essential** for creating testable code
3. **Moq provides powerful features** for configuring mock behavior and verifying interactions
4. **Well-structured tests** improve code quality and maintainability
5. **Comprehensive test coverage** includes both success and failure scenarios

This project serves as a complete reference for implementing mocking in .NET applications using the Moq framework with NUnit testing.
