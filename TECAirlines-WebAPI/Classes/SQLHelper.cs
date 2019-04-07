using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;

namespace TECAirlines_WebAPI.Classes
{
    public class SQLHelper
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

        public static bool AirplaneExists(string model, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select model from AIRPLANES where model = @model";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("model", model));

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

        public static bool AirportExists(string ap, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select ap_name from AIRPORT where ap_name = @name";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("name", ap));

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

        public static Tuple<int, int> GetPlaneDetails(string model, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select plane_id, capacity from AIRPLANES where model = @model";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("model", model));

            Tuple<int, int> result = null;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        result = new Tuple<int, int>(reader.GetInt32(0), reader.GetInt32(1));
                    }
                    connection.Close();
                    return result;
                }               
            }
            return result;
        }
    }
}