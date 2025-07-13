# Kafka Console Chat Application

## Overview
This is a real-time console-based chat application that uses Apache Kafka as the messaging backbone. Multiple users can join chat rooms and exchange messages in real-time through Kafka topics.

## Features

### 🚀 Core Features
- **Real-time Messaging**: Instant message delivery through Kafka
- **Multiple Users**: Support for multiple concurrent users
- **Chat Rooms**: Users can join different chat rooms
- **System Messages**: Automatic join/leave notifications
- **Message Persistence**: Messages are stored in Kafka topics
- **Fault Tolerance**: Built-in error handling and recovery

### 🎯 Technical Features
- **Kafka Integration**: Uses Confluent.Kafka .NET client
- **JSON Serialization**: Messages serialized as JSON
- **Partitioning**: Messages distributed across partitions by username
- **Consumer Groups**: Each user has their own consumer group
- **Async Processing**: Non-blocking message sending and receiving

## Prerequisites

### 1. Apache Kafka Setup
```bash
# Download Kafka from https://kafka.apache.org/downloads
# Extract to C:\kafka (or your preferred location)

# Start Zookeeper
cd C:\kafka
bin\windows\zookeeper-server-start.bat config\zookeeper.properties

# Start Kafka Server
bin\windows\kafka-server-start.bat config\server.properties
```

### 2. .NET Requirements
- .NET 6.0 or later
- Confluent.Kafka NuGet package
- Newtonsoft.Json NuGet package

## Installation & Setup

### 1. Clone and Build
```powershell
cd "Console Chat App\KafkaConsoleChat"
dotnet restore
dotnet build
```

### 2. Create Kafka Topic (Optional)
```bash
# Create chat-messages topic
kafka-topics.bat --create --topic chat-messages --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1
```

### 3. Run the Application
```powershell
dotnet run
```

## Usage

### Starting the Application
1. Run `dotnet run` in the project directory
2. Enter your username when prompted
3. Enter chat room name (or press Enter for 'general')
4. Start chatting!

### Available Commands
- `/help` - Show available commands
- `/quit` or `/exit` - Leave the chat application
- `/room <name>` - Switch to a different chat room
- `/users` - Show current user information
- `/stats` - Display connection statistics
- `/clear` - Clear the console screen

### Example Session
```
Enter your username: Alice
Enter chat room (default: general): gaming

🎯 Joining chat room 'gaming' as 'Alice'

💬 Start typing messages (type '/help' for commands, '/quit' to exit):
═══════════════════════════════════════════════════════════════

Alice> Hello everyone!
✅ Message sent to partition 1 at offset 42

📨 [12:34:56] SYSTEM: Bob has joined the chat room 'gaming' [P:0 O:43]
📨 [12:35:02] Bob: Hi Alice! [P:2 O:44]

Alice> /room general
🎯 Switched to room 'general'

Alice> /quit
👋 Leaving chat...
```

## Architecture

### Message Flow
```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   User A    │    │   User B    │    │   User C    │
│  Producer   │    │  Producer   │    │  Producer   │
└──────┬──────┘    └──────┬──────┘    └──────┬──────┘
       │                  │                  │
       └──────────────────┼──────────────────┘
                          │
              ┌───────────▼───────────┐
              │    Kafka Topic        │
              │   chat-messages       │
              │                       │
              │ ┌─────┐ ┌─────┐ ┌─────┐│
              │ │ P:0 │ │ P:1 │ │ P:2 ││
              │ └─────┘ └─────┘ └─────┘│
              └───────────┬───────────┘
                          │
       ┌──────────────────┼──────────────────┐
       │                  │                  │
┌──────▼──────┐    ┌──────▼──────┐    ┌──────▼──────┐
│   User A    │    │   User B    │    │   User C    │
│  Consumer   │    │  Consumer   │    │  Consumer   │
└─────────────┘    └─────────────┘    └─────────────┘
```

