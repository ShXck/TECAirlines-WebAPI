using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;

namespace TECAirlines_WebAPI.Classes
{
    public class SQLVerifier
    {
        public static bool UsernameExists(string username, string table, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select username from " + table + " where username = @user";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("user", username));

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    connection.Close();
                    return true;
                }
                else
                {
                    connection.Close();
                    return false;
                }
            }
        }
    }
}