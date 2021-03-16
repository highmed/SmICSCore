using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmICSWebApp.Helper
{
    public class RestClient
    {
       // HttpClient _client = new HttpClient();
        public string endPoint { get; set; }

        public RestClient()
        {
            endPoint = string.Empty;
        }

        public string GetResponse()
        {
            var strResponse = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                        if (responseStream != null)
                        {
                            StreamReader sr = new StreamReader(responseStream);
                            JsonReader jsonReader = new JsonTextReader(sr);
                            strResponse = sr.ReadToEnd();
                            sr.Close();
                        }
                }

                return strResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public string GetResponsePost()
        {
            var strResponse = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
                request.Method = "POST";

                var newStream = request.GetRequestStream();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                        if (responseStream != null)
                        {
                            StreamReader sr = new StreamReader(responseStream);
                            JsonReader jsonReader = new JsonTextReader(sr);
                            strResponse = sr.ReadToEnd();
                            sr.Close();
                        }
                }

                return strResponse;
            }
            catch (Exception)
            {
                return null;
            }

        }


        //public async Task<string> GetResponseAsync(string endPoint2)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string response = await client.GetStringAsync(endPoint2);
        //            return response;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
    }
}
