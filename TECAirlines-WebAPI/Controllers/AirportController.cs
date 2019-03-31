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
            int query_result = SQLHandler.InsertNewCustomer(JsonConvert.DeserializeObject<Customer>(cust_data));
            return CheckInsertionResult(query_result);

        }

        [HttpPost, Route("tecairlines/admin/new-flight")]
        public IHttpActionResult CreateFlight([FromBody]string flight_data)
        {
            System.Diagnostics.Debug.WriteLine(flight_data);
            int query_result = SQLHandler.CreateNewFlight(JsonConvert.DeserializeObject<Flight>(flight_data));
            return CheckInsertionResult(query_result);
        }

        [HttpPost, Route("tecairlines/admin/new-sale")]
        public IHttpActionResult CreateSale([FromBody]string sale_data)
        {
            int query_result = SQLHandler.CreateNewSale(JsonConvert.DeserializeObject<Sale>(sale_data));
            return CheckInsertionResult(query_result);
        }

        private IHttpActionResult CheckInsertionResult(int result)
        {
            if (result > 0) return Ok();
            else return InternalServerError();
        }
    }
}
