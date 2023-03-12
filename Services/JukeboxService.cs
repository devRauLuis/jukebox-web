using System.Net.Http.Json;
using System.Text.Json;
using JukeboxWeb.Model;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace JukeboxWeb.Services;

public class JukeboxService
{
    public JukeboxService(HttpClient client)
    {
        Client = client;
    }

    private HttpClient Client { get; set; }

    public Track? NowPlaying { get; set; }
    public List<Track>? Tracks { get; set; }
    public List<Track>? FilteredTracks { get; set; }

    public bool ErrorOcurred { get; set; }

    public bool ShowTracksLoading { get; set; }
    public bool ShowNowPlayingLoading { get; set; }

    public async Task GetTracks()
    {
        try
        {
            ToggleTracksLoading(true);
            var res = await Client.GetFromJsonAsync<List<Track>>("tracks");
            ToggleErrorOcurred(false);
            Tracks = res;
            FilteredTracks = Tracks;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ToggleErrorOcurred(true);
            throw;
        }
        finally
        {
            ToggleTracksLoading(false);
        }
    }

    public async Task GetNowPlaying()
    {
        try
        {
            ShowTracksLoading = true;
            var res = await Client.GetAsync("tracks-queue/now-playing");

            if (res.IsSuccessStatusCode)
                NowPlaying =
                    JsonSerializer.Deserialize<Track>(
                        await res.Content.ReadAsStreamAsync(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception e)
        {
            ErrorOcurred = true;
            Console.WriteLine(e);
        }
        finally
        {
            ShowTracksLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task AddToQueue(Track track)
    {
        try
        {
            ShowTracksLoading = true;
            var body = new { name = track.SongName };
            var res = await Client.PostAsJsonAsync("/tracks-queue", body);
            //
            // if (res.IsSuccessStatusCode)
            //     NowPlaying =
            //         JsonSerializer.Deserialize<Track>(
            //             await res.Content.ReadAsStreamAsync(),
            //             new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception e)
        {
            ErrorOcurred = true;
            Console.WriteLine(e);
        }
        finally
        {
            ShowTracksLoading = false;
            NotifyStateChanged();
        }
    }

    public void ToggleErrorOcurred(bool? val = null)
    {
        ErrorOcurred = val ?? !ErrorOcurred;
        NotifyStateChanged();
    }

    public void ToggleNowPlayingLoading(bool? val = null)
    {
        ShowNowPlayingLoading = val ?? !ShowNowPlayingLoading;
        NotifyStateChanged();
    }

    public void ToggleTracksLoading(bool? val = null)
    {
        ShowTracksLoading = val ?? !ShowTracksLoading;
        NotifyStateChanged();
    }

    public Action? OnChange { get; set; }

    private void NotifyStateChanged() => OnChange?.Invoke();
}