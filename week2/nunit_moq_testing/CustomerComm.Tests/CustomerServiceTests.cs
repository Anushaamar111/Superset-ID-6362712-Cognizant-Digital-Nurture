using NUnit.Framework;
using Moq;
using CustomerCommLib;
using System;
using System.Collections.Generic;

namespace CustomerComm.Tests
{
    /// <summary>
    /// Unit tests demonstrating database mocking with Moq
    /// Shows how to test business logic that depends on database operations without actual database
    /// </summary>
    [TestFixture]
    public class CustomerServiceTests
    {
        private Mock<ICustomerRepository> _mockCustomerRepository;
        private Mock<IMailSender> _mockMailSender;
        private CustomerService _customerService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockMailSender = new Mock<IMailSender>();
            _customerService = new CustomerService(_mockCustomerRepository.Object, _mockMailSender.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _mockCustomerRepository.Reset();
            _mockMailSender.Reset();
        }

        /// <summary>
        /// Test demonstrating how to mock database retrieval operations
        /// </summary>
        [Test]
        public void SendWelcomeEmail_WithValidCustomerId_ShouldSendEmailAndReturnTrue()
        {
            // Arrange
            int customerId = 123;
            var expectedCustomer = new Customer
            {
                Id = customerId,
                Name = "John Doe",
                Email = "john.doe@example.com",
                CreatedDate = DateTime.Now
            };

            // Mock database to return the expected customer
            _mockCustomerRepository.Setup(x => x.GetCustomerById(customerId))
                                  .Returns(expectedCustomer);

            // Mock mail sender to return success
            _mockMailSender.Setup(x => x.SendMail(expectedCustomer.Email, It.IsAny<string>()))
                          .Returns(true);

            // Act
            bool result = _customerService.SendWelcomeEmail(customerId);

            // Assert
            Assert.That(result, Is.True, "SendWelcomeEmail should return true when customer exists and email is sent successfully");

            // Verify database was queried with correct customer ID
            _mockCustomerRepository.Verify(x => x.GetCustomerById(customerId), Times.Once);

            // Verify email was sent to correct customer with welcome message
            _mockMailSender.Verify(x => x.SendMail(expectedCustomer.Email, 
                                                  It.Is<string>(msg => msg.Contains("Welcome") && msg.Contains(expectedCustomer.Name))), 
                                  Times.Once);
        }

        /// <summary>
        /// Test demonstrating how to mock database operations that return null
        /// </summary>
        [Test]
        public void SendWelcomeEmail_WithInvalidCustomerId_ShouldReturnFalse()
        {
            // Arrange
            int invalidCustomerId = 999;

            // Mock database to return null (customer not found)
            _mockCustomerRepository.Setup(x => x.GetCustomerById(invalidCustomerId))
                                  .Returns((Customer)null);

            // Act
            bool result = _customerService.SendWelcomeEmail(invalidCustomerId);

            // Assert
            Assert.That(result, Is.False, "SendWelcomeEmail should return false when customer is not found");

            // Verify database was queried
            _mockCustomerRepository.Verify(x => x.GetCustomerById(invalidCustomerId), Times.Once);

            // Verify email was NOT sent (since customer doesn't exist)
            _mockMailSender.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Test demonstrating how to mock database save operations
        /// </summary>
        [Test]
        public void CreateCustomerAndSendWelcome_WithValidData_ShouldSaveCustomerAndSendEmail()
        {
            // Arrange
            string customerName = "Jane Smith";
            string customerEmail = "jane.smith@example.com";

            // Mock database save to return success
            _mockCustomerRepository.Setup(x => x.SaveCustomer(It.IsAny<Customer>()))
                                  .Returns(true);

            // Mock mail sender to return success
            _mockMailSender.Setup(x => x.SendMail(customerEmail, It.IsAny<string>()))
                          .Returns(true);

            // Act
            bool result = _customerService.CreateCustomerAndSendWelcome(customerName, customerEmail);

            // Assert
            Assert.That(result, Is.True, "CreateCustomerAndSendWelcome should return true when customer is saved and email is sent");

            // Verify customer was saved with correct data
            _mockCustomerRepository.Verify(x => x.SaveCustomer(It.Is<Customer>(c => 
                c.Name == customerName && 
                c.Email == customerEmail && 
                c.CreatedDate <= DateTime.Now)), Times.Once);

            // Verify welcome email was sent
            _mockMailSender.Verify(x => x.SendMail(customerEmail, 
                                                  It.Is<string>(msg => msg.Contains("Welcome") && msg.Contains(customerName))), 
                                  Times.Once);
        }

        /// <summary>
        /// Test demonstrating how to simulate database save failures
        /// </summary>
        [Test]
        public void CreateCustomerAndSendWelcome_WhenDatabaseSaveFails_ShouldReturnFalse()
        {
            // Arrange
            string customerName = "Failed User";
            string customerEmail = "failed@example.com";

            // Mock database save to return failure
            _mockCustomerRepository.Setup(x => x.SaveCustomer(It.IsAny<Customer>()))
                                  .Returns(false);

            // Act
            bool result = _customerService.CreateCustomerAndSendWelcome(customerName, customerEmail);

            // Assert
            Assert.That(result, Is.False, "CreateCustomerAndSendWelcome should return false when database save fails");

            // Verify save was attempted
            _mockCustomerRepository.Verify(x => x.SaveCustomer(It.IsAny<Customer>()), Times.Once);

            // Verify email was NOT sent (since save failed)
            _mockMailSender.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Test demonstrating exception handling in database operations
        /// </summary>
        [Test]
        public void SendWelcomeEmail_WhenDatabaseThrowsException_ShouldPropagateException()
        {
            // Arrange
            int customerId = 123;

            // Mock database to throw an exception
            _mockCustomerRepository.Setup(x => x.GetCustomerById(customerId))
                                  .Throws(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _customerService.SendWelcomeEmail(customerId));
            Assert.That(exception.Message, Is.EqualTo("Database connection failed"));

            // Verify database was called
            _mockCustomerRepository.Verify(x => x.GetCustomerById(customerId), Times.Once);
        }

        /// <summary>
        /// Test demonstrating complex mock setups with multiple calls
        /// </summary>
        [Test]
        public void SendWelcomeEmail_WhenEmailSendingFails_ShouldReturnFalse()
        {
            // Arrange
            int customerId = 123;
            var customer = new Customer
            {
                Id = customerId,
                Name = "Test User",
                Email = "test@example.com",
                CreatedDate = DateTime.Now
            };

            // Mock successful database retrieval
            _mockCustomerRepository.Setup(x => x.GetCustomerById(customerId))
                                  .Returns(customer);

            // Mock failed email sending
            _mockMailSender.Setup(x => x.SendMail(customer.Email, It.IsAny<string>()))
                          .Returns(false);

            // Act
            bool result = _customerService.SendWelcomeEmail(customerId);

            // Assert
            Assert.That(result, Is.False, "SendWelcomeEmail should return false when email sending fails");

            // Verify both operations were attempted
            _mockCustomerRepository.Verify(x => x.GetCustomerById(customerId), Times.Once);
            _mockMailSender.Verify(x => x.SendMail(customer.Email, It.IsAny<string>()), Times.Once);
        }
    }
}
