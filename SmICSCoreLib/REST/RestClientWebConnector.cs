using System;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace SmICSCoreLib.REST
{
    public class RestClientWebConnector
    {
        public string EndPoint { get; set; }

        public RestClientWebConnector()
        {
            EndPoint = string.Empty;
        }

        public string GetResponse()
        {
            var strResponse = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(EndPoint);
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using Stream responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        StreamReader sr = new(responseStream);
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
    }
}
