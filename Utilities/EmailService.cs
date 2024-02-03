using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using MimeKit;
using MimeKit.Text;
using SecretSantaApi.Dto;
using SecretSantaApi.Interfaces;

namespace SecretSantaApi.Utilities
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly string _emailUsername;
        private readonly string _emailPassword;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
            _emailUsername = config["emailUsername"];
            _emailPassword = config["emailPassword"];
        }

        public async Task SendVerificationEmail(VerifyEmailDto request)
        {
           
            try
            {
                
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_emailUsername));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = "Secret Santa Verify Your Email";
                email.Body = new TextPart(TextFormat.Html) { Text = 
                    "<h1>Please click the link to verify your email.</h1>" +
                    "<br>" +
                    $"<h2><a href='{request.VerifyLink}'>Verify Email</a></h2>"
                };


                using var smtp = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput()));
                //smtp.CheckCertificateRevocation = true;
                
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTlsWhenAvailable);
                await smtp.AuthenticateAsync(_emailUsername, _emailPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);


            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error sending email: " + ex.Message);
                throw;
            }

        }

        public async Task PasswordResetEmail(PasswordResetEmailDto request)
        {

            try
            {

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_emailUsername));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = "Secret Santa Password reset";
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text =
                    "<h1>Please click the link to reset your password.</h1>" +
                    "<br>" +
                    $"<h2><a href='{request.ResetLink}'>Reset password</a></h2>"
                };


                using var smtp = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput()));
                //smtp.CheckCertificateRevocation = true;

                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTlsWhenAvailable);
                await smtp.AuthenticateAsync(_emailUsername, _emailPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email: " + ex.Message);
                throw;
            }

        }

        public async Task SendSecretSantasEmail(ICollection<PersonDto> santas)
        {
            foreach (var santa in santas)
            {
                try
                {

                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(_emailUsername));
                    email.To.Add(MailboxAddress.Parse(santa.Email));
                    email.Subject = "Your Secret Santa Info";
                    email.Body = new TextPart(TextFormat.Html)
                    {
                        Text =
                        "<h1>Congrats you are a secret Santa!</h1>" +
                        "<br>" +
                        $"<h2>You have to buy a gift for {santa.giverGiftee}</h2>"
                    };


                    using var smtp = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput()));
                    //smtp.CheckCertificateRevocation = true;

                    await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTlsWhenAvailable);
                    await smtp.AuthenticateAsync(_emailUsername, _emailPassword);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending email: " + ex.Message);
                    throw;
                }

            }

        }
    }
}
