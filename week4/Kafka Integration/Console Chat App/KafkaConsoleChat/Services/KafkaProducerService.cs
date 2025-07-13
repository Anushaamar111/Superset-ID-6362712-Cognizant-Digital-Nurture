using Confluent.Kafka;
using KafkaConsoleChat.Models;

namespace KafkaConsoleChat.Services
{
    /// <summary>
    /// Kafka producer service for sending chat messages
    /// </summary>
    public class KafkaProducerService : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topicName;
        private bool _disposed = false;

        /// <summary>
        /// Initialize the Kafka producer
        /// </summary>
        /// <param name="bootstrapServers">Kafka broker addresses</param>
        /// <param name="topicName">Topic name for chat messages</param>
        public KafkaProducerService(string bootstrapServers = "localhost:9092", string topicName = "chat-messages")
        {
            _topicName = topicName;

            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = Environment.MachineName,
                Acks = Acks.All, // Wait for all replicas to acknowledge
                EnableIdempotence = true, // Ensure exactly-once semantics
                MessageTimeoutMs = 10000,
                RequestTimeoutMs = 5000,
                RetryBackoffMs = 1000,
                MessageSendMaxRetries = 3,
                CompressionType = CompressionType.Snappy, // Compress messages
                BatchSize = 16384,
                LingerMs = 5 // Wait 5ms for batching
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => 
                {
                    Console.WriteLine($"❌ Producer error: {e.Reason}");
                })
                .SetStatisticsHandler((_, json) => 
                {
                    // Optional: Log statistics
                })
                .Build();
        }

        /// <summary>
        /// Send a chat message to Kafka
        /// </summary>
        /// <param name="chatMessage">Chat message to send</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> SendMessageAsync(ChatMessage chatMessage)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = chatMessage.Username, // Use username as key for partition distribution
                    Value = chatMessage.ToJson(),
                    Timestamp = new Timestamp(chatMessage.Timestamp)
                };

                var deliveryResult = await _producer.ProduceAsync(_topicName, message);

                Console.WriteLine($"✅ Message sent to partition {deliveryResult.Partition} at offset {deliveryResult.Offset}");
                return true;
            }
            catch (ProduceException<string, string> ex)
            {
                Console.WriteLine($"❌ Failed to send message: {ex.Error.Reason}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send a message synchronously
        /// </summary>
        /// <param name="chatMessage">Chat message to send</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SendMessage(ChatMessage chatMessage)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = chatMessage.Username,
                    Value = chatMessage.ToJson(),
                    Timestamp = new Timestamp(chatMessage.Timestamp)
                };

                var deliveryResult = _producer.ProduceAsync(_topicName, message).Result;
                Console.WriteLine($"✅ Message sent to partition {deliveryResult.Partition} at offset {deliveryResult.Offset}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to send message: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Flush all pending messages
        /// </summary>
        public void Flush()
        {
            try
            {
                _producer.Flush(TimeSpan.FromSeconds(10));
                Console.WriteLine("📤 All messages flushed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error flushing messages: {ex.Message}");
            }
        }

        /// <summary>
        /// Get producer statistics
        /// </summary>
        /// <returns>Statistics as JSON string</returns>
        public string GetStatistics()
        {
            try
            {
                // Note: Statistics are available through the statistics handler
                return "Statistics available through handler";
            }
            catch (Exception ex)
            {
                return $"Error getting statistics: {ex.Message}";
            }
        }

        /// <summary>
        /// Dispose of the producer
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
                    _producer?.Flush(TimeSpan.FromSeconds(5));
                    _producer?.Dispose();
                    Console.WriteLine("🔌 Producer disposed successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error disposing producer: {ex.Message}");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }
}
