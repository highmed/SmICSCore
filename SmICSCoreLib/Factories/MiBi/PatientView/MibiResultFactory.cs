using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class MibiResultFactory : IMibiResultFactory
    {
        public IRestDataAccess _restDataAccess { get; set; }
        private readonly ISpecimenFactory _specimenFac;

        public MibiResultFactory(IRestDataAccess restDataAccess, ISpecimenFactory specimenFac)
        {
            _restDataAccess = restDataAccess;
            _specimenFac = specimenFac;
        }

        public List<MiBiResult> Process(Patient patient)
        {
            List<MiBiResult> results = _restDataAccess.AQLQuery<MiBiResult>(MetaDataQuery(patient));
            foreach (MiBiResult result in results)
            {
                SpecimenParameter parameter = new SpecimenParameter() { UID = result.UID };
                result.Specimens = _specimenFac.Process(parameter);
                result.Requirements = _restDataAccess.AQLQuery<Requirement>(RequirementQuery(parameter as RequirementParameter));
                result.Sender = _restDataAccess.AQLQuery<PatientLocation>(AQLCatalog.PatientLocation(result.Specimens[0].SpecimenCollectionDateTime, patient.PatientID)).FirstOrDefault();
            }
            return results;
        }

        private AQLQuery MetaDataQuery(Patient patient)
        {
            return new AQLQuery()
            {
                Name = "Meta Daten - Mikrobiologischer Befund",
                Query = @$"SELECT v/items[at0001]/value/value as CaseID,
                        c/context/other_context[at0001]/items[at0005]/value/value as Status,
                        d/data[at0001]/events[at0002]/time/value as ResultDateTime,
                        d/protocol[at0004]/items[at0094]/items[at0063]/value/id as OrderID,
                        c/uid/value as UID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS (CLUSTER v[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND OBSERVATION d[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]) 
                        WHERE e/ehr_status/subject/external_ref/id/value='{patient.PatientID}'"
            };
        }

        private AQLQuery RequirementQuery(RequirementParameter parameter)
        {
            return new AQLQuery()
            {
                Name = "Anforderungen - Mikrobiologischer Befund",
                Query = @$"SELECT DISTINCT
                        a/protocol[at0004]/items[at0094]/items[at0106]/value/value as Name
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS (CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND OBSERVATION a[openEHR-EHR-OBSERVATION.laboratory_test_result.v1])
                        WHERE c/uid/value = '{parameter.UID}'"
            };

        }
    }
}
