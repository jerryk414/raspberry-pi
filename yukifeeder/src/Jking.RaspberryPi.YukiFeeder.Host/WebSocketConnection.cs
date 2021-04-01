using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jking.RaspberryPi.YukiFeeder.Host
{
    public class WebSocketConnection : IAsyncDisposable
    {
        #region Construction

        private WebSocketConnection(Action<string> onMessageReceived)
        {
            _webSocket = new ClientWebSocket();
            _cts = new CancellationTokenSource();
            _onMessageReceived = onMessageReceived;
        }

        #endregion

        #region Fields

        private static Uri _apiUri = new Uri(@"wss://3kfqv32kr2.execute-api.us-east-1.amazonaws.com/production");
        private static int _bufferSize = 8192;

        private readonly ClientWebSocket _webSocket;
        private readonly CancellationTokenSource _cts;
        private readonly Action<string> _onMessageReceived;

        #endregion

        #region Methods

        public static async Task<WebSocketConnection> BeginListen(Action<string> onMessageReceived)
        {
            WebSocketConnection connection = new WebSocketConnection(onMessageReceived);
            await connection.ConnectAsync();

            return connection;
        }

        public async Task ConnectAsync()
        {
            await _webSocket.ConnectAsync(_apiUri, _cts.Token).ConfigureAwait(false);

            Task.Run(async () => await LoopAsync().ConfigureAwait(false));
        }

        private async Task LoopAsync()
        {
            WebSocketReceiveResult receiveResult = null;
            byte[] buffer = new byte[_bufferSize];

            while (!_cts.Token.IsCancellationRequested)
            {
                using (MemoryStream outputStream = new MemoryStream(_bufferSize))
                {
                    do
                    {
                        receiveResult = await _webSocket.ReceiveAsync(buffer, _cts.Token);
                        if (receiveResult.MessageType != WebSocketMessageType.Close)
                            outputStream.Write(buffer, 0, receiveResult.Count);
                    }
                    while (!receiveResult.EndOfMessage);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                        break;

                    outputStream.Position = 0;

                    using (StreamReader reader = new StreamReader(outputStream))
                    {
                        string message = reader.ReadToEnd();

                        _onMessageReceived(message);
                    }
                }
            }
        }

        ~WebSocketConnection()
        {
            DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_webSocket != null)
            {
                if (_webSocket.State.Equals(WebSocketState.Open))
                { 
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).ConfigureAwait(false);
                    //await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).ConfigureAwait(false);
                }

                _webSocket.Dispose();
            }

            _cts?.Dispose();
        }

        #endregion
    }
}
