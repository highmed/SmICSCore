
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Employees.ContactTracing;
using SmICSCoreLib.Factories.Employees.PersonData;
using SmICSCoreLib.Factories.Employees.PersInfoInfecCtrl;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Employees
{
    public class EmployeeInformation : IEmployeeInformation
    {
        private IContactTracingFactory _empConTracFac;
        private IPersInfoInfecCtrlFactory _empPIICFac;
        private IPersonDataFactory _empPersDataFac;

        public EmployeeInformation(IContactTracingFactory empConTracFac, IPersInfoInfecCtrlFactory empPIICFac, IPersonDataFactory empPersDataFac) 
        {
            _empConTracFac = empConTracFac;
            _empPIICFac = empPIICFac;
            _empPersDataFac = empPersDataFac;

        }
        public List<ContactTracingReceiveModel> Employee_ContactTracing(PatientListParameter parameter)
        {
            return _empConTracFac.Process(parameter);
        }


        public List<PersInfoInfecCtrlModel> Employee_PersInfoInfecCtrl(PatientListParameter parameter)
        {
            return _empPIICFac.Process(parameter);
        }

        public List<PersonDataModel> Employee_PersonData(PatientListParameter parameter)
        {
            return _empPersDataFac.Process(parameter);
        }

    }

}
