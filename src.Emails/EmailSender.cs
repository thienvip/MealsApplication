
using Microsoft.Extensions.Options;
using MimeKit.Text;
using MimeKit;
using System.Net.Mail;
using MailKit.Security;
using MailKit.Net.Smtp;
using MailKit;

namespace src.Emails
{
    public class EmailSender: IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(
         IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmail(string emailAdress, string subject, string body, IEnumerable<MailboxAddress> ccMailboxAddresses, IEnumerable<MailboxAddress> bccMailBoxAddress)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));

            mimeMessage.To.Add(new MailboxAddress(emailAdress));

            if (ccMailboxAddresses != null)
            {
                foreach (var address in ccMailboxAddresses)

                    mimeMessage.Cc.Add(address);
            }
            if (bccMailBoxAddress != null)
            {
                foreach (var address in bccMailBoxAddress)

                    mimeMessage.Bcc.Add(address);
            }

            mimeMessage.Subject = subject;
           
            mimeMessage.Body = new TextPart(TextFormat.Html) { Text = body };

            //using (var client = new MailKit.Net.Smtp.SmtpClient())
            //{
            //    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            //    await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, false);

            //    await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
            //    await client.SendAsync(mimeMessage);
            //    await client.DisconnectAsync(true);

            //    client.Disconnect(true);
            //}
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.LocalDomain = _emailSettings.MailServer;
                client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                await client.ConnectAsync(_emailSettings.MailServer, 587, SecureSocketOptions.StartTlsWhenAvailable).ConfigureAwait(false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailSettings.Sender, _emailSettings.Password);
                await client.SendAsync(mimeMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }

        }

        public async Task SendEmailToPersonIncharge(string subject, string body,
                IEnumerable<MailboxAddress> toMailBoxAddress,
                IEnumerable<MailboxAddress> ccMailboxAddresses,
                IEnumerable<MailboxAddress> bccMailAddresses)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderName));

            if(toMailBoxAddress  != null)
            {
                foreach(var address in toMailBoxAddress)
                    mimeMessage.To.Add(address);
            }
            if (ccMailboxAddresses != null)
            {
                foreach (var address in ccMailboxAddresses)

                    mimeMessage.To.Add(address);
            }

            if (bccMailAddresses != null)
            {
                foreach (var address in bccMailAddresses)

                    mimeMessage.Bcc.Add(address);
            }

            mimeMessage.Subject = subject;

            mimeMessage.Body = new TextPart(TextFormat.Html) { Text = body };

            using (var client = new MailKit.Net.Smtp.SmtpClient()) {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
        }

       
    }
}
