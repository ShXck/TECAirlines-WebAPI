
using System;

namespace TECAirlines_WebAPI.Classes
{
    public class Flight
    {
        public string depart_ap;
        public string arrival_ap;
        public string flight_id;
        public DateTime depart_date;
        public string plane_model;
        public string status;
        public int normal_price;
        public int fc_price;
        public int seats_left;
        public int fc_seats_left;
    }
}