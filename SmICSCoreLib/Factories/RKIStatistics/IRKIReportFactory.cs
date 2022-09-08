using SmICSCoreLib.Factories.RKIStatistics.ReceiveModels;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.RKIStatistics
{
    public interface IRKIReportFactory
    {
        IRestDataAccess RestDataAccess { get; set; }
        public Task<RKIReportFeatures<RKIReportStateCaseModel>> GetStateData(int ID);
        public Task GetAllStates();
        public Task<List<RKIReportFeatures<RKIReportStateModel>>> GetStateByName();
        public Task<List<RKIReportFeatures<RKIReportDistrictModel>>> GetDistrictByName(List<string> gen);
        public double GetRValue(int value);
    }
}
