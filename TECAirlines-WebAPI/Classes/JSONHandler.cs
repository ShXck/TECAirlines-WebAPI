using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TECAirlines_WebAPI.Classes
{
    public class JSONHandler
    {
        /// <summary>
        /// Crea un JSON con los datos de un vuelo.
        /// </summary>
        /// <param name="fl_id">el id del vuelo.</param>
        /// <param name="fl_date">la fecha del vuelo.</param>
        /// <param name="price">el precio del vuelo.</param>
        /// <param name="fc_price">el precio de primera clase.</param>
        /// <returns>El JSON con la información.</returns>
        public static string BuildFlightSearchResult(string fl_id, string fl_date, int price, int fc_price)
        {
            JObject search_result = new JObject();
            search_result["flight_id"] = fl_id;
            search_result["depart_date"] = fl_date;
            search_result["price"] = price;
            search_result["fc_price"] = fc_price;
            return search_result.ToString();
        }

        /// <summary>
        /// Crea un JSON con los datos de vuelos encontrados.
        /// </summary>
        /// <param name="flight_id">el id del vuelo.</param>
        /// <param name="depart">el aeropuerto de salida.</param>
        /// <param name="arrival">El aeropuerto de llegada.</param>
        /// <returns>El JSON con la información.</returns>
        public static string BuildUserFlightResult(string flight_id, string depart, string arrival)
        {
            JObject search_result = new JObject();
            search_result["flight_id"] = flight_id;
            search_result["depart_ap"] = depart;
            search_result["arrival_ap"] = arrival;
            return search_result.ToString();
        }

        /// <summary>
        /// Crea un JSON array con resultado de una búsqueda.
        /// </summary>
        /// <param name="attribute">El nombre del atributo.</param>
        /// <param name="results">La lista de resultados.</param>
        /// <returns>El JSON con la información.</returns>
        public static string BuildListStrResult(string attribute, List<string> results)
        {
            JArray array = new JArray();
            for(int i = 0; i < results.Count; i++)
            {
                array.Add(results.ElementAt(i));
            }
            JObject result = new JObject();
            result["http_result"] = 1;
            result[attribute] = array;

            return result.ToString();
        }

        /// <summary>
        /// Crea un JSON con un mensaje y resultado de operación.
        /// </summary>
        /// <param name="result">Resultado de operación (1 o 0)</param>
        /// <param name="msg">El mensaje a ser enviado.</param>
        /// <returns>El JSON con la información.</returns>
        public static string BuildMsgJSON(int result, string msg)
        {
            JObject search_result = new JObject();
            search_result["msg"] = msg;
            search_result["http_result"] = result;
            return search_result.ToString();
        }

        /// <summary>
        /// Crea un JSON con información de vuelo.
        /// </summary>
        /// <param name="dep_ap">Aeropuerto de salida.</param>
        /// <param name="arr_ap">Aeropuerto de llegada.</param>
        /// <param name="price">Precio normal.</param>
        /// <param name="fc_price">Precio primera clase.</param>
        /// <returns>El JSON con la información.</returns>
        public static string BuildFlightDetails(string dep_ap, string arr_ap, int price, int fc_price)
        {
            JObject search_result = new JObject();
            search_result["depart_ap"] = dep_ap;
            search_result["arrival_ap"] = arr_ap;
            search_result["normal_price"] = price;
            search_result["fc_price"] = fc_price;
            search_result["http_result"] = 1;
            return search_result.ToString();
        }

        /// <summary>
        /// Construe un JSON con dos valores.
        /// </summary>
        /// <param name="key1">Primer atributo.</param>
        /// <param name="val1">Primer valor.</param>
        /// <param name="key2">Segundo atributo.</param>
        /// <param name="val2">Segundo valor.</param>
        /// <returns>El JSON con la información.</returns>
        public static string BuildPair(string key1, string val1, string key2, string val2)
        {
            JObject pair_json = new JObject();
            pair_json[key1] = val1;
            pair_json[key2] = val2;
            pair_json["http_result"] = 1;

            return pair_json.ToString();
        }

        /// <summary>
        /// Crea un JSON con el costo de una reservación.
        /// </summary>
        /// <param name="cost">El costo calculado.</param>
        /// <returns>El JSON con la información.</returns>
        public static string BuildCost(int cost)
        {
            JObject cost_json = new JObject();
            cost_json["cost"] = cost;
            cost_json["http_result"] = 1;
            return cost_json.ToString();
        }

        /// <summary>
        /// Crea un JSON con el número de personas de una reservación.
        /// </summary>
        /// <param name="ppl">Número de personas.</param>
        /// <returns>El JSON con la información.</returns>
        public static string BuildPeopleFlying(int ppl)
        {
            JObject cost_json = new JObject();
            cost_json["people"] = ppl;
            cost_json["http_result"] = 1;
            return cost_json.ToString();
        }

        /// <summary>
        /// Convierte un JArray a una lista de C#.
        /// </summary>
        /// <param name="jarray">El arreglo.</param>
        /// <returns>La lista de valores del arreglo.</returns>
        public static List<string> JArrayToList(string jarray)
        {
            JArray arr = JArray.Parse(jarray);
            return arr.ToObject<List<string>>();
        }

        /// <summary>
        /// Crea un JSON con datos de una promoción.
        /// </summary>
        /// <param name="discount">Descuento.</param>
        /// <param name="d_ap">Salida.</param>
        /// <param name="a_ap">Destino.</param>
        /// <returns>El JSON con la información.</returns>
        public static string BuildSale(int discount, string d_ap, string a_ap)
        {
            JObject cost_json = new JObject();
            cost_json["discount"] = discount;
            cost_json["depart_ap"] = d_ap;
            cost_json["arrival_ap"] = a_ap;
            return cost_json.ToString();
        }

        /// <summary>
        /// Pasa de objeto a string.
        /// </summary>
        /// <param name="obj">El objeto a convertir.</param>
        /// <returns>La representación de string.</returns>
        public static string FormatAsString(Object obj)
        {
            return String.Format("{0}", obj);
        }

        /// <summary>
        /// Convierte de objeto a int.
        /// </summary>
        /// <param name="obj">El objeto a convertir.</param>
        /// <returns>El objeto convertido.</returns>
        public static int FormatAsInt(Object obj)
        {
            return Convert.ToInt32(obj);
        }
    }
}