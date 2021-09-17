using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Employees.PersonData
{
    public interface IPersonDataFactory
    {
        List<PersonDataModel> Process(PatientListParameter parameter);
    }
}