namespace EpiTrello.API.Interfaces;

public interface IRequestTrackingService
{
    bool CheckRequestLimit(string clientIp);
    bool IsBanned(string clientIp, out DateTime bannedUntil);
    void BanClient(string clientIp);
}