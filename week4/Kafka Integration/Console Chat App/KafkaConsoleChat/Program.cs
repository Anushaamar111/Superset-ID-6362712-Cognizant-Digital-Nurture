using KafkaConsoleChat.Models;
using KafkaConsoleChat.Services;

namespace KafkaConsoleChat
{
    /// <summary>
    /// Main program for Kafka Console Chat Application
    /// </summary>
    class Program
    {
        private static KafkaProducerService? _producer;
        private static KafkaConsumerService? _consumer;
        private static string _username = string.Empty;
        private static string _room = "general";
        private static bool _isRunning = true;

        static async Task Main(string[] args)
        {
            // Display welcome banner
            DisplayWelcomeBanner();

            // Get configuration from user
            GetUserConfiguration();

            // Initialize Kafka services
            InitializeKafkaServices();

            // Start consumer in background
            var consumerTask = StartConsumerAsync();

            // Send welcome message
            await SendWelcomeMessage();

            // Start message input loop
            await StartMessageInputLoop();

            // Cleanup
            await Cleanup();
        }

        /// <summary>
        /// Display welcome banner
        /// </summary>
        private static void DisplayWelcomeBanner()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  KAFKA CONSOLE CHAT APPLICATION             ║");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("║  A real-time chat application using Apache Kafka            ║");
            Console.WriteLine("║  Messages are streamed through Kafka topics                 ║");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Get user configuration (username, room, etc.)
        /// </summary>
        private static void GetUserConfiguration()
        {
            // Get username
            while (string.IsNullOrWhiteSpace(_username))
            {
                Console.Write("Enter your username: ");
                _username = Console.ReadLine()?.Trim() ?? string.Empty;
                
                if (string.IsNullOrWhiteSpace(_username))
                {
                    Console.WriteLine("❌ Username cannot be empty. Please try again.");
                }
            }

            // Get room (optional)
            Console.Write($"Enter chat room (default: {_room}): ");
            var roomInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(roomInput))
            {
                _room = roomInput;
            }

            Console.WriteLine($"🎯 Joining chat room '{_room}' as '{_username}'");
            Console.WriteLine();
        }

