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
        /// <summary>
        /// Registra un nuevo cliente.
        /// </summary>
        /// <param name="cust_data">Los datos del cliente.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/signup")]
        public IHttpActionResult CreateCustomer([FromBody]string cust_data)
        {
            int query_result = AdminSQLHandler.InsertNewCustomer(JsonConvert.DeserializeObject<Customer>(cust_data));
            return CheckInsertionResult(query_result);
        }

        /// <summary>
        /// Registra un nuevo administrador.
        /// </summary>
        /// <param name="admin_data">Los datos del administrador.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/admin/signup")]
        public IHttpActionResult CreateAdmin([FromBody]string admin_data)
        {
            System.Diagnostics.Debug.WriteLine(admin_data);

            int query_result = AdminSQLHandler.CreateNewAdmin(JsonConvert.DeserializeObject<Admin>(admin_data));
            return CheckInsertionResult(query_result);
        }

        /// <summary>
        /// Regista un nuevo vuelo.
        /// </summary>
        /// <param name="flight_data">Los datos del vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/admin/new-flight")]
        public IHttpActionResult CreateFlight([FromBody]string flight_data)
        {
            System.Diagnostics.Debug.WriteLine(flight_data);
            int query_result = AdminSQLHandler.CreateNewFlight(JsonConvert.DeserializeObject<Flight>(flight_data));
            return CheckInsertionResult(query_result);
        }

        /// <summary>
        /// Registra una nueva promoción.
        /// </summary>
        /// <param name="sale_data">Los datos de la promoción.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/admin/new-sale")]
        public IHttpActionResult CreateSale([FromBody]string sale_data)
        {
            int query_result = AdminSQLHandler.CreateNewSale(JsonConvert.DeserializeObject<Sale>(sale_data));
            return CheckInsertionResult(query_result);
        }

        /// <summary>
        /// Obtiene todos los vuelos que están activos y llenos.
        /// </summary>
        /// <returns>Los vuelos activos y llenos.</returns>
        [HttpGet, Route("tecairlines/admin/flights/active")]
        public IHttpActionResult GetAllActiveFlights()
        {
            string query_result = AdminSQLHandler.GetActiveFlights();
            return Ok(query_result);
        }

        /// <summary>
        /// Obtiene todos los aeropuertos.
        /// </summary>
        /// <returns>Los aeropuertos registrados.</returns>
        [HttpGet, Route("tecairlines/admin/airports")]
        public IHttpActionResult GetAllAirports()
        {
            string query_result = AdminSQLHandler.GetAirports();
            return Ok(query_result);
        }

        /// <summary>
        /// Obtiene todos los aviones.
        /// </summary>
        /// <returns>Los modelos de avión registrados.</returns>
        [HttpGet, Route("tecairlines/admin/airplanes")]
        public IHttpActionResult GetAllAirplanes()
        {
            string query_result = AdminSQLHandler.GetAirplanes();
            return Ok(query_result);
        }

        /// <summary>
        /// Inicio de sesión de un administrador.
        /// </summary>
        /// <param name="adm_credentials">Los credenciales del admin.</param>
        /// <returns>El resultado de la operación.</returns>
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

        /// <summary>
        /// Obtiene las reservaciones de un vuelo.
        /// </summary>
        /// <param name="flight">El identificador de un vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpGet, Route("tecairlines/admin/{flight}/reservations")]
        public IHttpActionResult GetFlightReservation([FromUri] string flight)
        {
            string reservations = AdminSQLHandler.GetFlightReservations(flight);
            return Ok(reservations);
        }

        /// <summary>
        /// Cierra un vuelo activo.
        /// </summary>
        /// <param name="flight">El identificador del vuelo.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPut, Route("tecairlines/admin/close/{flight}")]
        public IHttpActionResult CloseFlight([FromUri] string flight)
        {
            int result = AdminSQLHandler.CloseFlight(flight);
            if (result == 1) return Ok(JSONHandler.BuildMsgJSON(1, "Flight Closed Successfully"));
            else return Ok(JSONHandler.BuildMsgJSON(0, "Task Failed"));
        }

        /// <summary>
        /// Agrega una nueva universidad.
        /// </summary>
        /// <param name="uni">El nombre de la universidad.</param>
        /// <returns>El resultado de la operación.</returns>
        [HttpPost, Route("tecairlines/admin/new-uni")]
        public IHttpActionResult InsertUniversity([FromBody] string uni)
        {
            int query_result = AdminSQLHandler.InsertNewUniversity(JsonConvert.DeserializeObject<University>(uni));
            return CheckInsertionResult(query_result);
        }

        /// <summary>
        /// Elimina un cliente.
        /// </summary>
        /// <param name="user">El nombre de usuario.</param>
        /// <returns>El resultado de la operación</returns>
        [HttpDelete, Route("tecairlines/users/delete/{user}")]
        public IHttpActionResult DeleteCustomer([FromUri] string user)
        {
            return Ok(CustomerSQLHandler.DeleteCustomer(user));
        }

        /// <summary>
        /// Elimina una universidad
        /// </summary>
        /// <param name="uni">El nombre de la Universidad</param>
        /// <returns>El resultado de la operación</returns>
        [HttpDelete, Route("tecairlines/universities/delete/{uni}")]
        public IHttpActionResult DeleteUni([FromUri] string uni)
        {
            return Ok(AdminSQLHandler.DeleteUni(uni));
        }

        /// <summary>
        /// Elimina un vuelo.
        /// </summary>
        /// <param name="flight">El id del vuelo.</param>
        /// <returns>El resultado de la operación</returns>
        [HttpDelete, Route("tecairlines/flights/delete/{flight}")]
        public IHttpActionResult DeleteFlight([FromUri] string flight)
        {
            return Ok(AdminSQLHandler.DeleteFlight(flight));
        }

        /// <summary>
        /// Elimina un aeropuerto.
        /// </summary>
        /// <param name="ap">El nombre del aeropuerto.</param>
        /// <returns>El resultado de la operación</returns>
        [HttpDelete, Route("tecairlines/airports/delete/{ap}")]
        public IHttpActionResult DeleteAirport([FromUri] string ap)
        {
            return Ok(AdminSQLHandler.DeleteAirport(ap));
        }

        /// <summary>
        /// Elimina un avión.
        /// </summary>
        /// <param name="plane">El id del avión.</param>
        /// <returns>El resultado de la operación</returns>
        [HttpDelete, Route("tecairlines/airplanes/delete/{plane}")]
        public IHttpActionResult DeletePlane([FromUri] int plane)
        {
            return Ok(AdminSQLHandler.DeletePlane(plane));
        }


        /*[HttpPost, Route("tecairlines/admin/new-airplane")]
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
        }*/

        /// <summary>
        /// verifica el estado de una inserción a la base.
        /// </summary>
        /// <param name="result">El resultado de la operación.</param>
        /// <returns>Mensaje de retorno.</returns>
        private IHttpActionResult CheckInsertionResult(int result)
        {
            switch(result)
            {
                case 1: return Ok(JSONHandler.BuildMsgJSON(1, "Task successfully executed"));
                case 0: return Ok(JSONHandler.BuildMsgJSON(0, "Task could not be completed"));
                case 2: return Ok(JSONHandler.BuildMsgJSON(0, "Resource already exists"));
            }
            return Ok(JSONHandler.BuildMsgJSON(0, "There was an internal error. Try again later"));
        }
    }
}
