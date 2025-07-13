using Newtonsoft.Json;

namespace KafkaWindowsChat.Models
{
    /// <summary>
    /// Represents a chat message in the Kafka Windows chat application
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Unique identifier for the message
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Username of the person who sent the message
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The actual message content
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the message was created
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Chat room or channel name
        /// </summary>
        [JsonProperty("room")]
        public string Room { get; set; } = "general";

        /// <summary>
        /// Message type (text, system, etc.)
        /// </summary>
        [JsonProperty("messageType")]
        public string MessageType { get; set; } = "text";

        /// <summary>
        /// Convert the chat message to JSON string
        /// </summary>
        /// <returns>JSON representation of the message</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }

        /// <summary>
        /// Create a ChatMessage from JSON string
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <returns>ChatMessage object</returns>
        public static ChatMessage? FromJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<ChatMessage>(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Display format for GUI
        /// </summary>
        /// <returns>Formatted string for display</returns>
        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {Username}: {Message}";
        }

        /// <summary>
        /// Display format with room information
        /// </summary>
        /// <returns>Formatted string with room</returns>
        public string ToStringWithRoom()
        {
            return $"[{Timestamp:HH:mm:ss}] [{Room}] {Username}: {Message}";
        }

        /// <summary>
        /// Create a system message
        /// </summary>
        /// <param name="message">System message content</param>
        /// <param name="room">Chat room</param>
        /// <returns>ChatMessage with system type</returns>
        public static ChatMessage CreateSystemMessage(string message, string room = "general")
        {
            return new ChatMessage
            {
                Username = "SYSTEM",
                Message = message,
                Room = room,
                MessageType = "system"
            };
        }

        /// <summary>
        /// Create a user message
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="message">Message content</param>
        /// <param name="room">Chat room</param>
        /// <returns>ChatMessage with text type</returns>
        public static ChatMessage CreateUserMessage(string username, string message, string room = "general")
        {
            return new ChatMessage
            {
                Username = username,
                Message = message,
                Room = room,
                MessageType = "text"
            };
        }
    }
}
