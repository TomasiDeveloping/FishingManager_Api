﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Api.Dtos;
using Api.Entities;
using Api.Helper;
using Api.Helper.Methods;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FishingManagerContext _context;
        private readonly DatabaseLogger _logger;
        private readonly IServiceRepository _serviceRepository;

        public UserRepository(FishingManagerContext context, DatabaseLogger logger, IServiceRepository serviceRepository)
        {
            _context = context;
            _logger = logger;
            _serviceRepository = serviceRepository;
        }
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Address)
                .Include(u => u.Right)
                .Select(u => new UserDto()
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RightId = u.RightId,
                    Active = u.Active,
                    RightName = u.Right.Name,
                    PictureUrl = u.PictureUrl,
                    Address = u.Address,
                    UserFlag = u.UserFlag
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<List<LicenceDto>> GetUserLicencesAsync(int userId)
        {
            return await _context.Licences
                .Include(l => l.User)
                .Include(l => l.Creator)
                .Where(l => l.UserId == userId)
                .Select(l => new LicenceDto()
                {
                    Paid = l.Paid,
                    CreatorId = l.CreatorId,
                    CreatorName = $"{l.Creator.FirstName} {l.Creator.LastName}",
                    UserName = $"{l.User.FirstName} {l.User.LastName}",
                    EndDate = l.EndDate,
                    StartDate = l.StartDate,
                    LicenceName = l.LicenseName,
                    LicenceId = l.Id,
                    UserId = l.UserId,
                    Year = l.Year
                })
                .AsNoTracking()
                .OrderByDescending(l => l.Year)
                .ThenBy(l => l.EndDate)
                .ToListAsync();
        }

        public async Task<List<StatisticDto>> GetUserStatisticsAsync(int userId)
        {
            var statistics = await _context.Statistics
                .Include(s => s.User)
                .Include(s => s.Licence)
                .Where(s => s.User.Id == userId)
                .AsNoTracking()
                .OrderByDescending(l => l.Year)
                .ToListAsync();
        
            var dtoList = new List<StatisticDto>();

            foreach (var item in statistics)
            {
                var dto = new StatisticDto();
                var xml = new XmlDocument();
                xml.LoadXml(item.StatisticXml);

                dto.Id = item.Id;
                dto.Year = item.Year;
                dto.FullName = $"{item.User.FirstName} {item.User.LastName}";
                dto.UserId = item.UserId;
                dto.LicenceName = item.Licence.LicenseName;
                var statistic = new CatchStatistic
                {
                    FishingClub = xml.SelectSingleNode("Statistik/Fischerverein")?.InnerText,
                    Year = xml.SelectSingleNode("Statistik/Jahr")?.InnerText,
                    FirstName = xml.SelectSingleNode("Statistik/Vorname")?.InnerText,
                    LastName = xml.SelectSingleNode("Statistik/Nachname")?.InnerText,
                    Months = new List<Months>()
                };

                var months = xml.SelectNodes("Statistik/Monate");
                if (months != null)
                    foreach (XmlNode month in months)
                    {
                        if (month.HasChildNodes == false) continue;
                        var newMonth = new Months
                        {
                            Month = month.SelectSingleNode("Monat")?.InnerText, Days = new List<Days>()
                        };
                        statistic.Months.Add(newMonth);

                        var days = month.SelectNodes("Tage");
                        if (days == null) continue;
                        foreach (XmlNode day in days)
                        {
                            if (day.HasChildNodes == false) continue;
                            var newTag = new Days
                            {
                                Day = day.SelectSingleNode("Tag")?.InnerText,
                                Hour = day.SelectSingleNode("Stunden")?.InnerText,
                                FishCatches = new List<FishCatch>()
                            };
                            newMonth.Days.Add(newTag);

                            var fishCatches = day.SelectNodes("Fang");
                            if (fishCatches == null) continue;
                            foreach (XmlNode fishCatch in fishCatches)
                            {
                                if (fishCatch.HasChildNodes == false) continue;
                                var newFang = new FishCatch
                                {
                                    Number = fishCatch.SelectSingleNode("Anzahl")?.InnerText,
                                    Fish = fishCatch.SelectSingleNode("Fisch")?.InnerText
                                };
                                newTag.FishCatches.Add(newFang);
                            }
                        }
                    }

                dto.Statistic = statistic;
                dtoList.Add(dto);
            }
            return dtoList;
        }

        public async Task<UserDto> InsertUserAsync(UserDto userDto)
        {
            var newPassword = CreatePassword.CreateNewPassword();
            await _context.AddAsync(new User()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                RightId = userDto.RightId,
                PictureUrl = userDto.PictureUrl,
                Active = userDto.Active,
                CreatedAt = DateTime.Now,
                Password = CreatePassword.CreateHash(newPassword),
                Address = userDto.Address,
                UserFlag = 1
            });
            var checkInsert = await Complete();
            if (!checkInsert) return null;
            _logger.InsertDatabaseLog(new DataBaseLog()
            {
                Type = "Neuer User",
                Message = $"Neuer User {userDto.FirstName} {userDto.LastName} wurde hinzugefügt",
                CreatedAt = DateTime.Now
            });
            await _serviceRepository.SendNewUserMailAsync(userDto, newPassword);
            return userDto;
        }
        
        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            var userToUpdate = await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == userDto.UserId);
            userToUpdate.FirstName = userDto.FirstName;
            userToUpdate.LastName = userDto.LastName;
            userToUpdate.Email = userDto.Email;
            userToUpdate.PictureUrl = userDto.PictureUrl;
            userToUpdate.Active = userDto.Active;
            userToUpdate.RightId = userDto.RightId;
            userToUpdate.UserFlag = userDto.UserFlag;
            userToUpdate.Address.City = userDto.Address.City;
            userToUpdate.Address.Phone = userDto.Address.Phone;
            userToUpdate.Address.Street = userDto.Address.Street;
            userToUpdate.Address.Title = userDto.Address.Title;
            userToUpdate.Address.Zip = userDto.Address.Zip;
            userToUpdate.Address.AddressAddition = userDto.Address.AddressAddition;

            var checkUpdate = await Complete();

            return checkUpdate ? userDto : null;
        }

        public async Task<bool> ChangeUserPassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(changePasswordDto.UserId);
            if (user == null) return false;
            user.Password = CreatePassword.CreateHash(changePasswordDto.Password);
            user.UserFlag = 0;
            return await Complete();
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Equals(email));
            return user != null;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var userToDelete = await _context.Users.FindAsync(userId);
            if (userToDelete == null) return false;
            _context.Users.Remove(userToDelete);

            var checkDelete = await Complete();
            if (!checkDelete) return false;
            _logger.InsertDatabaseLog(new DataBaseLog()
            {
                Type = "User gelöscht",
                Message = $"User mit der Id {userId} wurde gelöscht",
                CreatedAt = DateTime.Now
            });
            return true;
        }

        public async Task<bool> Complete()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.InsertDatabaseLog(new DataBaseLog()
                {
                    Type = "Error UserRepository",
                    Message = e.InnerException?.Message,
                    CreatedAt = DateTime.Now
                });
                return false;
            }
        }
    }
}