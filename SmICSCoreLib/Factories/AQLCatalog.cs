using SmICSCoreLib.Factories.ContactNetwork;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.EpiCurve;
using SmICSCoreLib.Factories.PatientMovement;
using System;
using SmICSCoreLib.Factories.OutbreakDetection;
using SmICSCoreLib.Factories.OutbreakDetection.ReceiveModel;
using SmICSCoreLib.Factories.Lab.MibiLabdata.ReceiveModel;
using SmICSCoreLib.Factories.MiBi;

namespace SmICSCoreLib.Factories
{
    public sealed class AQLCatalog
    {
        private AQLCatalog() { }        
        public static AQLQuery GetEHRID(string subjectID)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "GetEHRID",
                Query = $@"SELECT DISTINCT e/ehr_id/value as ID 
                                FROM EHR e CONTAINS COMPOSITION c 
                                WHERE e/ehr_status/subject/external_ref/id/value='{subjectID}' 
                                AND e/ehr_status/subject/external_ref/namespace='SmICSTests'"
            };
            return aql;
        }
        /// <summary>
        /// AQL Query for all Cases of given patient ordered by admission date in ascending order (earliest admission first).
        /// </summary>
        /// <param name="patient"></param>
        public static AQLQuery Cases(Patient patient)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "Aufnahme - Stationärer Versorgungsfall",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID, 
                        c/context/other_context[at0001]/items[at0003]/value/value as CaseID 
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                        CONTAINS ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                        WHERE c/name/value = 'Stationärer Versorgungsfall' 
                        AND e/ehr_status/subject/external_ref/id/value = '{ patient.PatientID }' 
                        ORDER BY p/data[at0001]/items[at0071]/value/value ASC"
            };
            return aql;
        }

        public static AQLQuery ContactPatientWards(ContactParameter parameter)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "ContactPatientWards",
                Query = $@"SELECT m/data[at0001]/items[at0004]/value/value as Beginn, 
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
                                ORDER BY m/data[at0001]/items[at0004]/value/value ASC"
            };
            return aql;
        }
        public static AQLQuery ContactPatients(ContactPatientsParameter parameter)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "ContactPatients",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                ORDER BY h/data[at0001]/items[at0004]/value/value"
            };
            return aql;
        }
        public static AQLQuery ContactPatients_WithoutWardInformation(ContactPatientsParameter parameter)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "ContactPatients",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                ORDER BY h/data[at0001]/items[at0004]/value/value"
            };
            return aql;
        }
        public static AQLQuery PatientStay(PatientListParameter patientList)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientStay",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                ORDER BY e/ehr_status/subject/external_ref/id/value ASC, h/data[at0001]/items[at0004]/value/value ASC"
            };
            return aql;
        }

        public static AQLQuery PatientStay(Patient patient)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientStay",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                AND e/ehr_status/subject/external_ref/id/value = '{patient}'
                                ORDER BY e/ehr_status/subject/external_ref/id/value ASC, h/data[at0001]/items[at0004]/value/value ASC"
            };
            return aql;
        }

        public static AQLQuery PatientStayFromStation(PatientListParameter patientList, string station, DateTime starttime, DateTime endtime)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientStay",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                ORDER BY e/ehr_status/subject/external_ref/id/value ASC, h/data[at0001]/items[at0004]/value/value ASC"
            };
            return aql;
        }

        public static AQLQuery PatientAdmission(EpsiodeOfCareParameter parameter)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientAdmission",
                Query = $@"SELECT p/data[at0001]/items[at0071]/value/value as Beginn
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                CONTAINS ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                                WHERE c/name/value = 'Stationärer Versorgungsfall' 
                                AND e/ehr_status/subject/external_ref/id/value = '{ parameter.PatientID }' 
                                AND c/context/other_context[at0001]/items[at0003]/value/value = '{ parameter.CaseID }'"
            };
            return aql;
        }

        public static AQLQuery PatientDischarge(EpsiodeOfCareParameter parameter)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientDischarge",
                Query = $@"SELECT b/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Ende 
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                CONTAINS ADMIN_ENTRY b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0] 
                                WHERE c/name/value = 'Stationärer Versorgungsfall' 
                                AND e/ehr_status/subject/external_ref/id/value = '{ parameter.PatientID }' 
                                AND c/context/other_context[at0001]/items[at0003]/value/value = '{ parameter.CaseID }'"
            };
            return aql;
        }

        public static AQLQuery PatientLaborData(PatientListParameter patientList)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientLaborData",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                ORDER BY a/items[at0015]/value/value ASC"
            };
            return aql;
        }

        public static AQLQuery NECPatientLaborData(string PatientID, TimespanParameter timespan)
        {
            //TODO: AQL musst zu eine MIBI AQL umgewandelt werden. 
            AQLQuery aql = new AQLQuery()
            {
                Name = "NECPatientLaborData",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                AND z/items[at0015]/value/value <= '{timespan.Endtime}'"
            };
            return aql;
        }

        public static AQLQuery LaborEpiCurve(DateTime date, ExtendedEpiCurveParameter parameter)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "LaborEpiCurve",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                AND m/items[at0015]/value/value<'{ date.AddDays(1).ToString("yyyy-MM-dd") }'"
            };
            return aql;
        }

        public static AQLQuery PatientLocation(DateTime date, string patientID)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientLocation",
                Query = $@"SELECT a/items[at0027]/value/value as Ward, 
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
                                ORDER BY u/data[at0001]/items[at0004]/value/value ASC"
            };
            return aql;
        }
        
        //items[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]/items[openEHR-EHR-CLUSTER.erregerdetails.v1]/items[at0018]
       
        
        //untested
        public static AQLQuery AntibiogramFromPathogen(MetaDataReceiveModel metaData, SampleReceiveModel sampleData, PathogenReceiveModel pathogenData)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "AntibiogramFromPathogen",
                Query = $@"SELECT w/feeder_audit/originating_system_audit/time/value as erregerZeit, 
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
                                AND b/items[at0024]/name='Antibiotikum' order by b/items[at0024]/value/value asc"
            };
            return aql;
        }

        public static AQLQuery Stationary(string patientId, string fallkennung, DateTime datum)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "Stationary",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value  as FallID, 
                                r/data[at0001]/items[at0071]/value/value  as Datum_Uhrzeit_der_Aufnahme,
                                p/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Datum_Uhrzeit_der_Entlassung,
                                r/data[at0001]/items[at0049,'Aufnahmeanlass']/value/value as Aufnahmeanlass,
                                p/data[at0001]/items[at0040]/value/value as Art_der_Entlassung, 
                                r/data[at0001]/items[at0013]/value/value as Versorgungsfallgrund 
                                FROM EHR e 
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                CONTAINS (ADMIN_ENTRY r[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                                AND ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0])  
                                WHERE e/ehr_status/subject/external_ref/id/value ='{patientId}' 
                                AND r/data[at0001]/items[at0071]/value/value < '{datum.Date.AddDays(-3).ToString("yyyy-MM-dd")}' 
                                AND c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value ='{fallkennung}'"
            };
            return aql;
        }
       
        public static AQLQuery StayFromCase(string patientId, string fallId)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "StayFromCase",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                AND c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value ='{fallId}'"
            };
            return aql;
        }

        public static AQLQuery StayFromDate(DateTime datum)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "StayFromCase",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID, 
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
                                WHERE j/data[at0001]/items[at0071]/value/value >= '{datum.Date.ToString("yyyy-MM-dd").Insert(10, "*")}'"
            };
            return aql;
        }

        public static AQLQuery CovidPat(string nachweis)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "CovidPat",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID, 
                                i/items[at0001]/value/value as Fallkennung, 
                                m/items[at0015]/value/value as Zeitpunkt_der_Probenentnahme
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION z[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                                AND CLUSTER m [openEHR-EHR-CLUSTER.specimen.v1])) 
                                WHERE a/items[at0001,'Nachweis']/value/defining_code/code_string='{nachweis}'
                                AND a/items[at0024]/value/defining_code/code_string MATCHES {{'94500-6','94558-4', '94745-7'}} 
                                ORDER BY m/items[at0015]/value/value ASC"
            };
            return aql;
        }

        public static AQLQuery CovidPatByID(string nachweis, PatientListParameter patientList)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "CovidPat",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID, 
                                i/items[at0001]/value/value as Fallkennung, 
                                m/items[at0015]/value/value as Zeitpunkt_der_Probenentnahme
                                FROM EHR e 
                                CONTAINS COMPOSITION c CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION z[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER m [openEHR-EHR-CLUSTER.specimen.v1])) 
                                WHERE  a/items[at0001,'Nachweis']/value/defining_code/code_string='{nachweis}'
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() } 
                                AND a/items[at0024]/value/defining_code/code_string MATCHES {{'94500-6','94558-4', '94745-7'}} 
                                ORDER BY m/items[at0015]/value/value ASC"
            };
            return aql;
        }

        public static AQLQuery GetPatientCaseList(OutbreakDetectionParameter parameter)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "OutbreakDetectionPatientCaseList",
                Query = $@"SELECT DISTINCT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                                        AND (h/data[at0001]/items[at0005]/value/value >= '{parameter.Starttime.ToString("yyyy-MM-dd")}' OR NOT EXISTS u/data[at0001]/items[at0005]/value/value)"
            };
            return aql;
        }

        public static AQLQuery GetPatientLabResultList(OutbreakDetectionParameter parameter, OutbreakDectectionPatient pat)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "OutbreakDetectionPatientLabResults",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value AS PatientID,
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
                                                        AND Result = '260373001'"
            };
            return aql;
        }

        public static AQLQuery GetFirstMovementFromStation(OutbreakDetectionParameter parameter)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "FirstMovementFromStation",
                Query = $@"SELECT min(h/data[at0001]/items[at0004]/value/value)
                                                        FROM EHR e
                                                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                                                        CONTAINS ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]
                                                        CONTAINS CLUSTER r[openEHR-EHR-CLUSTER.location.v1]
                                                        WHERE r/items[at0027]/value/value='{parameter.Ward}'"
            };
            return aql;
        }

        #region MiBi
        /// <summary>
        /// Returns all patients which admitted before or at the given date and was discharged at the given date
        /// </summary>
        public static AQLQuery GetAllPatientsDischarged(DateTime date)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "GetAllPatientsDischarged", 
                Query = $@"Select e/ehr_status/subject/external_ref/id/value as PatientID 
                                                    FROM EHR e CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                                    CONTAINS (ADMIN_ENTRY a[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                                                    AND ADMIN_ENTRY d[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) 
                                                    WHERE c/name/value='Stationärer Versorgungsfall' 
                                                    AND a/data[at0001]/items[at0071]/value/value <= '{date.ToString("yyyy-MM-dd")}' 
                                                    AND d/data[at0001]/items[at0011]/value/value >= '{date.ToString("yyyy-MM-dd")}'"
            };
            return aql;
        }
        /// <summary>
        /// Returns all patients which admitted before or at the given date and still aren't discharged
        /// </summary>
        public static AQLQuery GetAllPatients(DateTime date)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "GetAllPatients", 
                Query = $@"Select e/ehr_status/subject/external_ref/id/value as PatientID 
                                                    FROM EHR e CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
                                                    CONTAINS ADMIN_ENTRY a[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
                                                    WHERE c/name/value='Stationärer Versorgungsfall' 
                                                    AND a/data[at0001]/items[at0071]/value/value <= '{date.ToString("yyyy-MM-dd")}' 
                                                    AND NOT EXISTS c/content[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]"
            };
            return aql;
        }
        public static AQLQuery CasesWithResults(PatientListParameter patientList)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "CasesWithResults", 
                Query = $"SELECT DISTINCT e/ehr_status/subject/external_ref/id/value as PatientID, m/items[at0001]/value/value AS FallID, c/uid/value as UID FROM EHR e CONTAINS COMPOSITION c CONTAINS CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] WHERE c/name/value = 'Mikrobiologischer Befund' AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() } order by m/items[at0001]/value/value asc"
            };
            return aql;
        }
        public static AQLQuery ReportMeta(CaseIDReceiveModel caseID)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "ReportMeta", 
                Query = $"SELECT DISTINCT e/ehr_status/subject/external_ref/id/value as PatientID, m/items[at0001]/value/value as FallID, c/uid/value as UID FROM EHR e CONTAINS COMPOSITION c CONTAINS CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] WHERE e/ehr_status/subject/external_ref/id/value = '{ caseID.PatientID }' and m/items[at0001]/value/value = '{ caseID.FallID }' ORDER BY o/data[at0001]/events[at0002]/time/value DESC"
            };
            return aql;
        }
        public static AQLQuery Requirements(MetaDataReceiveModel metaData)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "Requirements", 
                Query= $"select distinct a/protocol[at0004]/items[at0094]/items[at0106]/value/value as anforderung from EHR e contains COMPOSITION c contains (CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] and OBSERVATION a[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]) where (e/ehr_status/subject/external_ref/id/value='{ metaData.PatientID }' and c/uid/value = '{ metaData.UID }' and m/items[at0001]/value/value = '{ metaData.FallID }' )"
            };
            return aql;
        }
        public static AQLQuery SamplesFromResult(MetaDataReceiveModel metaData)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "SamplesFromResult", 
                Query = $"SELECT b/items[at0029]/value/value as MaterialID, b/items[at0001]/value/id as LabordatenID, b/items[at0015]/value/value as ZeitpunktProbeentnahme, b/items[at0034]/value/value as ZeitpunktProbeneingang FROM EHR e CONTAINS COMPOSITION c CONTAINS (CLUSTER w[openEHR-EHR-CLUSTER.case_identification.v0] and CLUSTER b[openEHR-EHR-CLUSTER.specimen.v1] CONTAINS CLUSTER z[openEHR-EHR-CLUSTER.anatomical_location.v1]) where e/ehr_status/subject/external_ref/id/value = '{ metaData.PatientID }' and c/uid/value = '{ metaData.UID }' and  w/items[at0001]/value/value = '{ metaData.FallID }' order by b/items[at0015]/value/value desc"
            };
            return aql;
        }

        //items[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]/items[openEHR-EHR-CLUSTER.erregerdetails.v1]/items[at0018]
        public static AQLQuery PathogensFromResult(MetaDataReceiveModel metaData, SampleReceiveModel sampleData)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PathogensFromResult",
                Query = $"SELECT distinct d/items[at0001]/value/value as KeimID, d/items[at0027]/value/magnitude as IsolatNo, d/items[at0024]/value/value as Befund, z/items[at0018]/value/value as MREKlasse, d/items[at0003]/value/value as Befundkommentar FROM EHR e CONTAINS COMPOSITION c CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] and OBSERVATION j[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] CONTAINS (CLUSTER q[openEHR-EHR-CLUSTER.specimen.v1] and CLUSTER p[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] CONTAINS CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] CONTAINS CLUSTER z[openEHR-EHR-CLUSTER.erregerdetails.v1])) WHERE d/items[at0001]/name/value = 'Erregername' and d/items[at0024]/name/value='Nachweis?' and d/items[at0027]/name/value = 'Isolatnummer' and e/ehr_status/subject/external_ref/id/value = '{ metaData.PatientID }' and c/uid/value = '{ metaData.UID }' and i/items[at0001]/value/value = '{ metaData.FallID }' and q/items[at0001]/value/id = '{ sampleData.LabordatenID }'"
            };
            return aql;
        }

        

        #endregion
    }
}
 