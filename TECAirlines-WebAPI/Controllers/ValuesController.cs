using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TECAirlines_WebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            string conn_str = "Data Source=.;Initial Catalog=TADatabase;Integrated Security=True";
            SqlConnection connection = new SqlConnection(conn_str);
            connection.Open();
            string req = "insert into CUSTOMER VALUES (@full_name, @phone_numbr, @email, @is_student, @college_name, @student_id, @username, @password, @st_miles)";
            SqlCommand cmd = new SqlCommand(req, connection);
            cmd.Parameters.Add(new SqlParameter("full_name", "Marcelo"));
            cmd.Parameters.Add(new SqlParameter("phone_numbr", 15512));
            cmd.Parameters.Add(new SqlParameter("email", "msjs"));
            cmd.Parameters.Add(new SqlParameter("is_student", 1));
            cmd.Parameters.Add(new SqlParameter("college_name", "UCLA"));
            cmd.Parameters.Add(new SqlParameter("student_id", 1181811));
            cmd.Parameters.Add(new SqlParameter("username", "mss"));
            cmd.Parameters.Add(new SqlParameter("password", "sdansds5ad"));
            cmd.Parameters.Add(new SqlParameter("st_miles", 1));

            int result = cmd.ExecuteNonQuery();
            System.Diagnostics.Debug.WriteLine(result);
            connection.Close();
            return new string[] { "value1", "value2" };
        }



        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
