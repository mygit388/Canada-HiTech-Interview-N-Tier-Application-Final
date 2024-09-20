using Interview.Data;
using Interview.Model;
using Interview.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;


namespace Interview.Repository.Implementations
{
    public class ProfileRepository:IProfileRepository
    {
        
        ILoggerRepository _iLog;
        public ProfileRepository(ILoggerRepository iLog)
        {
            _iLog = iLog;
        }
        DBConnections Db = new DBConnections();
        string procedureName;
        SqlParameter[] parameters = null;
        DataTable dt;
        public void Delete(int ProfileId)
        {
            try
            {
                procedureName = "sp_DeleteProfile";
                parameters = new SqlParameter[]
                {
                 new SqlParameter("@ProfileId", SqlDbType.Int) { Value = ProfileId }
                };
                Db.ExecuteNonQuery(procedureName, parameters);
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
        }
        public List<ProfileModel> GetAll()
        {
            try
            {
                dt = new DataTable();
                List<ProfileModel> profileList = new List<ProfileModel>();
                procedureName = "sp_GetProfiles";
                dt = Db.ExecuteQuery(procedureName, parameters);
                foreach (DataRow dr in dt.Rows)
                {
                    profileList.Add(new ProfileModel
                    {
                        ProfileId = (int)dr["ProfileId"],
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        DateOfBirth = (DateTime)dr["DateOfBirth"],
                        PhoneNumber = dr["PhoneNumber"].ToString(),
                        EmailId = dr["EmailId"].ToString()
                    });
                }
                return profileList;
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }                                         
        }
        public ProfileModel GetById(int ProfileId)
        {
            try
            {
                ProfileModel profile = null;
                procedureName = "sp_GetProfilesByID";
                parameters = new SqlParameter[]
                {
                 new SqlParameter("@ProfileId", SqlDbType.Int) { Value=ProfileId }
                };
                dt = Db.ExecuteQuery(procedureName, parameters);
                if (dt.Rows.Count >= 1)
                {
                    profile = new ProfileModel
                    {
                        ProfileId = dt.Rows[0].Field<int>("ProfileId"),
                        FirstName = dt.Rows[0].Field<string>("FirstName"),
                        LastName = dt.Rows[0].Field<string>("LastName"),
                        DateOfBirth = dt.Rows[0].Field<DateTime>("DateOfBirth"),
                        PhoneNumber = dt.Rows[0].Field<string>("PhoneNumber"),
                        EmailId = dt.Rows[0].Field<string>("EmailId")
                    };
                }
                return profile;
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }

        }

        public bool Insert(ProfileModel model)
        {
         try
          {
                int id = 0;
                procedureName = "sp_InsertProfile";
                parameters = new SqlParameter[]
                {
                     new SqlParameter("@FirstName", SqlDbType.VarChar, 50) { Value = model.FirstName },
                     new SqlParameter("@LastName", SqlDbType.VarChar, 50) { Value = model.LastName },
                     new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = model.DateOfBirth },
                     new SqlParameter("@PhoneNumber", SqlDbType.VarChar, 15) { Value = model.PhoneNumber },
                     new SqlParameter("@EmailId", SqlDbType.VarChar, 100) { Value = model.EmailId }
                };
                id = Db.ExecuteNonQuery(procedureName, parameters);

                if (id > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
                
            }
        }
        
        public bool Update(ProfileModel model)
        {
            try
            {
                int id = 0;
                procedureName = "sp_UpdateProfile";
                parameters = new SqlParameter[]
                {
            new SqlParameter("@ProfileId", SqlDbType.Int) { Value = model.ProfileId },
            new SqlParameter("@FirstName", SqlDbType.VarChar, 50) { Value = model.FirstName },
            new SqlParameter("@LastName", SqlDbType.VarChar, 50) { Value = model.LastName },
            new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = model.DateOfBirth },
            new SqlParameter("@PhoneNumber", SqlDbType.VarChar, 15) { Value = model.PhoneNumber },
            new SqlParameter("@EmailId", SqlDbType.VarChar, 100) { Value = model.EmailId }
                };
                id = Db.ExecuteNonQuery(procedureName, parameters);
                if (id > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                _iLog.LogException(ex);
                throw new ApplicationException($"An error occurred : {ex.Message}", ex);
            }
        }
     }
}
