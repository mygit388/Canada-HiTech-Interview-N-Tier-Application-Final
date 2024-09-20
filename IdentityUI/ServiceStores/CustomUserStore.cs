using Dapper;
using IdentityUI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentityUI.ServiceStores
{
    public class CustomUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>,
                              IUserPhoneNumberStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser, string>, IUserLoginStore<ApplicationUser>,
                              IUserLockoutStore<ApplicationUser, string>, IUserRoleStore<ApplicationUser>
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        SqlConnection connection;
        public CustomUserStore(string connectionString)
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
        // GetPhoneNumberAsync
        public async Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await Task.FromResult(user.PhoneNumber);
        }

        // SetPhoneNumberAsync
        public async Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(user.Id)) throw new ArgumentException("User must have a valid Id.");

            user.PhoneNumber = phoneNumber;


            var parameters = new { user.Id, PhoneNumber = (object)phoneNumber ?? DBNull.Value };
            await connection.ExecuteAsync("sp_SetPhoneNumber", parameters, commandType: CommandType.StoredProcedure);

        }

        // GetPhoneNumberConfirmedAsync
        public async Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await Task.FromResult(user.PhoneNumberConfirmed);
        }

        // SetPhoneNumberConfirmedAsync
        public async Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(user.Id)) throw new ArgumentException("User must have a valid Id.");

            user.PhoneNumberConfirmed = confirmed;


            var parameters = new { user.Id, PhoneNumberConfirmed = confirmed };
            await connection.ExecuteAsync("sp_SetPhoneNumberConfirmed", parameters, commandType: CommandType.StoredProcedure);

        }
        public async Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.TwoFactorEnabled = enabled;


            var parameters = new { user.Id, TwoFactorEnabled = enabled };
            await connection.ExecuteAsync("sp_SetTwoFactorEnabledAsync", parameters, commandType: CommandType.StoredProcedure);

        }

        public async Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return await Task.FromResult(user.TwoFactorEnabled);
        }

        // Implementing IUserLoginStore methods

        public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (login == null) throw new ArgumentNullException(nameof(login));


            var parameters = new
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserId = user.Id
            };

            await connection.ExecuteAsync("sp_AddUserLogin", parameters, commandType: CommandType.StoredProcedure);

        }

        public async Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (login == null) throw new ArgumentNullException(nameof(login));


            var parameters = new
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserId = user.Id
            };

            await connection.ExecuteAsync("sp_RemoveUserLogin", parameters, commandType: CommandType.StoredProcedure);

        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var parameters = new { UserId = user.Id };
            var logins = await connection.QueryAsync<UserLoginInfo>(
                "sp_GetUserLogins",
                parameters,
                commandType: CommandType.StoredProcedure);

            return logins.AsList();

        }

        public async Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            if (login == null) throw new ArgumentNullException(nameof(login));


            var parameters = new
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey
            };

            var user = await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
                "sp_FindUserByLogin",
                parameters,
                commandType: CommandType.StoredProcedure);

            return user;

        }
         //Implementation of IUserLockoutStore methods


          public async Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
          {
              if (string.IsNullOrEmpty(user.Id)) throw new ArgumentNullException(nameof(user.Id));

              using (var connection = new SqlConnection(connectionString))
              {
                  var command = new SqlCommand("SELECT LockoutEnd FROM AspNetUsers WHERE Id = @Id", connection);
                  command.Parameters.AddWithValue("@Id", user.Id);

                  connection.Open();
                  var result = await command.ExecuteScalarAsync();
                  if (result == DBNull.Value)
                  {
                      // If the result is DBNull, return null
                      return DateTimeOffset.MinValue;
                  }

                  // Check if the result is of type DateTime
                  if (result is DateTime dateTimeResult)
                  {
                      // Convert DateTime to DateTimeOffset with the system's local time zone
                      return new DateTimeOffset(dateTimeResult, TimeZoneInfo.Local.GetUtcOffset(dateTimeResult));
                  }
                  if (result is DateTimeOffset dateTimeOffsetResult)
                  {
                      return dateTimeOffsetResult;
                  }


                  throw new InvalidCastException("The data retrieved is not of type DateTime.");

              }
          }

          public async Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
          {
              if (user == null)
              {
                  throw new ArgumentNullException(nameof(user));
              }

              user.LockoutEnd = lockoutEnd;
              using (var connection = new SqlConnection(connectionString))
              {
                  var command = new SqlCommand("UPDATE AspNetUsers SET LockoutEnd = @LockoutEnd WHERE Id = @Id", connection);
                  command.Parameters.AddWithValue("@Id", user.Id);
                  command.Parameters.AddWithValue("@LockoutEnd", user.LockoutEnd);

                  connection.Open();
                  await command.ExecuteNonQueryAsync();
              }
          }

          public async Task<int> GetAccessFailedCountAsync(ApplicationUser user)
          {
              if (string.IsNullOrEmpty(user.Id)) throw new ArgumentNullException(nameof(user.Id));

              using (var connection = new SqlConnection(connectionString))
              {
                  var command = new SqlCommand("SELECT AccessFailedCount FROM AspNetUsers WHERE Id = @Id", connection);
                  command.Parameters.AddWithValue("@Id", user.Id);

                  connection.Open();
                  var result = await command.ExecuteScalarAsync();
                  return result != null ? Convert.ToInt32(result) : 0;
              }
          }
          public async Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
          {
              if (string.IsNullOrEmpty(user.Id)) throw new ArgumentNullException(nameof(user.Id));

              using (var connection = new SqlConnection(connectionString))
              {
                  var command = new SqlCommand("SELECT LockoutEnabled FROM AspNetUsers WHERE Id = @Id", connection);
                  command.Parameters.AddWithValue("@Id", user.Id);

                  connection.Open();
                  var result = await command.ExecuteScalarAsync();
                  return result != null && Convert.ToBoolean(result);
              }
          }

          public async Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
          {
              if (string.IsNullOrEmpty(user.Id)) throw new ArgumentNullException(nameof(user.Id));

              using (var connection = new SqlConnection(connectionString))
              {
                  var command = new SqlCommand("UPDATE AspNetUsers SET LockoutEnabled = @LockoutEnabled WHERE Id = @Id", connection);
                  command.Parameters.AddWithValue("@Id", user.Id);
                  command.Parameters.AddWithValue("@LockoutEnabled", true);

                  connection.Open();
                  await command.ExecuteNonQueryAsync();
              }
          }
          public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
          {
              if (user == null)
              {
                  throw new ArgumentNullException(nameof(user));
              }
              user.AccessFailedCount++;

              using (var connection = new SqlConnection(connectionString))
              {
                  var command = new SqlCommand("UPDATE AspNetUsers SET AccessFailedCount = @AccessFailedCount WHERE Id = @Id", connection);
                  command.Parameters.AddWithValue("@Id", user.Id);
                  command.Parameters.AddWithValue("@AccessFailedCount", user.AccessFailedCount);

                  connection.Open();
                  await command.ExecuteNonQueryAsync();
                  var rowsAffected = await command.ExecuteNonQueryAsync();
                  return rowsAffected > 0 ? user.AccessFailedCount : 0;
              }

          }

          public async Task ResetAccessFailedCountAsync(ApplicationUser user)
          {
              if (user == null)
              {
                  throw new ArgumentNullException(nameof(user));
              }

              user.AccessFailedCount = 0;

              using (var connection = new SqlConnection(connectionString))
              {
                  var command = new SqlCommand("UPDATE AspNetUsers SET AccessFailedCount = @AccessFailedCount WHERE Id = @Id", connection);
                  command.Parameters.AddWithValue("@Id", user.Id);
                  command.Parameters.AddWithValue("@AccessFailedCount", user.AccessFailedCount);

                  connection.Open();
                  await command.ExecuteNonQueryAsync();
              }
          }
        
        /* Implementation of IUserLockoutStore methods


        public async Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {

            var result = await connection.QuerySingleOrDefaultAsync<DateTimeOffset?>(
                "sp_GetLockoutEndDate",
                new {Id = user.Id },
                commandType: System.Data.CommandType.StoredProcedure);

             return result ?? DateTimeOffset.MinValue;
           
        }

        public async Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEnd = lockoutEnd;


            await connection.ExecuteAsync(
                "sp_SetLockoutEndDate",
                new {Id = user.Id, LockoutEnd = user.LockoutEnd },
                commandType: System.Data.CommandType.StoredProcedure);

        }


        public async Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            var result = await connection.QuerySingleOrDefaultAsync<int>(
               "sp_GetAccessFailedCount",
               new {Id = user.Id },
               commandType: System.Data.CommandType.StoredProcedure);

           return result;
           
        }

        public async Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            var result = await connection.QuerySingleOrDefaultAsync<bool>(
                 "sp_GetLockoutEnabled",
                 new { Id= user.Id },
                 commandType: System.Data.CommandType.StoredProcedure);

            return result;
        }



        public async Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            if (string.IsNullOrEmpty(user.Id)) throw new ArgumentNullException(nameof(user.Id));

            await connection.ExecuteAsync(
                 "sp_SetLockoutEnabled",
                 new {Id = user.Id, LockoutEnabled = enabled },
                 commandType: System.Data.CommandType.StoredProcedure);
        }
        public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }


            var result = await connection.QuerySingleOrDefaultAsync<int>(
                "sp_IncrementAccessFailedCount",
                new {Id = user.Id },
                commandType: System.Data.CommandType.StoredProcedure);

            user.AccessFailedCount = result;
            return result;

        }

        public async Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await connection.ExecuteAsync(
                  "sp_ResetAccessFailedCount",
                  new { Id = user.Id },
                  commandType: System.Data.CommandType.StoredProcedure);
        }
        */


        //IUserRolestore

        public async Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(nameof(roleName));

            var roleId = await GetRoleIdByNameAsync(roleName);
            if (roleId == null) throw new InvalidOperationException("Role does not exist.");
            var parameters = new { UserId = user.Id, RoleId = roleId };

            try
            {
                await connection.ExecuteAsync("sp_AddToRole", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw new InvalidOperationException("An error occurred while adding the role to the user.", ex);
            }

        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(nameof(roleName));

            var roleId = await GetRoleIdByNameAsync(roleName);
            if (roleId == null) throw new InvalidOperationException("Role does not exist.");

            var parameters = new { UserId = user.Id, RoleId = roleId };

            try
            {
                await connection.ExecuteAsync("sp_RemoveFromRole", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw new InvalidOperationException("An error occurred while removing the role from the user.", ex);
            }

        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));


            var roles = await connection.QueryAsync<string>(
                "sp_GetRoles",
                new { UserId = user.Id },
                commandType: System.Data.CommandType.StoredProcedure);

            return roles.AsList();

        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(nameof(roleName));

            var roleId = await GetRoleIdByNameAsync(roleName);
            if (roleId == null) throw new InvalidOperationException("Role does not exist.");
            try
            {
                var parameters = new
                {
                    UserId = user.Id,
                    RoleId = roleId
                };

                var count = await connection.ExecuteScalarAsync<int>("sp_IsInRole",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                return count > 0;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw new InvalidOperationException("An error occurred while checking the user's role.", ex);
            }

        }

        private async Task<string> GetRoleIdByNameAsync(string roleName)
        {

            var result = await connection.QuerySingleOrDefaultAsync<string>(
                "sp_GetRoleIdByName",
                new { RoleName = roleName },
                commandType: System.Data.CommandType.StoredProcedure);

            return result;

        }

        // Dispose method
        public void Dispose()
        {
            // Dispose any resources if necessary
        }


    }
    
}