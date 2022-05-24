using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Employees.PersInfoInfecCtrl
{
    public class PersInfoInfecCtrlFactory : IPersInfoInfecCtrlFactory
    {
        public IRestDataAccess _restData;
        public PersInfoInfecCtrlFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<PersInfoInfecCtrlModel> Process(PatientListParameter parameter)
        {

            List<PersInfoInfecCtrlModel> ctList = _restData.AQLQuery<PersInfoInfecCtrlModel>(EmployeePersInfoInfecCtrl(parameter));

            if (ctList is null)
            {
                return new List<PersInfoInfecCtrlModel>();
            }

            return ctList;
        }

        private static AQLQuery EmployeePersInfoInfecCtrl(PatientListParameter patientList)
        {
            return new AQLQuery
            {
                Name = "EmployeePersInfoInfecCtrl",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                        AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }"
            };
        }
    }
}
