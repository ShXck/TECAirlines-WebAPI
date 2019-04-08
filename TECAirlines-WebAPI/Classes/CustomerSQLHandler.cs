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

            string encr_pass = Cipher.Encrypt(cust.password);

            cmd.Parameters.Add(new SqlParameter("user", cust.username));
            cmd.Parameters.Add(new SqlParameter("passw", encr_pass));

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

        public static int AddCreditCard(CCard card)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "insert into PAYMENT_METHOD VALUES (@username, @c_nmbr, @sec_code, @exp_date)";
            SqlCommand cmd = new SqlCommand(req, connection);

            string encr_cnumbr = Cipher.Encrypt(card.card_numbr);
            string encr_sec = Cipher.Encrypt(card.security_code);

            System.Diagnostics.Debug.WriteLine(encr_cnumbr.Length);
            System.Diagnostics.Debug.WriteLine(encr_sec.Length);

            cmd.Parameters.Add(new SqlParameter("username", card.username));
            cmd.Parameters.Add(new SqlParameter("c_nmbr", encr_cnumbr));
            cmd.Parameters.Add(new SqlParameter("sec_code", encr_sec));
            cmd.Parameters.Add(new SqlParameter("exp_date", card.exp_date));

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result;
        }
    }
}