using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TECAirlines_WebAPI.Classes
{
    public class CustomerSQLHandler
    {
        private static readonly string connect_str = "Data Source=.;Initial Catalog=TecAirlinesDB;Integrated Security=True";

        public static string FindFlight(Flight flight)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select status, flight_id, depart_date from FLIGHT where depart_ap = @depart and arrival_ap = @arrival";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("depart", flight.depart_ap));
            cmd.Parameters.Add(new SqlParameter("arrival", flight.arrival_ap));

            string result_str = "";

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read()) result_str = JSONHandler.BuildFlightSearchResult(JSONHandler.FormatAsString(reader["status"]),
                                                                                    JSONHandler.FormatAsString(reader["flight_id"]),
                                                                                    JSONHandler.FormatAsString(reader["depart_date"]));
                else result_str = JSONHandler.BuildErrorJSON("Flight Not Found");
            }

            connection.Close();

            return result_str;
        }

        public static int LoginCustomer(Customer cust)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select username, password from CUSTOMER where username = @user and password = @passw";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("user", cust.username));
            cmd.Parameters.Add(new SqlParameter("passw", cust.password));

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    connection.Close();
                    return (int)HTTPStatus.OK;
                }
                else
                {
                    connection.Close();
                    return (int)HTTPStatus.UNAUTHORIZED;
                }
            }
        }

        //public static int AddCreditCard(CCard card)
    }
}