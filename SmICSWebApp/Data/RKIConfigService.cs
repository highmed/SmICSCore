using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.RKIConfig;
using System.Threading;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL;

namespace SmICSWebApp.Data
{
    public class RKIConfigService
    {
        private IRestDataAccess _restData;

        public RKIConfigService(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public List<PatientMovementModel> GetAllStations()
        {
            List<PatientMovementModel> stationList = _restData.AQLQuery<PatientMovementModel>(AQLCatalog.GetAllStationsForConfig());

            if (stationList == null)
            {
                return new List<PatientMovementModel>();
            }
            return stationList;
        }
        //public Task<IEnumerable<RKIConfigTemplate>> GetStationAsync(CancellationToken ct = default)
        //{
        //    return;
        //}
    }
}
