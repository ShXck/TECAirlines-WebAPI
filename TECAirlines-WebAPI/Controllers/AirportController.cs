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
            System.Diagnostics.Debug.WriteLine(admin_data);

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

        [HttpGet, Route("tecairlines/admin/airports")]
        public IHttpActionResult GetAllAirports()
        {
            string query_result = AdminSQLHandler.GetAirports();
            return Ok(query_result);
        }

        [HttpGet, Route("tecairlines/admin/airplanes")]
        public IHttpActionResult GetAllAirplanes()
        {
            string query_result = AdminSQLHandler.GetAirplanes();
            System.Diagnostics.Debug.WriteLine(query_result);
            return Ok(query_result);
        }

        [HttpPost, Route("tecairlines/admin/login")]
        public IHttpActionResult LoginAdmin([FromBody]string adm_credentials)
        {
            System.Diagnostics.Debug.WriteLine(adm_credentials);
            int query_result = AdminSQLHandler.LoginAdmin(JsonConvert.DeserializeObject<Admin>(adm_credentials));

            switch (query_result)
            {
                case 200: return Ok(JSONHandler.BuildMsgJSON(1, "Login Successful"));
                case 401: return Ok(JSONHandler.BuildMsgJSON(0, "Login Failed"));
            }
            return Ok(JSONHandler.BuildMsgJSON(0, "There was an internal error"));
        }

        [HttpGet, Route("tecairlines/admin/{flight}/reservations")]
        public IHttpActionResult GetFlightReservation([FromUri] string flight)
        {
            string reservations = AdminSQLHandler.GetFlightReservations(flight);
            return Ok(reservations);
        }

        [HttpPut, Route("tecairlines/admin/close/{flight}")]
        public IHttpActionResult CloseFlight([FromUri] string flight)
        {
            int result = AdminSQLHandler.CloseFlight(flight);
            if (result == 1) return Ok(JSONHandler.BuildMsgJSON(1, "Flight Closed Successfully"));
            else return Ok(JSONHandler.BuildMsgJSON(0, "Task Failed"));
        }

        [HttpPost, Route("tecairlines/admin/new-airplane")]
        public IHttpActionResult InsertAirplane([FromBody] string airp_details)
        {
            int query_result = AdminSQLHandler.InsertNewAirplane(JsonConvert.DeserializeObject<Airplane>(airp_details));
            return CheckInsertionResult(query_result);
        }

        [HttpPost, Route("tecairlines/admin/new-airport")]
        public IHttpActionResult InsertAirport([FromBody] string ap_details)
        {
            int query_result = AdminSQLHandler.InsertNewAirport(JsonConvert.DeserializeObject<Airport>(ap_details));
            return CheckInsertionResult(query_result);
        }

        private IHttpActionResult CheckInsertionResult(int result)
        {
            switch(result)
            {
                case 1: return Ok(JSONHandler.BuildMsgJSON(1, "Task successfully executed"));
                case 0: return Ok(JSONHandler.BuildMsgJSON(0, "Task could not be completed"));
                case 2: return Ok(JSONHandler.BuildMsgJSON(0, "Username already exists"));
            }
            return Ok(JSONHandler.BuildMsgJSON(0, "There was an internal error. Try again later"));
        }
    }
}
