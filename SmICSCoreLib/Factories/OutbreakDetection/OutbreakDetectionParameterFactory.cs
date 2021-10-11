
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.OutbreakDetection.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.OutbreakDetection
{
    public class OutbreakDetectionParameterFactory
    {
        private IRestDataAccess _restData;
        public OutbreakDetectionParameterFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        //OutbreakDetectionParameter löschen für Config-Parameter
        public void Process(OutbreakDetectionParameter parameter, SmICSVersion version)
        {
            if(version == SmICSVersion.VIROLOGY)
            {
                ProcessViro(parameter);
            }
        }

        private void ProcessViro(OutbreakDetectionParameter parameter)
        {
            List<OutbreakDectectionPatient> patientList = _restData.AQLQuery<OutbreakDectectionPatient>(AQLCatalog.GetPatientCaseList(parameter));
            int[] PositivCounts = GetPatientLabResults(patientList, parameter);
        }

        private int[] GetPatientLabResults(List<OutbreakDectectionPatient> patientList, OutbreakDetectionParameter parameter)
        {
            int[] FirstPositiveCounts = new int[(int)(parameter.Endtime - parameter.Starttime).TotalDays];
            foreach (OutbreakDectectionPatient pat in patientList)
            {
                List<OutbreakDetectionLabResult> labResult = _restData.AQLQuery<OutbreakDetectionLabResult>(AQLCatalog.GetPatientLabResultList(parameter, pat));
                labResult = labResult.OrderBy(l => l.ResultDate).ToList();
                OutbreakDetectionLabResult result = labResult.Where(l => l.ResultDate >= parameter.Starttime && l.Result == (int)SarsCovResult.POSITIVE).FirstOrDefault();
                if(result != null)
                {
                    FirstPositiveCounts[(int)(result.ResultDate - parameter.Starttime).TotalDays] += 1; 
                }
            }

            return FirstPositiveCounts;
        }

        private void foo(Dictionary<OutbreakDectectionPatient, List<OutbreakDetectionLabResult>> labResults)
        {
            //Für jeden postiven Befund pro patient zu zeitpunkt x inkrementieren falls zu zeitpunkt x schon nicht negativ
        }
    }
}