        /// <summary>
        /// Initialize Kafka producer and consumer services
        /// </summary>
        private static void InitializeKafkaServices()
        {
            try
            {
                Console.WriteLine("🔧 Initializing Kafka services...");

                // Initialize producer
                _producer = new KafkaProducerService();
                Console.WriteLine("✅ Producer initialized");

                // Initialize consumer with unique group ID per user
                var groupId = $"chat-group-{_username}-{Environment.MachineName}";
                _consumer = new KafkaConsumerService(groupId);
                Console.WriteLine("✅ Consumer initialized");

                // Subscribe to consumer events
                _consumer.MessageReceived += OnMessageReceived;
                _consumer.ErrorOccurred += OnErrorOccurred;

                Console.WriteLine("🚀 Kafka services ready!");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to initialize Kafka services: {ex.Message}");
                Console.WriteLine("Make sure Kafka is running on localhost:9092");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Start the consumer in a background task
        /// </summary>
        /// <returns>Consumer task</returns>
        private static async Task<Task> StartConsumerAsync()
        {
            var consumerTask = Task.Run(() =>
            {
                try
                {
                    _consumer?.StartConsuming();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Consumer error: {ex.Message}");
                }
            });

            // Give consumer time to start
            await Task.Delay(1000);
            return consumerTask;
        }

        /// <summary>
        /// Send welcome message to the chat
        /// </summary>
        private static async Task SendWelcomeMessage()
        {
            try
            {
                var welcomeMessage = ChatMessage.CreateSystemMessage(
                    $"{_username} has joined the chat room '{_room}'", _room);
                
                await _producer!.SendMessageAsync(welcomeMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to send welcome message: {ex.Message}");
            }
        }

        /// <summary>
        /// Start the main message input loop
        /// </summary>
        private static async Task StartMessageInputLoop()
        {
            Console.WriteLine("💬 Start typing messages (type '/help' for commands, '/quit' to exit):");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            while (_isRunning)
            {
                try
                {
                    Console.Write($"{_username}> ");
                    var input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    // Handle commands
                    if (input.StartsWith("/"))
                    {
                        await HandleCommand(input);
                        continue;
                    }

                    // Send regular message
                    var chatMessage = ChatMessage.CreateUserMessage(_username, input, _room);
                    var success = await _producer!.SendMessageAsync(chatMessage);

                    if (!success)
                    {
                        Console.WriteLine("❌ Failed to send message. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error sending message: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Handle chat commands
        /// </summary>
        /// <param name="command">Command string</param>
        private static async Task HandleCommand(string command)
        {
            var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLower();

            switch (cmd)
            {
                case "/help":
                    DisplayHelp();
                    break;

                case "/quit":
                case "/exit":
                    await HandleQuit();
                    break;

                case "/room":
                    await HandleRoomChange(parts);
                    break;

                case "/users":
                    DisplayUserInfo();
                    break;

                case "/stats":
                    DisplayStats();
                    break;

                case "/clear":
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine($"❌ Unknown command: {cmd}. Type '/help' for available commands.");
                    break;
            }
        }

        /// <summary>
        /// Display help information
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("📖 Available Commands:");
            Console.WriteLine("   /help     - Show this help message");
            Console.WriteLine("   /quit     - Exit the chat application");
            Console.WriteLine("   /room <name> - Change chat room");
            Console.WriteLine("   /users    - Show current user info");
            Console.WriteLine("   /stats    - Show connection statistics");
            Console.WriteLine("   /clear    - Clear the console screen");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Handle quit command
        /// </summary>
        private static async Task HandleQuit()
        {
            try
            {
                Console.WriteLine("👋 Leaving chat...");
                
                // Send goodbye message
                var goodbyeMessage = ChatMessage.CreateSystemMessage(
                    $"{_username} has left the chat room '{_room}'", _room);
                await _producer!.SendMessageAsync(goodbyeMessage);
                
                _isRunning = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during quit: {ex.Message}");
                _isRunning = false;
            }
        }

        /// <summary>
        /// Handle room change command
        /// </summary>
        /// <param name="parts">Command parts</param>
        private static async Task HandleRoomChange(string[] parts)
        {
            if (parts.Length < 2)
            {
                Console.WriteLine("❌ Usage: /room <room_name>");
                return;
            }

            try
            {
                var newRoom = parts[1];
                
                // Send leave message to current room
                var leaveMessage = ChatMessage.CreateSystemMessage(
                    $"{_username} has left the chat room", _room);
                await _producer!.SendMessageAsync(leaveMessage);

                _room = newRoom;

                // Send join message to new room
                var joinMessage = ChatMessage.CreateSystemMessage(
                    $"{_username} has joined the chat room", _room);
                await _producer!.SendMessageAsync(joinMessage);

                Console.WriteLine($"🎯 Switched to room '{_room}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error changing room: {ex.Message}");
            }
        }

        /// <summary>
        /// Display current user information
        /// </summary>
        private static void DisplayUserInfo()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("👤 Current User Info:");
            Console.WriteLine($"   Username: {_username}");
            Console.WriteLine($"   Room: {_room}");
            Console.WriteLine($"   Machine: {Environment.MachineName}");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Display connection statistics
        /// </summary>
        private static void DisplayStats()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("📊 Connection Statistics:");
            Console.WriteLine($"   Consumer Assignment: {_consumer?.GetAssignment()}");
            Console.WriteLine($"   Consumer Position: {_consumer?.GetPosition()}");
            Console.WriteLine($"   Producer Status: {(_producer != null ? "Connected" : "Disconnected")}");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Handle received messages
        /// </summary>
        /// <param name="message">Received chat message</param>
        private static void OnMessageReceived(ChatMessage message)
        {
            // Don't display our own messages (they'll be echoed back)
            if (message.Username == _username)
                return;

            // Only display messages from our current room
            if (message.Room != _room)
                return;

            // Message is already displayed by the consumer service
        }

        /// <summary>
        /// Handle consumer errors
        /// </summary>
        /// <param name="error">Error message</param>
        private static void OnErrorOccurred(string error)
        {
            Console.WriteLine($"🔴 Consumer Error: {error}");
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        private static async Task Cleanup()
        {
            try
            {
                Console.WriteLine("🧹 Cleaning up resources...");

                // Dispose services
                _consumer?.Dispose();
                _producer?.Dispose();

                Console.WriteLine("✅ Cleanup completed");
                Console.WriteLine("Thank you for using Kafka Console Chat! 👋");
                
                // Wait a moment before exiting
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during cleanup: {ex.Message}");
            }
        }
    }
}
