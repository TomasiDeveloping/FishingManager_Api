using System;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Api.Helper;
using Api.Helper.Methods;
using Api.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace Api.Data.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly DatabaseLogger _logger;
        private readonly FishingManagerContext _context;

        public ServiceRepository(EmailConfiguration emailConfiguration, DatabaseLogger logger, FishingManagerContext context)
        {
            _emailConfiguration = emailConfiguration;
            _logger = logger;
            _context = context;
        }
        public async Task<bool> SendContactMailAsync(ContactDto contactDto)
        {
            var message = new Message()
            {
                Content = contactDto.Message,
                Subject = "Kontaktformular Fischer Lizenz Manager",
                EmailFrom = contactDto.Email,
                EmailTo = _emailConfiguration.SupportEmail
            };
            var emailMessage = EmailService.CreateEmailMessage(message);
            return await SendAsync(emailMessage);
        }

        public async Task<bool> SendNewPasswordMailAsync(User user)
        {
            var newPassword = CreatePassword.CreateNewPassword();
            var hashPassword = CreatePassword.CreateHash(newPassword);
            var content = EmailService.CreateNewPasswordText(newPassword);
            var message = new Message()
            {
                Content = content,
                Subject = "Neues Password",
                EmailTo = user.Email,
                EmailFrom = _emailConfiguration.From,
            };
            var emailMessage = EmailService.CreateEmailMessage(message);
            var checkMailSend = await SendAsync(emailMessage);
            if (!checkMailSend) return false;
            var userToUpdate = await _context.Users.FindAsync(user.Id);
            userToUpdate.Password = hashPassword;
            userToUpdate.UserFlag = 1;
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SendNewUserMailAsync(UserDto userDto, string clearTextPassword)
        {
            var content = EmailService.CreateNewUserText(userDto, clearTextPassword);
            var message = new Message()
            {
                Content = content,
                Subject = "Willkommen beim Fischer Lizenz Manager",
                EmailFrom = _emailConfiguration.From,
                EmailTo = userDto.Email
            };
            var emailMessage = EmailService.CreateEmailMessage(message);
            return await SendAsync(emailMessage);
        }


        private async Task<bool> SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfiguration.UserName, _emailConfiguration.Password);
                await client.SendAsync(mailMessage);
                return true;
            }
            catch(Exception ex)
            {
                _logger.InsertDatabaseLog(new DataBaseLog()
                {
                    Type = "Email Send Fehler",
                    Message = ex.Message,
                    CreatedAt = DateTime.Now
                });
                return false;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }

}