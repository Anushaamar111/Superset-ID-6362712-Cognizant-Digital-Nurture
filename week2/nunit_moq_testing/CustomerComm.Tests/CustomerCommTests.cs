using NUnit.Framework;
using Moq;
using CustomerCommLib;

namespace CustomerComm.Tests
{
    /// <summary>
    /// Unit tests for CustomerComm class demonstrating mocking with Moq
    /// This test class shows how to isolate the unit under test by mocking dependencies
    /// </summary>
    [TestFixture]
    public class CustomerCommTests
    {
        private Mock<IMailSender> _mockMailSender;
        private CustomerCommLib.CustomerComm _customerComm;

        /// <summary>
        /// Setup method that runs once before all tests
        /// Initializes the mock objects and dependencies
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Create a mock object for IMailSender interface
            _mockMailSender = new Mock<IMailSender>();
            
            // Inject the mock dependency into the class under test
            _customerComm = new CustomerCommLib.CustomerComm(_mockMailSender.Object);
        }

        /// <summary>
        /// Setup method that runs before each test
        /// Resets the mock to ensure clean state for each test
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Reset the mock before each test to ensure clean state
            _mockMailSender.Reset();
        }

        /// <summary>
        /// Test case demonstrating basic mocking functionality
        /// Configures mock to return true for any string parameters
        /// </summary>
        [Test]
        [TestCase]
        public void SendMailToCustomer_WhenCalled_ShouldReturnTrue()
        {
            // Arrange
            // Configure the mock to return true when SendMail is called with any two string arguments
            _mockMailSender.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(true);

            // Act
            // Call the method under test
            bool result = _customerComm.SendMailToCustomer();

            // Assert
            // Verify the method returns the expected value
            Assert.That(result, Is.True, "SendMailToCustomer should return true when mail sending succeeds");
            
            // Verify that the mock method was called exactly once
            _mockMailSender.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>()), 
                                  Times.Once, 
                                  "SendMail should be called exactly once");
        }

        /// <summary>
        /// Test case demonstrating mock verification with specific parameters
        /// Verifies that the correct email and message are passed to the dependency
        /// </summary>
        [Test]
        public void SendMailToCustomer_WhenCalled_ShouldCallSendMailWithCorrectParameters()
        {
            // Arrange
            _mockMailSender.Setup(x => x.SendMail("cust123@abc.com", "Some Message"))
                          .Returns(true);

            // Act
            bool result = _customerComm.SendMailToCustomer();

            // Assert
            Assert.That(result, Is.True);
            
            // Verify that SendMail was called with the exact expected parameters
            _mockMailSender.Verify(x => x.SendMail("cust123@abc.com", "Some Message"), 
                                  Times.Once,
                                  "SendMail should be called with correct email and message");
        }

        /// <summary>
        /// Test case demonstrating how to test failure scenarios
        /// Shows how mock can simulate dependency failures
        /// </summary>
        [Test]
        public void SendMailToCustomer_WhenMailSendingFails_ShouldReturnFalse()
        {
            // Arrange
            // Configure mock to return false (simulating mail sending failure)
            _mockMailSender.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(false);

            // Act
            bool result = _customerComm.SendMailToCustomer();

            // Assert
            Assert.That(result, Is.False, "SendMailToCustomer should return false when mail sending fails");
        }

        /// <summary>
        /// Test case demonstrating parameterized testing with custom inputs
        /// Tests the overloaded method with different email and message combinations
        /// </summary>
        [Test]
        [TestCase("test@example.com", "Test message", true)]
        [TestCase("user@domain.com", "Welcome message", true)]
        [TestCase("invalid@test.com", "Error message", false)]
        public void SendMailToCustomer_WithCustomParameters_ShouldReturnExpectedResult(
            string email, string message, bool expectedResult)
        {
            // Arrange
            _mockMailSender.Setup(x => x.SendMail(email, message))
                          .Returns(expectedResult);

            // Act
            bool result = _customerComm.SendMailToCustomer(email, message);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
            _mockMailSender.Verify(x => x.SendMail(email, message), Times.Once);
        }

        /// <summary>
        /// Test demonstrating mock verification without caring about return value
        /// Focuses on verifying that the dependency method was called correctly
        /// </summary>
        [Test]
        public void SendMailToCustomer_WhenCalled_ShouldInvokeMailSenderOnce()
        {
            // Arrange
            _mockMailSender.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(true);

            // Act
            _customerComm.SendMailToCustomer();

            // Assert
            // Verify interaction - ensure the dependency was called
            _mockMailSender.Verify(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>()), 
                                  Times.Once);
        }

        /// <summary>
        /// Test demonstrating advanced mock setup with callbacks
        /// Shows how to capture and validate arguments passed to mocked methods
        /// </summary>
        [Test]
        public void SendMailToCustomer_WhenCalled_ShouldPassValidEmailFormat()
        {
            // Arrange
            string capturedEmail = string.Empty;
            string capturedMessage = string.Empty;

            _mockMailSender.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>()))
                          .Callback<string, string>((email, message) => 
                          {
                              capturedEmail = email;
                              capturedMessage = message;
                          })
                          .Returns(true);

            // Act
            _customerComm.SendMailToCustomer();

            // Assert
            Assert.That(capturedEmail, Does.Contain("@"), "Email should contain @ symbol");
            Assert.That(capturedMessage, Is.Not.Empty, "Message should not be empty");
        }
    }
}
