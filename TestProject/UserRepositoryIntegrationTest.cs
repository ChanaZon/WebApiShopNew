using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{

    public class UserRepositoryIntegrationTest : IClassFixture<DataBaseFixture>
    {

        private readonly IUserRepository _userRepository;
        private readonly MyShopContext _context;

        public UserRepositoryIntegrationTest(DataBaseFixture fixture)
        {
            _context = fixture.Context;
            _userRepository = new UserRepository(_context, NullLogger<UserRepository>.Instance);
        }

        [Fact]
        public async Task AddUser_ValidUser_ShouldSaveToDatabase()
        {
            var user = new User { FirstName = "Chana", LastName = "Zon", UserName = "Chana@Zon", Password = "AAAaaa!!!234" };

            var savedUser = await _userRepository.AddUserAsync(user);

            Assert.NotNull(savedUser);
            Assert.NotEqual(0, savedUser.UserId);
            Assert.Equal("Chana@Zon", savedUser.UserName);
        }


        [Fact]
        public async Task UpdateUser_NonExistingUser_ShouldNotThrowException()
        {
            var nonExistingUser = new User { FirstName = "Chana", LastName = "Zon", UserName = "Chana@Zon", Password = "AAAaaa!!!234" };

            var result = await _userRepository.UpdateUserAsync(90099, nonExistingUser);
            Assert.Null(result);
        }


        [Fact]
        public async Task LoginUser_InvalidCredentials_ReturnNull()
        {
            var result = await _userRepository.Login("invalid@invalid", "WrongPass123");
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUser_ValidUser_ShouldUpdateSuccessfully()
        {
            var user = new User { FirstName = "Chana", LastName = "Zon", UserName = "Chana@Zon", Password = "AAAaaa!!!234" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var updatedUser = new User { FirstName = "Chana1", LastName = "Zon1", UserName = "Chana@Zon1", Password = "AAAaaa!!!2341" };

            var result = await _userRepository.UpdateUserAsync(user.UserId, updatedUser);


            Assert.NotNull(result);
            Assert.Equal(updatedUser.FirstName, result.FirstName);
            Assert.Equal(updatedUser.UserName, result.UserName);
        }
        [Fact]
        public async Task UpdateUser_DuplicateUserName_ReturnsUserWithNullUserName()
        {
            // Arrange
            var existingUser = new User { UserName = "duplicateuser@aaa", FirstName = "Existing", LastName = "User", Password = "Password123" };
            await _context.Users.AddAsync(existingUser);
            await _context.SaveChangesAsync();

            var duplicateUser = new User { UserName = "duplicateuser@aaa", FirstName = "Duplicate", LastName = "User", Password = "Password456" };


            // Act
            var result = await _userRepository.UpdateUserAsync(2, duplicateUser);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.UserName); 
        }

    }
}
