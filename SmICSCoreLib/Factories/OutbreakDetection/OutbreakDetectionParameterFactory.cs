
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.OutbreakDetection.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.OutbreakDetection
{
    public class OutbreakDetectionParameterFactory : IOutbreakDetectionParameterFactory
    {
        private IRestDataAccess _restData;
        public OutbreakDetectionParameterFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public int[][] Process(OutbreakDetectionParameter parameter, SmICSVersion version)
        {
            if(parameter.Retro)
            {
                EarliestMovement firstMove = _restData.AQLQuery<EarliestMovement>(AQLCatalog.GetFirstMovementFromStation(parameter)).FirstOrDefault();
                parameter.Starttime = firstMove.MinDate;
            }
            if (version == SmICSVersion.VIROLOGY)
            {
                return ProcessViro(parameter);
            }
            return null;
        }

        private int[][] ProcessViro(OutbreakDetectionParameter parameter)
        {
            List<OutbreakDectectionPatient> patientList = _restData.AQLQuery<OutbreakDectectionPatient>(AQLCatalog.GetPatientCaseList(parameter));
            int[] PositivCounts = GetPatientLabResults(patientList, parameter);
            int[] Epochs = GenerateEpochsArray(parameter);
            int[][] epochs_and_outbreaks = new int[][] { Epochs, PositivCounts };
            return epochs_and_outbreaks;
        }

        private int[] GetPatientLabResults(List<OutbreakDectectionPatient> patientList, OutbreakDetectionParameter parameter)
        {
            int[] FirstPositiveCounts = new int[(int)(parameter.Endtime - parameter.Starttime).TotalDays];

            foreach (OutbreakDectectionPatient pat in patientList)
            {
                List<OutbreakDetectionLabResult> labResult = _restData.AQLQuery<OutbreakDetectionLabResult>(AQLCatalog.GetPatientLabResultList(parameter, pat));
                labResult = labResult.OrderBy(l => l.ResultDate).ToList();
                OutbreakDetectionLabResult result = labResult.Where(l => l.ResultDate >= parameter.Starttime && l.Result == (int)SarsCovResult.POSITIVE).FirstOrDefault();
                if (result != null)
                {
                    FirstPositiveCounts[(int)(result.ResultDate - parameter.Starttime).TotalDays] += 1;
                }
            }

            return FirstPositiveCounts;
        }

        private int[] GenerateEpochsArray(OutbreakDetectionParameter parameter)
        {
            int[] epochs = new int[(int)(parameter.Endtime - parameter.Starttime).TotalDays];
            int i = 0;
            DateTime referenceDate = new DateTime(1970, 1, 1);
            for (DateTime date = parameter.Starttime; date < parameter.Endtime; date = date.AddDays(1.0))
            {
                epochs[i] = (int)(date.Date - referenceDate.Date).TotalDays;
                i += 1;
            }
            return epochs;
        }
    }
}
