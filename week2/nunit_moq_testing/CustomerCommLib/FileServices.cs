using System.IO;

namespace CustomerCommLib
{
    /// <summary>
    /// Interface for file system operations - enables mocking file operations
    /// </summary>
    public interface IFileService
    {
        string ReadAllText(string filePath);
        void WriteAllText(string filePath, string content);
        bool FileExists(string filePath);
        void DeleteFile(string filePath);
        string[] ReadAllLines(string filePath);
    }

    /// <summary>
    /// Real file service implementation that interacts with the file system
    /// </summary>
    public class FileService : IFileService
    {
        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public void WriteAllText(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public string[] ReadAllLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }
    }

    /// <summary>
    /// Service class that performs logging operations using file system
    /// This class demonstrates how to make file-dependent code testable
    /// </summary>
    public class LogService
    {
        private readonly IFileService _fileService;
        private readonly string _logFilePath;

        public LogService(IFileService fileService, string logFilePath = "application.log")
        {
            _fileService = fileService;
            _logFilePath = logFilePath;
        }

        /// <summary>
        /// Logs a message to the file system
        /// </summary>
        public bool LogMessage(string message)
        {
            try
            {
                string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logEntry = $"[{timestamp}] {message}";

                // Check if log file exists, if not create it
                if (!_fileService.FileExists(_logFilePath))
                {
                    _fileService.WriteAllText(_logFilePath, logEntry + System.Environment.NewLine);
                }
                else
                {
                    // Append to existing file
                    string existingContent = _fileService.ReadAllText(_logFilePath);
                    string newContent = existingContent + logEntry + System.Environment.NewLine;
                    _fileService.WriteAllText(_logFilePath, newContent);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the last N log entries
        /// </summary>
        public string[] GetLastLogEntries(int count)
        {
            if (!_fileService.FileExists(_logFilePath))
                return new string[0];

            string[] allLines = _fileService.ReadAllLines(_logFilePath);
            
            if (allLines.Length <= count)
                return allLines;

            string[] lastEntries = new string[count];
            System.Array.Copy(allLines, allLines.Length - count, lastEntries, 0, count);
            return lastEntries;
        }

        /// <summary>
        /// Clears the log file
        /// </summary>
        public bool ClearLog()
        {
            try
            {
                if (_fileService.FileExists(_logFilePath))
                {
                    _fileService.DeleteFile(_logFilePath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Configuration service that reads settings from files
    /// </summary>
    public class ConfigurationService
    {
        private readonly IFileService _fileService;
        private readonly string _configFilePath;

        public ConfigurationService(IFileService fileService, string configFilePath = "app.config")
        {
            _fileService = fileService;
            _configFilePath = configFilePath;
        }

        /// <summary>
        /// Reads a configuration value from file
        /// </summary>
        public string GetConfigValue(string key)
        {
            if (!_fileService.FileExists(_configFilePath))
                return null;

            string[] lines = _fileService.ReadAllLines(_configFilePath);
            
            foreach (string line in lines)
            {
                if (line.StartsWith($"{key}="))
                {
                    return line.Substring(key.Length + 1);
                }
            }

            return null;
        }

        /// <summary>
        /// Sets a configuration value in file
        /// </summary>
        public bool SetConfigValue(string key, string value)
        {
            try
            {
                string newLine = $"{key}={value}";
                
                if (!_fileService.FileExists(_configFilePath))
                {
                    _fileService.WriteAllText(_configFilePath, newLine + System.Environment.NewLine);
                    return true;
                }

                string[] lines = _fileService.ReadAllLines(_configFilePath);
                bool keyFound = false;
                
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith($"{key}="))
                    {
                        lines[i] = newLine;
                        keyFound = true;
                        break;
                    }
                }

                if (!keyFound)
                {
                    string[] newLines = new string[lines.Length + 1];
                    System.Array.Copy(lines, newLines, lines.Length);
                    newLines[lines.Length] = newLine;
                    lines = newLines;
                }

                string content = string.Join(System.Environment.NewLine, lines) + System.Environment.NewLine;
                _fileService.WriteAllText(_configFilePath, content);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
