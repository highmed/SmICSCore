
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Employees.ContactTracing;
using SmICSCoreLib.AQL.Employees.PersInfoInfecCtrl;
using SmICSCoreLib.AQL.Employees.PersonData;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Employees
{
    public interface IEmployeeInformation
    {
        List<ContactTracingReceiveModel> Employee_ContactTracing(PatientListParameter parameter);
        List<PersInfoInfecCtrlModel> Employee_PersInfoInfecCtrl(PatientListParameter parameter);
        List<PersonDataModel> Employee_PersonData(PatientListParameter parameter);

    }
}
