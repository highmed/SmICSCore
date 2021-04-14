using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;


namespace SmICSCoreLib.AQL.Employees.ContactTracing
{
    public class ContactTracingFactory : IContactTracingFactory
    {
        private IRestDataAccess _restData;

        public ContactTracingFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<ContactTracingModel> Process(PatientListParameter parameter)
        {

            List<ContactTracingModel> ctList = _restData.AQLQuery<ContactTracingModel>(AQLCatalog.EmployeeContactTracing(parameter));

            if (ctList is null)
            {
                return new List<ContactTracingModel>();
            }

            return ctList;
        }

        //public bool ContactTracingSaveComposition(string j_string, string filepath)
        //{
        //    using (var content = new StringContent(JsonConvert.SerializeObject(j_string), System.Text.Encoding.UTF8, filepath))
        //    {
        //        HttpResponseMessage result = _restData.CreateEhrIDWithStatus("SmICSTest", "Patient35").Result;
        //        string ehr_id = result.IsSuccessStatusCode.ToString();

        //        if (ehr_id != null)
        //        {
        //            HttpResponseMessage responseMessage = _restData.CreateComposition(ehr_id, j_string).Result;
        //            if (responseMessage.StatusCode != System.Net.HttpStatusCode.Created)
        //            {
        //                string returnValue = responseMessage.Content.ReadAsStringAsync().Result;
        //                throw new Exception($"Failed to POST data: ({responseMessage.StatusCode}): {returnValue}");
        //            }
        //            else
        //                return true;
        //        }
        //        else
        //            throw new Exception($"Failed to POST data");

        //    }
        //}
    }
}
