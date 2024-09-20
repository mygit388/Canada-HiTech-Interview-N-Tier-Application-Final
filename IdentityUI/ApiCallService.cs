using Dapper;
using IdentityUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace IdentityUI
{
    public class ApiCallService
    {
        private readonly HttpClient client = new HttpClient();
        
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
      
        public ApiCallService()
        {
            client = new HttpClient();
        }



        public async Task StoreRefreshTokenAsync(string Email, string refreshToken, DateTime ExpiryDate)
        {
            var parameters = new
            {
                Email = Email,
                Token = refreshToken,
                ExpiryDate = ExpiryDate
            };

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("sp_StoreRefreshToken", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<ApplicationUser> GetUserByRefreshToken(string refreshToken)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new { Token = refreshToken };
                var user = await connection.QuerySingleOrDefaultAsync<ApplicationUser>("sp_GetUserByRefreshToken", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return user;
            }
        }
       
    }
}
    