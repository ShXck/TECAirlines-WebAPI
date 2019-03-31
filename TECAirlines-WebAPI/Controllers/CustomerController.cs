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
            string query_result = SQLHandler.FindFlight(JsonConvert.DeserializeObject<Flight>(search_params));
            return Ok(query_result);
        }
    }
}
