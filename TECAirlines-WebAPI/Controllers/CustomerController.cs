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

        /// <summary>
        /// Busca vuelos a partir de los puntos de destino y salida.
        /// </summary>
        /// <param name="search_params"> Los puntos de salida y destino.</param> 
        /// <returns>Los vuelos que cumplen con los parámetros dados.</returns>
        [HttpPost, Route("tecairlines/flights")]
        public IHttpActionResult SearchFlight([FromBody]string search_params)
        {
            System.Diagnostics.Debug.WriteLine(search_params);
            string query_result = CustomerSQLHandler.FindFlight(JsonConvert.DeserializeObject<Flight>(search_params));
            return Ok(query_result);
        }

        /// <summary>
        /// Reserva un vuelo.
        /// </summary>
        /// <param name="book_det">La información de la reservación.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/booking")]
        public IHttpActionResult BookFlight([FromBody] string book_det)
        {
            string query_result = CustomerSQLHandler.BookFlight(JsonConvert.DeserializeObject<Reservation>(book_det));
            return Ok(query_result);
        }

        /// <summary>
        /// Obtiene el costo de una reservación.
        /// </summary>
        /// <param name="reservation">La información de la reservación</param>
        /// <returns>El costo de la reservación.</returns>
        [HttpGet, Route("tecairlines/cost")]
        public IHttpActionResult GetReservationCost([FromBody] string reservation)
        {
            Tuple<int, string> cost = CustomerSQLHandler.GetReservationCost(JsonConvert.DeserializeObject<Reservation>(reservation));
            return Ok(cost.Item2);
        }

        /// <summary>
        /// Paga por una reservación hecha.
        /// </summary>
        /// <param name="paym_det">Detalles de la forma de pago.</param>
        /// <param name="user">El usuario que está pagando.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/{user}/pay-flight")]
        public IHttpActionResult PayFlight([FromBody] string paym_det, [FromUri] string user)
        {
            CCard card = JsonConvert.DeserializeObject<CCard>(paym_det);
            string result = CustomerSQLHandler.PayFlight(card.card_number, card.security_code, user);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene las tarjetas registradas por un usuario.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpGet, Route("tecairlines/{username}/cards")]
        public IHttpActionResult GetUserCards([FromUri] string username)
        {
            string cards_json = CustomerSQLHandler.GetCards(username);
            return Ok(cards_json);
        }

        /// <summary>
        /// Obtiene los detalles de un vuelo específico.
        /// </summary>
        /// <param name="flight">La identificación del vuelo.</param>
        /// <returns>Los detalles del vuelo especificado.</returns>
        [HttpGet, Route("tecairlines/flights/{flight}")]
        public IHttpActionResult GetFlightDetails([FromUri] string flight)
        {
            string result = CustomerSQLHandler.GetFlightDetails(flight);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene los vuelos reservados de un usuario.
        /// </summary>
        /// <param name="user">El nombre de usuario.</param>
        /// <returns>Los vuelos del cliente especificado.</returns>
        [HttpGet, Route("tecairlines/{user}/flights")]
        public IHttpActionResult GetUserFlights([FromUri] string user)
        {
            string result = CustomerSQLHandler.GetUserFlights(user);
            return Ok(result);
        }

        /// <summary>
        /// Inicio de sesión de un cliente.
        /// </summary>
        /// <param name="usr_credentials">Los credenciales del usuario.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/login")]
        public IHttpActionResult CustomerLogin([FromBody] string usr_credentials)
        {
            int query_result = CustomerSQLHandler.LoginCustomer(JsonConvert.DeserializeObject<Customer>(usr_credentials));
            return CheckQueryResult(query_result, JSONHandler.BuildMsgJSON(1, "Login successful"));
        }

        /// <summary>
        /// Agrega un método de pago.
        /// </summary>
        /// <param name="card_details">Los detalles del método de pago.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/payment")]
        public IHttpActionResult AddPaymentMethod([FromBody] string card_details)
        {
            int query_result = CustomerSQLHandler.AddCreditCard(JsonConvert.DeserializeObject<CCard>(card_details));
            if (query_result == 1) return Ok(JSONHandler.BuildMsgJSON(1, "Success"));
            else if(query_result == 2) return Ok(JSONHandler.BuildMsgJSON(0, "Card already exists."));
            else return Ok(JSONHandler.BuildMsgJSON(0, "There was a problem adding your credit card."));
        }

        /// <summary>
        /// Obtiene las universidades registradas.
        /// </summary>
        /// <returns>El resultado de la operación.</returns>
        [HttpGet, Route("tecairlines/universities")]
        public IHttpActionResult GetUniversities()
        {
            string result = CustomerSQLHandler.GetUniversities();
            return Ok(result);
        }

        /// <summary>
        /// Verifica si un usuario es estudiante o no.
        /// </summary>
        /// <param name="username">El nombre de usuario.</param>
        /// <returns>Si es estudiante o no.</returns>
        [HttpGet, Route("tecairlines/{username}/student")]
        public IHttpActionResult CheckStudent([FromUri] string username)
        {
            if(CustomerSQLHandler.IsStudent(username))
            {
                return Ok(JSONHandler.BuildMsgJSON(1, "True"));
            } else
            {
                return Ok(JSONHandler.BuildMsgJSON(1, "False"));
            }
        }

        /// <summary>
        /// Prechequeo de pasajeros.
        /// </summary>
        /// <param name="user">El nombre de usuario</param>
        /// <param name="flight">La identificación de vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/precheck/{user}/{flight}")]
        public IHttpActionResult PreCheckCustomer([FromUri] string user, [FromUri] string flight) 
        {
            string query_result = CustomerSQLHandler.PreCheckCustomer(user, flight);
            return Ok(query_result);
        }

        /*[HttpGet, Route("tecairlines/{user}/{flight}/people")]
        public IHttpActionResult GetPeopleFlying([FromUri] string user, [FromUri] string flight)
        {
            string query_result = CustomerSQLHandler.GetPeopleFlying(user, flight);
            return Ok(query_result);
        }*/

        /// <summary>
        /// Obtiene todas las promociones que hay.
        /// </summary>
        /// <returns>Las promociones de los vuelos.</returns>
        [HttpGet, Route("tecairlines/sales")]
        public IHttpActionResult GetSales()
        {
            return Ok(CustomerSQLHandler.GetFlightSales());
        }

        /// <summary>
        /// Verifica el estado de la operación.
        /// </summary>
        /// <param name="result">El resultado de la operación.</param>
        /// <param name="message">Mensaje de retorno.</param>
        /// <returns>El resultado de la operación.</returns>
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
