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

        public List<LabResult> Process(Patient patient, PathogenParameter pathogen)
        {
            List<LabResult> results = new List<LabResult>();
            List<Case> cases = RestDataAccess.AQLQuery<Case>(AQLCatalog.Cases(patient));
            foreach(Case c in cases)
            {
                List<LabResult> tmpResult = Process(c, pathogen.MedicalField, pathogen);
                if (tmpResult is not null)
                {
                    results.AddRange(tmpResult);
                }
            }
            return results;
        }

        public List<LabResult> Process(Patient patient, string MedicalField)
        {
            List<LabResult> results = new List<LabResult>();
            List<Case> cases = RestDataAccess.AQLQuery<Case>(AQLCatalog.Cases(patient));
            foreach (Case c in cases)
            {
                List<LabResult> tmpResult = Process(c, MedicalField);
                results.AddRange(tmpResult);
            }
            return results;
        }

        public List<LabResult> Process(Case Case, string MedicalField)
        {
            return Process(Case, MedicalField, null);
        }

        public List<LabResult> Process(Case Case, PathogenParameter pathogen)
        {
            return Process(Case, pathogen.MedicalField, pathogen);
        }

        private List<LabResult> Process(Case Case, string MedicalField, PathogenParameter pathogen = null)
        {
            List<UID> uids = null;
            if (pathogen != null)
            {
                uids = RestDataAccess.AQLQuery<UID>(GetPathogenCompositionsUIDs(Case, pathogen));
            }
            List<LabResult> results = RestDataAccess.AQLQuery<LabResult>(MetaDataQuery(Case, MedicalField, uids));
            if (results is not null)
            { 
                foreach (LabResult result in results)
                {
                    SpecimenParameter parameter = new SpecimenParameter() { UID = result.UID, MedicalField = MedicalField };
                    result.Specimens = _specimenFac.Process(parameter, pathogen);
                    result.Specimens.RemoveAll(spec => spec.Pathogens == null);
                    if (result.Specimens.Count > 0)
                    {
                        List<PatientLocation> patLocation = RestDataAccess.AQLQuery<PatientLocation>(AQLCatalog.PatientLocation(result.Specimens[0].SpecimenCollectionDateTime, Case.PatientID));
                        if(patLocation is not null)
                        {
                            result.Sender = patLocation.FirstOrDefault();
                        }
                        else 
                        {
                            result.Sender = new PatientLocation() { Departement = "N.A", Ward = "N.A"};
                        }
                    }
                }
                results.RemoveAll(lab => lab.Specimens.Count == 0);
            }
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
                            and m/items[at0001]/value/defining_code/code_string = {pathogen.PathogenCodesToAqlMatchString() }
                            and e/ehr_status/subject/external_ref/id/value='{Case.PatientID}'"
            };
        }

        private AQLQuery MetaDataQuery(Case Case, string MedicalField , List<UID> UIDs = null)
        {
            string uidMatch = "";
            if(UIDs != null)
            {
                uidMatch = " AND c/uid/value MATCHES {'" + string.Join("','", UIDs.Select(u => u.ID)) + "'} ";
            }
            return new AQLQuery()
            {
                Name = $"Meta Daten - {MedicalField}",
                Query = @$"SELECT v/items[at0001]/value/value as CaseID,
                        c/context/other_context[at0001]/items[at0005]/value/value as Status,
                        d/data[at0001]/events[at0002]/time/value as ResultDateTime,
                        d/protocol[at0004]/items[at0094]/items[at0063]/value/id as OrderID,
                        d/protocol[at0004]/items[at0094]/items[at0106]/value/value as Requirement,
                        c/uid/value as UID,
                        e/ehr_status/subject/external_ref/id/value as PatientID
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                        CONTAINS (CLUSTER v[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND OBSERVATION d[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]) 
                        WHERE c/name/value = '{MedicalField}' 
                        AND e/ehr_status/subject/external_ref/id/value='{Case.PatientID}'
                        AND v/items[at0001]/value='{Case.CaseID}'
                        {uidMatch}
                        ORDER BY d/protocol[at0004]/items[at0094]/items[at0063]/value/id ASC"
            };
        }
    }
}
