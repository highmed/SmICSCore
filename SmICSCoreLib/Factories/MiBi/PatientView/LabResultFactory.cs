using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class LabResultFactory : ILabResultFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }

        private readonly ISpecimenFactory _specimenFac;

        public LabResultFactory(IRestDataAccess restDataAccess, ISpecimenFactory specimenFac)
        {
            RestDataAccess = restDataAccess;
            _specimenFac = specimenFac;
        }

        public List<LabResult> Process(Patient patient, PathogenParameter pathogen = null)
        {
            List<LabResult> results = new List<LabResult>();
            List<Case> cases = RestDataAccess.AQLQuery<Case>(AQLCatalog.Cases(patient));
            foreach(Case c in cases)
            {
                List<LabResult> tmpResult = Process(c, pathogen);
                results.AddRange(tmpResult);
            }
            return results;
        }

        public List<LabResult> Process(Case Case, PathogenParameter pathogen = null)
        {
            List<UID> uids = null;
            if (pathogen != null)
            {
                uids = RestDataAccess.AQLQuery<UID>(GetPathogenCompositionsUIDs(Case, pathogen));
            }
            List<LabResult> results = RestDataAccess.AQLQuery<LabResult>(MetaDataQuery(Case, pathogen, uids));
            foreach (LabResult result in results)
            {
                SpecimenParameter parameter = new SpecimenParameter() { UID = result.UID };
                result.Specimens = _specimenFac.Process(parameter, pathogen);
                result.Specimens.RemoveAll(spec => spec.Pathogens == null);
                result.Requirements = RestDataAccess.AQLQuery<Requirement>(RequirementQuery(new RequirementParameter(parameter)));
                if(result.Specimens.Count > 0)
                {
                    result.Sender = RestDataAccess.AQLQuery<PatientLocation>(AQLCatalog.PatientLocation(result.Specimens[0].SpecimenCollectionDateTime, Case.PatientID)).FirstOrDefault();
                }
            }
            results.RemoveAll(lab => lab.Specimens.Count == 0);
            return results;
        }


        private AQLQuery GetPathogenCompositionsUIDs(Case Case, PathogenParameter pathogen)
        {
            return new AQLQuery()
            {
                Name = $"LabDataUID - {pathogen.MedicalField}",
                Query = @$"SELECT DISTINCT c/uid/value as ID
                            FROM EHR e
                            CONTAINS COMPOSITION c
                            CONTAINS CLUSTER m[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                            WHERE c/name/value='{pathogen.MedicalField}'
                            and m/items[at0001]/name/value = 'Erregername'
                            and m/items[at0001]/value/value = '{pathogen.Name}'
                            and e/ehr_status/subject/external_ref/id/value='{Case.PatientID}'"
            };
        }

        private AQLQuery MetaDataQuery(Case Case, PathogenParameter pathogen, List<UID> UIDs = null)
        {
            string uidMatch = "";
            if(UIDs != null)
            {
                uidMatch = " AND c/uid/value MATCHES {'" + string.Join("','", UIDs.Select(u => u.ID)) + "'} ";
            }
            return new AQLQuery()
            {
                Name = $"Meta Daten - {pathogen.MedicalField}",
                Query = @$"SELECT v/items[at0001]/value/value as CaseID,
                        c/context/other_context[at0001]/items[at0005]/value/value as Status,
                        d/data[at0001]/events[at0002]/time/value as ResultDateTime,
                        d/protocol[at0004]/items[at0094]/items[at0063]/value/id as OrderID,
                        c/uid/value as UID
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                        CONTAINS (CLUSTER v[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND OBSERVATION d[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]) 
                        WHERE c/name/value = '{pathogen.MedicalField}' 
                        AND e/ehr_status/subject/external_ref/id/value='{Case.PatientID}'
                        AND v/items[at0001]/value='{Case.CaseID}'
                        {uidMatch}
                        ORDER BY d/protocol[at0004]/items[at0094]/items[at0063]/value/id ASC"
            };
        }

        private AQLQuery RequirementQuery(RequirementParameter parameter)
        {
            return new AQLQuery()
            {
                Name = "Anforderungen",
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
