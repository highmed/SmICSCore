using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Employees.PersonData
{
    public interface IPersonDataFactory
    {
        List<PersonDataModel> Process(PatientListParameter parameter);
    }
}