using Confluent.Kafka;
using KafkaWindowsChat.Models;

namespace KafkaWindowsChat.Services
{
    /// <summary>
    /// Kafka consumer service for Windows Forms chat application
    /// </summary>
    public class KafkaConsumerService : IDisposable
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly string _topicName;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;
        private Task? _consumerTask;

        /// <summary>
        /// Event triggered when a new message is received
        /// </summary>
        public event Action<ChatMessage>? MessageReceived;

        /// <summary>
        /// Event triggered when an error occurs
        /// </summary>
        public event Action<string>? ErrorOccurred;

        /// <summary>
        /// Event triggered when consumer status changes
        /// </summary>
        public event Action<string>? StatusChanged;

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
                AutoOffsetReset = AutoOffsetReset.Latest,
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
                    ErrorOccurred?.Invoke(errorMsg);
                })
                .SetPartitionsAssignedHandler((c, partitions) => 
                {
                    var statusMsg = $"Assigned partitions: [{string.Join(", ", partitions)}]";
                    StatusChanged?.Invoke(statusMsg);
                })
                .SetPartitionsRevokedHandler((c, partitions) => 
                {
                    var statusMsg = $"Revoked partitions: [{string.Join(", ", partitions)}]";
                    StatusChanged?.Invoke(statusMsg);
                })
                .Build();
        }

        /// <summary>
        /// Start consuming messages from Kafka
        /// </summary>
        public void StartConsuming()
        {
            if (_consumerTask != null && !_consumerTask.IsCompleted)
            {
                return; // Already consuming
            }

            _consumerTask = Task.Run(() =>
            {
                try
                {
                    _consumer.Subscribe(_topicName);
                    StatusChanged?.Invoke($"Started consuming from topic '{_topicName}'");

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
                                    // Trigger event for subscribers (UI thread should handle marshaling)
                                    MessageReceived?.Invoke(chatMessage);
                                }
                                else
                                {
                                    ErrorOccurred?.Invoke($"Failed to parse message: {consumeResult.Message.Value}");
                                }
                            }
                        }
                        catch (ConsumeException ex)
                        {
                            ErrorOccurred?.Invoke($"Consume error: {ex.Error.Reason}");
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
                    ErrorOccurred?.Invoke($"Unexpected consumer error: {ex.Message}");
                }
                finally
                {
                    try
                    {
                        _consumer.Close();
                        StatusChanged?.Invoke("Consumer closed successfully");
                    }
                    catch (Exception ex)
                    {
                        ErrorOccurred?.Invoke($"Error closing consumer: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// Stop consuming messages
        /// </summary>
        public void StopConsuming()
        {
            try
            {
                _cancellationTokenSource.Cancel();
                StatusChanged?.Invoke("Stopping consumer...");
                
                if (_consumerTask != null)
                {
                    _consumerTask.Wait(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Error stopping consumer: {ex.Message}");
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
        /// Check if the consumer is running
        /// </summary>
        /// <returns>True if running</returns>
        public bool IsRunning()
        {
            return _consumerTask != null && !_consumerTask.IsCompleted && !_cancellationTokenSource.Token.IsCancellationRequested;
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
                    StatusChanged?.Invoke("Consumer disposed successfully");
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke($"Error disposing consumer: {ex.Message}");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }
}
