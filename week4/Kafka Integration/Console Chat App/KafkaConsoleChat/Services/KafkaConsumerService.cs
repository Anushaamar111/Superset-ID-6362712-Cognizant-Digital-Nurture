using Confluent.Kafka;
using KafkaConsoleChat.Models;

namespace KafkaConsoleChat.Services
{
    /// <summary>
    /// Kafka consumer service for receiving chat messages
    /// </summary>
    public class KafkaConsumerService : IDisposable
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly string _topicName;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;

        /// <summary>
        /// Event triggered when a new message is received
        /// </summary>
        public event Action<ChatMessage>? MessageReceived;

        /// <summary>
        /// Event triggered when an error occurs
        /// </summary>
        public event Action<string>? ErrorOccurred;

        /// <summary>
        /// Initialize the Kafka consumer
        /// </summary>
        /// <param name="groupId">Consumer group ID</param>
        /// <param name="bootstrapServers">Kafka broker addresses</param>
        /// <param name="topicName">Topic name for chat messages</param>
        public KafkaConsumerService(string groupId, string bootstrapServers = "localhost:9092", string topicName = "chat-messages")
        {
            _topicName = topicName;
            _cancellationTokenSource = new CancellationTokenSource();

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                ClientId = $"{Environment.MachineName}-{groupId}",
                AutoOffsetReset = AutoOffsetReset.Latest, // Start from latest messages
                EnableAutoCommit = true,
                AutoCommitIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                MaxPollIntervalMs = 300000,
                EnablePartitionEof = false,
                AllowAutoCreateTopics = true,
                IsolationLevel = IsolationLevel.ReadCommitted
            };

            _consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => 
                {
                    var errorMsg = $"Consumer error: {e.Reason}";
                    Console.WriteLine($"❌ {errorMsg}");
                    ErrorOccurred?.Invoke(errorMsg);
                })
                .SetPartitionsAssignedHandler((c, partitions) => 
                {
                    Console.WriteLine($"🎯 Assigned partitions: [{string.Join(", ", partitions)}]");
                })
                .SetPartitionsRevokedHandler((c, partitions) => 
                {
                    Console.WriteLine($"🔄 Revoked partitions: [{string.Join(", ", partitions)}]");
                })
                .Build();
        }

        /// <summary>
        /// Start consuming messages from Kafka
        /// </summary>
        public async Task StartConsumingAsync()
        {
            await Task.Run(() => StartConsuming());
        }

        /// <summary>
        /// Start consuming messages synchronously
        /// </summary>
        public void StartConsuming()
        {
            try
            {
                _consumer.Subscribe(_topicName);
                Console.WriteLine($"🎧 Started consuming from topic '{_topicName}'");

                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(_cancellationTokenSource.Token);

                        if (consumeResult?.Message?.Value != null)
                        {
                            var chatMessage = ChatMessage.FromJson(consumeResult.Message.Value);
                            
                            if (chatMessage != null)
                            {
                                // Display message in console
                                DisplayMessage(chatMessage, consumeResult);
                                
                                // Trigger event for subscribers
                                MessageReceived?.Invoke(chatMessage);
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ Failed to parse message: {consumeResult.Message.Value}");
                            }
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        var errorMsg = $"Consume error: {ex.Error.Reason}";
                        Console.WriteLine($"❌ {errorMsg}");
                        ErrorOccurred?.Invoke(errorMsg);
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when cancellation is requested
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"Unexpected consumer error: {ex.Message}";
                Console.WriteLine($"❌ {errorMsg}");
                ErrorOccurred?.Invoke(errorMsg);
            }
            finally
            {
                try
                {
                    _consumer.Close();
                    Console.WriteLine("🔌 Consumer closed successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error closing consumer: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Display a message in the console with formatting
        /// </summary>
        /// <param name="message">Chat message</param>
        /// <param name="consumeResult">Kafka consume result</param>
        private void DisplayMessage(ChatMessage message, ConsumeResult<string, string> consumeResult)
        {
            // Choose color based on message type
            ConsoleColor originalColor = Console.ForegroundColor;
            
            try
            {
                if (message.MessageType == "system")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else if (message.Username == "SYSTEM")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                // Display message with partition and offset info
                Console.WriteLine($"📨 {message} [P:{consumeResult.Partition} O:{consumeResult.Offset}]");
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// Stop consuming messages
        /// </summary>
        public void StopConsuming()
        {
            try
            {
                _cancellationTokenSource.Cancel();
                Console.WriteLine("🛑 Stopping consumer...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error stopping consumer: {ex.Message}");
            }
        }

        /// <summary>
        /// Get consumer assignment information
        /// </summary>
        /// <returns>Assignment information</returns>
        public string GetAssignment()
        {
            try
            {
                var assignment = _consumer.Assignment;
                return $"Assigned partitions: {string.Join(", ", assignment)}";
            }
            catch (Exception ex)
            {
                return $"Error getting assignment: {ex.Message}";
            }
        }

        /// <summary>
        /// Get consumer position information
        /// </summary>
        /// <returns>Position information</returns>
        public string GetPosition()
        {
            try
            {
                var assignment = _consumer.Assignment;
                var positions = assignment.Select(tp => 
                {
                    var position = _consumer.Position(tp);
                    return $"{tp}: {position}";
                });
                return $"Positions: {string.Join(", ", positions)}";
            }
            catch (Exception ex)
            {
                return $"Error getting position: {ex.Message}";
            }
        }

        /// <summary>
        /// Dispose of the consumer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose method
        /// </summary>
        /// <param name="disposing">True if disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                try
                {
                    StopConsuming();
                    _cancellationTokenSource?.Dispose();
                    _consumer?.Dispose();
                    Console.WriteLine("🔌 Consumer disposed successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error disposing consumer: {ex.Message}");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }
}
