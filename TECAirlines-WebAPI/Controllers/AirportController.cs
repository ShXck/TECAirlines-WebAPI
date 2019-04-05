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
            int query_result = AdminSQLHandler.InsertNewCustomer(JsonConvert.DeserializeObject<Customer>(cust_data));
            return CheckInsertionResult(query_result);
        }

        [HttpPost, Route("tecairlines/admin/signup")]
        public IHttpActionResult CreateAdmin([FromBody]string admin_data)
        {
            int query_result = AdminSQLHandler.CreateNewAdmin(JsonConvert.DeserializeObject<Admin>(admin_data));
            return CheckInsertionResult(query_result);
        }

        [HttpPost, Route("tecairlines/admin/new-flight")]
        public IHttpActionResult CreateFlight([FromBody]string flight_data)
        {
            System.Diagnostics.Debug.WriteLine(flight_data);
            int query_result = AdminSQLHandler.CreateNewFlight(JsonConvert.DeserializeObject<Flight>(flight_data));
            return CheckInsertionResult(query_result);
        }

        [HttpPost, Route("tecairlines/admin/new-sale")]
        public IHttpActionResult CreateSale([FromBody]string sale_data)
        {
            int query_result = AdminSQLHandler.CreateNewSale(JsonConvert.DeserializeObject<Sale>(sale_data));
            return CheckInsertionResult(query_result);
        }

        [HttpGet, Route("tecairlines/admin/flights")]
        public IHttpActionResult GetAllActiveFlights()
        {
            string query_result = AdminSQLHandler.GetActiveFlights();
            return Ok(query_result);
        }

        [HttpPost, Route("tecairlines/admin/login")]
        public IHttpActionResult LoginAdmin([FromBody]string adm_credentials)
        {
            int query_result = AdminSQLHandler.LoginAdmin(JsonConvert.DeserializeObject<Admin>(adm_credentials));

            switch (query_result)
            {
                case 200: return Ok();
                case 401: return Unauthorized();
            }
            return InternalServerError();
        }

        private IHttpActionResult CheckInsertionResult(int result)
        {
            switch(result)
            {
                case 1: return Ok();
                case 0: return InternalServerError();
                case 2: return Conflict();
            }
            return InternalServerError();
        }
    }
}
