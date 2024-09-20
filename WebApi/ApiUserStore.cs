 using Dapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApi.Models;

namespace WebApi
{
    public class ApiUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        SqlConnection connection;
        PasswordHasher passwordHasher  = new PasswordHasher();
        public ApiUserStore(string connectionString)
        {

            connection = new SqlConnection(connectionString);
            //connection.Open(); // Open the connection once during initialization

        }
        public async Task CreateAsync(ApplicationUser user)
        {
            user.SecurityStamp = Guid.NewGuid().ToString();
            try
            {

                var parameters = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.PasswordHash,
                    user.SecurityStamp,
                    PhoneNumber = (object)user.PhoneNumber ?? DBNull.Value,
                    user.PhoneNumberConfirmed,
                    user.TwoFactorEnabled,
                    LockoutEnd = (object)user.LockoutEnd ?? DBNull.Value,
                    user.LockoutEnabled,
                    user.AccessFailedCount,
                    user.EmailConfirmed
                };

                await connection.ExecuteAsync("sp_CreateUser", parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not create user", ex);
            }
        }

        // DeleteAsync method
        public async Task DeleteAsync(ApplicationUser user)
        {
            try
            {

                var parameters = new { user.Id };
                await connection.ExecuteAsync("sp_DeleteUser", parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not delete user", ex);
            }
        }

        // UpdateAsync method
        public async Task UpdateAsync(ApplicationUser user)
        {
            try
            {

                var parameters = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.PasswordHash,
                    user.SecurityStamp,
                    user.LockoutEnabled,
                    user.AccessFailedCount
                };
                await connection.ExecuteAsync("sp_UpdateUser", parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not update user", ex);
            }
        }

        // FindByIdAsync method
        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var parameters = new { Id = userId };
            var user = await connection.QuerySingleOrDefaultAsync<ApplicationUser>("sp_FindUserById", parameters, commandType: CommandType.StoredProcedure);
            return user;
        }

        // FindByNameAsync method
        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var parameters = new { UserName = userName };
            var user = await connection.QuerySingleOrDefaultAsync<ApplicationUser>("sp_FindUserByName", parameters, commandType: CommandType.StoredProcedure);
            return user;
        }

        // SetPasswordHashAsync method
        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                throw new ArgumentException("User must have a valid Id.");
            }

            user.PasswordHash = passwordHash;

            var parameters = new { user.Id, PasswordHash = passwordHash };
            await connection.ExecuteAsync("sp_SetPasswordHashAsync", parameters, commandType: CommandType.StoredProcedure);
        }

        // GetPasswordHashAsync method
        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {


            return Task.FromResult(user.PasswordHash);
        }

        // HasPasswordAsync method
        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }
        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var verificationResult = passwordHasher.VerifyHashedPassword(user.PasswordHash , password);
            return Task.FromResult(verificationResult == PasswordVerificationResult.Success);
        }
        public async Task SetEmailAsync(ApplicationUser user, string email)
        {
            user.Email = email;
            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new { user.Id, Email = email };
                await connection.ExecuteAsync("sp_SetEmailAsync", parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            // Assuming you have a column named "EmailConfirmed" in your database
            return Task.FromResult(false); // Adjust this according to your implementation\
                                           //  return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            // Implement this method if you have an "EmailConfirmed" column
            return Task.CompletedTask; // Adjust this according to your implementation
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var parameters = new { Email = email };
            var user = await connection.QuerySingleOrDefaultAsync<ApplicationUser>("sp_FindUserByEmail", parameters, commandType: CommandType.StoredProcedure);
            return user;
        }

        // Dispose method
        public void Dispose()
        {
            // Dispose any resources if necessary
        }

    }
}