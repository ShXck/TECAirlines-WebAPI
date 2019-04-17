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
            search_result["http_result"] = 1;
            search_result["status"] = status;
            search_result["flight_id"] = fl_id;
            search_result["depart_date"] = fl_date;                

            return search_result.ToString();
        }

        public static string BuildListStrResult(string attribute, List<string> results)
        {
            JArray array = new JArray();
            for(int i = 0; i < results.Count; i++)
            {
                array.Add(results.ElementAt(i));
            }
            JObject result = new JObject();
            result[attribute] = array;
            result["http_result"] = 1;

            return result.ToString();
        }

        public static string BuildMsgJSON(int result, string msg)
        {
            JObject search_result = new JObject();
            search_result["http_result"] = result;
            search_result["msg"] = msg;
            return search_result.ToString();
        }

        public static string BuildFlightDetails(string dep_ap, string arr_ap, int price, int fc_price)
        {
            JObject search_result = new JObject();
            search_result["http_result"] = 1;
            search_result["depart_ap"] = dep_ap;
            search_result["arrival_ap"] = arr_ap;
            search_result["normal_price"] = price;
            search_result["fc_price"] = fc_price;
            return search_result.ToString();
        }

        public static string BuildPair(string key1, string val1, string key2, string val2)
        {
            JObject pair_json = new JObject();
            pair_json[key1] = val1;
            pair_json[key2] = val2;
            pair_json["http_result"] = 1;

            return pair_json.ToString();
        }

        public static string BuildCost(int cost)
        {
            JObject cost_json = new JObject();
            cost_json["cost"] = cost;
            cost_json["http_result"] = 1;
            return cost_json.ToString();
        }

        public static string FormatAsString(Object obj)
        {
            return String.Format("{0}", obj);
        }

        public static int FormatAsInt(Object obj)
        {
            return Convert.ToInt32(obj);
        }
    }
}