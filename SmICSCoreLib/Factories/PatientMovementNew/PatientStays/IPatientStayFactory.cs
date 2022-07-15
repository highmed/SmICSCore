using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.PatientMovementNew.PatientStays
{
    public interface IPatientStayFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Task<List<PatientStay>> ProcessAsync(Case Case);
        Task<List<PatientStay>> ProcessAsync(WardParameter wardParameter);
    }
}