using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.ViroLabData.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.Lab.ViroLabData
{
    public class ViroLabDataFactory : IViroLabDataFactory
    {
        private readonly string INCONCLUSIVE = "419984006";
        protected IRestDataAccess _restData;
        private readonly ILogger<ViroLabDataFactory> _logger;
        public ViroLabDataFactory(IRestDataAccess restData, ILogger<ViroLabDataFactory> logger)
        {
            _logger = logger;
            _restData = restData;
        }
        public List<LabDataModel> Process(PatientListParameter parameter)
        {
            List<LabDataReceiveModel> receiveLabDataList = _restData.AQLQuery<LabDataReceiveModel>(AQLCatalog.PatientLaborData(parameter));

            if (receiveLabDataList is null)
            {
                return new List<LabDataModel>();
            }

            return LabDataConstructor(receiveLabDataList);


        }

        private List<LabDataModel> LabDataConstructor(List<LabDataReceiveModel> receiveLabDataList)
        {
            List<LabDataModel> labDataModels = new List<LabDataModel>();
            foreach (LabDataReceiveModel labData in receiveLabDataList)
            {
                if(labData.BefundCode == INCONCLUSIVE)
                {
                    continue;
                }
                labDataModels.Add(new LabDataModel(labData));
            }
            return labDataModels;
        }
        
    }
}
