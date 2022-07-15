using System.Collections.Generic;
using SmICSCoreLib.REST;

namespace SmICSCoreLib.Factories.RKIConfig
{
    public class RKILabDataFactory : IRKILabDataFactory
    {
        protected IRestDataAccess _restData;

        public RKILabDataFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public List<LabDataKeimReceiveModel> ProcessGetErreger(string name)
        {
            List<LabDataKeimReceiveModel> erregerList = new List<LabDataKeimReceiveModel>();
            if (name == "SARS-CoV-2")
            {
                erregerList = _restData.AQLQueryAsync<LabDataKeimReceiveModel>(AQLCatalog.GetErregernameFromViro(name)).GetAwaiter().GetResult();
            }
            else
            {
                erregerList = _restData.AQLQueryAsync<LabDataKeimReceiveModel>(AQLCatalog.GetErregernameFromMikro(name)).GetAwaiter().GetResult();
            }

            return erregerList;
        }
    }
}
