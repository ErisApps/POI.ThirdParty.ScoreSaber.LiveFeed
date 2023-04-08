using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using IWebsocketClientLite.PCL;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Helpers.Json;
using POI.ThirdParty.ScoreSaber.LiveFeed.Contracts.Models.ThirdParty.ScoreSaber.Websocket;
using WebsocketClientLite.PCL;
using WebsocketClientLite.PCL.CustomException;

namespace POI.ThirdParty.ScoreSaber.LiveFeed.Client
{
    internal abstract class BaseScoreSaberWebSocketWorker : IHostedService
    {
        private readonly ScoreSaberSerializerContext _scoreSaberSerializerContext;

        private readonly Subject<(IDataframe? dataframe, ConnectionStatus state)> _websocketConnectionSubject;

        private readonly IDisposable _connectionStatusObservable;
        private readonly IDisposable _messageReceivedObservable;

        private MessageWebsocketRx? _websocketClient;
        private IObservable<(IDataframe? dataframe, ConnectionStatus state)>? _websocketConnectObservable;

        public BaseScoreSaberWebSocketWorker()
        {
            var jsonSerializerOptions =
                new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = false }
                    .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            _scoreSaberSerializerContext = new ScoreSaberSerializerContext(jsonSerializerOptions);

            _websocketConnectionSubject = new Subject<(IDataframe? dataframe, ConnectionStatus state)>();

            _connectionStatusObservable = _websocketConnectionSubject
                .Select(static tuple => tuple.state)
                .Where(static state => state
                    is not ConnectionStatus.DataframeReceived
                    and not ConnectionStatus.SingleFrameSending)
                .Do(static state => Console.WriteLine($"Current state: {state}"))
                .Subscribe();

            _messageReceivedObservable = _websocketConnectionSubject
                .Where(static tuple =>
                    tuple is { state: ConnectionStatus.DataframeReceived, dataframe.Message: not null } &&
                    tuple.dataframe.Message.StartsWith('{')) // Only dataframe messages that are json
                .Select(static tuple => tuple.dataframe!.Message!)
                .Select(message => Observable.FromAsync(() => HandleCommand(message)))
                .Concat()
                .Subscribe();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _websocketClient = new MessageWebsocketRx { ExcludeZeroApplicationDataInPong = true };
            _websocketConnectObservable = _websocketClient
                .WebsocketConnectWithStatusObservable(new Uri("wss://scoresaber.com/ws"),
                    handshakeTimeout: TimeSpan.FromSeconds(15))
                .ObserveOn(System.Reactive.Concurrency.ThreadPoolScheduler.Instance)
                .Catch<(IDataframe? dataframe, ConnectionStatus state), WebsocketClientLiteTcpConnectException>(
                    _ => Observable.Return<(IDataframe? dataframe, ConnectionStatus state)>((null,
                        ConnectionStatus.ConnectionFailed)))
                .Catch<(IDataframe? dataframe, ConnectionStatus state), WebsocketClientLiteException>(_ =>
                {
                    Console.WriteLine("A websocket error occurred");
                    return Observable.Return<(IDataframe? dataframe, ConnectionStatus state)>((null,
                        ConnectionStatus.ConnectionFailed));
                });

            _websocketConnectObservable.Subscribe(_websocketConnectionSubject);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _websocketClient?.Dispose();
            _websocketClient = null;

            return Task.CompletedTask;
        }

        private async Task HandleCommand(string message)
        {
            if (!message.StartsWith('{'))
            {
                return;
            }

            var jsonDocument = JsonDocument.Parse(message);

            var commandName = jsonDocument.RootElement.GetProperty("commandName").GetString();
            switch (commandName)
            {
                case "score":
                    var scoreCommand = jsonDocument.Deserialize(_scoreSaberSerializerContext.ScoreCommand);
                    if (scoreCommand is null)
                    {
                        return;
                    }

                    await HandleScoreCommand(scoreCommand);
                    return;
            }
        }

        protected abstract async Task HandleScoreCommand(ScoreCommand command);


        ~BaseScoreSaberWebSocketWorker()
        {
            _websocketConnectionSubject.Dispose();
            _messageReceivedObservable.Dispose();
            _connectionStatusObservable.Dispose();
        }
    }
}