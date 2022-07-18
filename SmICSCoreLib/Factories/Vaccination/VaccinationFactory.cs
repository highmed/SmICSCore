using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;


namespace SmICSCoreLib.Factories.Vaccination
{
    public class VaccinationFactory : IVaccinationFactory
    {
        public IRestDataAccess RestDataAccess { get; }
        private readonly ILogger<VaccinationFactory> _logger;
        public VaccinationFactory(IRestDataAccess restData, ILogger<VaccinationFactory> logger)
        {
            _logger = logger;
            RestDataAccess = restData;
        }
        public List<VaccinationModel> Process(PatientListParameter parameter)
        {

            List<VaccinationModel> vaccList = RestDataAccess.AQLQuery<VaccinationModel>(PatientVaccination(parameter));

            if (vaccList is null)
            {
                return new List<VaccinationModel>();
            }

            return vaccList;
        }

        public List<VaccinationModel> ProcessSpecificVaccination(PatientListParameter parameter, string vaccination)
        {
            List<VaccinationModel> vaccList = RestDataAccess.AQLQuery<VaccinationModel>(SpecificVaccination(parameter, vaccination ));

            if (vaccList is null)
            {
                return new List<VaccinationModel>();
            }

            return vaccList;
        }

        public static AQLQuery PatientVaccination(PatientListParameter patientList)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientVaccination",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
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
                                AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }"
            };
            return aql;
        }

        public static AQLQuery SpecificVaccination(PatientListParameter patientList, string vaccination)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "SpecificVaccination",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientenID,
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
                                AND a/description[at0017]/items[at0021]/value/value='{vaccination}' "
            };
            return aql;
        }
    }
}
