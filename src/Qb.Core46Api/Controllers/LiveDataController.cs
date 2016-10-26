using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict;
using Qb.Core46Api.Helpers;
using Qb.Core46Api.Models;

namespace Qb.Core46Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class LiveDataController : Controller
    {
        private readonly ConcurrentDictionary<WebSocket, LiveDataConnection> _allSockets;
        private readonly OpenIddictUserManager<QbUser> _userManager;

        public LiveDataController(OpenIddictUserManager<QbUser> userManager)
        {
            _userManager = userManager;
            _allSockets = new ConcurrentDictionary<WebSocket, LiveDataConnection>();
        }

        [HttpGet]
        public async Task<IActionResult> Start()
        {
            var user = await _userManager.GetUserAsync(User);
            var userGuid = Guid.Parse(user.Id);

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                _allSockets.TryAdd(webSocket, new LiveDataConnection(userGuid, webSocket));

                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // Websocket messages can arrive in chunks, wait for full message before continuing.

                    bool endofmessage;
                    WebSocketReceiveResult msg;
                    var messagebuffer = new byte[10240];
                    var offset = 0;

                    do
                    {
                        var part = new ArraySegment<byte>(messagebuffer, offset, messagebuffer.Length - offset);
                        msg = await webSocket.ReceiveAsync(part, CancellationToken.None);
                        endofmessage = msg.EndOfMessage;
                        offset += msg.Count;
                    } while (!endofmessage);

                    switch (msg.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            var data = new ArraySegment<byte>(messagebuffer, 0, offset);
                            foreach (var entry in _allSockets)
                            {
                                var connection = entry.Value;
                                if (connection.PersonId == userGuid)
                                    connection.QueueSend(data);
                            }
                            break;
                        case WebSocketMessageType.Close:
                            LiveDataConnection ignoredVar;
                            _allSockets.TryRemove(webSocket, out ignoredVar);
                            break;
                        case WebSocketMessageType.Binary:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // This is the end of a WebSocket, shouldn't send enything after the end.
                return new EmptyResult();
            }

            // Not a websocket request hence it is an error.
            return Res.JsonErrorResult("Non websocket request recieved on websocket path.", 400);
        }

        public class LiveDataConnection
        {
            private readonly object _continuationLock = new object();
            private readonly WebSocket _webSocket;
            private Task _previousTask = Task.CompletedTask;

            public LiveDataConnection(Guid personId, WebSocket webSocket)
            {
                _webSocket = webSocket;
                PersonId = personId;
            }

            public Guid PersonId { get; }

            /// <summary>WebSocket only supports a single send at a time so each send must be queued via tasks.</summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public void QueueSend(ArraySegment<byte> data)
            {
                Task<Task> continuation;
                lock (_continuationLock)
                {
                    continuation = _previousTask.ContinueWith(async prev =>
                    {
                        // If the connection is not open the continuation will do nothing.
                        if (_webSocket.State == WebSocketState.Open)
                            await _webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                    });
                    _previousTask = continuation;
                }
                //return continuation;
            }
        }

        public struct SensorReading
        {
            public SensorReading(Guid id, double value, DateTimeOffset timestamp, TimeSpan duration)
            {
                Value = value;
                Duration = duration;
                Timestamp = timestamp;
                SensorId = id;
            }

            public double Value { get; }

            public TimeSpan Duration { get; }

            public DateTimeOffset Timestamp { get; }

            public Guid SensorId { get; }
        }
    }
}