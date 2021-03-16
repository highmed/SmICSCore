using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_DiagnosticResult
{
    public class DiagnosticResultFactory : IDiagnosticResultFactory
    {
        private IRestDataAccess _restData;
        public DiagnosticResultFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<DiagnosticResultModel> Process(PatientListParameter parameter)
        {
            List<DiagnosticResultModel> diagnosticResultList = _restData.AQLQuery<DiagnosticResultModel>(AQLCatalog.PatientDiagnosticResults(parameter).Query);

            if (diagnosticResultList is null)
            {
                return new List<DiagnosticResultModel>();
            }

            return diagnosticResultList;
        }
    }
}
