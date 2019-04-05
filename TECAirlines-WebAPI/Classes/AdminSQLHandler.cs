using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TECAirlines_WebAPI.Classes
{
    public class AdminSQLHandler
    {
        private static readonly string connect_str = "Data Source=.;Initial Catalog=TADatabase;Integrated Security=True";

        public static int InsertNewCustomer(Customer customer)
        {
            int result = 2;

            if (!SQLVerifier.UsernameExists(customer.username, "CUSTOMER", connect_str))
            {

                SqlConnection connection = new SqlConnection(connect_str);
                connection.Open();

                string req = "insert into CUSTOMER VALUES (@full_name, @phone_numbr, @email, @is_student, @college_name, @student_id, @username, @password, @st_miles)";
                SqlCommand cmd = new SqlCommand(req, connection);

                cmd.Parameters.Add(new SqlParameter("full_name", customer.full_name));
                cmd.Parameters.Add(new SqlParameter("phone_numbr", customer.phone_numbr));
                cmd.Parameters.Add(new SqlParameter("email", customer.email));
                cmd.Parameters.Add(new SqlParameter("is_student", customer.is_student));
                cmd.Parameters.Add(new SqlParameter("college_name", customer.college_name));
                cmd.Parameters.Add(new SqlParameter("student_id", customer.student_id));
                cmd.Parameters.Add(new SqlParameter("username", customer.username));
                cmd.Parameters.Add(new SqlParameter("password", customer.password));
                cmd.Parameters.Add(new SqlParameter("st_miles", customer.st_miles));

                result = cmd.ExecuteNonQuery();

                System.Diagnostics.Debug.WriteLine(result);

                connection.Close();
                return result;
            }

            return result; // 1 = success, 0 = insertion error, 2 = username already exists.
        }

        public static int CreateNewAdmin(Admin admin)
        {
            int result = 2;

            if (!SQLVerifier.UsernameExists(admin.username, "ADMIN", connect_str))
            {
                SqlConnection connection = new SqlConnection(connect_str);
                connection.Open();

                string req = "insert into ADMIN VALUES (@full_name, @phone_numbr, @email, @username, @password, @role)";
                SqlCommand cmd = new SqlCommand(req, connection);

                cmd.Parameters.Add(new SqlParameter("full_name", admin.full_name));
                cmd.Parameters.Add(new SqlParameter("phone_numbr", admin.phone_numbr));
                cmd.Parameters.Add(new SqlParameter("email", admin.email));
                cmd.Parameters.Add(new SqlParameter("username", admin.username));
                cmd.Parameters.Add(new SqlParameter("password", admin.password));
                cmd.Parameters.Add(new SqlParameter("role", admin.role));

                result = cmd.ExecuteNonQuery();

                connection.Close();
                return result;
            }

            return result; // 1 = success, 0 = insertion error, 2 = username already exists.
        }

        public static int CreateNewFlight(Flight flight)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "insert into FLIGHT VALUES (@depart_ap, @arrival_ap, @capacity, @flight_id, @depart_date, @plane_id, @status)";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("depart_ap", flight.depart_ap));
            cmd.Parameters.Add(new SqlParameter("arrival_ap", flight.arrival_ap));
            cmd.Parameters.Add(new SqlParameter("capacity", flight.capacity));
            cmd.Parameters.Add(new SqlParameter("flight_id", flight.flight_id));
            cmd.Parameters.Add(new SqlParameter("depart_date", flight.depart_date));
            cmd.Parameters.Add(new SqlParameter("plane_id", flight.plane_id));
            cmd.Parameters.Add(new SqlParameter("status", flight.status));

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result;
        }

        public static int CreateNewSale(Sale sale)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "insert into SALE VALUES (@flight_id, @discount, @exp_date, @sale_id)";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("flight_id", sale.flight_id));
            cmd.Parameters.Add(new SqlParameter("discount", sale.discount));
            cmd.Parameters.Add(new SqlParameter("exp_date", sale.exp_date));
            cmd.Parameters.Add(new SqlParameter("sale_id", sale.sale_id));

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result;
        }

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
                    return JSONHandler.BuildActiveFlightsResult(active_fl);
                } else
                {
                    connection.Close();
                    return JSONHandler.BuildErrorJSON("No active flights were found");
                }
            }

        }

        public static int LoginAdmin(Admin admin)
        {
            SqlConnection connection = new SqlConnection(connect_str);
            connection.Open();
            string req = "select username, password from ADMIN where username = @user and password = @passw";
            SqlCommand cmd = new SqlCommand(req, connection);

            cmd.Parameters.Add(new SqlParameter("user", admin.username));
            cmd.Parameters.Add(new SqlParameter("passw", admin.password));

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
    }
}