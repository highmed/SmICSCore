
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.OutbreakDetection.ReceiveModel;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.OutbreakDetection
{
    public class OutbreakDetectionParameterFactory : IOutbreakDetectionParameterFactory
    {
        private IRestDataAccess _restData;
        public OutbreakDetectionParameterFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public int[][] Process(OutbreakDetectionParameter parameter)
        {
            if(parameter.Retro)
            {
                EarliestMovement firstMove = _restData.AQLQueryAsync<EarliestMovement>(AQLCatalog.GetFirstMovementFromStation(parameter)).GetAwaiter().GetResult().FirstOrDefault();
                parameter.Starttime = firstMove.MinDate;
            }
            List<OutbreakDectectionPatient> patientList = _restData.AQLQueryAsync<OutbreakDectectionPatient>(GetPatientCaseList(parameter)).GetAwaiter().GetResult();
            int[] PositivCounts = GetPatientLabResults(patientList, parameter);
            int[] Epochs = GenerateEpochsArray(parameter);
            int[][] epochs_and_outbreaks = new int[][] { Epochs, PositivCounts };
            return epochs_and_outbreaks;
        }

        private int[] GetPatientLabResults(List<OutbreakDectectionPatient> patientList, OutbreakDetectionParameter parameter)
        {
            int[] FirstPositiveCounts = new int[(int)(parameter.Endtime - parameter.Starttime).TotalDays];

            foreach (OutbreakDectectionPatient pat in patientList)
            {
                if (parameter.MedicalField == MedicalField.VIROLOGY)
                { 
                    List<OutbreakDetectionLabResult> labResult = _restData.AQLQueryAsync<OutbreakDetectionLabResult>(GetPatientViroLabResultList(parameter, pat)).GetAwaiter().GetResult();
                    labResult = labResult.OrderBy(l => l.SpecimenCollectionDateTime).ToList();
                    OutbreakDetectionLabResult result = labResult.Where(l => l.SpecimenCollectionDateTime >= parameter.Starttime && l.Result == (int)SarsCovResult.POSITIVE).FirstOrDefault();
                    if (result != null)
                    {
                        FirstPositiveCounts[(int)(result.SpecimenCollectionDateTime - parameter.Starttime).TotalDays] += 1;
                    }
                }
                else if(parameter.MedicalField == MedicalField.MICROBIOLOGY)
                {
                    List<OutbreakDetectionLabResult> labResult = _restData.AQLQueryAsync<OutbreakDetectionLabResult>(GetPatientMibiLabResultList(parameter, pat)).GetAwaiter().GetResult();
                    foreach (OutbreakDetectionLabResult lab in labResult)
                    {
                        if (lab.SpecimenCollectionDateTime >= parameter.Starttime)
                        {
                            List<Antibiogram> antibiogram = _restData.AQLQueryAsync<Antibiogram>(GetAntibiogram(lab)).GetAwaiter().GetResult(); 
                            Pathogen pathogen = new Pathogen()
                            {
                                Name = parameter.PathogenIDs.First(),
                                Antibiograms = antibiogram
                            };
                            List<string> resistances = Rules.GetResistances(pathogen);
                            if (resistances.Contains(parameter.Resistance))
                            {
                                FirstPositiveCounts[(int)(lab.SpecimenCollectionDateTime - parameter.Starttime).TotalDays] += 1;
                                break;
                            }
                        }
                    }
                }
            }

            return FirstPositiveCounts;
        }

        private int[] GenerateEpochsArray(OutbreakDetectionParameter parameter)
        {
            int[] epochs = new int[(int)(parameter.Endtime - parameter.Starttime).TotalDays];
            int i = 0;
            DateTime referenceDate = new DateTime(1970, 1, 1);
            for (DateTime date = parameter.Starttime.Date; date < parameter.Endtime.Date; date = date.AddDays(1.0))
            {
                epochs[i] = (int)(date.Date - referenceDate.Date).TotalDays;
                i += 1;
            }
            return epochs;
        }

        public AQLQuery GetPatientCaseList(OutbreakDetectionParameter parameter)
        {
            return new AQLQuery()
            {
                Name = "OutbreakDetectionPatientCaseList",
                Query = $@"SELECT DISTINCT e/ehr_status/subject/external_ref/id/value as PatientID,
                        r/items[at0027]/value/value as Ward,
                        u/items[at0001]/value/value as CaseID
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                        CONTAINS (CLUSTER u[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]
                        CONTAINS CLUSTER r[openEHR-EHR-CLUSTER.location.v1]) 
                        WHERE c/name/value ='Patientenaufenthalt' 
                        AND u/items[at0001]/name/value = 'Zugehöriger Versorgungsfall (Kennung)'
                        AND Ward = '{parameter.Ward}' 
                        AND h/data[at0001]/items[at0004]/value/value < '{parameter.Endtime.ToString("yyyy-MM-dd")}' 
                        AND (h/data[at0001]/items[at0005]/value/value >= '{parameter.Starttime.ToString("yyyy-MM-dd")}' 
                        OR NOT EXISTS u/data[at0001]/items[at0005]/value/value)"
            };
        }
        private AQLQuery GetPatientMibiLabResultList(OutbreakDetectionParameter parameter, OutbreakDectectionPatient pat)
        {
            return new AQLQuery() 
            { 
                Name="OutbreakDetectionPatientLabResults",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                        a/items[at0015]/value/value as SpecimenCollectionDateTime,
                        v/items[at0024]/value/value as Result,
                        r/items[at0001]/value/value as CaseID,
                        c/uid/value as UID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS (CLUSTER r[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND OBSERVATION d[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]
                        CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.specimen.v1] AND 
                        CLUSTER v[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])) 
                        WHERE v/items[at0024]/name/value = 'Nachweis?' AND v/items[at0024]/value/value='Nachweis' 
                        AND e/ehr_status/subject/external_ref/id/value = '{pat.PatientID}' 
                        AND v/items[at0001,'Erregername']/value/value matches '{ parameter.PathogenIDs }'
                        AND c/context/other_context[at0001]/items[at0005]/value/value = 'Endbefund'
                        ORDER BY d/data[at0001]/events[at0002]/time/value ASC"
            };
        }
        public AQLQuery GetPatientViroLabResultList(OutbreakDetectionParameter parameter, OutbreakDectectionPatient pat)
        {
            return new AQLQuery("OutbreakDetectionPatientLabResults", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                                        y/items[at0001,'Nachweis']/value/defining_code/code_string as Result,
                                                        j/items[at0015]/value/value as SpecimenCollectionDateTime,
                                                        q/items[at0001]/value/value as CaseID
                                                        FROM EHR e
                                                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                                                        CONTAINS (CLUSTER q[openEHR-EHR-CLUSTER.case_identification.v0] AND OBSERVATION n[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]
                                                        CONTAINS (CLUSTER j[openEHR-EHR-CLUSTER.specimen.v1] AND CLUSTER y[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]))
                                                        WHERE c/name/value= 'Virologischer Befund' 
                                                        AND PatientID = '{pat.PatientID}' 
                                                        AND y/items[at0024,'Virusnachweistest']/value/defining_code/code_string matches {parameter.ToAQLMatchString()}
                                                        AND c/context/other_context[at0001]/items[at0005]/value/value = 'final'
                                                        AND Result = '260373001'");
        }
        private AQLQuery GetAntibiogram(OutbreakDetectionLabResult labResult)
        {
            return new AQLQuery()
            {
                Name = "OutbreakDetection - AntibiogramInformation",
                Query = $@"SELECT b/items[at0024,'Antibiotikum']/value/value as Antibiotic,
                            b/items[at0024,'Antibiotikum']/value/defining_code/code_string as AntibioticID,
                            b/items[at0004,'Resistenz']/value/value as Resistance,
                            b/items[at0001,'Minimale Hemmkonzentration']/value/magnitude as MinInhibitorConcentration,
                            b/items[at0001,'Minimale Hemmkonzentration']/value/units as MICUnit
                            FROM EHR e
                            CONTAINS COMPOSITION c
                            CONTAINS CLUSTER b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                            WHERE c/uid/value='{labResult.UID}'"
            };
        }
    }
}
