using NUnit.Framework;
using Moq;
using CustomerCommLib;
using System;

namespace CustomerComm.Tests
{
    /// <summary>
    /// Unit tests demonstrating file system mocking with Moq
    /// Shows how to test file operations without actually touching the file system
    /// </summary>
    [TestFixture]
    public class LogServiceTests
    {
        private Mock<IFileService> _mockFileService;
        private LogService _logService;
        private const string TestLogFilePath = "test.log";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockFileService = new Mock<IFileService>();
            _logService = new LogService(_mockFileService.Object, TestLogFilePath);
        }

        [SetUp]
        public void SetUp()
        {
            _mockFileService.Reset();
        }

        /// <summary>
        /// Test demonstrating how to mock file creation scenario
        /// </summary>
        [Test]
        public void LogMessage_WhenLogFileDoesNotExist_ShouldCreateNewFile()
        {
            // Arrange
            string testMessage = "Test log message";

            // Mock file doesn't exist
            _mockFileService.Setup(x => x.FileExists(TestLogFilePath))
                           .Returns(false);

            // Act
            bool result = _logService.LogMessage(testMessage);

            // Assert
            Assert.That(result, Is.True, "LogMessage should return true when logging succeeds");

            // Verify file existence was checked
            _mockFileService.Verify(x => x.FileExists(TestLogFilePath), Times.Once);

            // Verify new file was created with timestamped message
            _mockFileService.Verify(x => x.WriteAllText(TestLogFilePath, 
                                                       It.Is<string>(content => 
                                                           content.Contains(testMessage) && 
                                                           content.Contains(DateTime.Now.ToString("yyyy-MM-dd")))), 
                                   Times.Once);

            // Verify file was not read (since it doesn't exist)
            _mockFileService.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Test demonstrating how to mock file append scenario
        /// </summary>
        [Test]
        public void LogMessage_WhenLogFileExists_ShouldAppendToExistingFile()
        {
            // Arrange
            string testMessage = "New log entry";
            string existingContent = "[2023-01-01 10:00:00] Previous log entry" + Environment.NewLine;

            // Mock file exists
            _mockFileService.Setup(x => x.FileExists(TestLogFilePath))
                           .Returns(true);

            // Mock reading existing content
            _mockFileService.Setup(x => x.ReadAllText(TestLogFilePath))
                           .Returns(existingContent);

            // Act
            bool result = _logService.LogMessage(testMessage);

            // Assert
            Assert.That(result, Is.True, "LogMessage should return true when logging succeeds");

            // Verify file operations were performed in correct sequence
            _mockFileService.Verify(x => x.FileExists(TestLogFilePath), Times.Once);
            _mockFileService.Verify(x => x.ReadAllText(TestLogFilePath), Times.Once);
            _mockFileService.Verify(x => x.WriteAllText(TestLogFilePath, 
                                                       It.Is<string>(content => 
                                                           content.Contains(existingContent.TrimEnd()) && 
                                                           content.Contains(testMessage))), 
                                   Times.Once);
        }

        /// <summary>
        /// Test demonstrating how to mock file reading operations
        /// </summary>
        [Test]
        public void GetLastLogEntries_WhenFileExists_ShouldReturnLastEntries()
        {
            // Arrange
            int requestedCount = 2;
            string[] mockFileLines = new string[]
            {
                "[2023-01-01 10:00:00] First entry",
                "[2023-01-01 11:00:00] Second entry",
                "[2023-01-01 12:00:00] Third entry",
                "[2023-01-01 13:00:00] Fourth entry"
            };

            // Mock file exists
            _mockFileService.Setup(x => x.FileExists(TestLogFilePath))
                           .Returns(true);

            // Mock reading all lines
            _mockFileService.Setup(x => x.ReadAllLines(TestLogFilePath))
                           .Returns(mockFileLines);

            // Act
            string[] result = _logService.GetLastLogEntries(requestedCount);

            // Assert
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Length, Is.EqualTo(requestedCount), "Should return requested number of entries");
            Assert.That(result[0], Is.EqualTo(mockFileLines[2]), "Should return third entry as first result");
            Assert.That(result[1], Is.EqualTo(mockFileLines[3]), "Should return fourth entry as second result");

            // Verify file operations
            _mockFileService.Verify(x => x.FileExists(TestLogFilePath), Times.Once);
            _mockFileService.Verify(x => x.ReadAllLines(TestLogFilePath), Times.Once);
        }

        /// <summary>
        /// Test demonstrating how to mock scenarios where file doesn't exist
        /// </summary>
        [Test]
        public void GetLastLogEntries_WhenFileDoesNotExist_ShouldReturnEmptyArray()
        {
            // Arrange
            int requestedCount = 5;

            // Mock file doesn't exist
            _mockFileService.Setup(x => x.FileExists(TestLogFilePath))
                           .Returns(false);

            // Act
            string[] result = _logService.GetLastLogEntries(requestedCount);

            // Assert
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Length, Is.EqualTo(0), "Should return empty array when file doesn't exist");

            // Verify file existence was checked
            _mockFileService.Verify(x => x.FileExists(TestLogFilePath), Times.Once);

