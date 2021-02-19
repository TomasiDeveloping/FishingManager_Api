using System.Text;
using Api.Dtos;
using MimeKit;

namespace Api.Helper.Methods
{
    public static class EmailService
    {
        public static MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(message.EmailFrom));
            emailMessage.To.Add(new MailboxAddress(message.EmailTo));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"<h2>{message.Subject}</h2><br><br><p>{GetText(message.Content)}</p>"
            };
            return emailMessage;
        }
        
        private static string GetText(string messageContent)
        {
            var stringBuilder = new StringBuilder();
            var lines = messageContent.Split("\n");
            foreach (var line in lines)
            {
                stringBuilder.Append(line + "<br>\n");
            }

            return stringBuilder.ToString();
        }
        
        public static string CreateNewPasswordText(string newPassword)
        {
            return $"<h2>Neues Passwort Fischer Lizenz Manager</h2><br>" +
                   $"<p>Neues Passwort: <b>{newPassword}</b><br>" +
                   $"<p>Bitte ändere das Passwort beim nächsten Login.</p><br>" +
                   $"<p>Freundliche Grüsse</p>" +
                   $"<p>Fischer Lizenz Manager</p>" +
                   $"<br>" +
                   $"<small>Diese E-Mail wurde automatisch generiert, bitte nicht auf diese E-Mail Antworten</small>";
        }
        
        public static string CreateNewUserText(UserDto userDto, string clearTextPassword)
        {
            return $"<h2>Willkommen beim Fischer Lizenz Manager</h2><br>" +
                   $"<h3>Hallo {userDto.FirstName} {userDto.LastName}<h3>" +
                   $"<p>Du kannst ab sofort den Lizenz Manager unter <a href='localhost:4200'>Fischer Manager</a> verwenden</p><br>" +
                   $"<p>Login E-Mail: <b>{userDto.Email}</b></p>" +
                   $"<p>Passwort: <b>{clearTextPassword}</b></p><br>" +
                   $"<p>Freundliche Grüsse</p>" +
                   $"<p>Fischer Lizenz Manager</p><br>" +
                   $"<small>Diese E-Mail wurde automatisch generiert, bitte nicht auf diese E-Mail Antworten</small>";
        }
    }
}