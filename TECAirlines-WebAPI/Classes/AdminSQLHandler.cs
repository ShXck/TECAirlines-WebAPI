using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TECAirlines_WebAPI.Classes
{
    public class AdminSQLHandler
    {
        private static readonly string connect_str = "Data Source=.;Initial Catalog=TecAirlinesDB;Integrated Security=True";

        /// <summary>
        /// Hace los querys SQL necesarios para insertar un cliente.
        /// </summary>
        /// <param name="customer">Los datos del cliente.</param>
        /// <returns>El resultado de la operación.</returns>
        public static int InsertNewCustomer(Customer customer)
        {
            int result = 2;

            if (!SQLHelper.UsernameExists(customer.username, "CUSTOMER", connect_str))
            {
                // TODO: separate it customers and students.
                SqlConnection connection = new SqlConnection(connect_str);
                connection.Open();

                string req = "insert into CUSTOMER VALUES (@full_name, @phone_numbr, @email, @is_student, @username, @password)";
                SqlCommand cmd = new SqlCommand(req, connection);

                string encr_pass = Cipher.Encrypt(customer.password);
                string encr_phone = Cipher.Encrypt(customer.phone_numbr);

                cmd.Parameters.Add(new SqlParameter("full_name", customer.full_name));
                cmd.Parameters.Add(new SqlParameter("phone_numbr", encr_phone));
                cmd.Parameters.Add(new SqlParameter("email", customer.email));
                cmd.Parameters.Add(new SqlParameter("is_student", customer.is_student));
                cmd.Parameters.Add(new SqlParameter("username", customer.username));
                cmd.Parameters.Add(new SqlParameter("password", encr_pass));

                result = cmd.ExecuteNonQuery();

                if (customer.is_student)
                {
                    CreateNewStudent(customer.username, customer.college_name, customer.student_id);
                } 

                connection.Close();
                return result;
            }

            return result; // 1 = success, 0 = insertion error, 2 = username already exists.
        }

        /// <summary>
        /// Hace los querys necesarios para insertar un estudiante a la base.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <param name="uni">La universidad</param>
        /// <param name="st_id">El ID de estudiante.</param>
        private static void CreateNewStudent(string username, string uni, int st_id)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "insert into STUDENTS VALUES (@id, @username, @uni, @miles)";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", st_id));
            cmd.Parameters.Add(new SqlParameter("username", username));
            cmd.Parameters.Add(new SqlParameter("uni", uni));
            cmd.Parameters.Add(new SqlParameter("miles", 1));

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserta un nuevo administrdor en la base.
        /// </summary>
        /// <param name="admin">Los datos del admin.</param>
        /// <returns>El resultado de la operación.</returns>
        public static int CreateNewAdmin(Admin admin)
        {
            int result = 2;

            if (!SQLHelper.UsernameExists(admin.username, "ADMIN", connect_str))
            {
                SqlConnection connection = new SqlConnection(connect_str);
                connection.Open();

                string req = "insert into ADMIN VALUES (@full_name, @phone_numbr, @email, @username, @password, @role)";
                SqlCommand cmd = new SqlCommand(req, connection);

                string encr_phone = Cipher.Encrypt(admin.phone_numbr);
                string encr_pass = Cipher.Encrypt(admin.password);

                cmd.Parameters.Add(new SqlParameter("full_name", admin.full_name));
                cmd.Parameters.Add(new SqlParameter("phone_numbr", encr_phone));
                cmd.Parameters.Add(new SqlParameter("email", admin.email));
                cmd.Parameters.Add(new SqlParameter("username", admin.username));
                cmd.Parameters.Add(new SqlParameter("password", encr_pass));
                cmd.Parameters.Add(new SqlParameter("role", admin.role));

                result = cmd.ExecuteNonQuery();

                connection.Close();
                return result;
            }

            return result; // 1 = success, 0 = insertion error, 2 = username already exists.
        }

        /// <summary>
        /// Inserta un nuevo vuelo en la base.
        /// </summary>
        /// <param name="flight">Los datos del vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
        public static int CreateNewFlight(Flight flight)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "insert into FLIGHT VALUES (@depart_ap, @arrival_ap, @capacity, @flight_id, @depart_date, @plane_id, @status, @normal_price, @fc_price, @seats_left, @fc_seats_left, @miles)";
            SqlCommand cmd = new SqlCommand(req, connection);

            Tuple<int, int, int> plane_data = SQLHelper.GetPlaneDetails(flight.plane_model, connect_str); // returns <plane_id, capacity, fc_capacity>

            int normal_seats = plane_data.Item2 - plane_data.Item3;
            Random rd = new Random();

            cmd.Parameters.Add(new SqlParameter("depart_ap", flight.depart_ap));
            cmd.Parameters.Add(new SqlParameter("arrival_ap", flight.arrival_ap));
            cmd.Parameters.Add(new SqlParameter("capacity", plane_data.Item2));
            cmd.Parameters.Add(new SqlParameter("flight_id", flight.flight_id));
            cmd.Parameters.Add(new SqlParameter("depart_date", flight.depart_date));
            cmd.Parameters.Add(new SqlParameter("plane_id", plane_data.Item1));
            cmd.Parameters.Add(new SqlParameter("status", "Active"));
            cmd.Parameters.Add(new SqlParameter("normal_price", flight.normal_price));
            cmd.Parameters.Add(new SqlParameter("fc_price", flight.fc_price));
            cmd.Parameters.Add(new SqlParameter("seats_left", normal_seats));
            cmd.Parameters.Add(new SqlParameter("fc_seats_left", plane_data.Item3));
            cmd.Parameters.Add(new SqlParameter("miles", rd.Next(1500, 10000)));

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result;
        }

        /// <summary>
        /// Inserta una nueva promoción en la base.
        /// </summary>
        /// <param name="sale">Los datos de la promoción.</param>
        /// <returns>El resultado de la operación.</returns>
        public static int CreateNewSale(Sale sale)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "insert into SALE VALUES (@flight_id, @discount, @start_date, @exp_date)";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("flight_id", sale.flight_id));
            cmd.Parameters.Add(new SqlParameter("discount", sale.discount));
            cmd.Parameters.Add(new SqlParameter("start_date", DateTime.Today));
            cmd.Parameters.Add(new SqlParameter("exp_date", sale.exp_date));

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result;
        }

        /// <summary>
        /// Obtiene los vuelos activos de la base.
        /// </summary>
        /// <returns>Los vuelos activos.</returns>
        public static string GetActiveFlights()
        {
            /**
             * Flights valid status:
             * - Active (meaning is available for booking, not at full capacity)
             * - Full (meaning is active but is completely booked)
             * - Closed (No longer available for booking, not active. Happens 1 hour before departure)
             * */
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select flight_id from FLIGHT where status = 'Active' or status = 'Full'";
            SqlCommand cmd = new SqlCommand(req, connection);

            List<string> active_fl = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        active_fl.Add(reader.GetString(0));
                    }
                    connection.Close();
                    return JSONHandler.BuildListStrResult("flights", active_fl);
                } else
                {
                    connection.Close();
                    return JSONHandler.BuildMsgJSON(0, "No active flights were found");
                }
            }
        }

        /// <summary>
        /// Obtiene los aeropuertos en la base.
        /// </summary>
        /// <returns>La lista de aeropuertos.</returns>
        public static string GetAirports()
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select ap_name from AIRPORT";
            SqlCommand cmd = new SqlCommand(req, connection);

            List<string> airports_lst = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        airports_lst.Add(reader.GetString(0));
                    }
                    connection.Close();
                    return JSONHandler.BuildListStrResult("airports", airports_lst);
                }
                else
                {
                    connection.Close();
                    return JSONHandler.BuildMsgJSON(0, "No airports were found");
                }
            }
        }

        /// <summary>
        /// Obtiene los aviones en la base.
        /// </summary>
        /// <returns>La lista de aviones.</returns>
        public static string GetAirplanes()
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select model from AIRPLANES";
            SqlCommand cmd = new SqlCommand(req, connection);

            List<string> airplanes_lst = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        airplanes_lst.Add(reader.GetString(0));
                    }
                    connection.Close();
                    return JSONHandler.BuildListStrResult("airplanes", airplanes_lst);
                }
                else
                {
                    connection.Close();
                    return JSONHandler.BuildMsgJSON(0, "No airplanes were found");
                }
            }
        }

        /// <summary>
        /// Verifica los credenciales de un admin para el inicio de sesión.
        /// </summary>
        /// <param name="admin">Los credenciales del admin.</param>
        /// <returns>El resultado de la operación.</returns>
        public static int LoginAdmin(Admin admin)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select username, password from ADMIN where username = @user and password = @password";
            SqlCommand cmd = new SqlCommand(req, connection);

            string encr_pass = Cipher.Encrypt(admin.password);

            System.Diagnostics.Debug.WriteLine(encr_pass);

            cmd.Parameters.Add(new SqlParameter("user", admin.username));
            cmd.Parameters.Add(new SqlParameter("password", encr_pass));

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
        /// Obtiene las reservaciones de un vuelo.
        /// </summary>
        /// <param name="flight">El ID del vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
        public static string GetFlightReservations(string flight)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select username, is_first_class from RESERVATION where flight_id = @flight";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("flight", flight));

            List<string> res_lst = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        res_lst.Add(JSONHandler.BuildPair("username", reader.GetString(0),
                                                          "first_class", reader.GetBoolean(1).ToString()));
                    }
                    connection.Close();
                    return JSONHandler.BuildListStrResult("reservations", res_lst);
                }
                else
                {
                    connection.Close();
                    return JSONHandler.BuildMsgJSON(0, "No reservations were found for this flight");
                }
            }
        }

        /// <summary>
        /// Cambia el estado de un vuelo en la base.
        /// </summary>
        /// <param name="flight">El identificador del vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
        public static int CloseFlight(string flight)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "update FLIGHT set status = @stat where flight_id = @flight";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("stat", "Closed"));
            cmd.Parameters.Add(new SqlParameter("flight", flight));

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result;
        }

        /// <summary>
        /// Inserta una nueva universidad en la base.
        /// </summary>
        /// <param name="uni">Los datos de la universidad.</param>
        /// <returns>El resultado de la operación.</returns>
        public static int InsertNewUniversity(University uni)
        {
            int result = 2;

            if (!SQLHelper.UniExists(uni.uni_name, connect_str))
            {
                SqlConnection connection = new SqlConnection(connect_str);
                connection.Open();
                string req = "insert into UNIVERSITY VALUES (@name)";
                SqlCommand cmd = new SqlCommand(req, connection);

                cmd.Parameters.Add(new SqlParameter("name", uni.uni_name));

                result = cmd.ExecuteNonQuery();

                connection.Close();
            }

            return result;
        }

        public static int InsertNewAirplane(Airplane ap)
        {
            int result = 2;

            if (!SQLHelper.AirplaneExists(ap.model, connect_str))
            {
                SqlConnection connection = new SqlConnection(connect_str);
                connection.Open();
                string req = "insert into AIRPLANES VALUES (@model, @capacity, @plane_id)";
                SqlCommand cmd = new SqlCommand(req, connection);

                cmd.Parameters.Add(new SqlParameter("model", ap.model));
                cmd.Parameters.Add(new SqlParameter("capacity", ap.capacity));
                cmd.Parameters.Add(new SqlParameter("plane_id", ap.plane_id));

                result = cmd.ExecuteNonQuery();

                connection.Close();
            }

            return result;
        }

        public static int InsertNewAirport(Airport ap)
        {
            int result = 2;

            if (!SQLHelper.AirportExists(ap.ap_name, connect_str))
            {
                SqlConnection connection = new SqlConnection(connect_str);
                connection.Open();
                string req = "insert into AIRPORT VALUES (@name, @short_name)";
                SqlCommand cmd = new SqlCommand(req, connection);

                cmd.Parameters.Add(new SqlParameter("name", ap.ap_name));
                cmd.Parameters.Add(new SqlParameter("short_name", ap.ap_short_name));

                result = cmd.ExecuteNonQuery();

                connection.Close();

                return result;
            }

            return result;
        }
    }
}