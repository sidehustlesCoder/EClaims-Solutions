namespace eClaims.Core.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
