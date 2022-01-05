using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.Contact
{
    public interface IContactFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<PatientMovementNew.PatientStays.PatientStay> Process(Hospitalization hospitalization);
  
    }
}