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

        public static string BuildActiveFlightsResult(List<string> results)
        {
            JArray array = new JArray();
            for(int i = 0; i < results.Count; i++)
            {
                array.Add(results.ElementAt(i));
            }
            JObject result = new JObject();
            result["flights"] = array;

            return result.ToString();
        }

        public static string BuildErrorJSON(string error_msg)
        {
            JObject search_result = new JObject();
            search_result["http_result"] = 0;
            search_result["error_msg"] = error_msg;
            return search_result.ToString();
        }

        public static string FormatAsString(Object obj)
        {
            return String.Format("{0}", obj);
        }
    }
}