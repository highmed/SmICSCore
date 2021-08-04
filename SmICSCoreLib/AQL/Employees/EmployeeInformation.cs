using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Employees;
using SmICSCoreLib.AQL.Employees.ContactTracing;
using SmICSCoreLib.AQL.Employees.PersonData;
using SmICSCoreLib.AQL.Employees.PersInfoInfecCtrl;
using SmICSCoreLib.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Employees
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
