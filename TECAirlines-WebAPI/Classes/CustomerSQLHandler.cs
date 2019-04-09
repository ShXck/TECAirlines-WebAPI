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

            cmd.Parameters.Add(new SqlParameter("username", card.username));
            cmd.Parameters.Add(new SqlParameter("c_nmbr", encr_cnumbr));
            cmd.Parameters.Add(new SqlParameter("sec_code", encr_sec));
            cmd.Parameters.Add(new SqlParameter("exp_date", card.exp_date));

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result;
        }

        public static int GetReservationCost(Reservation res)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select normal_price, fc_price from FLIGHT where flight_id = @id";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", res.flight_id));

            int cost = 0;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    if (!res.is_first_class) cost += res.people_flying * reader.GetInt32(0);
                    else cost += res.people_flying * reader.GetInt32(1);

                    if (res.type.Equals("Ida y Vuelta")) cost *= 2;
                }
            }
            connection.Close();
            return cost;
        }

        public static string BookFlight(Reservation b_detail)
        {
            if (SQLHelper.CheckFlightState(b_detail.flight_id, connect_str).Equals("Active"))
            {
                SqlConnection connection = new SqlConnection(connect_str);
                connection.Open();

                string req = "insert into RESERVATION VALUES (@username, @flight_id, @type, @is_fc, @people)";
                SqlCommand cmd = new SqlCommand(req, connection);

                cmd.Parameters.Add(new SqlParameter("username", b_detail.username));
                cmd.Parameters.Add(new SqlParameter("flight_id", b_detail.flight_id));
                cmd.Parameters.Add(new SqlParameter("type", b_detail.type));
                cmd.Parameters.Add(new SqlParameter("is_fc", b_detail.is_first_class));
                cmd.Parameters.Add(new SqlParameter("people", b_detail.people_flying));

                int result = cmd.ExecuteNonQuery();

                connection.Close();

                if (result == 1) return JSONHandler.BuildSuccessJSON("Flight is Active");
                else return JSONHandler.BuildErrorJSON("Reservation could not be completed");
            } else
            {
                return JSONHandler.BuildErrorJSON("The flight selected is no longer available for booking");
            }
        }

  
    }
}