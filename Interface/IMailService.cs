namespace ISLE.Interfaces{
    public interface IMailService{
        Task<bool> SendMailAsync(string to, string subject, string body);
    }
}