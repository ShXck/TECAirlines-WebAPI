using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TECAirlines_WebAPI.Classes
{
    public class SQLHandler
    {
        private static readonly string connect_str = "Data Source=.;Initial Catalog=TADatabase;Integrated Security=True";

        public static int InsertNewCustomer(Customer customer)
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

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result;
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
                else result_str = JSONHandler.BuildErrorJSON();
            }

            connection.Close();

            return result_str;
        }
    }
}