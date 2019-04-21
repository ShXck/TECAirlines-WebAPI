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

        [HttpPost, Route("tecairlines/flights")]
        public IHttpActionResult SearchFlight([FromBody]string search_params)
        {
            System.Diagnostics.Debug.WriteLine(search_params);
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
            string cost = CustomerSQLHandler.GetReservationCost(JsonConvert.DeserializeObject<Reservation>(reservation));
            return Ok(cost);
        }

        [HttpPost, Route("tecairlines/{user}/pay-flight")]
        public IHttpActionResult PayFlight([FromBody] string paym_det, [FromUri] string user)
        {
            CCard card = JsonConvert.DeserializeObject<CCard>(paym_det);
            string result = CustomerSQLHandler.PayFlight(card.card_number, card.security_code, user);
            return Ok(result);
        }

        [HttpGet, Route("tecairlines/{username}/cards")]
        public IHttpActionResult GetUserCards([FromUri] string username)
        {
            string cards_json = CustomerSQLHandler.GetCards(username);
            return Ok(cards_json);
        }

        [HttpGet, Route("tecairlines/flights/{flight}")]
        public IHttpActionResult GetFlightDetails([FromUri] string flight)
        {
            string result = CustomerSQLHandler.GetFlightDetails(flight);
            return Ok(result);
        }

        [HttpGet, Route("tecairlines/{user}/flights")]
        public IHttpActionResult GetUserFlights([FromUri] string user)
        {
            string result = CustomerSQLHandler.GetUserFlights(user);
            return Ok(result);
        }

        [HttpPost, Route("tecairlines/login")]
        public IHttpActionResult CustomerLogin([FromBody] string usr_credentials)
        {
            int query_result = CustomerSQLHandler.LoginCustomer(JsonConvert.DeserializeObject<Customer>(usr_credentials));
            return CheckQueryResult(query_result, JSONHandler.BuildMsgJSON(1, "Login successful"));
        }

        [HttpPost, Route("tecairlines/payment")]
        public IHttpActionResult AddPaymentMethod([FromBody] string card_details)
        {
            int query_result = CustomerSQLHandler.AddCreditCard(JsonConvert.DeserializeObject<CCard>(card_details));
            if (query_result == 1) return Ok(JSONHandler.BuildMsgJSON(1, "Success"));
            else return CheckQueryResult(500, String.Empty);
        }

        [HttpGet, Route("tecairlines/universities")]
        public IHttpActionResult GetUniversities()
        {
            string result = CustomerSQLHandler.GetUniversities();
            return Ok(result);
        }

        private IHttpActionResult CheckQueryResult(int result, string message)
        {
            switch (result)
            {
                case 200: return Ok(message);
                case 500: return Ok(JSONHandler.BuildMsgJSON(0, "There was an internal error"));
                case 401: return Ok(JSONHandler.BuildMsgJSON(0, "Your username or password is incorrect"));
                case 404: return Ok(JSONHandler.BuildMsgJSON(0, "The resource was not found"));
            }
            return Ok(JSONHandler.BuildMsgJSON(0, "There was an internal error"));
        }
    }
}
