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
        /// <summary>
        /// Verifica si un usuario ya existe en la base.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <param name="table">La tabla por consultar.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>Si ya existe el usuario.</returns>
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

        public static bool CardExists(string number, string username, string connect_str)
        {
            System.Diagnostics.Debug.WriteLine(number);
            System.Diagnostics.Debug.WriteLine(username);


            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select exp_date from PAYMENT_METHOD where card_number = @numbr and username = @name";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("name", username));
            cmd.Parameters.Add(new SqlParameter("numbr", Cipher.Encrypt(number)));

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

        /// <summary>
        /// Obtiene datos de un avión.
        /// </summary>
        /// <param name="model">Modelo del avión.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>Datos del avión.</returns>
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
                    while (reader.Read())
                    {
                        result = new Tuple<int, int, int>(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2));
                    }
                    connection.Close();
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// Verifica el estado de un vuelo.
        /// </summary>
        /// <param name="flight_id">El id del vuelo.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>El estado del vuelo.</returns>
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

        /// <summary>
        /// Obtiene el número de asientos disponibles.
        /// </summary>
        /// <param name="seat_type">La categoría de asientos.</param>
        /// <param name="flight_id">El id del vuelo.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>Número de asientos disponibles.</returns>
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
                    while (reader.Read())
                    {
                        result = Convert.ToInt32(reader[0]);
                    }
                }
            }
            connection.Close();
            return result;
        }

        /// <summary>
        /// Verifica si un vuelo está lleno.
        /// </summary>
        /// <param name="flight_id">El id del vuelo.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>Si esta lleno o no.</returns>
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

        /// <summary>
        /// Verifica si el vuelo tiene descuento.
        /// </summary>
        /// <param name="flight">El id del vuelo.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>La cantidad de descuento.</returns>
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

        /// <summary>
        /// Verifica si una universidad ya existe en la base.
        /// </summary>
        /// <param name="uni">Nombe de universidad.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>Si existe o no.</returns>
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

        /// <summary>
        /// Obtiene las millas de un estudiante.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>Las millas de la persona.</returns>
        public static int GetStudentMiles(string username, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select st_miles from STUDENTS where username = @name";

            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("name", username));

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
        /// Agrega millas a un usuario.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <param name="miles">Las millas por actualizar.</param>
        /// <param name="connect_str">El string de conexión.</param>
        public static void AddStudentMiles(string username, int miles, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "update STUDENTS set st_miles = @miles where username = @user";

            SqlCommand cmd = new SqlCommand(req, connection);

            int curr_miles = GetStudentMiles(username, connect_str);
            int new_miles = curr_miles + miles;

            cmd.Parameters.Add(new SqlParameter("miles", new_miles));
            cmd.Parameters.Add(new SqlParameter("user", username));

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Obtiene los ids de vuelos de un pasajero.
        /// </summary>
        /// <param name="user">El nombre de usuario.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>La lista de ids.</returns>
        public static List<string> GetUserFlightID(string user, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select flight_id from RESERVATION where username = @user";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("user", user));

            List<string> fl_ids = new List<string>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        fl_ids.Add(reader.GetString(0));
                    }
                }
            }
            connection.Close();
            return fl_ids;
        }

        /// <summary>
        /// Obtiene los aeropuertos de llegada y salida de un vuelo.
        /// </summary>
        /// <param name="flight_id">El id del vuelo.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>La info del aeropuertos.</returns>
        public static string GetAirportFlightData(string flight_id, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select depart_ap, arrival_ap from FLIGHT where flight_id = @fl";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("fl", flight_id));

            string data = "";

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data = JSONHandler.BuildUserFlightResult(flight_id, reader.GetString(0), reader.GetString(1));
                    }
                }
            }
            connection.Close();
            return data;
        }

        /// <summary>
        /// Obtiene la id de prechequeo de un pasajero.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>La id de prechequeo.</returns>
        public static int GetPreCheckId(string username, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select id_prechecking from PRE_CHECKING where username = @user";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("user", username));

            int id = 0;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                }
            }
            connection.Close();
            return id;
        }

        /// <summary>
        /// Agrega asientos al prechequeo de un pasajero.
        /// </summary>
        /// <param name="precheck">La id de prechequeo.</param>
        /// <param name="seat">El asiento.</param>
        /// <param name="connect_str">El string de conexión.</param>
        public static void AddCustomerSeat(int precheck, string seat, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "insert into PRE_CHECKING_SEATS VALUES(@id, @seat)";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", precheck));
            cmd.Parameters.Add(new SqlParameter("seat", seat));

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Obtiene la capacidad de un avión.
        /// </summary>
        /// <param name="flight">La id del vuelo.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>La capcidad del avión.</returns>
        public static int GetPlaneCapacity(string flight, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select capacity from FLIGHT where flight_id = @id";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", flight));

            int capacity = 0;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        capacity = reader.GetInt32(0);
                    }
                }
            }
            connection.Close();
            return capacity;
        } 

        public static bool IsChecked(string username, string flight, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select id_prechecking from PRE_CHECKING where flight_id = @id and username = @name";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", flight));
            cmd.Parameters.Add(new SqlParameter("name", username));

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

        /// <summary>
        /// Obtiene el email de un usuario.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <param name="connect_str">El string de conexión.</param>
        /// <returns>El email del usuario.</returns>
        public static string GetUserEmail(string username, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select email from CUSTOMER where username = @name";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("name", username));

            string result = String.Empty;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        result = reader.GetString(0);
                    }
                }
            }

            connection.Close();
            return result;
        }

        public static int GetFlightMilesPrice(string flight, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "select miles_price from FLIGHT where flight_id = @id";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("id", flight));

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

        public static void UpdateMiles(string user, int amount, string connect_str)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();

            string req = "update STUDENTS set st_miles = @miles  where username = @name";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("name", user));
            cmd.Parameters.Add(new SqlParameter("miles", amount));

            cmd.ExecuteNonQuery();

            connection.Close();
        }
    }
}