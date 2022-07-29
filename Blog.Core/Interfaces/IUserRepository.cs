using Blog.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> AddDefaultUser(User user);
        Task<User> Login(string username, string password);
        Task<User> GetUser(long userId);
        Task<bool> CheckUsername(string username, long currentUserId);
        Task<bool> EditUsername(long userId, string username);
        Task<bool> EditFullname(long userId, string fullname);
        Task<bool> EditPassword(long userId, string password);
        Task<bool> EditImage(long userId, byte[] image);
    }

}
