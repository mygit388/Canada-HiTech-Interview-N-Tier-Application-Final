using Dapper;
using IdentityUI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentityUI.ServiceStores
{
    public class CustomRoleStore : IRoleStore<ApplicationRole>
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        SqlConnection connection;
        public CustomRoleStore(string connectionString)
        {

            connection = new SqlConnection(connectionString);
            // connection.Open(); // Open the connection once during initialization

        }
        public async Task DeleteAsync(ApplicationRole role)
        {
            try
            {

                await connection.ExecuteAsync(
                    "sp_DeleteRole",
                    new { Id = role.Id },
                    commandType: System.Data.CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not delete role", ex);
            }
        }

        public async Task<ApplicationRole> FindByIdAsync(string id)
        {
            var role = await connection.QuerySingleOrDefaultAsync<ApplicationRole>(
                    "sp_FindRoleById",
                    new { Id = id },
                    commandType: System.Data.CommandType.StoredProcedure);
            return role;
        }

        public async Task<ApplicationRole> FindByNameAsync(string name)
        {
            var role = await connection.QuerySingleOrDefaultAsync<ApplicationRole>(
                     "sp_FindRoleByName",
                     new { Name = name },
                     commandType: System.Data.CommandType.StoredProcedure);
            return role;
        }
        public async Task UpdateAsync(ApplicationRole role)
        {
            try
            {

                await connection.ExecuteAsync(
                    "sp_UpdateRole",
                    new { Id = role.Id, Name = role.Name },
                    commandType: System.Data.CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not update role", ex);
            }
        }


        public async Task CreateAsync(ApplicationRole role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            try
            {

                await connection.ExecuteAsync(
                    "sp_CreateRole",
                    new { Id = role.Id, Name = role.Name },
                    commandType: System.Data.CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not create role", ex);
            }
        }
        public void Dispose()
        {
            // Dispose any resources if necessary
        }
    }
}