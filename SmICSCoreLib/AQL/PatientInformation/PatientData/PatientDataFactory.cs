using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;



namespace SmICSCoreLib.AQL.PatientInformation.PatientData
{
    public class PatientDataFactory
    {
        public PatientDataFactory()
        {

        }

        public PatientData Process(string PatientID)
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient Client = new HttpClient(handler);
            Client.Timeout = TimeSpan.FromMilliseconds(300000);

            string request = String.Format("MSH|^~\\&|OpenEHR-Fragebogen|PDQv2ConsumerDevice|||{datetime}||QBP^Q22^QBP_Q21|15017|P|2.5\nQPD|IHE PDQ Query|15018|@PID.3.1^{patID}~@PID.3.4.1^MHHPatID~@PID.3.4.2^1.2.276.0.76.3.1.278.1.1.9904.1.1~@PID.3.4.3^ISO|||||\n^^^MHHPatID&1.2.276.0.76.3.1.278.1.1.9904.1.1&ISO\nRCP|I", DateTime.Now.ToString("yyyyMMddhhmmss"), PatientID);
            HttpContent content = new StringContent(request, Encoding.UTF8, "application/text");
            HttpResponseMessage response = Client.PostAsync("https://hagen.mh-hannover.local/csp/ensemble/AuftragHTTPin/EnsLib.HL7.Service.HTTPService.cls?CfgItem=RestPixPdqAbfrage", content).Result;
            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                PatientData data = JsonConvert.DeserializeObject<PatientData>(json);
                return data;
            }
            return null;
        }
    }
}
