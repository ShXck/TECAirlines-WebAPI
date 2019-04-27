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

        /// <summary>
        /// Busca vuelos en la base a partir de los datos del vuelo.
        /// </summary>
        /// <param name="flight">Los datos del vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
        public static string FindFlight(Flight flight)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select flight_id, depart_date, normal_price, fc_price from FLIGHT where depart_ap = @depart and arrival_ap = @arrival and status = @stat";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("depart", flight.depart_ap));
            cmd.Parameters.Add(new SqlParameter("arrival", flight.arrival_ap));
            cmd.Parameters.Add(new SqlParameter("stat", "Active"));

            List<string> flight_lst = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        flight_lst.Add(JSONHandler.BuildFlightSearchResult(reader.GetString(0),
                                                                           reader.GetDateTime(1).ToString(),
                                                                           reader.GetInt32(2),
                                                                           reader.GetInt32(3)));
                    }
                    connection.Close();
                    return JSONHandler.BuildListStrResult("flights", flight_lst);
                }
                else
                {
                    connection.Close();
                    return JSONHandler.BuildMsgJSON(0, "No flights were found.");
                }
            }
        }

        /// <summary>
        /// Verifica las credenciales de inicio de un Cliente.
        /// </summary>
        /// <param name="cust">Los datos del cliente.</param>
        /// <returns>El resultado de la operación.</returns>
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

        /// <summary>
        /// Agrega una nueva tarjeta a la base.
        /// </summary>
        /// <param name="card">Los datos de la tarjeta.</param>
        /// <returns>El resultado de la operación.</returns>
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

        /// <summary>
        /// Calcula el costo de la reservación.
        /// </summary>
        /// <param name="res">Los datos de reservación.</param>
        /// <returns>El resultado de la operación.</returns>
        public static Tuple<int, string> GetReservationCost(Reservation res)
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
            return new Tuple<int, string>((int)cost, JSONHandler.BuildCost((int)cost));
        }

        /// <summary>
        /// Crea una reservación en la base.
        /// </summary>
        /// <param name="b_detail">Los detalles de la reservación.</param>
        /// <returns>El resultado de la operación.</returns>
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
                        return JSONHandler.BuildMsgJSON(0, "Your party's size exceeds the amount of seats available for this category.");
                    }
                    else
                    {
                        return JSONHandler.BuildMsgJSON(0, "Something went wrong while placing your reservation. Try again later.");
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
                        return JSONHandler.BuildMsgJSON(0, "Your party's size exceeds the amount of seats available for this category.");
                    }
                    else
                    {
                        return JSONHandler.BuildMsgJSON(0, "Something went wrong while placing your reservation. Try again later.");
                    }
                }
            } else
            {
                return JSONHandler.BuildMsgJSON(0, "The flight selected is no longer available for booking");
            }
        }

        /// <summary>
        /// Inserta la reservación en la base.
        /// </summary>
        /// <param name="res">Detalles de la reservación.</param>
        /// <param name="curr_seats">Asientos disponibles.</param>
        /// <returns>El resultado de la operación.</returns>
        private static string SetReservation(Reservation res, int curr_seats)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "insert into RESERVATION VALUES (@username, @flight_id, @type, @is_fc, @people, @cost)";
            SqlCommand cmd = new SqlCommand(req, connection);

            Tuple<int, string> res_cost = GetReservationCost(res);

            cmd.Parameters.Add(new SqlParameter("username", res.username));
            cmd.Parameters.Add(new SqlParameter("flight_id", res.flight_id));
            cmd.Parameters.Add(new SqlParameter("type", res.type));
            cmd.Parameters.Add(new SqlParameter("is_fc", res.is_first_class));
            cmd.Parameters.Add(new SqlParameter("people", res.people_flying));
            cmd.Parameters.Add(new SqlParameter("cost", res_cost.Item1));

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            if (result == 1)
            {
                int amount = curr_seats - res.people_flying;

                if (res.is_first_class) ReduceSeatsLeft(res.flight_id, "fc_seats_left", amount);
                else ReduceSeatsLeft(res.flight_id, "seats_left", amount);
                
                if(IsStudent(res.username))
                {
                    Random rand = new Random();
                    SQLHelper.AddStudentMiles(res.username, rand.Next(500, 1000), connect_str);
                }

                connection.Close();
                return JSONHandler.BuildMsgJSON(1, "Reservation was placed succesfully. Thank you.");
            }
            else
            {
                connection.Close();
                return JSONHandler.BuildMsgJSON(0, "Reservation could not be completed");
            }
        }

        /// <summary>
        /// Reduce los asientos disponibles en el vuelo.
        /// </summary>
        /// <param name="flight_id">El id del vuelo.</param>
        /// <param name="type">El tipo de vuelo.</param>
        /// <param name="amount">Número de asientos disponibles luego de actualización.</param>
        private static void ReduceSeatsLeft(string flight_id, string type, int amount)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "update FLIGHT set " + type + " = @amount where flight_id = @flight";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("amount", amount));
            cmd.Parameters.Add(new SqlParameter("flight", flight_id));

            cmd.ExecuteNonQuery();

            //if (SQLHelper.IsFlightFull(flight_id, connect_str)) SetFullFlight(flight_id);
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

        /// <summary>
        /// Paga un vuelo y cambia el estado en la base.
        /// </summary>
        /// <param name="card_number">Número de tarjeta.</param>
        /// <param name="sec_code">Código de seguridad.</param>
        /// <param name="user">El nombre de usuario.</param>
        /// <returns>El resultado de la operación.</returns>
        public static string PayFlight(string card_number, string sec_code, string user)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select security_code from PAYMENT_METHOD where card_number = @cnumbr and username = @user";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("cnumbr", Cipher.Encrypt(card_number)));
            cmd.Parameters.Add(new SqlParameter("user", user));

            string result = JSONHandler.BuildMsgJSON(0, "Security code does not match. Try Again."); 

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string og_code = Cipher.Decrypt(reader.GetString(0));
                        if (sec_code.Equals(og_code))
                        {
                            result = JSONHandler.BuildMsgJSON(1, "Card Authentication Succeded.");
                        }
                    }
                } else
                {
                    result = JSONHandler.BuildMsgJSON(0, "There was a problem while retrieving your credit card information. Try again later.");
                }
            }
            connection.Close();

            return result;
        }

        /// <summary>
        /// Obtiene los vuelos de un usuario.
        /// </summary>
        /// <param name="user">El nombre de usuario.</param>
        /// <returns>El resultado de la operación.</returns>
        public static string GetUserFlights(string user)
        {
            List<string> flight_ids = SQLHelper.GetUserFlightID(user, connect_str);

            List<string> flights = new List<string>();

            for(int i = 0; i < flight_ids.Count; i++)
            {
                flights.Add(SQLHelper.GetAirportFlightData(flight_ids.ElementAt(i), connect_str));
            }
            return JSONHandler.BuildListStrResult("flights", flights);
        }

        /// <summary>
        /// Obtiene las tarjetas de un cliente.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <returns>El resultado de la operación.</returns>
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
                    return JSONHandler.BuildMsgJSON(0, "No cards were found.");
                }
            }
        }

        /// <summary>
        /// Obtiene las universidades registradas.
        /// </summary>
        /// <returns>Universidades en la base.</returns>
        public static string GetUniversities()
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select uni_name from UNIVERSITY";
            SqlCommand cmd = new SqlCommand(req, connection);

            List<string> unis_lst = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        unis_lst.Add(reader.GetString(0));
                    }
                    connection.Close();
                    return JSONHandler.BuildListStrResult("universities", unis_lst);
                }
                else
                {
                    connection.Close();
                    return JSONHandler.BuildMsgJSON(0, "No universities were found.");
                }
            }
        }

        /// <summary>
        /// Obtiene los detalles de un vuelo.
        /// </summary>
        /// <param name="flight_id">El id del vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
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
                    message = JSONHandler.BuildMsgJSON(0, message);
                }
            }
            connection.Close();
            return message;
        }

        /// <summary>
        /// Verifica si un cliente es también estudiante.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <returns>El resultado de la operación.</returns>
        public static bool IsStudent(string username)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select is_student from CUSTOMER where username = @name";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("name", username));

            bool result = false;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = reader.GetBoolean(0);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Prechequea a un cliente en la base.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <param name="flight">El identificador del vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
        public static string PreCheckCustomer(string username, string flight)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "insert into PRE_CHECKING VALUES(@flight, @user)";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("flight", flight));
            cmd.Parameters.Add(new SqlParameter("user", username));

            int result = cmd.ExecuteNonQuery();

            if(result == 1)
            {
                return SetCustomerSeats(username, flight);
            } else
            {
                connection.Close();
                return JSONHandler.BuildMsgJSON(0, "Pre Checking could not be completed. Try again later.");
            }
        }

        /// <summary>
        /// Asigna asientos a los clientes.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <param name="flight">El id del identificador.</param>
        /// <returns>El resultado de la operación.</returns>
        private static string SetCustomerSeats(string username, string flight)
        {
            int id_precheck = SQLHelper.GetPreCheckId(username, connect_str);
            int capacity = SQLHelper.GetPlaneCapacity(flight, connect_str);
            int people = GetPeopleFlying(username, flight);

            if (id_precheck != 0)
            {
                Random rd = new Random();
                for (int i = 0; i < people; i++)
                {
                    SQLHelper.AddCustomerSeat(id_precheck, rd.Next(1, capacity).ToString(), connect_str);
                }
                return JSONHandler.BuildMsgJSON(1, "Seats Succesfully placed.");
            } else
            {
                return JSONHandler.BuildMsgJSON(0, "There was an error while placing your seats. Try again later.");
            }
        }

        public static int GetPeopleFlying(string username, string flight)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select people_flying from RESERVATION where flight_id = @flight and username = @user";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("flight", flight));
            cmd.Parameters.Add(new SqlParameter("user", username));

            int result = 0;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            connection.Close();
            return result;
        }

        /// <summary>
        /// Obtiene las promociones de los vuelos registradas en la base.
        /// </summary>
        /// <returns>El resultado de la operación.</returns>
        public static string GetFlightSales()
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "SELECT discount, depart_ap, arrival_ap FROM SALE JOIN FLIGHT ON FLIGHT.flight_id = SALE.flight_id";

            SqlCommand cmd = new SqlCommand(req, connection);

            List<string> sales = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sales.Add(JSONHandler.BuildSale(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                    }
                    connection.Close();
                    return JSONHandler.BuildListStrResult("sales", sales);
                }
            }
            connection.Close();
            return JSONHandler.BuildMsgJSON(0, "No sales were found");
        }
    }
}