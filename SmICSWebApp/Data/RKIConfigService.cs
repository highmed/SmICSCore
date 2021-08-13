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
using Newtonsoft.Json.Linq;

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

        public void StoreRules(JObject storedValues, string zeitpunkt)
        {
            try
            {
                Console.WriteLine(storedValues.ToString());
                //var storedValuestojson = JsonConvert.DeserializeObject(JObject.Parse(storedValues.ToString()));
            }
            catch(Exception)
            {
                throw new Exception($"Failed to store data");
            }
        }
        //public Task<IEnumerable<RKIConfigTemplate>> GetStationAsync(CancellationToken ct = default)
        //{
        //    return;
        //}
    }
}