### Class Structure
```
KafkaConsoleChat/
├── Models/
│   └── ChatMessage.cs          # Message model with JSON serialization
├── Services/
│   ├── KafkaProducerService.cs # Kafka message producer
│   └── KafkaConsumerService.cs # Kafka message consumer
└── Program.cs                  # Main application logic
```

## Configuration

### Kafka Configuration
The application uses the following Kafka settings:

**Producer Config:**
- Bootstrap Servers: localhost:9092
- Acks: All (wait for all replicas)
- Enable Idempotence: True
- Compression: Snappy
- Batch Size: 16KB
- Linger: 5ms

**Consumer Config:**
- Bootstrap Servers: localhost:9092
- Auto Offset Reset: Latest
- Enable Auto Commit: True
- Session Timeout: 6 seconds
- Group ID: chat-group-{username}-{machine}

### Message Format
```json
{
  "id": "uuid-string",
  "username": "Alice",
  "message": "Hello everyone!",
  "timestamp": "2025-07-13T10:30:00Z",
  "room": "general",
  "messageType": "text"
}
```

## Testing Multiple Users

### Option 1: Multiple Terminals
1. Open multiple command prompts
2. Navigate to the project directory in each
3. Run `dotnet run` in each terminal
4. Use different usernames
5. Chat between the instances

### Option 2: Multiple Machines
1. Deploy the application to multiple machines
2. Ensure all machines can access the Kafka broker
3. Run the application on each machine
4. Users can chat across machines

## Troubleshooting

### Common Issues

#### 1. Connection Refused
```
❌ Producer error: Connection refused
```
**Solution**: Ensure Kafka is running on localhost:9092

#### 2. Topic Not Found
```
❌ Consumer error: Topic 'chat-messages' not found
```
**Solution**: Enable auto-topic creation or create topic manually:
```bash
kafka-topics.bat --create --topic chat-messages --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1
```

#### 3. Messages Not Appearing
- Check if users are in the same chat room
- Verify Kafka broker is running
- Check consumer group assignments

### Debug Commands

#### List Topics
```bash
kafka-topics.bat --list --bootstrap-server localhost:9092
```

#### Describe Topic
```bash
kafka-topics.bat --describe --topic chat-messages --bootstrap-server localhost:9092
```

#### Monitor Consumer Groups
```bash
kafka-consumer-groups.bat --bootstrap-server localhost:9092 --list
kafka-consumer-groups.bat --bootstrap-server localhost:9092 --group chat-group-Alice-DESKTOP --describe
```

#### View Messages
```bash
kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic chat-messages --from-beginning
```

## Advanced Features

### Message Filtering
The application filters messages by:
- Chat room (only shows messages from current room)
- Username (doesn't echo back own messages)

### Error Handling
- Automatic retry for failed message sends
- Graceful handling of consumer errors
- Connection recovery mechanisms

### Performance Optimizations
- Message batching for better throughput
- Compression to reduce network traffic
- Efficient JSON serialization

## Security Considerations

### For Production Use
1. **Authentication**: Implement SASL authentication
2. **Authorization**: Use Kafka ACLs
3. **Encryption**: Enable SSL/TLS
4. **Input Validation**: Sanitize user inputs
5. **Rate Limiting**: Prevent message flooding

### Example Security Config
```csharp
var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
    SecurityProtocol = SecurityProtocol.SaslSsl,
    SaslMechanism = SaslMechanism.Plain,
    SaslUsername = "username",
    SaslPassword = "password"
};
```

## Monitoring

### Application Metrics
- Messages sent/received per second
- Consumer lag
- Error rates
- Connection status

### Kafka Monitoring
- Use Kafka Manager or Confluent Control Center
- Monitor broker health
- Track topic partitions and offsets

This console chat application demonstrates the power of Kafka for real-time messaging and provides a foundation for building more complex chat systems.
