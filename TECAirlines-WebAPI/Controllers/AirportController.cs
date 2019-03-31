using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TECAirlines_WebAPI.Classes;

namespace TECAirlines_WebAPI.Controllers
{
    public class AirportController : ApiController
    {
        [HttpPost, Route("tecairlines/signup")]
        public IHttpActionResult CreateCustomer([FromBody]string cust_data)
        {
            string conn_str = "Data Source=.;Initial Catalog=TADatabase;Integrated Security=True";
            SqlConnection connection = new SqlConnection(conn_str);
            connection.Open();
            string req = "insert into CUSTOMER VALUES (@full_name, @phone_numbr, @email, @is_student, @college_name, @student_id, @username, @password, @st_miles)";
            SqlCommand cmd = new SqlCommand(req, connection);

            Customer new_cust = JsonConvert.DeserializeObject<Customer>(cust_data);

            cmd.Parameters.Add(new SqlParameter("full_name", new_cust.full_name));
            cmd.Parameters.Add(new SqlParameter("phone_numbr", new_cust.phone_numbr));
            cmd.Parameters.Add(new SqlParameter("email", new_cust.email));
            cmd.Parameters.Add(new SqlParameter("is_student", new_cust.is_student));
            cmd.Parameters.Add(new SqlParameter("college_name", new_cust.college_name));
            cmd.Parameters.Add(new SqlParameter("student_id", new_cust.student_id));
            cmd.Parameters.Add(new SqlParameter("username", new_cust.username));
            cmd.Parameters.Add(new SqlParameter("password", new_cust.password));
            cmd.Parameters.Add(new SqlParameter("st_miles", new_cust.st_miles));

            int result = cmd.ExecuteNonQuery();

            if (result > 0)
            {
                connection.Close();
                return Ok();
            }
            else return InternalServerError();
        
        }
    }
}
