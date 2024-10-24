namespace EpiTrello.API.Interfaces;

public class RequestTrackingService : IRequestTrackingService
{
    private Dictionary<string, List<DateTime>> _requestTimestamps;
    private Dictionary<string, DateTime> _bannedUntil;
    private const int RequestLimit = 5;
    private const int BanDurationInMinutes = 5;
    private const int TimeWindowInSeconds = 10;

    public RequestTrackingService()
    {
        _requestTimestamps = new Dictionary<string, List<DateTime>>();
        _bannedUntil = new Dictionary<string, DateTime>();
    }

    public bool CheckRequestLimit(string clientIp)
    {
        var now = DateTime.UtcNow;

        if (!_requestTimestamps.ContainsKey(clientIp))
        {
            _requestTimestamps[clientIp] = new List<DateTime>();
        }

        _requestTimestamps[clientIp] = _requestTimestamps[clientIp].Where(t => (now - t).TotalSeconds <= TimeWindowInSeconds).ToList();

        if (_requestTimestamps[clientIp].Count >= RequestLimit)
        {
            return false;
        }

        _requestTimestamps[clientIp].Add(now);

        return true;
    }

    public bool IsBanned(string clientIp, out DateTime bannedUntil)
    {
        return _bannedUntil.TryGetValue(clientIp, out bannedUntil) && DateTime.UtcNow < bannedUntil;
    }

    public void BanClient(string clientIp)
    {
        _bannedUntil[clientIp] = DateTime.UtcNow.AddMinutes(BanDurationInMinutes);
    }
}