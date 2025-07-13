using System.Drawing;
using System.Windows.Forms;
using KafkaWindowsChat.Models;
using KafkaWindowsChat.Services;

namespace KafkaWindowsChat;

public partial class ChatForm : Form
{
    private KafkaProducerService? _producer;
    private KafkaConsumerService? _consumer;
    private bool _isConnected = false;
    private readonly object _lockObject = new object();

    public ChatForm()
    {
        InitializeComponent();
        this.FormClosing += ChatForm_FormClosing;
    }

    private async void BtnConnect_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            MessageBox.Show("Please enter a username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            btnConnect.Enabled = false;
            lblStatus.Text = "Connecting...";
            lblStatus.ForeColor = Color.Orange;

            // Initialize Kafka services
            string room = cmbRoom.SelectedItem?.ToString() ?? "general";
            string topic = $"chat-{room}";
            string groupId = $"chat-group-{room}";

            _producer = new KafkaProducerService("localhost:9092", topic);
            _consumer = new KafkaConsumerService(groupId, "localhost:9092", topic);

            // Subscribe to consumer events
            _consumer.MessageReceived += OnMessageReceived;
            _consumer.ErrorOccurred += OnErrorOccurred;

            // Start consuming messages
            _consumer.StartConsuming();

            // Update UI state
            lock (_lockObject)
            {
                _isConnected = true;
                UpdateConnectionState();
            }

            // Send join message
            var joinMessage = new ChatMessage
            {
                Username = txtUsername.Text,
                Room = room,
                Message = $"{txtUsername.Text} joined the room",
                Timestamp = DateTime.UtcNow,
                MessageType = "system"
            };

            await _producer.SendMessageAsync(joinMessage);
            AddMessageToList($"Connected to room: {room}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            btnConnect.Enabled = true;
            lblStatus.Text = "Disconnected";
            lblStatus.ForeColor = Color.Red;
        }
    }

    private async void BtnDisconnect_Click(object sender, EventArgs e)
    {
        await DisconnectAsync();
    }

    private async Task DisconnectAsync()
    {
        try
        {
            if (_isConnected && _producer != null)
            {
                // Send leave message
                var leaveMessage = new ChatMessage
                {
                    Username = txtUsername.Text,
                    Room = cmbRoom.SelectedItem?.ToString() ?? "general",
                    Message = $"{txtUsername.Text} left the room",
                    Timestamp = DateTime.UtcNow,
                    MessageType = "system"
                };

                await _producer.SendMessageAsync(leaveMessage);
            }

            // Stop consumer
            _consumer?.StopConsuming();
            _consumer?.Dispose();

            // Dispose producer
            _producer?.Dispose();

            lock (_lockObject)
            {
                _isConnected = false;
                UpdateConnectionState();
            }

            AddMessageToList("Disconnected from chat");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Disconnect error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async void BtnSend_Click(object sender, EventArgs e)
    {
        await SendMessageAsync();
    }

    private async void TxtMessage_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            e.Handled = true;
            await SendMessageAsync();
        }
    }

    private async Task SendMessageAsync()
    {
        if (!_isConnected || _producer == null || string.IsNullOrWhiteSpace(txtMessage.Text))
            return;

        try
        {
            var message = new ChatMessage
            {
                Username = txtUsername.Text,
                Room = cmbRoom.SelectedItem?.ToString() ?? "general",
                Message = txtMessage.Text.Trim(),
                Timestamp = DateTime.UtcNow,
                MessageType = "user"
            };

            await _producer.SendMessageAsync(message);
            txtMessage.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to send message: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnMessageReceived(ChatMessage message)
    {
        // Marshal to UI thread
        if (InvokeRequired)
        {
            Invoke(new Action<ChatMessage>(OnMessageReceived), message);
            return;
        }

        string displayMessage = message.MessageType switch
        {
            "system" => $"[{message.Timestamp:HH:mm:ss}] {message.Message}",
            "user" => $"[{message.Timestamp:HH:mm:ss}] {message.Username}: {message.Message}",
            _ => $"[{message.Timestamp:HH:mm:ss}] {message.Username}: {message.Message}"
        };

        AddMessageToList(displayMessage);
    }

    private void OnErrorOccurred(string error)
    {
        // Marshal to UI thread
        if (InvokeRequired)
        {
            Invoke(new Action<string>(OnErrorOccurred), error);
            return;
        }

        AddMessageToList($"Error: {error}");
    }

    private void AddMessageToList(string message)
    {
        lstMessages.Items.Add(message);
        
        // Auto-scroll to bottom
        lstMessages.TopIndex = lstMessages.Items.Count - 1;
        
        // Limit message history to prevent memory issues
        if (lstMessages.Items.Count > 1000)
        {
            lstMessages.Items.RemoveAt(0);
        }
    }

    private void UpdateConnectionState()
    {
        // Update UI based on connection state
        btnConnect.Enabled = !_isConnected;
        btnDisconnect.Enabled = _isConnected;
        txtUsername.Enabled = !_isConnected;
        cmbRoom.Enabled = !_isConnected;
        txtMessage.Enabled = _isConnected;
        btnSend.Enabled = _isConnected;

        lblStatus.Text = _isConnected ? "Connected" : "Disconnected";
        lblStatus.ForeColor = _isConnected ? Color.Green : Color.Red;

        if (_isConnected)
        {
            txtMessage.Focus();
        }
    }

    private async void ChatForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_isConnected)
        {
            await DisconnectAsync();
        }
    }
}
