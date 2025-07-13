using Confluent.Kafka;
using KafkaWindowsChat.Models;

namespace KafkaWindowsChat.Services
{
    /// <summary>
    /// Kafka producer service for Windows Forms chat application
    /// </summary>
    public class KafkaProducerService : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topicName;
        private bool _disposed = false;

        /// <summary>
        /// Event fired when a message is successfully sent
        /// </summary>
        public event Action<string>? MessageSent;

        /// <summary>
        /// Event fired when an error occurs
        /// </summary>
        public event Action<string>? ErrorOccurred;

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
                Acks = Acks.All,
                EnableIdempotence = true,
                MessageTimeoutMs = 10000,
                RequestTimeoutMs = 5000,
                RetryBackoffMs = 1000,
                MessageSendMaxRetries = 3,
                CompressionType = CompressionType.Snappy,
                BatchSize = 16384,
                LingerMs = 5
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => 
                {
                    var errorMsg = $"Producer error: {e.Reason}";
                    ErrorOccurred?.Invoke(errorMsg);
                })
                .Build();
        }

        /// <summary>
        /// Send a chat message to Kafka asynchronously
        /// </summary>
        /// <param name="chatMessage">Chat message to send</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> SendMessageAsync(ChatMessage chatMessage)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = chatMessage.Username,
                    Value = chatMessage.ToJson(),
                    Timestamp = new Timestamp(chatMessage.Timestamp)
                };

                var deliveryResult = await _producer.ProduceAsync(_topicName, message);
                
                var successMsg = $"Message sent to partition {deliveryResult.Partition} at offset {deliveryResult.Offset}";
                MessageSent?.Invoke(successMsg);
                return true;
            }
            catch (ProduceException<string, string> ex)
            {
                var errorMsg = $"Failed to send message: {ex.Error.Reason}";
                ErrorOccurred?.Invoke(errorMsg);
                return false;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Unexpected error: {ex.Message}";
                ErrorOccurred?.Invoke(errorMsg);
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
                var task = SendMessageAsync(chatMessage);
                task.Wait(TimeSpan.FromSeconds(5));
                return task.Result;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Failed to send message: {ex.Message}";
                ErrorOccurred?.Invoke(errorMsg);
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
                MessageSent?.Invoke("All messages flushed successfully");
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Error flushing messages: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if the producer is connected
        /// </summary>
        /// <returns>True if connected</returns>
        public bool IsConnected()
        {
            // Note: Kafka producer doesn't have a direct "IsConnected" method
            // This is a simplified check
            return _producer != null && !_disposed;
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
                    MessageSent?.Invoke("Producer disposed successfully");
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke($"Error disposing producer: {ex.Message}");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }
}
