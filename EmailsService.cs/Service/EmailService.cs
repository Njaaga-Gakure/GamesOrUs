using EmailsService.Models;
using MailKit.Net.Smtp;
using MimeKit;


namespace EmailsService.Service
{
    public class EmailService
    {

        private readonly string _email;
        private readonly string _password;
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _email = _configuration.GetValue<string>("AzureEmailSettings:Email");
            _password = _configuration.GetValue<string>("AzureEmailSettings:Password");
        }

        public async Task sendEmail(NewUserMessageDTO user, string message, string? subject = "Find your next gaming adventure here!")
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress("GamesOrUs ", _email));

            mimeMessage.To.Add(new MailboxAddress(user.Name, user.Email));

            mimeMessage.Subject = subject;
            var body = new TextPart("html")
            {
                Text = message.ToString()
            };

            mimeMessage.Body = body;

            var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);

            client.Authenticate(_email, _password);

            await client.SendAsync(mimeMessage);

            await client.DisconnectAsync(true);
        }

    }
}