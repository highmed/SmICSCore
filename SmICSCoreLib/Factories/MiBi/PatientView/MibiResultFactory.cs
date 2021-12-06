using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class MibiResultFactory : IMibiResultFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }

        private readonly ISpecimenFactory _specimenFac;

        public MibiResultFactory(IRestDataAccess restDataAccess, ISpecimenFactory specimenFac)
        {
            RestDataAccess = restDataAccess;
            _specimenFac = specimenFac;
        }

        public List<MiBiResult> Process(Patient patient)
        {
            List<MiBiResult> results = new List<MiBiResult>();
            List<Case> cases = RestDataAccess.AQLQuery<Case>(AQLCatalog.Cases(patient));
            foreach(Case c in cases)
            {
                List<MiBiResult> tmpResult = Process(c);
                results.AddRange(tmpResult);
            }
            return results;
        }

        public List<MiBiResult> Process(Case Case)
        {
            List<MiBiResult> results = RestDataAccess.AQLQuery<MiBiResult>(MetaDataQuery(Case));
            foreach (MiBiResult result in results)
            {
                SpecimenParameter parameter = new SpecimenParameter() { UID = result.UID };
                result.Specimens = _specimenFac.Process(parameter);
                result.Requirements = RestDataAccess.AQLQuery<Requirement>(RequirementQuery(parameter as RequirementParameter));
                result.Sender = RestDataAccess.AQLQuery<PatientLocation>(AQLCatalog.PatientLocation(result.Specimens[0].SpecimenCollectionDateTime, Case.PatientID)).FirstOrDefault();
            }
            return results;
        }

        private AQLQuery MetaDataQuery(Case Case)
        {
            return new AQLQuery()
            {
                Name = "Meta Daten - Mikrobiologischer Befund",
                Query = @$"SELECT v/items[at0001]/value as CaseID,
                        t/context/other_context[at0001]/items[at0005]/value as Status,
                        d/data[at0001]/events[at0002]/time/value as ResultDateTime,
                        d/protocol[at0004]/items[at0094]/items[at0063]/value/id as OrderID,
                        c/uid/value as UID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS COMPOSITION t[openEHR-EHR-COMPOSITION.report-result.v1]
                        CONTAINS (CLUSTER v[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND OBSERVATION d[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]) 
                        WHERE e/ehr_status/subject/external_ref/id/value='{Case.PatientID}
                        AND v/items[at0001]/value as CaseID = '{Case.CaseID}'"
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
