using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        [HttpGet, Route("tecairlines/flights")]
        public IHttpActionResult SearchFlight([FromBody]string search_params)
        {
            string query_result = CustomerSQLHandler.FindFlight(JsonConvert.DeserializeObject<Flight>(search_params));
            return Ok(query_result);
        }

        [HttpPost, Route("tecairlines/booking")]
        public IHttpActionResult BookFlight([FromBody] string book_det)
        {
            string query_result = CustomerSQLHandler.BookFlight(JsonConvert.DeserializeObject<Reservation>(book_det));
            return Ok(query_result);
        }

        [HttpGet, Route("tecairlines/cost")]
        public IHttpActionResult GetReservationCost([FromBody] string reservation)
        {
            int cost = CustomerSQLHandler.GetReservationCost(JsonConvert.DeserializeObject<Reservation>(reservation));
            return Ok(cost);
        }

        // TODO: Test flight payment
        [HttpPost, Route("tecairlines/pay-flight")]
        public IHttpActionResult PayFlight([FromBody] string paym_det)
        {
            JObject json = new JObject(paym_det);
            string result = CustomerSQLHandler.PayFlight(Convert.ToInt32(json["method"]), json["sec_code"].ToString());
            return Ok(result);
        }

        [HttpGet, Route("tecairlines/{username}/cards")]
        public IHttpActionResult GetUserCards([FromUri] string username)
        {

        }

        [HttpGet, Route("tecairlines/flights/{flight}")]
        public IHttpActionResult GetFlightDetails([FromUri] string flight)
        {
            string result = CustomerSQLHandler.GetFlightDetails(flight);
            return Ok(result);
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
