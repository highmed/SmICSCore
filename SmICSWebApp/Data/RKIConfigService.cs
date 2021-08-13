using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.RKIConfig;
using System.Threading;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL;

namespace SmICSWebApp.Data
{
    public class RKIConfigService
    {
        private IRestDataAccess _restData;
        private readonly IPatientInformation _patientInformation;

        public RKIConfigService(IPatientInformation patientInformation, IRestDataAccess restData)
        {
            _patientInformation = patientInformation;
            _restData = restData;
        }

        public List<PatientMovementModel> GetAllStations()
        {
            try
            {
                List<PatientMovementModel> stationListe = _patientInformation.All_Stations();
                return stationListe;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public Task<IEnumerable<RKIConfigTemplate>> GetStationAsync(CancellationToken ct = default)
        //{
        //    return;
        //}
    }
}
