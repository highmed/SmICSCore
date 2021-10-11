
using System;
using System.IO;
using System.Collections.Generic;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.InfectionsStatus;

namespace SmICSCoreLib.AQL.DetectionAlgorithmInterface
{
    public class BuildDetectionInput
    {
        private IRestDataAccess _restData;

        public BuildDetectionInput(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public int[][] GetTimeSeriesForDetectionAlgorithm(TimespanParameter timespanParameter,
                                                          string kindOfInfection,
                                                          string stationID)
        {
            InfectionsStatusFactory infectionsStatusFactory = new InfectionsStatusFactory(_restData);
            Dictionary<string, SortedDictionary<DateTime, int>> infectionsStatus_Process = infectionsStatusFactory.Process(timespanParameter, kindOfInfection);

            int[] epochs = new int[infectionsStatus_Process[stationID].Count];
            int[] observed = new int[infectionsStatus_Process[stationID].Count];

            try
            {
                int countEpochs = 0;
                foreach (var item0 in infectionsStatus_Process[stationID])
                {
                    epochs[countEpochs] = (int) (item0.Key - new DateTime(1970, 1, 1)).TotalDays;
                    observed[countEpochs] = item0.Value;
                    countEpochs++;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return new [] {epochs, observed};
        }
    }
}
