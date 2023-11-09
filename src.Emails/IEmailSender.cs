using MimeKit;


namespace src.Emails
{
    public interface IEmailSender
    {
        Task SendEmail(string emailAdress, string subject, string body, IEnumerable<MailboxAddress> ccMailboxAddresses, IEnumerable<MailboxAddress> bccMailBoxAddress);
        Task SendEmailToPersonIncharge(string subject, string body, IEnumerable<MailboxAddress> toMailBoxAddress, IEnumerable<MailboxAddress> ccMailboxAddresses, IEnumerable<MailboxAddress> bccMailAd);
    }
}
