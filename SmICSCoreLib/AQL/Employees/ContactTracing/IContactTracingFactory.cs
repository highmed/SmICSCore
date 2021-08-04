using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Employees.ContactTracing
{
    public interface IContactTracingFactory
    {
        List<ContactTracingReceiveModel> Process(PatientListParameter parameter);
    }
}