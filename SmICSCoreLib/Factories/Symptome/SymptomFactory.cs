using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.Symptome
{
    public class SymptomFactory : ISymptomFactory
    {
        public IRestDataAccess RestDataAccess { get; }
        private readonly ILogger<SymptomFactory> _logger;
        public SymptomFactory(IRestDataAccess restData, ILogger<SymptomFactory> logger)
        {
            _logger = logger;
            RestDataAccess = restData;
        }
        public List<SymptomModel> Process(PatientListParameter parameter)
        {
            List<SymptomModel> symptomList = new List<SymptomModel>();

            try
            {
                List<SymptomModel> symptomList_VS = RestDataAccess.AQLQuery<SymptomModel>(PatientSymptom_VS(parameter));

                if (symptomList_VS != null)
                {
                    symptomList = symptomList_VS;
                }

                List<SymptomModel> symptomList_AS = RestDataAccess.AQLQuery<SymptomModel>(PatientSymptom_AS(parameter));

                if (symptomList_AS != null)
                {
                    symptomList = symptomList.Concat(symptomList_AS).ToList();
                }

                List<SymptomModel> symptomList_US = RestDataAccess.AQLQuery<SymptomModel>(PatientSymptom_US(parameter));

                if (symptomList_US != null)
                {
                    symptomList = symptomList.Concat(symptomList_US).ToList();
                }

                _logger.LogInformation("Information found.");
            } 
            catch (Exception e)
            {
                _logger.LogError(e, "This Information could not be found.");
            } 

            return symptomList;

        }

        public List<SymptomModel> ProcessNoParam()
        {
            List<SymptomModel> symptomList = new List<SymptomModel>();

            List<SymptomModel> symptomList_VS = RestDataAccess.AQLQuery<SymptomModel>(PatientSymptom());

            if (symptomList_VS != null)
            {
                symptomList = symptomList_VS;
            }
            return symptomList;
        }

        public List<SymptomModel> PatientBySymptom(string symptom)
        {
            List<SymptomModel> symptomList = new List<SymptomModel>();

            List<SymptomModel> symptomList_VS = RestDataAccess.AQLQuery<SymptomModel>(PatientBySymptom_AQL(symptom));

            if (symptomList_VS != null)
            {
                symptomList = symptomList_VS;
            }
            return symptomList;
        }

        public List<SymptomModel> SymptomByPatient(string patientId, DateTime datum)
        {
            List<SymptomModel> symptomList = new List<SymptomModel>();

            List<SymptomModel> symptomList_VS = RestDataAccess.AQLQuery<SymptomModel>(SymptomsByPatient(patientId, datum));

            if (symptomList_VS != null)
            {
                symptomList = symptomList_VS;
            }
            return symptomList;
        }

        public static AQLQuery PatientSymptom_VS(PatientListParameter patientList)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientSymptom_VS",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
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
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }"
            };
            return aql;
        }
        public static AQLQuery PatientSymptom_AS(PatientListParameter patientList)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientSymptom_AS",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
                                c/context/start_time/value as BefundDatum,
                                a/data[at0001]/items[at0002]/value/value as AusschlussAussage, 
                                a/data[at0001]/items[at0003]/value/value as Diagnose 
                                FROM EHR e 
                                CONTAINS COMPOSITION c 
                                CONTAINS EVALUATION a[openEHR-EHR-EVALUATION.exclusion_specific.v1] 
                                WHERE c/archetype_details/template_id='Symptom' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }"
            };
            return aql;
        }

        public static AQLQuery PatientSymptom_US(PatientListParameter patientList)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientSymptom_US",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
                                c/context/start_time/value as BefundDatum,
                                a/data[at0001]/items[at0002]/value/value as UnbekanntesSymptom, 
                                a/data[at0001]/items[at0005]/value/value as AussageFehlendeInfo 
                                FROM EHR e 
                                CONTAINS COMPOSITION c  
                                CONTAINS EVALUATION a[openEHR-EHR-EVALUATION.absence.v2] 
                                WHERE c/archetype_details/template_id='Symptom' 
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }"
            };
            return aql;
        }

        public static AQLQuery PatientSymptom()
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientBySymptom",
                Query = $@"SELECT a/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                COUNT(e/ehr_status/subject/external_ref/id/value) as Anzahl_Patienten 
                                FROM EHR e 
                                CONTAINS COMPOSITION c 
                                CONTAINS OBSERVATION a[openEHR-EHR-OBSERVATION.symptom_sign.v0] 
                                WHERE c/archetype_details/template_id='Symptom' 
                                GROUP BY NameDesSymptoms"
            };
            return aql;
        }

        public static AQLQuery PatientBySymptom_AQL(string symptom)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientBySymptom",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID, 
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value as Beginn, 
                                t/data[at0190]/events[at0191]/data[at0192]/items[at0161]/value/value as Rueckgang 
                                FROM EHR e 
                                CONTAINS COMPOSITION c [openEHR-EHR-COMPOSITION.registereintrag.v1] 
                                CONTAINS OBSERVATION t[openEHR-EHR-OBSERVATION.symptom_sign.v0] 
                                WHERE c/name/value='COVID-19 Symptom' 
                                AND t/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value = '{symptom}'"
            };
            return aql;
        }

        public static AQLQuery SymptomsByPatient(string patientId, DateTime datum)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "SymptomsByPatient",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID, 
                                k/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value as NameDesSymptoms, 
                                k/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value as Beginn, 
                                k/data[at0190]/events[at0191]/data[at0192]/items[at0161]/value/value as Rueckgang 
                                FROM EHR e 
                                CONTAINS COMPOSITION c 
                                CONTAINS OBSERVATION k[openEHR-EHR-OBSERVATION.symptom_sign.v0] 
                                WHERE e/ehr_status/subject/external_ref/id/value ='{patientId}' 
                                AND k/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value >= '{datum.Date.ToString("yyyy-MM-dd")}'
                                AND k/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value < '{datum.Date.AddDays(1.0).ToString("yyyy-MM-dd")}'"
            };
            return aql;
        }
    }
}
