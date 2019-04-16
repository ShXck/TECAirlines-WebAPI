using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TECAirlines_WebAPI.Classes
{
    public class Reservation
    {
        public string flight_id;
        public string type; // Ida vuelta o solo Ida.
        public bool is_first_class;
        public int people_flying;
        public string username;
        public int total_cost;
    }
}