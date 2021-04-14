using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Employees.ContactTracing
{
    public interface IContactTracingFactory
    {
        List<ContactTracingModel> Process(PatientListParameter parameter);
        //bool ContactTracingSaveComposition(string j_string, string filepath);
    }
}