using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.ViroLabData.ReceiveModel;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Lab.ViroLabData
{
    public class ViroLabDataFactory : IViroLabDataFactory
    {
        private readonly string INCONCLUSIVE = "419984006";
        public IRestDataAccess RestDataAccess { get; }
        private readonly ILogger<ViroLabDataFactory> _logger;
        public ViroLabDataFactory(IRestDataAccess restData, ILogger<ViroLabDataFactory> logger)
        {
            _logger = logger;
            RestDataAccess = restData;
        }
        public List<LabDataModel> Process(PatientListParameter parameter)
        {
            List<LabDataReceiveModel> receiveLabDataList = RestDataAccess.AQLQuery<LabDataReceiveModel>(AQLCatalog.PatientLaborData(parameter));

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
