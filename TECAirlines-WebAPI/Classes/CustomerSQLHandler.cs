using System;
using System.Collections.Generic;
using System.Data;
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

            string encr_cnumbr = Cipher.Encrypt(card.card_number);
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

            double cost = 0;
            double discount = SQLHelper.CheckFlightDiscount(res.flight_id, connect_str) / 100;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (!res.is_first_class) cost += res.people_flying * reader.GetInt32(0);
                        else cost += res.people_flying * reader.GetInt32(1);

                        if (res.type.Equals("Ida y Vuelta")) cost *= 2;
                    }
                }
            }
            cost -= cost * discount;
            connection.Close();
            return (int)cost;
        }

        public static string BookFlight(Reservation b_detail)
        {
            if (SQLHelper.CheckFlightState(b_detail.flight_id, connect_str).Equals("Active"))
            {
                if (b_detail.is_first_class)
                {
                    int fc_seats_left = SQLHelper.GetSeatsLeft("fc_seats_left", b_detail.flight_id, connect_str);

                    if (b_detail.people_flying <= fc_seats_left)
                    {
                        return SetReservation(b_detail, fc_seats_left);

                    }
                    else if (fc_seats_left == 0)
                    {
                        return JSONHandler.BuildSuccessJSON("Your party's size exceeds the amount of seats available for this category.");
                    }
                    else
                    {
                        return JSONHandler.BuildErrorJSON("Something went wrong while placing your reservation. Try again later.");
                    }
                }
                else
                {
                    int normal_seats_left = SQLHelper.GetSeatsLeft("seats_left", b_detail.flight_id, connect_str);

                    if (b_detail.people_flying <= normal_seats_left)
                    {
                        return SetReservation(b_detail, normal_seats_left);

                    }
                    else if (normal_seats_left == 0)
                    {
                        return JSONHandler.BuildSuccessJSON("Your party's size exceeds the amount of seats available for this category.");
                    }
                    else
                    {
                        return JSONHandler.BuildErrorJSON("Something went wrong while placing your reservation. Try again later.");
                    }
                }
            } else
            {
                return JSONHandler.BuildErrorJSON("The flight selected is no longer available for booking");
            }
        }

        private static string SetReservation(Reservation res, int curr_seats)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "insert into RESERVATION VALUES (@username, @flight_id, @type, @is_fc, @people, @cost)";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("username", res.username));
            cmd.Parameters.Add(new SqlParameter("flight_id", res.flight_id));
            cmd.Parameters.Add(new SqlParameter("type", res.type));
            cmd.Parameters.Add(new SqlParameter("is_fc", res.is_first_class));
            cmd.Parameters.Add(new SqlParameter("people", res.people_flying));
            cmd.Parameters.Add(new SqlParameter("cost", GetReservationCost(res)));

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            if (result == 1)
            {
                int amount = curr_seats - res.people_flying;

                if (res.is_first_class) ReduceSeatsLeft(res.flight_id, "fc_seats_left", amount);
                else ReduceSeatsLeft(res.flight_id, "seats_left", amount);
                connection.Close();
                return JSONHandler.BuildSuccessJSON("Reservation was placed succesfully. Thank you.");
            }
            else
            {
                connection.Close();
                return JSONHandler.BuildErrorJSON("Reservation could not be completed");
            }
        }

        private static void ReduceSeatsLeft(string flight_id, string type, int amount)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "update FLIGHT set " + type + " = @amount where flight_id = @flight";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("amount", amount));
            cmd.Parameters.Add(new SqlParameter("flight", flight_id));

            cmd.ExecuteNonQuery();

            if (SQLHelper.IsFlightFull(flight_id, connect_str)) SetFullFlight(flight_id);
        }

        private static void SetFullFlight(string flight_id)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "update FLIGHT set status = @stat where flight_id = @flight";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("stat", "Full"));
            cmd.Parameters.Add(new SqlParameter("flight", flight_id));

            cmd.ExecuteNonQuery();
        }


        public static string PayFlight(string card_number, string sec_code, string user)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select security_code from PAYMENT_METHOD where card_number = @cnumbr and username = @user";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("cnumbr", Cipher.Encrypt(card_number)));
            cmd.Parameters.Add(new SqlParameter("user", user));

            string result = JSONHandler.BuildErrorJSON("Security code does not match. Try Again."); 

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string og_code = Cipher.Decrypt(reader.GetString(0));
                        if (sec_code.Equals(og_code))
                        {
                            result = JSONHandler.BuildSuccessJSON("Card Authentication Succeded.");
                        }
                    }
                } else
                {
                    result = JSONHandler.BuildErrorJSON("There was a problem while retrieving your credit card information");
                }
            }
            connection.Close();

            return result;
        }

        public static string GetUserFlights(string user)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select flight_id from RESERVATION where username = @user";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("user", user));

            List<string> user_res = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user_res.Add(reader.GetString(0));
                    }
                    connection.Close();
                    return JSONHandler.BuildListStrResult("flights", user_res);
                }
                else
                {
                    connection.Close();
                    return JSONHandler.BuildErrorJSON("No flights were found.");
                }
            }
        }

        public static string GetCards(string username)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select card_number from PAYMENT_METHOD where username = @user";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("user", username));

            List<string> cards_lst = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        cards_lst.Add(Cipher.Decrypt(reader.GetString(0)));
                    }
                    connection.Close();
                    return JSONHandler.BuildListStrResult("cards", cards_lst);
                }
                else
                {
                    connection.Close();
                    return JSONHandler.BuildErrorJSON("No cards were found.");
                }
            }
        }
        
        public static string GetFlightDetails(string flight_id)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select depart_ap, arrival_ap, normal_price, fc_price FROM FLIGHT where flight_id = @id";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", flight_id));

            string message = "Error retrieving the selected flight";

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        message = JSONHandler.BuildFlightDetails(JSONHandler.FormatAsString(reader[0]),
                                                              JSONHandler.FormatAsString(reader[2]),
                                                              JSONHandler.FormatAsInt(reader[2]),
                                                              JSONHandler.FormatAsInt(reader[3]));
                    }

                } else
                {
                    message = JSONHandler.BuildErrorJSON(message);
                }
            }
            connection.Close();
            return message;
        }
    }
}