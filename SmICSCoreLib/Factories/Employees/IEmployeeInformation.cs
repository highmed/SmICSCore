
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Employees.ContactTracing;
using SmICSCoreLib.Factories.Employees.PersInfoInfecCtrl;
using SmICSCoreLib.Factories.Employees.PersonData;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Employees
{
    public interface IEmployeeInformation
    {
        List<ContactTracingReceiveModel> Employee_ContactTracing(PatientListParameter parameter);
        List<PersInfoInfecCtrlModel> Employee_PersInfoInfecCtrl(PatientListParameter parameter);
        List<PersonDataModel> Employee_PersonData(PatientListParameter parameter);

    }
}
