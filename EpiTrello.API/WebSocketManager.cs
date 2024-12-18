using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace EpiTrello.API;

public class WebSocketManager
{
    private readonly ConcurrentDictionary<long, List<WebSocket>> _boardClients = new();

    public void Subscribe(long boardId, WebSocket webSocket)
    {
        _boardClients.AddOrUpdate(
            boardId,
            _ => new List<WebSocket> { webSocket },
            (_, list) =>
            {
                list.Add(webSocket);
                return list;
            });
    }

    public void Unsubscribe(long boardId, WebSocket webSocket)
    {
        if (_boardClients.TryGetValue(boardId, out var clients))
        {
            clients.Remove(webSocket);
            if (clients.Count == 0)
            {
                _boardClients.TryRemove(boardId, out _);
            }
        }
    }

    public async Task NotifyAsync(long boardId, object update)
    {
        if (_boardClients.TryGetValue(boardId, out var clients))
        {
            var message = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(update));

            foreach (var socket in clients.ToList())
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(
                        new ArraySegment<byte>(message, 0, message.Length),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
                else
                {
                    Unsubscribe(boardId, socket);
                }
            }
        }
    }
}