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

        // TODO: Test BookFlight, GetReservationCost
        [HttpPost, Route("tecairlines/booking")]
        public IHttpActionResult BookFlight([FromBody] string book_det)
        {
            string query_result = CustomerSQLHandler.BookFlight(JsonConvert.DeserializeObject<Reservation>(book_det));
            // TODO: Reduce available seats, for that add new column in FLIGHT table, with starting value of capacity equal of the whole plain capacity.
            return Ok(query_result);
        }

        [HttpGet, Route("tecairlines/cost")]
        public IHttpActionResult GetReservationCost([FromBody] string reservation)
        {
            int cost = CustomerSQLHandler.GetReservationCost(JsonConvert.DeserializeObject<Reservation>(reservation));
            return Ok(cost);
        }

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

        [HttpPost, Route("tecairlines/payment")]
        public IHttpActionResult AddPaymentMethod([FromBody] string card_details)
        {
            int query_result = CustomerSQLHandler.AddCreditCard(JsonConvert.DeserializeObject<CCard>(card_details));
            if (query_result == 1) return Ok();
            else return InternalServerError();
        }

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
