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

        public static Tuple<int, int, int> GetPlaneDetails(string model, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select plane_id, capacity, fc_capacity from AIRPLANES where model = @model";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("model", model));

            Tuple<int, int, int> result = null;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        result = new Tuple<int, int, int>(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2));
                    }
                    connection.Close();
                    return result;
                }               
            }
            return result;
        }

        public static string CheckFlightState(string flight_id, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select status from FLIGHT where flight_id = @id";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", flight_id));

            string status = "";

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        status = reader.GetString(0);
                    }
                }
            }

            connection.Close();

            return status;
        }

        public static int GetSeatsLeft(string seat_type, string flight_id, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select " + seat_type + " from FLIGHT where flight_id = @id";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", flight_id));

            int result = -1;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        result = Convert.ToInt32(reader[0]);
                    }
                }
            }
            connection.Close();
            return result;
        }

        public static bool IsFlightFull(string flight_id, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select seats_left, fc_seats_left from FLIGHT where flight_id = @id";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", flight_id));

            bool result = false;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if ((int)reader[0] == 0 && (int)reader[1] == 0) result = true;
                        else result = false;
                    }
                }
            }
            connection.Close();
            return result;
        }

        public static float CheckFlightDiscount(string flight, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select exp_date, discount from SALE where flight_id = @id";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", flight));

            DateTime today = DateTime.Today;

            int result = 0;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DateTime sale_exp = reader.GetDateTime(0);
                        if (today <= sale_exp)
                        {
                            result = reader.GetInt32(1);
                        }
                    }
                }
            }
            connection.Close();
            return result;
        }

        public static bool UniExists(string uni, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select * from UNIVERSITY where uni_name = @name";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("name", uni));

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