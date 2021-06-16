using SmICSCoreLib.AQL.Contact_Nth_Network;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Lab.EpiKurve;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten.ReceiveModel;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.Util;
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace SmICSCoreLib.AQL
{
    public sealed class AQLCatalog
    {
        private AQLCatalog() { }        
        public static AQLQuery GetEHRID(string subjectID)
        {
            return new AQLQuery("GetEHRID",$"SELECT DISTINCT e/ehr_id/value as PatientID FROM EHR e CONTAINS COMPOSITION c WHERE e/ehr_status/subject/external_ref/id/value='{subjectID}' and e/ehr_status/subject/external_ref/namespace='SmICSTests'");
        }
        public static AQLQuery ContactPatientWards(ContactParameter parameter)
        {
            return new AQLQuery("ContactPatientWards",$@"SELECT m/data[at0001]/items[at0004]/value/value as Beginn, 
                                m/data[at0001]/items[at0005]/value/value as Ende, 
                                o/items[at0024]/value/defining_code/code_string as Fachabteilung, 
                                k/items[at0027]/value/value as StationID 
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                CONTAINS ADMIN_ENTRY m[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                CONTAINS (CLUSTER k[openEHR-EHR-CLUSTER.location.v1] and CLUSTER o[openEHR-EHR-CLUSTER.organization.v0])
                                WHERE c/name/value='Patientenaufenthalt' 
                                and e/ehr_id/value = '{ parameter.PatientID }' 
                                and m/data[at0001]/items[at0004]/value/value <= '{ parameter.Endtime.ToString("o") }' 
                                and (m/data[at0001]/items[at0005]/value/value >= '{ parameter.Starttime.ToString("o") }'
                                or NOT EXISTS m/data[at0001]/items[at0005]/value/value)
                                ORDER BY m/data[at0001]/items[at0004]/value/value ASC");
        }
        public static AQLQuery ContactPatients(ContactPatientsParameter parameter)
        {
            return new AQLQuery("ContactPatients",$@"SELECT e/ehr_id/value as PatientID,
                                h/data[at0001]/items[at0004]/value/value as Beginn, 
                                h/data[at0001]/items[at0005]/value/value as Ende FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                CONTAINS ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                CONTAINS (CLUSTER l[openEHR-EHR-CLUSTER.location.v1] and CLUSTER o[openEHR-EHR-CLUSTER.organization.v0])
                                WHERE c/name/value='Patientenaufenthalt' 
                                and h/data[at0001]/items[at0004]/value/value <= '{ parameter.Endtime.ToString("o") }' 
                                and (h/data[at0001]/items[at0005]/value/value >= '{ parameter.Starttime.ToString("o") }'
                                or NOT EXISTS h/data[at0001]/items[at0005]/value/value)
                                and o/items[at0024]/value/defining_code/code_string = '{ parameter.Departement }' 
                                and l/items[at0027]/value/value = '{ parameter.WardID }' 
                                ORDER BY h/data[at0001]/items[at0004]/value/value");
        }
        public static AQLQuery ContactPatients_WithoutWardInformation(ContactPatientsParameter parameter)
        {
            return new AQLQuery("ContactPatients", $@"SELECT e/ehr_id/value as PatientID,
                                h/data[at0001]/items[at0004]/value/value as Beginn, 
                                h/data[at0001]/items[at0005]/value/value as Ende FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                CONTAINS ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                CONTAINS (CLUSTER l[openEHR-EHR-CLUSTER.location.v1] and CLUSTER o[openEHR-EHR-CLUSTER.organization.v0])
                                WHERE c/name/value='Patientenaufenthalt' 
                                and h/data[at0001]/items[at0004]/value/value <= '{ parameter.Endtime.ToString("o") }' 
                                and (h/data[at0001]/items[at0005]/value/value >= '{ parameter.Starttime.ToString("o") }'
                                or NOT EXISTS h/data[at0001]/items[at0005]/value/value)
                                and l/items[at0027]/value/value = '{ parameter.WardID }' 
                                ORDER BY h/data[at0001]/items[at0004]/value/value");
        }
        public static AQLQuery PatientStay(PatientListParameter patientList)
        {
            return new AQLQuery("PatientStay",$@"SELECT e/ehr_id/value as PatientID,
                                i/items[at0001]/value/value as FallID,
                                h/data[at0001]/items[at0004]/value/value as Beginn,
                                h/data[at0001]/items[at0005]/value/value as Ende,
                                h/data[at0001]/items[at0006]/value/value as Bewegungsart_l,
                                s/items[at0027]/value/value as StationID,
                                s/items[at0029]/value/value as Raum,
                                f/items[at0024]/value/value as Fachabteilung,
                                f/items[at0024]/value/defining_code/code_string as FachabteilungsID
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                    CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0]
                                    AND ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]
                                        CONTAINS (CLUSTER s[openEHR-EHR-CLUSTER.location.v1]
                                        AND CLUSTER f[openEHR-EHR-CLUSTER.organization.v0]))
                                WHERE c/name/value = 'Patientenaufenthalt'
                                AND i/items[at0001]/name/value = 'Zugehöriger Versorgungsfall (Kennung)'
                                AND e/ehr_id/value MATCHES {patientList.ToAQLMatchString()}
                                ORDER BY e/ehr_id/value ASC, h/data[at0001]/items[at0004]/value/value ASC");
        }

        public static AQLQuery PatientStayFromStation(PatientListParameter patientList, string station, DateTime starttime, DateTime endtime)
        {
            return new AQLQuery("PatientStay", $@"SELECT e/ehr_id/value as PatientID,
                                i/items[at0001]/value/value as FallID,
                                h/data[at0001]/items[at0004]/value/value as Beginn,
                                h/data[at0001]/items[at0005]/value/value as Ende,
                                h/data[at0001]/items[at0006]/value/value as Bewegungsart_l,
                                s/items[at0027]/value/value as StationID,
                                s/items[at0029]/value/value as Raum,
                                f/items[at0024]/value/value as Fachabteilung,
                                f/items[at0024]/value/defining_code/code_string as FachabteilungsID
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                    CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0]
                                    AND ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]
                                        CONTAINS (CLUSTER s[openEHR-EHR-CLUSTER.location.v1]
                                        AND CLUSTER f[openEHR-EHR-CLUSTER.organization.v0]))
                                WHERE c/name/value = 'Patientenaufenthalt'
                                AND i/items[at0001]/name/value = 'Zugehöriger Versorgungsfall (Kennung)'
                                AND h/data[at0001]/items[at0004]/value/value <= '{ endtime.ToString("o") }' 
                                and (h/data[at0001]/items[at0005]/value/value >= '{ starttime.ToString("o") }'
                                or NOT EXISTS h/data[at0001]/items[at0005]/value/value)
                                AND e/ehr_id/value MATCHES {patientList.ToAQLMatchString()}
                                AND Fachabteilung =  '{ station }'
                                ORDER BY e/ehr_id/value ASC, h/data[at0001]/items[at0004]/value/value ASC");
        }
        //public static AQLQuery PatientStayFromStation(PatientListParameter patientList, string station)
        //{
        //    return new AQLQuery("PatientStay", $@"SELECT e/ehr_id/value as PatientID,
        //                        i/items[at0001]/value/value as FallID,
        //                        h/data[at0001]/items[at0004]/value/value as Beginn,
        //                        h/data[at0001]/items[at0005]/value/value as Ende,
        //                        h/data[at0001]/items[at0006]/value/value as Bewegungsart_l,
        //                        s/items[at0027]/value/value as StationID,
        //                        s/items[at0029]/value/value as Raum,
        //                        f/items[at0024]/value/value as Fachabteilung,
        //                        f/items[at0024]/value/defining_code/code_string as FachabteilungsID
        //                        FROM EHR e 
        //                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
        //                            CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0]
        //                            AND ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]
        //                                CONTAINS (CLUSTER s[openEHR-EHR-CLUSTER.location.v1]
        //                                AND CLUSTER f[openEHR-EHR-CLUSTER.organization.v0]))
        //                        WHERE c/name/value = 'Patientenaufenthalt'
        //                        AND i/items[at0001]/name/value = 'Zugehöriger Versorgungsfall (Kennung)'
        //                        AND e/ehr_id/value MATCHES {patientList.ToAQLMatchString()}
        //                        AND Fachabteilung =  '{ station }'
        //                        ORDER BY e/ehr_id/value ASC, h/data[at0001]/items[at0004]/value/value ASC");
        //}


        public static AQLQuery PatientAdmission(EpsiodeOfCareParameter parameter)
        {
            return new AQLQuery("PatientAdmission", $@"SELECT p/data[at0001]/items[at0071]/value/value as Beginn
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                CONTAINS ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                                WHERE c/name/value = 'Stationärer Versorgungsfall' 
                                and e/ehr_id/value = '{ parameter.PatientID }' 
                                and c/context/other_context[at0001]/items[at0003]/value/value = '{ parameter.CaseID }'");
        }
        public static AQLQuery PatientDischarge(EpsiodeOfCareParameter parameter)
        {
            return new AQLQuery("PatientDischarge", $@"SELECT b/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Ende 
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                CONTAINS ADMIN_ENTRY b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0] 
                                WHERE c/name/value = 'Stationärer Versorgungsfall' 
                                and e/ehr_id/value = '{ parameter.PatientID }' 
                                and c/context/other_context[at0001]/items[at0003]/value/value = '{ parameter.CaseID }'");
        }
        public static AQLQuery PatientLaborData(PatientListParameter patientList)
        {
            return new AQLQuery("PatientLaborData",$@"SELECT e/ehr_id/value as PatientID,
                                    c/context/start_time/value as Befunddatum,
                                    y/items[at0001]/value/value as FallID,
                                    a/items[at0001]/value/id as LabordatenID,
                                    a/items[at0029]/value/defining_code/code_string as MaterialID,
                                    a/items[at0029]/value/value as Material_l,
                                    a/items[at0015]/value/value as ZeitpunktProbenentnahme,
                                    a/items[at0034]/value/value as ZeitpunktProbeneingang,
                                    d/items[at0024]/value/value as Keim_l,
                                    d/items[at0024]/value/defining_code/code_string as KeimID,
                                    d/items[at0001,'Nachweis']/value/value as Befund,
                                    d/items[at0001,'Nachweis']/value/defining_code/code_string as BefundCode,
                                    d/items[at0001,'Quantitatives Ergebnis']/value/magnitude as Viruslast,
                                    l/data[at0001]/events[at0002]/data[at0003]/items[at0101]/value/value as Befundkommentar
                                    FROM EHR e
                                    CONTAINS COMPOSITION c 
                                        CONTAINS (CLUSTER y[openEHR-EHR-CLUSTER.case_identification.v0] 
                                        and OBSERVATION l[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]
                                            CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.specimen.v1]
                                            and CLUSTER b[openEHR-EHR-CLUSTER.laboratory_test_panel.v0]
                                                CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])))
                                    WHERE c/name/value = 'Virologischer Befund'
                                    AND e/ehr_id/value MATCHES { patientList.ToAQLMatchString() }
                                    ORDER BY a/items[at0015]/value/value ASC");
        }
        public static AQLQuery NECPatientLaborData(string PatientID, TimespanParameter timespan)
        {
            //TODO: AQL musst zu eine MIBI AQL umgewandelt werden. 
            return new AQLQuery("NECPatientLaborData",$"SELECT e/ehr_id/value as PatientID, z/items[at0015]/value/value as ZeitpunktProbenentnahme, z/items[at0029]/value/defining_code/code_string/value as MaterialID, g/items[at0001]/value/value as Befund, g/items[at0024]/value/defining_code/code_string as KeimID, c/context/start_time/value as Befunddatum FROM EHR e CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1] CONTAINS (CLUSTER f[openEHR-EHR-CLUSTER.case_identification.v0] and CLUSTER z[openEHR-EHR-CLUSTER.specimen.v1] and CLUSTER j[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] CONTAINS CLUSTER g[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) WHERE c/name/value = 'Virologischer Befund' and g/items[at0001]/name = 'Nachweis' and g/items[at0024]/name = 'Virus' and e/ehr_id/value = '{PatientID}' and z/items[at0015]/value/value >= '{timespan.Starttime}' and z/items[at0015]/value/value <= '{timespan.Endtime}'");
        }
        public static AQLQuery LaborEpiCurve(DateTime date, EpiCurveParameter parameter)
        {
            return new AQLQuery("LaborEpiCurve",$@"SELECT e/ehr_id/value as PatientID,
                                   i/items[at0001]/value/value as FallID,
                                   d/items[at0001]/value/defining_code/code_string as Flag,
                                   d/items[at0024]/value/defining_code/code_string as VirusCode,
                                   d/items[at0024]/value/value as Virus,
                                   m/items[at0015]/value/value as Datum
                            FROM EHR e
                            CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                                CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                                and OBSERVATION v[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                    CONTAINS (CLUSTER m[openEHR-EHR-CLUSTER.specimen.v1] 
                                    and CLUSTER s[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                                        CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])))
                            WHERE c/name/value='Virologischer Befund' 
                            and d/items[at0001]/name/value='Nachweis' 
                            and d/items[at0024]/value/defining_code/code_string MATCHES { parameter.PathogenCodesToAqlMatchString() }
                            and m/items[at0015]/value/value>='{ date.ToString("yyyy-MM-dd") }' and m/items[at0015]/value/value<'{ date.AddDays(1).ToString("yyyy-MM-dd") }'");
        }
        public static AQLQuery PatientLocation(DateTime date, string patientID)
        {
            return new AQLQuery("PatientLocation",$@"SELECT a/items[at0027]/value/value as Ward, o/items[at0024]/value/defining_code/code_string as Departement 
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                CONTAINS ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.location.v1]
                                and CLUSTER o[openEHR-EHR-CLUSTER.organization.v0])
                                WHERE c/name/value = 'Patientenaufenthalt' and 
                                e/ehr_id/value = '{ patientID }' and 
                                u/data[at0001]/items[at0004]/value/value <= '{ date.ToString("O") }' and 
                                (u/data[at0001]/items[at0005]/value/value >= '{ date.ToString("O") }' or NOT EXISTS u/data[at0001]/items[at0005]/value/value)
                                ORDER BY u/data[at0001]/items[at0004]/value/value ASC");
        }
        public static AQLQuery GetAllPatients(DateTime date) 
        {
            return new AQLQuery("GetAllPatients",$"Select e/ehr_id/value as PatientID FROM EHR e CONTAINS COMPOSITION c CONTAINS ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] WHERE c/name/value='Patientenaufenthalt' and u/data[at0001]/items[at0004]/value/value = '{date.ToString("yyyy-MM-dd")}'");
        }
        public static AQLQuery CasesWithResults(PatientListParameter patientList)
        {
            return new AQLQuery("CasesWithResults",$"SELECT DISTINCT e/ehr_id/value as PatientID, m/items[at0001]/value/value AS FallID, c/uid/value as UID FROM EHR e CONTAINS COMPOSITION c CONTAINS CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] WHERE c/name/value = 'Mikrobiologischer Befund' AND e/ehr_id/value matches { patientList.ToAQLMatchString() } order by m/items[at0001]/value/value asc");
        }
        public static AQLQuery ReportMeta(CaseIDReceiveModel caseID)
        {
            return new AQLQuery("ReportMeta",$"SELECT DISTINCT e/ehr_id/value as PatientID, m/items[at0001]/value/value as FallID, c/uid/value as UID FROM EHR e CONTAINS COMPOSITION c CONTAINS CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] WHERE e/ehr_id/value = '{ caseID.PatientID }' and m/items[at0001]/value/value = '{ caseID.FallID }' ORDER BY o/data[at0001]/events[at0002]/time/value DESC");
        }
        public static AQLQuery Requirements(MetaDataReceiveModel metaData)
        {
            return new AQLQuery("Requirements",$"select distinct a/protocol[at0004]/items[at0094]/items[at0106]/value/value as anforderung from EHR e contains COMPOSITION c contains (CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] and OBSERVATION a[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]) where (e/ehr_id/value='{ metaData.PatientID }' and c/uid/value = '{ metaData.UID }' and m/items[at0001]/value/value = '{ metaData.FallID }' )");
        }
        public static AQLQuery SamplesFromResult(MetaDataReceiveModel metaData)
        {
            return new AQLQuery("SamplesFromResult", $"SELECT b/items[at0029]/value/value as MaterialID, b/items[at0001]/value/id as LabordatenID, b/items[at0015]/value/value as ZeitpunktProbeentnahme, b/items[at0034]/value/value as ZeitpunktProbeneingang FROM EHR e CONTAINS COMPOSITION c CONTAINS (CLUSTER w[openEHR-EHR-CLUSTER.case_identification.v0] and CLUSTER b[openEHR-EHR-CLUSTER.specimen.v1] CONTAINS CLUSTER z[openEHR-EHR-CLUSTER.anatomical_location.v1]) where e/ehr_id/value = '{ metaData.PatientID }' and c/uid/value = '{ metaData.UID }' and  w/items[at0001]/value/value = '{ metaData.FallID }' order by b/items[at0015]/value/value desc");
        }

        //items[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]/items[openEHR-EHR-CLUSTER.erregerdetails.v1]/items[at0018]
        public static AQLQuery PathogensFromResult(MetaDataReceiveModel metaData, SampleReceiveModel sampleData)
        {
            return new AQLQuery("PathogensFromResult", $"SELECT distinct d/items[at0001]/value/value as KeimID, d/items[at0027]/value/magnitude as IsolatNo, d/items[at0024]/value/value as Befund, z/items[at0018]/value/value as MREKlasse, d/items[at0003]/value/value as Befundkommentar FROM EHR e CONTAINS COMPOSITION c CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] and OBSERVATION j[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] CONTAINS (CLUSTER q[openEHR-EHR-CLUSTER.specimen.v1] and CLUSTER p[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] CONTAINS CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] CONTAINS CLUSTER z[openEHR-EHR-CLUSTER.erregerdetails.v1])) WHERE d/items[at0001]/name/value = 'Erregername' and d/items[at0024]/name/value='Nachweis?' and d/items[at0027]/name/value = 'Isolatnummer' and e/ehr_id/value = '{ metaData.PatientID }' and c/uid/value = '{ metaData.UID }' and i/items[at0001]/value/value = '{ metaData.FallID }' and q/items[at0001]/value/id = '{ sampleData.LabordatenID }'");
        }
        public static AQLQuery PatientSymptom_VS(PatientListParameter patientList)
        {
            return new AQLQuery("PatientSymptom_VS",$"SELECT e/ehr_id/value as PatientenID, a/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, a/data[at0190]/events[at0191]/data[at0192/items[at0151]/value/value as Lokalisation, a/ data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value as Beginn, a/ data[at0190]/events[at0191]/data[at0192]/items[at0021]/value/value as Schweregrad, a/ data[at0190]/events[at0191]/data[at0192]/items[at0161] as Rueckgang FROM EHR e CONTAINS COMPOSITION c CONTAINS OBSERVATION a[openEHR-EHR-OBSERVATION.symptom_sign.v0] WHERE c/archetype_details/template_id='Symptom' and e/ehr_id/value matches { patientList.ToAQLMatchString() }");
        }
        public static AQLQuery PatientSymptom_AS(PatientListParameter patientList)
        {
            return new AQLQuery("PatientSymptom_AS",$"SELECT e/ehr_id/value as PatientenID, a/data[at0001]/items[at0002]/value/value as AusschlussAussage, a/ data[at0001]/items[at0003]/value/value as Diagnose FROM EHR e CONTAINS COMPOSITION c CONTAINS EVALUATION a[openEHR-EHR-EVALUATION.exclusion_specific.v1] WHERE c/archetype_details/template_id='Symptom' and e/ehr_id/value matches { patientList.ToAQLMatchString() }");
        }

        public static AQLQuery PatientSymptom_US(PatientListParameter patientList)
        {
            return new AQLQuery("PatientSymptom_US",$"SELECT e/ehr_id/value as PatientenID, a/data[at0001]/items[at0002]/value/value as UnbekanntesSymptom, a/ data[at0001]/items[at0005]/value/value as AussageFehlendeInfo FROM EHR e CONTAINS COMPOSITION c CONTAINS EVALUATION a[openEHR-EHR-EVALUATION.absence.v2] WHERE c/archetype_details/template_id='Symptom' and e/ehr_id/value matches { patientList.ToAQLMatchString() }");
        }
        //untested
        public static AQLQuery AntibiogramFromPathogen(MetaDataReceiveModel metaData, SampleReceiveModel sampleData, PathogenReceiveModel pathogenData)
        {
            return new AQLQuery("AntibiogramFromPathogen", $"SELECT w/feeder_audit/originating_system_audit/time/value as erregerZeit, b/items[at0024]/value/value as antibiotikum, b/items[at0004]/value/defining_code/code_string as resistenz, b/items[at0001]/value/magnitude as mhkMagnitude, b/items[at0001]/value/magnitude_status as mhkMagnitudeStatus, b/items[at0001]/value/units as mhkUnits, u/feeder_audit/originating_system_audit/time/value as antibiogrammZeit, b/feeder_audit/original_content/value as original FROM EHR e CONTAINS COMPOSITION c contains (CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] and OBSERVATION j[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] CONTAINS CLUSTER w[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] CONTAINS CLUSTER z[openEHR-EHR-CLUSTER.erregerdetails.v1] CONTAINS CLUSTER u[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] CONTAINS CLUSTER b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) where e/ehr_id/value = '{ metaData.PatientID }' and c/uid/value = '{ metaData.UID }' and  w/items[at0001]/value/value = '{ metaData.FallID }' and erregerName = '{ pathogenData.KeimID }' and isolatNummer = '{ pathogenData.IsolatNo }' and w/items[at0001]/name='Erregername' and b/items[at0024]/name='Antibiotikum' order by b/items[at0024]/value/value asc");
        }

        public static AQLQuery Stationary(string patientId, string fallkennung, DateTime datum)
        {
            return new AQLQuery("Stationary", $"SELECT e/ehr_id/value as PatientID, " +
                                $"c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value  as FallID, " +
                                $"r/data[at0001]/items[at0071]/value/value  as Datum_Uhrzeit_der_Aufnahme, " +
                                $"p/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Datum_Uhrzeit_der_Entlassung, " +
                                $"r/data[at0001]/items[at0049,'Aufnahmeanlass']/value/value as Aufnahmeanlass, " +
                                $"p/data[at0001]/items[at0040]/value/value as Art_der_Entlassung, " +
                                $"r/data[at0001]/items[at0013]/value/value as Versorgungsfallgrund " +
                                $"FROM EHR e " +
                                $"CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] " +
                                $"CONTAINS (ADMIN_ENTRY r[openEHR-EHR-ADMIN_ENTRY.admission.v0] CONTAINS (CLUSTER w[openEHR-EHR-CLUSTER.location.v1]) and ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0])  " +
                                $"where PatientID='{patientId}' and  " +
                                $"Datum_Uhrzeit_der_Aufnahme <= '{datum.Date.AddDays(-4).ToString("yyyy-MM-dd")}' and " +
                                $"FallID='{fallkennung}'");
        }

        public static AQLQuery Case(DateTime date)
        {
            return new AQLQuery("Case",$"SELECT COUNT(DISTINCT e/ehr_id/value) as Anzahl_Faelle FROM EHR e CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1] CONTAINS (CLUSTER x[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER p[openEHR-EHR-CLUSTER.specimen.v1]) WHERE c/name/value='Virologischer Befund' and x/items[at0001,'Nachweis']/value/value='positiv' and x/items[at0024,'Virus']/value/value='SARS-Cov-2' and p/items[at0015]/value/value = '{date.Date.ToString("yyyy-MM-dd")}'");
        }

        public static AQLQuery WeekCase(DateTime startDate, DateTime endDate)
        {
            return new AQLQuery("WeekCase",$"SELECT COUNT(DISTINCT e/ehr_id/value) as Anzahl_Faelle" +
                                $" FROM EHR e " +
                                $"CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1] " +
                                $"CONTAINS (CLUSTER x[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and " +
                                $"CLUSTER p[openEHR-EHR-CLUSTER.specimen.v1]) " +
                                $"WHERE c/name/value='Virologischer Befund' and " +
                                $"x/items[at0001,'Nachweis']/value/value='positiv' and" +
                                $" x/items[at0024,'Virus']/value/value='SARS-Cov-2' and " +
                                $"p/items[at0015]/value/value >= '{startDate.Date.ToString("yyyy-MM-dd")}' and " +
                                $"p/items[at0015]/value/value <= '{endDate.Date.ToString("yyyy-MM-dd")}' ");
        }

        public static AQLQuery SymptomsByPatient(string patientId, DateTime datum)
        {
            return new AQLQuery("SymptomsByPatient", $@"SELECT e/ehr_id/value as PatientenID, 
                                k/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                k/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value as Beginn, 
                                k/data[at0190]/events[at0191]/data[at0192]/items[at0161]/value/value as Rueckgang " +
                                $"FROM EHR e " +
                                $"CONTAINS COMPOSITION c " +
                                $"CONTAINS OBSERVATION k[openEHR-EHR-OBSERVATION.symptom_sign.v0] " +
                                $"where PatientenID='{patientId}' " +
                                $"and Beginn like '{datum.Date.ToString("yyyy-MM-dd").Insert(10,"*")}'");
        }
       
        public static AQLQuery StayFromCase(string patientId, string fallId)
        {
            return new AQLQuery("StayFromCase", $@"SELECT e/ehr_id/value as PatientID,
                                 c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value as FallID,
                                 u/data[at0001]/items[at0013]/value/value as Versorgungsfallgrund,
                                 u/data[at0001]/items[at0049,'Aufnahmeanlass']/value/value as Aufnahmeanlass,
                                 u/data[at0001]/items[at0071]/value/value as Datum_Uhrzeit_der_Aufnahme,
                                 f/data[at0001]/items[at0040]/value/value as Art_der_Entlassung,
                                 f/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Datum_Uhrzeit_der_Entlassung,
                                 d/items[at0027]/value/value as Station
                                 FROM EHR e
                                 CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1]
                                 CONTAINS (ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.admission.v0] and ADMIN_ENTRY f[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0] and CLUSTER d[openEHR-EHR-CLUSTER.location.v1])
                                 WHERE PatientID='{patientId}' and FallID='{fallId}'"
                
                
                
                //"StayFromCase", $@"SELECT e/ehr_id/value as PatientID, 
                //                    c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value as FallID,
                //                    j/data[at0001]/items[at0071]/value/value  as Datum_Uhrzeit_der_Aufnahme,
                //                    j/data[at0001]/items[at0013]/value/value as Versorgungsfallgrund, 
                //                    j/data[at0001]/items[at0049,'Aufnahmeanlass']/value/value as Aufnahmeanlass,
                //                    w/data[at0001]/items[at0040]/value/value as Art_der_Entlassung, 
                //                    w/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Datum_Uhrzeit_der_Entlassung,
                //                    i/items[at0027]/value/value as Station
                //                    FROM EHR e 
                //                    CONTAINS COMPOSITION c#Stationärer_Versorgungsfall 
                //                    CONTAINS (ADMIN_ENTRY j#Aufnahmedaten CONTAINS (CLUSTER i#Vorheriger_Patientenstandort__vor_Aufnahme_) and 
                //                    ADMIN_ENTRY w#Entlassungsdaten) 
                //                    WHERE PatientID='{patientId}' and FallID='{fallId}'"
                                    
                                    
                                    
                                    );
        }

        public static AQLQuery StayFromDate(DateTime datum)
        {
            return new AQLQuery("StayFromCase", $@"SELECT e/ehr_id/value as PatientID, 
                                    c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value as FallID,
                                    j/data[at0001]/items[at0071]/value/value  as Datum_Uhrzeit_der_Aufnahme,
                                    j/data[at0001]/items[at0013]/value/value as Versorgungsfallgrund, 
                                    j/data[at0001]/items[at0049,'Aufnahmeanlass']/value/value as Aufnahmeanlass,
                                    w/data[at0001]/items[at0040]/value/value as Art_der_Entlassung, 
                                    w/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Datum_Uhrzeit_der_Entlassung
                                    FROM EHR e 
                                    CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                    CONTAINS (ADMIN_ENTRY j[openEHR-EHR-ADMIN_ENTRY.admission.v0] and 
                                    ADMIN_ENTRY w[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) 
                                    WHERE Datum_Uhrzeit_der_Aufnahme >= '{datum.Date.ToString("yyyy-MM-dd").Insert(10, "*")}'");
        }

        public static AQLQuery CovidPat(string nachweis)
        {          
            return new AQLQuery("CovidPat", $@"SELECT e/ehr_id/value as PatientID, 
                                i/items[at0001]/value/value as Fallkennung, 
                                m/items[at0034]/value/value as Zeitpunkt_des_Probeneingangs 
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] and 
                                OBSERVATION z[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER m [openEHR-EHR-CLUSTER.specimen.v1])) 
                                WHERE  a/items[at0001,'Nachweis']/value/defining_code/code_string='{nachweis}'and 
                                a/items[at0024]/value/defining_code/code_string MATCHES {{'94500-6','94558-4', '94745-7'}} ORDER BY Zeitpunkt_des_Probeneingangs ASC");
        }

        public static AQLQuery PatientBySymptom(string symptom)
        {
            return new AQLQuery("PatientBySymptom", $@"SELECT e/ehr_id/value as PatientenID, 
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value as Beginn, 
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0161]/value/value as Rueckgang 
                                FROM EHR e 
                                CONTAINS COMPOSITION c [openEHR-EHR-COMPOSITION.registereintrag.v1] 
                                CONTAINS OBSERVATION t[openEHR-EHR-OBSERVATION.symptom_sign.v0] 
                                WHERE c/name/value='COVID-19 Symptom' and  
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value = '{symptom}'");
        }

        public static AQLQuery PatientSymptom()
        {
            return new AQLQuery("PatientBySymptom", $@"SELECT a/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                COUNT(e/ehr_id/value) as Anzahl_Patienten 
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS OBSERVATION a[openEHR-EHR-OBSERVATION.symptom_sign.v0] 
                                WHERE c/archetype_details/template_id='Symptom' GROUP BY NameDesSymptoms");
        }

    }
}
 