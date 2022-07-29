using Blog.Core.Helpers;
using Blog.Core.Interfaces;
using Blog.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.EF.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly BlogDbContext _context;
        public UserRepository(ILogger<UserRepository> logger,
            BlogDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<bool> AddDefaultUser(User user)
        {
            try
            {
                if (_context.Users.Count() == 0)
                {
                    await _context.Users.AddAsync(user);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<User> Login(string username, string password)
        {
            try
            {
                var _user = await _context.Users.SingleOrDefaultAsync(a => a.Username == username);
                if (_user != null)
                {
                    string salt = _user.PasswordSalt;
                    string hashPassword = PasswordHelper.GenerateHash(password, salt);
                    if (string.Equals(hashPassword, _user.Passwordhash, StringComparison.Ordinal))
                    {
                        return _user;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<User> GetUser(long userId)
        {
            try
            {
                User userTaregt = new User();
                User user = await _context.Users
                    .SingleOrDefaultAsync(a => a.Id == userId
                    && a.IsDeleted == false);
                if (user == null)
                    return null;
                if (user != null)
                    userTaregt.Id = user.Id;
                userTaregt.Fullname = user.Fullname;
                userTaregt.Username = user.Username;
                userTaregt.Image = user.Image;
                userTaregt.CreatedDate = user.CreatedDate;

                return userTaregt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<bool> CheckUsername(string username, long currentUserId)
        {
            try
            {
                return await _context.Users.AnyAsync(a => a.Username == username && a.Id != currentUserId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<bool> EditFullname(long userId, string Fullname)
        {
            try
            {
                User user = await _context.Users.SingleOrDefaultAsync(a => a.Id == userId
                && a.IsDeleted == false);
                if (user != null || userId == user.Id)
                {
                    user.Fullname = Fullname;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<bool> EditImage(long userId, byte[] Image)
        {
            try
            {
                User user = await _context.Users.SingleOrDefaultAsync(a => a.Id == userId
                && a.IsDeleted == false);
                if (user != null || userId == user.Id)
                {
                    user.Image = Image;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<bool> EditPassword(long userId, string Password)
        {
            try
            {
                string salt = PasswordHelper.GenerateSalt();
                User user = await _context.Users.SingleOrDefaultAsync(a => a.Id == userId
                && a.IsDeleted == false);

                if (user != null || userId == user.Id)
                {
                    user.PasswordSalt = salt;
                    user.Passwordhash = PasswordHelper.GenerateHash(Password, salt);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<bool> EditUsername(long userId, string Username)
        {
            try
            {
                User user = await _context.Users.SingleOrDefaultAsync(a => a.Id == userId
                && a.IsDeleted == false);
                if (user != null || userId == user.Id)
                {
                    user.Username = Username;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }


    }
}
