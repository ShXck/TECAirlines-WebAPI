using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TECAirlines_WebAPI.Classes
{
    public class JSONHandler
    {
        public static string BuildFlightSearchResult(string status, string fl_id, string fl_date)
        {
            JObject search_result = new JObject();
            search_result["query_result"] = 1;
            search_result["status"] = status;
            search_result["flight_id"] = fl_id;
            search_result["depart_date"] = fl_date;                

            return search_result.ToString();
        }

        public static string BuildErrorJSON()
        {
            JObject search_result = new JObject();
            search_result["http_result"] = 0;
            return search_result.ToString();
        }

        public static string FormatAsString(Object obj)
        {
            return String.Format("{0}", obj);
        }
    }
}