using JukeboxWeb.Model;
using Newtonsoft.Json.Linq;
using SocketIOClient;

namespace JukeboxWeb.Services;

public class JukeboxSocketService
{
    public JukeboxSocketService(JukeboxService jukeboxService)
    {
        JukeboxService = jukeboxService;
    }

    private SocketIO Socket { get; set; }
    public Action? OnChange { get; set; }
    public List<Track>? Queue { get; set; }
    private JukeboxService JukeboxService { get; set; }

    public async Task Init(Action updateQueue)
    {
        const string baseAddress = "http://localhost:8080";
        Socket = new SocketIO(baseAddress, new SocketIOOptions { Reconnection = true, });
        Socket.OnConnected += (sender, e) => { Console.WriteLine("Socket connected"); };
        Socket.On("queueUpdated", (res) =>
        {
            try
            {
                var queueAsString = res.GetValue().ToString();

                var parsed = JArray.Parse(queueAsString).ToObject<List<Dictionary<string, dynamic>>>();
                var tracks = parsed.Select(json => Track.FromJson(json)).ToList();
                tracks.ForEach((t) => Console.WriteLine($"queue {tracks.IndexOf(t)} {t.Title}"));
                Queue = tracks;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (JukeboxService.NowPlaying?.PosId != Queue.FirstOrDefault()?.PosId)
                {
                    updateQueue.Invoke();
                }

                NotifyStateChanged();
            }
        });

        await Socket.ConnectAsync();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}