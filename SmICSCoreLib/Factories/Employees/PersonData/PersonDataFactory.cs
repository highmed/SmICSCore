using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Employees.PersonData
{
    public class PersonDataFactory : IPersonDataFactory
    {
        public IRestDataAccess _restData;
        public PersonDataFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<PersonDataModel> Process(PatientListParameter parameter)
        {

            List<PersonDataModel> ctList = _restData.AQLQuery<PersonDataModel>(EmployeePersonData(parameter));

            if (ctList is null)
            {
                return new List<PersonDataModel>();
            }

            return ctList;
        }

        private static AQLQuery EmployeePersonData(PatientListParameter patientList)
        {
            return new AQLQuery
            {
                Name= "EmployeePersonData",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                        AND e/ehr_status/subject/external_ref/id/value matches { patientList.ToAQLMatchString() }"
            };
        }
    }
}
