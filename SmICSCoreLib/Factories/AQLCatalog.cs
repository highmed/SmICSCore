using SmICSCoreLib.Factories.ContactNetwork;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.EpiCurve;
using SmICSCoreLib.Factories.LabData.MibiLabdata.ReceiveModel;
using SmICSCoreLib.Factories.PatientMovement;
using System;
using SmICSCoreLib.Factories.OutbreakDetection;
using SmICSCoreLib.Factories.OutbreakDetection.ReceiveModel;

namespace SmICSCoreLib.Factories
{
    public sealed class AQLCatalog
    {
        private AQLCatalog() { }        
        public static AQLQuery GetEHRID(string subjectID)
        {
            return new AQLQuery("GetEHRID",$@"SELECT DISTINCT e/ehr_id/value as ID 
                                FROM EHR e CONTAINS COMPOSITION c 
                                WHERE e/ehr_status/subject/external_ref/id/value='{subjectID}' 
                                AND e/ehr_status/subject/external_ref/namespace='SmICSTests'");
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
                                AND e/ehr_status/subject/external_ref/id/value = '{ parameter.PatientID }' 
                                AND m/data[at0001]/items[at0004]/value/value <= '{ parameter.Endtime.ToString("o") }' 
                                AND (m/data[at0001]/items[at0005]/value/value >= '{ parameter.Starttime.ToString("o") }'
                                OR NOT EXISTS m/data[at0001]/items[at0005]/value/value)
                                ORDER BY m/data[at0001]/items[at0004]/value/value ASC");
        }
        public static AQLQuery ContactPatients(ContactPatientsParameter parameter)
        {
            return new AQLQuery("ContactPatients",$@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                h/data[at0001]/items[at0004]/value/value as Beginn, 
                                h/data[at0001]/items[at0005]/value/value as Ende FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                CONTAINS ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                CONTAINS (CLUSTER l[openEHR-EHR-CLUSTER.location.v1] and CLUSTER o[openEHR-EHR-CLUSTER.organization.v0])
                                WHERE c/name/value='Patientenaufenthalt' 
                                AND h/data[at0001]/items[at0004]/value/value <= '{ parameter.Endtime.ToString("o") }' 
                                AND (h/data[at0001]/items[at0005]/value/value >= '{ parameter.Starttime.ToString("o") }'
                                OR NOT EXISTS h/data[at0001]/items[at0005]/value/value)
                                AND o/items[at0024]/value/defining_code/code_string = '{ parameter.Departement }' 
                                AND l/items[at0027]/value/value = '{ parameter.WardID }' 
                                ORDER BY h/data[at0001]/items[at0004]/value/value");
        }
        public static AQLQuery ContactPatients_WithoutWardInformation(ContactPatientsParameter parameter)
        {
            return new AQLQuery("ContactPatients", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                h/data[at0001]/items[at0004]/value/value as Beginn, 
                                h/data[at0001]/items[at0005]/value/value as Ende FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                CONTAINS ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                CONTAINS (CLUSTER l[openEHR-EHR-CLUSTER.location.v1] and CLUSTER o[openEHR-EHR-CLUSTER.organization.v0])
                                WHERE c/name/value='Patientenaufenthalt' 
                                AND h/data[at0001]/items[at0004]/value/value <= '{ parameter.Endtime.ToString("o") }' 
                                AND (h/data[at0001]/items[at0005]/value/value >= '{ parameter.Starttime.ToString("o") }'
                                OR NOT EXISTS h/data[at0001]/items[at0005]/value/value)
                                AND l/items[at0027]/value/value = '{ parameter.WardID }' 
                                ORDER BY h/data[at0001]/items[at0004]/value/value");
        }
        public static AQLQuery PatientStay(PatientListParameter patientList)
        {
            return new AQLQuery("PatientStay",$@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                AND e/ehr_status/subject/external_ref/id/value MATCHES {patientList.ToAQLMatchString()}
                                ORDER BY e/ehr_status/subject/external_ref/id/value ASC, h/data[at0001]/items[at0004]/value/value ASC");
        }

        public static AQLQuery PatientStayFromStation(PatientListParameter patientList, string station, DateTime starttime, DateTime endtime)
        {
            return new AQLQuery("PatientStay", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                AND (h/data[at0001]/items[at0005]/value/value >= '{ starttime.ToString("o") }'
                                OR NOT EXISTS h/data[at0001]/items[at0005]/value/value)
                                AND e/ehr_status/subject/external_ref/id/value MATCHES {patientList.ToAQLMatchString()}
                                AND s/items[at0027]/value/value = '{ station }'
                                ORDER BY e/ehr_status/subject/external_ref/id/value ASC, h/data[at0001]/items[at0004]/value/value ASC");
        }
      

        public static AQLQuery PatientAdmission(EpsiodeOfCareParameter parameter)
        {
            return new AQLQuery("PatientAdmission", $@"SELECT p/data[at0001]/items[at0071]/value/value as Beginn
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                CONTAINS ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                                WHERE c/name/value = 'Stationärer Versorgungsfall' 
                                AND e/ehr_status/subject/external_ref/id/value = '{ parameter.PatientID }' 
                                AND c/context/other_context[at0001]/items[at0003]/value/value = '{ parameter.CaseID }'");
        }
        public static AQLQuery PatientDischarge(EpsiodeOfCareParameter parameter)
        {
            return new AQLQuery("PatientDischarge", $@"SELECT b/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Ende 
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                CONTAINS ADMIN_ENTRY b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0] 
                                WHERE c/name/value = 'Stationärer Versorgungsfall' 
                                AND e/ehr_status/subject/external_ref/id/value = '{ parameter.PatientID }' 
                                AND c/context/other_context[at0001]/items[at0003]/value/value = '{ parameter.CaseID }'");
        }
        public static AQLQuery PatientLaborData(PatientListParameter patientList)
        {
            return new AQLQuery("PatientLaborData",$@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                AND OBSERVATION l[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]
                                CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.specimen.v1]
                                AND CLUSTER b[openEHR-EHR-CLUSTER.laboratory_test_panel.v0]
                                CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])))
                                WHERE c/name/value = 'Virologischer Befund'
                                AND e/ehr_status/subject/external_ref/id/value MATCHES { patientList.ToAQLMatchString() }
                                ORDER BY a/items[at0015]/value/value ASC");
        }
        public static AQLQuery NECPatientLaborData(string PatientID, TimespanParameter timespan)
        {
            //TODO: AQL musst zu eine MIBI AQL umgewandelt werden. 
            return new AQLQuery("NECPatientLaborData",$@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                z/items[at0015]/value/value as ZeitpunktProbenentnahme,
                                z/items[at0029]/value/defining_code/code_string/value as MaterialID, 
                                g/items[at0001]/value/value as Befund, 
                                g/items[at0024]/value/defining_code/code_string as KeimID,
                                c/context/start_time/value as Befunddatum 
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1] 
                                CONTAINS (CLUSTER f[openEHR-EHR-CLUSTER.case_identification.v0]
                                AND CLUSTER z[openEHR-EHR-CLUSTER.specimen.v1] 
                                AND CLUSTER j[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                                CONTAINS CLUSTER g[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) 
                                WHERE c/name/value = 'Virologischer Befund' 
                                AND g/items[at0001]/name = 'Nachweis' 
                                AND g/items[at0024]/name = 'Virus' 
                                AND e/ehr_status/subject/external_ref/id/value = '{PatientID}'
                                AND z/items[at0015]/value/value >= '{timespan.Starttime}' 
                                AND z/items[at0015]/value/value <= '{timespan.Endtime}'");
        }
        public static AQLQuery LaborEpiCurve(DateTime date, EpiCurveParameter parameter)
        {
            return new AQLQuery("LaborEpiCurve",$@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                i/items[at0001]/value/value as FallID,
                                d/items[at0001]/value/defining_code/code_string as Flag,
                                d/items[at0024]/value/defining_code/code_string as VirusCode,
                                d/items[at0024]/value/value as Virus,
                                m/items[at0015]/value/value as Datum
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                                CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION v[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS (CLUSTER m[openEHR-EHR-CLUSTER.specimen.v1] 
                                AND CLUSTER s[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                                CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])))
                                WHERE c/name/value='Virologischer Befund' 
                                AND d/items[at0001]/name/value='Nachweis' 
                                AND d/items[at0024]/value/defining_code/code_string MATCHES { parameter.PathogenCodesToAqlMatchString() }
                                AND m/items[at0015]/value/value>='{ date.ToString("yyyy-MM-dd") }' 
                                AND m/items[at0015]/value/value<'{ date.AddDays(1).ToString("yyyy-MM-dd") }'");
        }
        public static AQLQuery PatientLocation(DateTime date, string patientID)
        {
            return new AQLQuery("PatientLocation",$@"SELECT a/items[at0027]/value/value as Ward, 
                                o/items[at0024]/value/defining_code/code_string as Departement 
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                                CONTAINS ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.location.v1]
                                AND CLUSTER o[openEHR-EHR-CLUSTER.organization.v0])
                                WHERE c/name/value = 'Patientenaufenthalt' 
                                AND e/ehr_status/subject/external_ref/id/value = '{ patientID }' 
                                AND u/data[at0001]/items[at0004]/value/value <= '{ date.ToString("O") }' 
                                AND (u/data[at0001]/items[at0005]/value/value >= '{ date.ToString("O") }' 
                                OR NOT EXISTS u/data[at0001]/items[at0005]/value/value)
                                ORDER BY u/data[at0001]/items[at0004]/value/value ASC");
        }
        public static AQLQuery GetAllPatients(DateTime date) 
        {
            return new AQLQuery("GetAllPatients",$@"Select e/ehr_status/subject/external_ref/id/value as PatientID 
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                WHERE c/name/value='Patientenaufenthalt' 
                                and u/data[at0001]/items[at0004]/value/value = '{date.ToString("yyyy-MM-dd")}'");
        }
        public static AQLQuery CasesWithResults(PatientListParameter patientList)
        {
            return new AQLQuery("CasesWithResults",$@"SELECT DISTINCT e/ehr_status/subject/external_ref/id/value as PatientID,
                                m/items[at0001]/value/value AS FallID,
                                c/uid/value as UID  
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] 
                                WHERE c/name/value = 'Mikrobiologischer Befund' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() } 
                                ORDER by m/items[at0001]/value/value asc");
        }
        public static AQLQuery ReportMeta(CaseIDReceiveModel caseID)
        {
            return new AQLQuery("ReportMeta",$@"SELECT DISTINCT e/ehr_status/subject/external_ref/id/value as PatientID,
                                m/items[at0001]/value/value as FallID, 
                                c/uid/value as UID 
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] 
                                WHERE e/ehr_status/subject/external_ref/id/value = '{ caseID.PatientID }' 
                                AND m/items[at0001]/value/value = '{ caseID.FallID }' 
                                ORDER BY o/data[at0001]/events[at0002]/time/value DESC");
        }
        public static AQLQuery Requirements(MetaDataReceiveModel metaData)
        {
            return new AQLQuery("Requirements",$@"select distinct a/protocol[at0004]/items[at0094]/items[at0106]/value/value as anforderung 
                                FROM EHR e 
                                CONTAINS COMPOSITION c contains (CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION a[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]) 
                                WHERE (e/ehr_status/subject/external_ref/id/value='{ metaData.PatientID }'
                                AND c/uid/value = '{ metaData.UID }' 
                                AND m/items[at0001]/value/value = '{ metaData.FallID }' )");
        }
        public static AQLQuery SamplesFromResult(MetaDataReceiveModel metaData)
        {
            return new AQLQuery("SamplesFromResult", $@"SELECT b/items[at0029]/value/value as MaterialID
                                b/items[at0001]/value/id as LabordatenID,
                                b/items[at0015]/value/value as ZeitpunktProbeentnahme,
                                b/items[at0034]/value/value as ZeitpunktProbeneingang 
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS (CLUSTER w[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND CLUSTER b[openEHR-EHR-CLUSTER.specimen.v1] 
                                CONTAINS CLUSTER z[openEHR-EHR-CLUSTER.anatomical_location.v1])
                                WHERE e/ehr_status/subject/external_ref/id/value = '{ metaData.PatientID }'
                                AND c/uid/value = '{ metaData.UID }' 
                                AND  w/items[at0001]/value/value = '{ metaData.FallID }' 
                                ORDER BY b/items[at0015]/value/value desc");
        }

        //items[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]/items[openEHR-EHR-CLUSTER.erregerdetails.v1]/items[at0018]
        public static AQLQuery PathogensFromResult(MetaDataReceiveModel metaData, SampleReceiveModel sampleData)
        {
            return new AQLQuery("PathogensFromResult", $@"SELECT distinct d/items[at0001]/value/value as KeimID,
                                d/items[at0027]/value/magnitude as IsolatNo,
                                d/items[at0024]/value/value as Befund, 
                                z/items[at0018]/value/value as MREKlasse,
                                d/items[at0003]/value/value as Befundkommentar 
                                FROM EHR e 
                                CONTAINS COMPOSITION c 
                                CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION j[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS (CLUSTER q[openEHR-EHR-CLUSTER.specimen.v1] 
                                AND CLUSTER p[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                                CONTAINS CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                                CONTAINS CLUSTER z[openEHR-EHR-CLUSTER.erregerdetails.v1])) 
                                WHERE d/items[at0001]/name/value = 'Erregername' and d/items[at0024]/name/value='Nachweis?'
                                AND d/items[at0027]/name/value = 'Isolatnummer' 
                                AND e/ehr_status/subject/external_ref/id/value = '{ metaData.PatientID }'
                                AND c/uid/value = '{ metaData.UID }' 
                                AND i/items[at0001]/value/value = '{ metaData.FallID }' 
                                AND q/items[at0001]/value/id = '{ sampleData.LabordatenID }'");
        }
        public static AQLQuery PatientSymptom_VS(PatientListParameter patientList)
        {
            return new AQLQuery("PatientSymptom_VS", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
                                c/context/start_time/value as BefundDatum,
                                a/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                a/data[at0190]/events[at0191]/data[at0192]/items[at0151]/value/value as Lokalisation, 
                                a/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value as Beginn, 
                                a/data[at0190]/events[at0191]/data[at0192]/items[at0021]/value/value as Schweregrad, 
                                a/data[at0190]/events[at0191]/data[at0192]/items[at0161]/value/value as Rueckgang 
                                FROM EHR e 
                                CONTAINS COMPOSITION c 
                                CONTAINS OBSERVATION a[openEHR-EHR-OBSERVATION.symptom_sign.v0] 
                                WHERE c/archetype_details/template_id='Symptom' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }");
        }
        public static AQLQuery PatientSymptom_AS(PatientListParameter patientList)
        {
            return new AQLQuery("PatientSymptom_AS", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
                                c/context/start_time/value as BefundDatum,
                                a/data[at0001]/items[at0002]/value/value as AusschlussAussage, 
                                a/data[at0001]/items[at0003]/value/value as Diagnose 
                                FROM EHR e 
                                CONTAINS COMPOSITION c 
                                CONTAINS EVALUATION a[openEHR-EHR-EVALUATION.exclusion_specific.v1] 
                                WHERE c/archetype_details/template_id='Symptom' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }");
        }

        public static AQLQuery PatientSymptom_US(PatientListParameter patientList)
        {
            return new AQLQuery("PatientSymptom_US", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
                                c/context/start_time/value as BefundDatum,
                                a/data[at0001]/items[at0002]/value/value as UnbekanntesSymptom, 
                                a/data[at0001]/items[at0005]/value/value as AussageFehlendeInfo 
                                FROM EHR e 
                                CONTAINS COMPOSITION c  
                                CONTAINS EVALUATION a[openEHR-EHR-EVALUATION.absence.v2] 
                                WHERE c/archetype_details/template_id='Symptom' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }");
        }
        //untested
        public static AQLQuery AntibiogramFromPathogen(MetaDataReceiveModel metaData, SampleReceiveModel sampleData, PathogenReceiveModel pathogenData)
        {
            return new AQLQuery("AntibiogramFromPathogen", $@"SELECT w/feeder_audit/originating_system_audit/time/value as erregerZeit, 
                                b/items[at0024]/value/value as antibiotikum, 
                                b/items[at0004]/value/defining_code/code_string as resistenz, 
                                b/items[at0001]/value/magnitude as mhkMagnitude, 
                                b/items[at0001]/value/magnitude_status as mhkMagnitudeStatus, 
                                b/items[at0001]/value/units as mhkUnits, 
                                u/feeder_audit/originating_system_audit/time/value as antibiogrammZeit, 
                                b/feeder_audit/original_content/value as original 
                                FROM EHR e
                                CONTAINS COMPOSITION c 
                                CONTAINS (CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION j[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS CLUSTER w[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                                CONTAINS CLUSTER z[openEHR-EHR-CLUSTER.erregerdetails.v1] 
                                CONTAINS CLUSTER u[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                                CONTAINS CLUSTER b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) 
                                WHERE e/ehr_status/subject/external_ref/id/value = '{ metaData.PatientID }' 
                                AND c/uid/value = '{ metaData.UID }' 
                                AND  w/items[at0001]/value/value = '{ metaData.FallID }' 
                                AND erregerName = '{ pathogenData.KeimID }' 
                                AND isolatNummer = '{ pathogenData.IsolatNo }'
                                AND w/items[at0001]/name='Erregername'
                                AND b/items[at0024]/name='Antibiotikum' order by b/items[at0024]/value/value asc");
        }

        public static AQLQuery Stationary(string patientId, string fallkennung, DateTime datum)
        {
            return new AQLQuery("Stationary", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value  as FallID, 
                                r/data[at0001]/items[at0071]/value/value  as Datum_Uhrzeit_der_Aufnahme,
                                p/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Datum_Uhrzeit_der_Entlassung,
                                r/data[at0001]/items[at0049,'Aufnahmeanlass']/value/value as Aufnahmeanlass,
                                p/data[at0001]/items[at0040]/value/value as Art_der_Entlassung, 
                                r/data[at0001]/items[at0013]/value/value as Versorgungsfallgrund 
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                CONTAINS (ADMIN_ENTRY r[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                                CONTAINS (CLUSTER w[openEHR-EHR-CLUSTER.location.v1]) 
                                AND ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0])  
                                WHERE e/ehr_status/subject/external_ref/id/value ='{patientId}' 
                                AND r/data[at0001]/items[at0071]/value/value < '{datum.Date.AddDays(-3).ToString("yyyy-MM-dd")}' 
                                AND c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value ='{fallkennung}'");
        }

        public static AQLQuery SymptomsByPatient(string patientId, DateTime datum)
        {
            return new AQLQuery("SymptomsByPatient", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID, 
                                k/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                k/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value as Beginn, 
                                k/data[at0190]/events[at0191]/data[at0192]/items[at0161]/value/value as Rueckgang 
                                FROM EHR e 
                                CONTAINS COMPOSITION c 
                                CONTAINS OBSERVATION k[openEHR-EHR-OBSERVATION.symptom_sign.v0] 
                                WHERE e/ehr_status/subject/external_ref/id/value ='{patientId}' 
                                AND k/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value >= '{datum.Date.ToString("yyyy-MM-dd")}'
                                AND k/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value < '{datum.Date.AddDays(1.0).ToString("yyyy-MM-dd")}'"); 
        }
       
        public static AQLQuery StayFromCase(string patientId, string fallId)
        {
            return new AQLQuery("StayFromCase", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value as FallID,
                                u/data[at0001]/items[at0013]/value/value as Versorgungsfallgrund,
                                u/data[at0001]/items[at0049,'Aufnahmeanlass']/value/value as Aufnahmeanlass,
                                u/data[at0001]/items[at0071]/value/value as Datum_Uhrzeit_der_Aufnahme,
                                f/data[at0001]/items[at0040]/value/value as Art_der_Entlassung,
                                f/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Datum_Uhrzeit_der_Entlassung,
                                d/items[at0027]/value/value as Station
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1]
                                CONTAINS (ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                                OR ADMIN_ENTRY f[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0] 
                                AND CLUSTER d[openEHR-EHR-CLUSTER.location.v1])
                                WHERE e/ehr_status/subject/external_ref/id/value ='{patientId}' 
                                AND c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value ='{fallId}'");
        }

        public static AQLQuery StayFromDate(DateTime datum)
        {
            return new AQLQuery("StayFromCase", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID, 
                                c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value as FallID,
                                j/data[at0001]/items[at0071]/value/value  as Datum_Uhrzeit_der_Aufnahme,
                                j/data[at0001]/items[at0013]/value/value as Versorgungsfallgrund, 
                                j/data[at0001]/items[at0049,'Aufnahmeanlass']/value/value as Aufnahmeanlass,
                                w/data[at0001]/items[at0040]/value/value as Art_der_Entlassung, 
                                w/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Datum_Uhrzeit_der_Entlassung
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                CONTAINS (ADMIN_ENTRY j[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                                AND ADMIN_ENTRY w[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) 
                                WHERE j/data[at0001]/items[at0071]/value/value >= '{datum.Date.ToString("yyyy-MM-dd").Insert(10, "*")}'");
        }

        public static AQLQuery CovidPat(string nachweis)
        {          
            return new AQLQuery("CovidPat", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID, 
                                i/items[at0001]/value/value as Fallkennung, 
                                m/items[at0034]/value/value as Zeitpunkt_des_Probeneingangs 
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION z[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                                AND CLUSTER m [openEHR-EHR-CLUSTER.specimen.v1])) 
                                WHERE a/items[at0001,'Nachweis']/value/defining_code/code_string='{nachweis}'
                                AND a/items[at0024]/value/defining_code/code_string MATCHES {{'94500-6','94558-4', '94745-7'}} 
                                ORDER BY Zeitpunkt_des_Probeneingangs ASC");
        }

        public static AQLQuery CovidPatByID(string nachweis, PatientListParameter patientList)
        {
            return new AQLQuery("CovidPat", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID, 
                                i/items[at0001]/value/value as Fallkennung, 
                                m/items[at0034]/value/value as Zeitpunkt_des_Probeneingangs 
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION z[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER m [openEHR-EHR-CLUSTER.specimen.v1])) 
                                WHERE  a/items[at0001,'Nachweis']/value/defining_code/code_string='{nachweis}'
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() } 
                                AND a/items[at0024]/value/defining_code/code_string MATCHES {{'94500-6','94558-4', '94745-7'}} 
                                ORDER BY Zeitpunkt_des_Probeneingangs ASC");
        }

        public static AQLQuery PatientBySymptom(string symptom)
        {
            return new AQLQuery("PatientBySymptom", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID, 
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value as Beginn, 
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0161]/value/value as Rueckgang 
                                FROM EHR e 
                                CONTAINS COMPOSITION c [openEHR-EHR-COMPOSITION.registereintrag.v1] 
                                CONTAINS OBSERVATION t[openEHR-EHR-OBSERVATION.symptom_sign.v0] 
                                WHERE c/name/value='COVID-19 Symptom' 
                                AND t/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value = '{symptom}'");
        }

        public static AQLQuery PatientSymptom()
        {
            return new AQLQuery("PatientBySymptom", $@"SELECT a/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                COUNT(e/ehr_status/subject/external_ref/id/value) as Anzahl_Patienten 
                                FROM EHR e 
                                CONTAINS COMPOSITION c 
                                CONTAINS OBSERVATION a[openEHR-EHR-OBSERVATION.symptom_sign.v0] 
                                WHERE c/archetype_details/template_id='Symptom' 
                                GROUP BY NameDesSymptoms");
        }

        public static AQLQuery PatientVaccination(PatientListParameter patientList)
        {
            return new AQLQuery("PatientVaccination", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
                                c/context/start_time/value as DokumentationsID,
                                a/description[at0017]/items[at0020]/value/value as Impfstoff, 
                                x/items[at0164]/value/magnitude as Dosierungsreihenfolge, 
                                x/items[at0144]/value/magnitude as Dosiermenge, 
                                a/description[at0017]/items[at0021]/value/value as ImpfungGegen 
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.registereintrag.v1] 
                                CONTAINS ACTION a[openEHR-EHR-ACTION.medication.v1] 
                                CONTAINS (CLUSTER x[openEHR-EHR-CLUSTER.dosage.v1]) 
                                WHERE c/archetype_details/template_id='Impfstatus' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }");
        }

        public static AQLQuery SpecificVaccination(PatientListParameter patientList, string vaccination)
        {
            return new AQLQuery("PatientVaccination", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
                                c/context/start_time/value as DokumentationsID,
                                a/description[at0017]/items[at0020]/value/value as Impfstoff, 
                                x/items[at0164]/value/magnitude as Dosierungsreihenfolge, 
                                x/items[at0144]/value/magnitude as Dosiermenge, 
                                a/description[at0017]/items[at0021]/value/value as ImpfungGegen 
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.registereintrag.v1] 
                                CONTAINS ACTION a[openEHR-EHR-ACTION.medication.v1] 
                                CONTAINS (CLUSTER x[openEHR-EHR-CLUSTER.dosage.v1]) 
                                WHERE c/archetype_details/template_id='Impfstatus' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }
                                AND a/description[at0017]/items[at0021]/value/value='{vaccination}' ");
        }

        public static AQLQuery EmployeeContactTracing(PatientListParameter patientList)
        {
            return new AQLQuery("EmployeeContactTracing", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                c/context/start_time/value as DokumentationsID,
                                c/context/other_context[at0001]/items[at0002]/value/value as BerichtID,
                                z/items[at0001]/value/value as EventKennung,
                                z/items[at0002]/value/value as EventArt,
                                z/items[at0007]/items[at0011]/value/value as ArtDerPerson,
                                z/items[at0007]/items[at0010]/value/id as PersonenID,
                                z/items[at0004]/value/value as EventKategorie,
                                z/items[at0006]/value/value as EventKommentar,
                                a/description[at0001]/items[at0009]/value/value as Beschreibung,
                                a/description[at0001]/items[at0006]/value/value as Beginn,
                                a/description[at0001]/items[at0016]/value/value as Ende,
                                a/description[at0001]/items[at0017]/value/value as Ort,
                                a/description[at0001]/items[at0003]/value/value as Gesamtdauer,
                                a/description[at0001]/items[at0008]/value/value as Abstand,
                                x/items[at0001]/value/value as Schutzkleidung,
                                x/items[at0002]/value/value as Person,
                                a/description[at0001]/items[at0007]/value/value as Kommentar
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report.v1] 
                                CONTAINS (CLUSTER z[openEHR-EHR-CLUSTER.eventsummary.v0] 
                                OR ACTION a[openEHR-EHR-ACTION.contact.v0] 
                                CONTAINS (CLUSTER x[openEHR-EHR-CLUSTER.protective_clothing_.v0]))
                                WHERE c/archetype_details/template_id='Bericht zur Kontaktverfolgung' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }");
        }

        public static AQLQuery EmployeePersInfoInfecCtrl(PatientListParameter patientList)
        {
            return new AQLQuery("EmployeePersInfoInfecCtrl", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                c/context/start_time/value as DokumentationsID,
                                c/context/other_context[at0001]/items[at0002]/value/value as BerichtID,
                                o/data[at0001]/events[at0002]/data[at0003]/items[at0028]/value/value as SymptomVorhanden,
                                o/data[at0001]/events[at0002]/data[at0003]/items[at0029]/value/value as AufgetretenSeit,
                                o/data[at0001]/events[at0002]/data[at0003]/items[at0022]/items[at0004]/value/value as Symptom,
                                o/data[at0001]/events[at0002]/data[at0003]/items[at0025]/value/value as SymptomKommentar,
                                a/data[at0001]/items[at0005]/value/value as Nachweis,
                                a/data[at0001]/items[at0012]/value/value as Erregername,
                                a/data[at0001]/items[at0015]/value/value as Zeitpunkt,
                                a/data[at0001]/items[at0011]/value/value as KlinischerNachweis,
                                a/protocol[at0003]/items[at0004]/value/value as LetzteAktualisierung,
                                b/data[at0001]/items[at0008]/value/value as Freistellung,
                                b/data[at0001]/items[at0005]/value/value as Grund,
                                b/data[at0001]/items[at0002]/value/value as Beschreibung,
                                b/data[at0001]/items[at0003]/value/value as Startdatum,
                                b/data[at0001]/items[at0004]/value/value as Enddatum,
                                b/data[at0001]/items[at0007]/value/value as AbwesendheitKommentar,
                                d/data[at0001]/items[at0009]/value/value as Meldung,
                                d/data[at0001]/items[at0003]/value/value as Ereignis,
                                d/data[at0001]/items[at0004]/value/value as Ereignisbeschreibung,
                                d/data[at0001]/items[at0005]/value/value as Datum,
                                d/data[at0001]/items[at0006]/value/value as Ereignisgrund,
                                d/data[at0001]/items[at0007]/value/value as EreignisKommentar
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report.v1]
                                CONTAINS (OBSERVATION o[openEHR-EHR-OBSERVATION.symptom_sign_screening.v0] and 
                                EVALUATION a[openEHR-EHR-EVALUATION.flag_pathogen.v0] and 
                                EVALUATION b[openEHR-EHR-EVALUATION.exemption_from_work.v0] 
                                AND ADMIN_ENTRY d[openEHR-EHR-ADMIN_ENTRY.report_to_health_department.v0]) 
                                WHERE c/archetype_details/template_id='Personeninformation zur Infektionskontrolle' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }");
        }

        public static AQLQuery EmployeePersonData(PatientListParameter patientList)
        {
            return new AQLQuery("EmployeePersonData", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                               c/context/start_time/value as DokumentationsID,
                               c/context/other_context[at0003]/items[at0004]/value/value as PersonID,
                               a/data[at0001]/items[at0008]/value/value as ArtDerPerson,
                               b/items[at0002]/items[at0017]/value/value as Titel,
                               b/items[at0002]/items[at0003]/value/value as Vorname,
                               b/items[at0002]/items[at0004]/value/value as WeitererVorname,
                               b/items[at0002]/items[at0005]/value/value as Nachname,
                               b/items[at0002]/items[at0018]/value/value as Suffix,
                               d/items[at0001]/value/value as Geburtsdatum,
                               f/items[at0011]/value/value as Zeile,
                               f/items[at0012]/value/value as Stadt,
                               f/items[at0014]/value/value as Plz,
                               g/items[at0001]/items[at0004]/value/value as Kontakttyp,
                               g/items[at0001]/items[at0003]/items[at0007]/value/value as Nummer,
                               h/items[at0003]/items[at0006]/value/value as Fachbezeichnung,
                               f/items[at0011]/value/value AS HeilZeile,
                               f/items[at0012]/value/value AS HeilStadt,
                               f/items[at0014]/value/value AS HeilPLZ
                               FROM EHR e
                               CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.personendaten.v0]
                               CONTAINS ADMIN_ENTRY a[openEHR-EHR-ADMIN_ENTRY.person_data.v0]
                               CONTAINS (CLUSTER b[openEHR-EHR-CLUSTER.person_name.v0] AND
                               CLUSTER d[openEHR-DEMOGRAPHIC-CLUSTER.person_birth_data_iso.v0] AND
                               CLUSTER f[openEHR-EHR-CLUSTER.address_cc.v0] AND
                               CLUSTER g[openEHR-EHR-CLUSTER.telecom_details.v0] AND
                               CLUSTER h[openEHR-EHR-CLUSTER.individual_professional.v0])
                               WHERE c/archetype_details/template_id='Personendaten' 
                               AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }");
        }

        public static AQLQuery GetAllStationsForConfig()
        {
            return new AQLQuery("RKIConfig", $@"SELECT DISTINCT b/items[at0027]/value/value as StationID
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                                CONTAINS CLUSTER b[openEHR-EHR-CLUSTER.location.v1]");
        }

        public static AQLQuery GetErregernameFromViro(string name)
        {
            return new AQLQuery("RKIConfig", $@"SELECT DISTINCT n/items[at0024,'Virusnachweistest']/value/defining_code/code_string AS KeimID
                                FROM EHR e
                                CONTAINS COMPOSITION c
                                CONTAINS CLUSTER n[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]
                                WHERE n/items[at0024,'Virusnachweistest']/value/value LIKE '{name}*'");
        }
        //to-do: muss auf akt. Template angepasst werden
        public static AQLQuery GetErregernameFromMikro(string name)
        {
            return new AQLQuery("RKIConfig", $@"SELECT DISTINCT
                                w/items[at0001,'Erregername']/value/encoding/code_string AS KeimID
                                FROM EHR e
                                CONTAINS COMPOSITION c
                                CONTAINS CLUSTER w[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                                WHERE w/items[at0001,'Erregername']/value/value LIKE '*{name}*'");
        }

        public static AQLQuery GetPatientCaseList(OutbreakDetectionParameter parameter)
        {
            return new AQLQuery("OutbreakDetectionPatientCaseList", $@"SELECT DISTINCT e/ehr_status/subject/external_ref/id/value as PatientID,
                                                        r/items[at0027]/value/value as Ward,
                                                        u/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value/value as CaseID
                                                        FROM EHR e
                                                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                                                        CONTAINS (CLUSTER u[openEHR-EHR-CLUSTER.case_identification.v0] AND ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]
                                                        CONTAINS CLUSTER r[openEHR-EHR-CLUSTER.location.v1]) 
                                                        WHERE c/name/value ='Patientenaufenthalt' 
                                                        AND u/items[at0001]/name/value = 'Zugehöriger Versorgungsfall (Kennung)'
                                                        AND Ward = '{parameter.Ward}' 
                                                        AND h/data[at0001]/items[at0004]/value/value < '{parameter.Endtime.ToString("yyyy-MM-dd")}' 
                                                        AND (h/data[at0001]/items[at0005]/value/value >= '{parameter.Starttime.ToString("yyyy-MM-dd")}' OR NOT EXISTS u/data[at0001]/items[at0005]/value/value)");
        }

        public static AQLQuery GetPatientLabResultList(OutbreakDetectionParameter parameter, OutbreakDectectionPatient pat)
        {
            return new AQLQuery("OutbreakDetectionPatientLabResults", $@"SELECT e/ehr_status/subject/external_ref/id/value AS PatientID,
                                                        y/items[at0001,'Nachweis']/value/defining_code/code_string AS Result,
                                                        j/items[at0015]/value/value AS ResultDate,
                                                        q/items[at0001]/value/value as CaseID
                                                        FROM EHR e
                                                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                                                        CONTAINS (CLUSTER q[openEHR-EHR-CLUSTER.case_identification.v0] AND OBSERVATION n[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]
                                                        CONTAINS (CLUSTER j[openEHR-EHR-CLUSTER.specimen.v1] AND CLUSTER y[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]))
                                                        WHERE c/name/value= 'Virologischer Befund' 
                                                        AND PatientID = '{pat.PatientID}' 
                                                        AND y/items[at0024,'Virusnachweistest']/value/defining_code/code_string matches {parameter.ToAQLMatchString()}
                                                        AND Result = '260373001'");
        }

        public static AQLQuery GetFirstMovementFromStation(OutbreakDetectionParameter parameter)
        {
            return new AQLQuery("FirstMovementFromStation", $@"SELECT min(h/data[at0001]/items[at0004]/value/value)
                                                        FROM EHR e
                                                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                                                        CONTAINS ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]
                                                        CONTAINS CLUSTER r[openEHR-EHR-CLUSTER.location.v1]
                                                        WHERE r/items[at0027]/value/value='{parameter.Ward}'");
        }

    }
}
 