            // Verify file was not read (since it doesn't exist)
            _mockFileService.Verify(x => x.ReadAllLines(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Test demonstrating how to mock file deletion operations
        /// </summary>
        [Test]
        public void ClearLog_WhenFileExists_ShouldDeleteFile()
        {
            // Arrange
            // Mock file exists
            _mockFileService.Setup(x => x.FileExists(TestLogFilePath))
                           .Returns(true);

            // Act
            bool result = _logService.ClearLog();

            // Assert
            Assert.That(result, Is.True, "ClearLog should return true when operation succeeds");

            // Verify file operations
            _mockFileService.Verify(x => x.FileExists(TestLogFilePath), Times.Once);
            _mockFileService.Verify(x => x.DeleteFile(TestLogFilePath), Times.Once);
        }

        /// <summary>
        /// Test demonstrating how to mock file operation exceptions
        /// </summary>
        [Test]
        public void LogMessage_WhenFileOperationThrowsException_ShouldReturnFalse()
        {
            // Arrange
            string testMessage = "Test message";

            // Mock file exists
            _mockFileService.Setup(x => x.FileExists(TestLogFilePath))
                           .Returns(true);

            // Mock read operation throws exception
            _mockFileService.Setup(x => x.ReadAllText(TestLogFilePath))
                           .Throws(new UnauthorizedAccessException("Access denied"));

            // Act
            bool result = _logService.LogMessage(testMessage);

            // Assert
            Assert.That(result, Is.False, "LogMessage should return false when file operation fails");

            // Verify operations were attempted
            _mockFileService.Verify(x => x.FileExists(TestLogFilePath), Times.Once);
            _mockFileService.Verify(x => x.ReadAllText(TestLogFilePath), Times.Once);
        }
    }

    /// <summary>
    /// Unit tests for ConfigurationService demonstrating file-based configuration mocking
    /// </summary>
    [TestFixture]
    public class ConfigurationServiceTests
    {
        private Mock<IFileService> _mockFileService;
        private ConfigurationService _configService;
        private const string TestConfigFilePath = "test.config";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockFileService = new Mock<IFileService>();
            _configService = new ConfigurationService(_mockFileService.Object, TestConfigFilePath);
        }

        [SetUp]
        public void SetUp()
        {
            _mockFileService.Reset();
        }

        /// <summary>
        /// Test demonstrating how to mock configuration file reading
        /// </summary>
        [Test]
        public void GetConfigValue_WhenKeyExists_ShouldReturnValue()
        {
            // Arrange
            string searchKey = "DatabaseConnectionString";
            string expectedValue = "Server=localhost;Database=TestDB;";
            string[] mockConfigLines = new string[]
            {
                "AppName=MyApplication",
                $"{searchKey}={expectedValue}",
                "LogLevel=Debug"
            };

            // Mock file exists and return config lines
            _mockFileService.Setup(x => x.FileExists(TestConfigFilePath))
                           .Returns(true);
            _mockFileService.Setup(x => x.ReadAllLines(TestConfigFilePath))
                           .Returns(mockConfigLines);

            // Act
            string result = _configService.GetConfigValue(searchKey);

            // Assert
            Assert.That(result, Is.EqualTo(expectedValue), "Should return the correct configuration value");

            // Verify file operations
            _mockFileService.Verify(x => x.FileExists(TestConfigFilePath), Times.Once);
            _mockFileService.Verify(x => x.ReadAllLines(TestConfigFilePath), Times.Once);
        }

        /// <summary>
        /// Test demonstrating configuration file writing/updating
        /// </summary>
        [Test]
        public void SetConfigValue_WhenFileDoesNotExist_ShouldCreateNewFile()
        {
            // Arrange
            string key = "NewSetting";
            string value = "NewValue";

            // Mock file doesn't exist
            _mockFileService.Setup(x => x.FileExists(TestConfigFilePath))
                           .Returns(false);

            // Act
            bool result = _configService.SetConfigValue(key, value);

            // Assert
            Assert.That(result, Is.True, "SetConfigValue should return true when operation succeeds");

            // Verify file operations
            _mockFileService.Verify(x => x.FileExists(TestConfigFilePath), Times.Once);
            _mockFileService.Verify(x => x.WriteAllText(TestConfigFilePath, 
                                                       It.Is<string>(content => content.Contains($"{key}={value}"))), 
                                   Times.Once);
        }

        /// <summary>
        /// Test demonstrating configuration value updates in existing file
        /// </summary>
        [Test]
        public void SetConfigValue_WhenKeyExistsInFile_ShouldUpdateExistingKey()
        {
            // Arrange
            string key = "LogLevel";
            string newValue = "Error";
            string[] existingConfigLines = new string[]
            {
                "AppName=MyApplication",
                "LogLevel=Debug",
                "DatabaseConnectionString=Server=localhost;"
            };

            // Mock file exists and return existing config
            _mockFileService.Setup(x => x.FileExists(TestConfigFilePath))
                           .Returns(true);
            _mockFileService.Setup(x => x.ReadAllLines(TestConfigFilePath))
                           .Returns(existingConfigLines);

            // Act
            bool result = _configService.SetConfigValue(key, newValue);

            // Assert
            Assert.That(result, Is.True, "SetConfigValue should return true when update succeeds");

            // Verify file operations
            _mockFileService.Verify(x => x.FileExists(TestConfigFilePath), Times.Once);
            _mockFileService.Verify(x => x.ReadAllLines(TestConfigFilePath), Times.Once);
            _mockFileService.Verify(x => x.WriteAllText(TestConfigFilePath, 
                                                       It.Is<string>(content => 
                                                           content.Contains($"{key}={newValue}") && 
                                                           content.Contains("AppName=MyApplication"))), 
                                   Times.Once);
        }
    }
}
