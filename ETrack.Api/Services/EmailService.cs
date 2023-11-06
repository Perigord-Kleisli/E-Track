using MailKit.Net.Smtp;
using ETrack.Api.Services.Contracts;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace ETrack.Api.Services
{
    public class EmailDto
    {
       public required string To { get; set; } 
       public string Subject { get; set; } = "";
       public required string Body { get; set; }
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void sendEmail(EmailDto emailDto)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("chadrick.hoppe@ethereal.email"));
            email.To.Add(MailboxAddress.Parse(emailDto.To));
            email.Subject = emailDto.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = emailDto.Body };

            using var smtp = new SmtpClient();

            smtp.Connect(configuration["Smtp:Host"], 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(configuration["Smtp:User"], configuration["Smtp:Pass"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }
}