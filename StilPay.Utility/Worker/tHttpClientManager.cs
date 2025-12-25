using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StilPay.Utility.Worker
{
    public class tHttpClientManager<T> where T : class, new()
    {
        public static T PostJsonDataGetJson(string urlApi, Dictionary<string, string> header, Dictionary<string, object> body)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    foreach (var h in header)
                        client.DefaultRequestHeaders.Add(h.Key, h.Value);

                    var json = JsonConvert.SerializeObject(body, Formatting.Indented);
                    var sc = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = Task.Run(() => client.PostAsync(urlApi, sc));
                    response.Wait();

                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = Task.Run(() => response.Result.Content.ReadAsStringAsync());
                        result.Wait();

                        return JsonConvert.DeserializeObject<T>(string.IsNullOrEmpty(result.Result) ? "{}" : result.Result);
                    }

                    return null;
                }
            }
            catch { }

            return null;
        }

        public static async Task<T> PostJsonDataGetJsonAsync(string urlApi, Dictionary<string, string> header, Dictionary<string, object> body)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    foreach (var h in header)
                        client.DefaultRequestHeaders.Add(h.Key, h.Value);

                    var json = JsonConvert.SerializeObject(body, Formatting.Indented);
                    var sc = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlApi, sc);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<T>(string.IsNullOrEmpty(result) ? "{}" : result);
                    }

                    return null; 
                }
            }
            catch { }

            return null;
        }

        public static T PostFormDataGetJson(string urlApi, Dictionary<string, string> header, Dictionary<string, object> body)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    foreach (var h in header)
                        client.DefaultRequestHeaders.Add(h.Key, h.Value);

                    var fec = new FormUrlEncodedContent(body.ToDictionary(k => k.Key, k => k.Value.ToString()));

                    var response = Task.Run(() => client.PostAsync(urlApi, fec));
                    response.Wait();

                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        var result = Task.Run(() => response.Result.Content.ReadAsStringAsync());
                        result.Wait();

                        return JsonConvert.DeserializeObject<T>(JsonConvert.DeserializeObject<string>(result.Result));
                    }
                }
            }
            catch { }

            return null;
        }

        public static JObject GetRequestGetJObject(string urlApi, Dictionary<string, string> header)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    foreach (var h in header)
                        client.DefaultRequestHeaders.Add(h.Key, h.Value);

                    var response = Task.Run(() => client.GetAsync(urlApi));
                    response.Wait();

                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        var result = Task.Run(() => response.Result.Content.ReadAsStringAsync());
                        result.Wait();

                        return JsonConvert.DeserializeObject<JObject>(result.Result);
                    }
                }
            }
            catch { }

            return null;
        }

        public static T PostFormDataGetXML(string urlApi, Dictionary<string, string> header, Dictionary<string, object> body)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var fec = new FormUrlEncodedContent(body.ToDictionary(k => k.Key, k => k.Value.ToString()));

                    var response = Task.Run(() => client.PostAsync(urlApi, fec));
                    response.Wait();

                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        var result = Task.Run(() => response.Result.Content.ReadAsStreamAsync());
                        result.Wait();


                        using (StreamReader reader = new StreamReader(result.Result, Encoding.GetEncoding("iso-8859-9"), false))
                        {
                            string text = reader.ReadToEnd();

                            XmlSerializer serializer = new XmlSerializer(typeof(T));
                            using (StringReader sr = new StringReader(text))
                            {
                                return (T)serializer.Deserialize(sr);
                            }
                        }
                    }
                }
            }
            catch { }

            return null;
        }
    }
}
