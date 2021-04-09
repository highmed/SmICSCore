using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SmICSCoreLib.AQL.Employees.ContactTracing
{
    public class ContactTracingFactory : IContactTracingFactory
    {
        private IRestDataAccess _restData;
        private RestClientConnector _client;
        public ContactTracingFactory(IRestDataAccess restData, RestClientConnector client)
        {
            _restData = restData;
            _client = client;
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

        public void SaveContactTracing(string filepath)
        {
            HttpResponseMessage response = _client.Client.GetAsync(OpenehrConfig.openehrEndpoint + filepath).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {

            }
        }
    }
}
