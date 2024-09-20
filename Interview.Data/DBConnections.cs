using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace Interview.Data
{
    public class DBConnections
    {
        private string str = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        public SqlConnection OpenConnection()
        {
            con = new SqlConnection(str);
            con.Open();
            return con;
        }
        public void CloseConnection(SqlConnection con)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        public DataTable ExecuteQuery(string procedureName, SqlParameter[] parameters)
        {
            try
            {
                DataTable dt;
                SqlDataAdapter da;
                con = OpenConnection();
                cmd = new SqlCommand(procedureName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                CloseConnection(con);
                return dt;
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                return null;
            }

        }
        public int ExecuteNonQuery(string procedureName, SqlParameter[] parameters)
        {
            try
            {
                int id = 0;
                DataTable dt;
                SqlDataAdapter da;
                con = OpenConnection();
                cmd = new SqlCommand(procedureName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                id = cmd.ExecuteNonQuery();
                CloseConnection(con);
                return id;
            }
            catch (Exception Ex)
            {

                return 0;
            }
        }
        public SqlDataReader Executereader(string procedureName, SqlParameter[] parameters)
        {
            try
            {
                SqlDataReader dr;
                SqlDataAdapter da;
                con = OpenConnection();
                cmd = new SqlCommand(procedureName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                dr = cmd.ExecuteReader();

                return dr;
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                return null;

            }

        }

    }
}

