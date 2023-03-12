namespace JukeboxWeb.Model;

public class Track
{
    public string SongName { get; set; }
    public string Title { get; set; }
    public string TrackNumber { get; set; }
    public string Genre { get; set; }
    public string Artist { get; set; }
    public string ReleaseTime { get; set; }
    public string Year { get; set; }
    public string Album { get; set; }
    public string Image { get; set; }
    public string PosId { get; set; }

    public static Track FromJson(Dictionary<string, dynamic> json)
    {
        var track = new Track
        {
            SongName = json["songName"],
            Title = json["title"],
            TrackNumber = json["trackNumber"],
            Genre = json["genre"],
            Artist = json["artist"],
            ReleaseTime = json["releaseTime"],
            Year = json["year"],
            Album = json["album"],
            Image = json["image"],
            PosId = json["posId"]
        };
        return track;
    }
}