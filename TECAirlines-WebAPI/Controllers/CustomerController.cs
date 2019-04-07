using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TECAirlines_WebAPI.Classes;

namespace TECAirlines_WebAPI.Controllers
{
    public class CustomerController : ApiController
    {

        [HttpGet, Route("tecairlines/find-flights")]
        public IHttpActionResult SearchFlight([FromBody]string search_params)
        {
            string query_result = CustomerSQLHandler.FindFlight(JsonConvert.DeserializeObject<Flight>(search_params));
            return Ok(query_result);
        }

        /*public IHttpActionResult BookFlight([FromBody] string fl_details)
        {

        }*/ //TODO: Implement

        public IHttpActionResult GetFlightDetails([FromBody] string details)
        {
            return Ok(); // TODO: Join tables customer and flight_in, to display in the web page, all flights user has booked. Then, when selected, search for the flight details and display them on the screen.
        }

        [HttpPost, Route("tecairlines/login")]
        public IHttpActionResult CustomerLogin([FromBody]string usr_credentials)
        {
            int query_result = CustomerSQLHandler.LoginCustomer(JsonConvert.DeserializeObject<Customer>(usr_credentials));
            return CheckQueryResult(query_result, String.Empty);
        }

        /*public IHttpActionResult AddPaymentMethod([FromBody] string card_details)
        {

        }*/

        private IHttpActionResult CheckQueryResult(int result, string message)
        {
            switch (result)
            {
                case 200: return Ok(message);
                case 500: return InternalServerError();
                case 401: return Unauthorized();
                case 404: return NotFound();
            }
            return InternalServerError();
        }
    }
}
