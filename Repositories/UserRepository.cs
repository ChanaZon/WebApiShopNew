//using Entities;
using DTO;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Repositories

{
    public class UserRepository : IUserRepository
    {
        MyShopContext _context;
        ILogger<UserRepository> _logger;
        public UserRepository(MyShopContext context,ILogger<UserRepository> logger)
        {
            _context = context; 
            _logger = logger;
        }



        public async Task<User> AddUserAsync(User user)
        {
            User duplicate = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (duplicate!=null)
            {
                _logger.LogInformation("user repository: duplicate user");
                user.UserName = null;
                return user;
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async  Task<User> Login(string Password,string UserName)
        {

            User user = await _context.Users.FirstOrDefaultAsync(u =>  u.UserName == UserName && u.Password == Password );
           if(user == null) 
                return null;
            return user;

        }

        public async Task<User> UpdateUserAsync(int id, User userToUpdate)
        {
            if (userToUpdate == null)
            {
                return null;
            }

            User existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return null; 
            }

            User duplicate = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userToUpdate.UserName);
            if (duplicate != null && duplicate.UserId != userToUpdate.UserId)
            {
                _logger.LogInformation("user repository: duplicate user");
                userToUpdate.UserName = null; 
                return userToUpdate;
            }

            existingUser.UserName = userToUpdate.UserName;
            existingUser.FirstName = userToUpdate.FirstName;
            existingUser.LastName = userToUpdate.LastName;
            existingUser.Password = userToUpdate.Password;


            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}
