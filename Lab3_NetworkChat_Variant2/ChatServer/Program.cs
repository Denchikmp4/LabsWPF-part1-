using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

const int port = 8888;
var clients = new ConcurrentDictionary<string, TcpClient>();
var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
listener.Start();
Console.WriteLine($"Сервер запущен на 127.0.0.1:{port}");

while (true)
{
    var tcp = listener.AcceptTcpClient();
    _ = Task.Run(() => ProcessClient(tcp));
}

async Task ProcessClient(TcpClient tcp)
{
    string user = "";
    try
    {
        using var stream = tcp.GetStream();
        while (tcp.Connected)
        {
            string message = await ReadMessage(stream);
            if (message.Length == 0) break;
            string[] parts = message.Split('|', 3);
            if (parts[0] == "USER" && parts.Length >= 2)
            {
                user = parts[1];
                clients[user] = tcp;
                Console.WriteLine($"Подключен пользователь: {user}");
                await BroadcastInfo($"Пользователь {user} подключен");
                await SendUsers();
            }
            else if (parts[0] == "MSG" && parts.Length == 3)
            {
                string recipient = parts[1];
                string text = parts[2];
                if (recipient == "Все")
                    await Broadcast($"FROM|{user}|{text}", except: "");
                else if (clients.TryGetValue(recipient, out var receiver))
                    await Send(receiver, $"FROM|{user}|{text}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        if (!string.IsNullOrWhiteSpace(user))
        {
            clients.TryRemove(user, out _);
            await BroadcastInfo($"Пользователь {user} отключен");
            await SendUsers();
        }
        tcp.Close();
    }
}

async Task<string> ReadMessage(NetworkStream stream)
{
    byte[] data = new byte[4096];
    int bytes = await stream.ReadAsync(data, 0, data.Length);
    return bytes == 0 ? "" : Encoding.Unicode.GetString(data, 0, bytes);
}

async Task Send(TcpClient client, string message)
{
    if (!client.Connected) return;
    byte[] data = Encoding.Unicode.GetBytes(message);
    await client.GetStream().WriteAsync(data, 0, data.Length);
}

async Task Broadcast(string message, string except)
{
    foreach (var pair in clients)
    {
        if (pair.Key != except) await Send(pair.Value, message);
    }
}

async Task BroadcastInfo(string text) => await Broadcast($"INFO|{text}", except: "");
async Task SendUsers() => await Broadcast("USERS|Все," + string.Join(',', clients.Keys.OrderBy(x => x)), except: "");
