using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;



namespace SmICSCoreLib.AQL.PatientInformation.PatientData
{
    public class PatientDataFactory : IPatientDataFactory
    {
        public PatientDataFactory()
        {

        }

        public PatientData Process(string PatientID)
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient Client = new HttpClient(handler);

            var byteArray = Encoding.ASCII.GetBytes("geckohttp:>W9N[Y3$6Ue\\NspG[zF<+Y");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            Client.Timeout = TimeSpan.FromMilliseconds(300000);
            string datetime = DateTime.Now.ToString("yyyyMMddhhmmss");
            //string request = $"MSH\u007c\u02C4\u007E\u005C&\u007cOpenEHR-Fragebogen\u007cPDQv2ConsumerDevice\u007c\u007c\u007c{datetime}\u007c\u007cQBP\u02C4Q22\u02C4QBP_Q21\u007c15017\u007cP\u007c2.5\u000DQPD\u007cIHE PDQ Query\u007c15018\u007c@PID.3.1\u02C4{PatientID}\u007E@PID.3.4.1\u02C4MHHPatID\u007E@PID.3.4.2\u02C41.2.276.0.76.3.1.278.1.1.9904.1.1\u007E@PID.3.4.3\u02C4ISO\u007c\u007c\u007c\u007c\u007c\u02C4\u02C4\u02C4MHHPatID&1.2.276.0.76.3.1.278.1.1.9904.1.1&ISO\u000DRCP\u007cI";
            string request = $"MSH|^~\\&|OpenEHR-Fragebogen|PDQv2ConsumerDevice|||{datetime}||QBP^Q22^QBP_Q21|15017|P|2.5\rQPD|IHE PDQ Query|15018|@PID.3.1^{PatientID}~@PID.3.4.1^MHHPatID~@PID.3.4.2^1.2.276.0.76.3.1.278.1.1.9904.1.1~@PID.3.4.3^ISO|||||^^^MHHPatID&1.2.276.0.76.3.1.278.1.1.9904.1.1&ISO\rRCP|I";
            //string request = String.Format(message, dt, PatientID);
            HttpContent content = new StringContent(request, Encoding.UTF8, "text/plain");
            HttpResponseMessage response = Client.PostAsync("https://hagen.mh-hannover.local/csp/ensemble/AuftragHTTPin/EnsLib.HL7.Service.HTTPService.cls?CfgItem=RestPixPdqAbfrage", content).Result;
            
            if (response.IsSuccessStatusCode)
            {
                string hl7 = response.Content.ReadAsStringAsync().Result;
                PatientData data = Hl7Parser(hl7);
                return data;
            }
            return null;
        }

        private PatientData Hl7Parser(string message)
        {
            try { 
                string lastLine = message.Split("\r")[4];
                string[] cells = lastLine.Split("|");
                PatientData patientData = new PatientData()
                {
                    PatientID = cells[3].Split("^")[0],
                    Name = cells[5].Replace("^", "da "),
                    Birthdate = DateTime.ParseExact(cells[7], "yyyyMMdd", CultureInfo.InvariantCulture),
                    Sex = cells[8]
                };
                return patientData;
            }
            catch
            {
                return null;
            }
        }
    }
}
