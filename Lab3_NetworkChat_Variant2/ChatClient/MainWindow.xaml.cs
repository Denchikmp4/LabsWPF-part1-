using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        private const int Port = 8888;
        private const string Address = "127.0.0.1";
        private TcpClient? client;
        private NetworkStream? stream;
        private Thread? listenThread;
        private string userName = "";

        public MainWindow()
        {
            InitializeComponent();
            RecipientBox.Items.Add("Все");
            RecipientBox.SelectedIndex = 0;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            userName = NameBox.Text.Trim();
            if (userName.Length == 0) { MessageBox.Show("Введите имя пользователя."); return; }
            client = new TcpClient(Address, Port);
            stream = client.GetStream();
            SendRaw($"USER|{userName}");
            listenThread = new Thread(Listen) { IsBackground = true };
            listenThread.Start();
            ConnectButton.IsEnabled = false;
            DisconnectButton.IsEnabled = true;
            MessagesList.Items.Add("Подключение выполнено.");
        }

        private void Listen()
        {
            try
            {
                byte[] data = new byte[4096];
                while (client != null && client.Connected && stream != null)
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    if (bytes == 0) break;
                    string message = Encoding.Unicode.GetString(data, 0, bytes);
                    Dispatcher.BeginInvoke(new Action(() => HandleServerMessage(message)));
                }
            }
            catch { }
        }

        private void HandleServerMessage(string message)
        {
            string[] parts = message.Split('|', 3);
            if (parts[0] == "USERS" && parts.Length >= 2)
            {
                string selected = RecipientBox.SelectedItem?.ToString() ?? "Все";
                RecipientBox.Items.Clear();
                foreach (string user in parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries))
                    if (user != userName) RecipientBox.Items.Add(user);
                RecipientBox.SelectedItem = RecipientBox.Items.Contains(selected) ? selected : "Все";
            }
            else if (parts[0] == "FROM" && parts.Length == 3)
                MessagesList.Items.Add($"{parts[1]}: {parts[2]}");
            else if (parts[0] == "INFO" && parts.Length >= 2)
                MessagesList.Items.Add($"[Сервер] {parts[1]}");
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (stream == null) return;
            string recipient = RecipientBox.SelectedItem?.ToString() ?? "Все";
            string text = MessageBox.Text.Trim();
            if (text.Length == 0) return;
            SendRaw($"MSG|{recipient}|{text}");
            MessagesList.Items.Add($"Я -> {recipient}: {text}");
            MessageBox.Clear();
        }

        private void SendRaw(string message)
        {
            if (stream == null) return;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e) => Disconnect();
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => Disconnect();

        private void Disconnect()
        {
            stream?.Close();
            client?.Close();
            ConnectButton.IsEnabled = true;
            DisconnectButton.IsEnabled = false;
        }
    }
}
