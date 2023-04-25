using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MiBi.Contact
{
    public interface IContactFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Task<List<PatientMovementNew.PatientStays.PatientStay>> ProcessAsync(Hospitalization hospitalization);
        Task<Dictionary<Hospitalization, List<PatientMovementNew.PatientStays.PatientStay>>> ProcessAsync(Patient parameter);

    }
}