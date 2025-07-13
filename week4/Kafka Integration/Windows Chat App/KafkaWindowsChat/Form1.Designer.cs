using System.Drawing;
using System.Windows.Forms;

namespace KafkaWindowsChat;

partial class ChatForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    // Connection Controls
    private Label lblUsername;
    private TextBox txtUsername;
    private Label lblRoom;
    private ComboBox cmbRoom;
    private Button btnConnect;
    private Button btnDisconnect;
    private Label lblStatus;
    
    // Message Display
    private ListBox lstMessages;
    
    // Message Input
    private TextBox txtMessage;
    private Button btnSend;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        
        // Initialize controls
        this.lblUsername = new Label();
        this.txtUsername = new TextBox();
        this.lblRoom = new Label();
        this.cmbRoom = new ComboBox();
        this.btnConnect = new Button();
        this.btnDisconnect = new Button();
        this.lblStatus = new Label();
        this.lstMessages = new ListBox();
        this.txtMessage = new TextBox();
        this.btnSend = new Button();
        
        this.SuspendLayout();
        
        // Form properties
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(800, 600);
        this.Text = "Kafka Chat Application";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(600, 400);
        
        // Username label
        this.lblUsername.AutoSize = true;
        this.lblUsername.Location = new Point(12, 15);
        this.lblUsername.Size = new Size(70, 15);
        this.lblUsername.Text = "Username:";
        
        // Username textbox
        this.txtUsername.Location = new Point(90, 12);
        this.txtUsername.Size = new Size(150, 23);
        this.txtUsername.Text = "User" + new Random().Next(100, 999);
        
        // Room label
        this.lblRoom.AutoSize = true;
        this.lblRoom.Location = new Point(260, 15);
        this.lblRoom.Size = new Size(40, 15);
        this.lblRoom.Text = "Room:";
        
        // Room combobox
        this.cmbRoom.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbRoom.FormattingEnabled = true;
        this.cmbRoom.Items.AddRange(new object[] { "general", "tech", "random", "support" });
        this.cmbRoom.Location = new Point(310, 12);
        this.cmbRoom.Size = new Size(120, 23);
        this.cmbRoom.SelectedIndex = 0;
        
        // Connect button
        this.btnConnect.Location = new Point(450, 12);
        this.btnConnect.Size = new Size(75, 23);
        this.btnConnect.Text = "Connect";
        this.btnConnect.UseVisualStyleBackColor = true;
        this.btnConnect.Click += new EventHandler(this.BtnConnect_Click);
        
        // Disconnect button
        this.btnDisconnect.Enabled = false;
        this.btnDisconnect.Location = new Point(535, 12);
        this.btnDisconnect.Size = new Size(75, 23);
        this.btnDisconnect.Text = "Disconnect";
        this.btnDisconnect.UseVisualStyleBackColor = true;
        this.btnDisconnect.Click += new EventHandler(this.BtnDisconnect_Click);
        
        // Status label
        this.lblStatus.AutoSize = true;
        this.lblStatus.ForeColor = Color.Red;
        this.lblStatus.Location = new Point(630, 15);
        this.lblStatus.Size = new Size(80, 15);
        this.lblStatus.Text = "Disconnected";
        
        // Messages listbox
        this.lstMessages.FormattingEnabled = true;
        this.lstMessages.ItemHeight = 15;
        this.lstMessages.Location = new Point(12, 50);
        this.lstMessages.Size = new Size(776, 450);
        this.lstMessages.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        
        // Message textbox
        this.txtMessage.Enabled = false;
        this.txtMessage.Location = new Point(12, 520);
        this.txtMessage.Size = new Size(700, 23);
        this.txtMessage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.txtMessage.KeyPress += new KeyPressEventHandler(this.TxtMessage_KeyPress);
        
        // Send button
        this.btnSend.Enabled = false;
        this.btnSend.Location = new Point(720, 520);
        this.btnSend.Size = new Size(68, 23);
        this.btnSend.Text = "Send";
        this.btnSend.UseVisualStyleBackColor = true;
        this.btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.btnSend.Click += new EventHandler(this.BtnSend_Click);
        
        // Add controls to form
        this.Controls.Add(this.lblUsername);
        this.Controls.Add(this.txtUsername);
        this.Controls.Add(this.lblRoom);
        this.Controls.Add(this.cmbRoom);
        this.Controls.Add(this.btnConnect);
        this.Controls.Add(this.btnDisconnect);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.lstMessages);
        this.Controls.Add(this.txtMessage);
        this.Controls.Add(this.btnSend);
        
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
}
