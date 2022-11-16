using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;


namespace SmICSCoreLib.Factories.Employees.ContactTracing
{
    public class ContactTracingFactory : IContactTracingFactory
    {
        public IRestDataAccess _restData;

        public ContactTracingFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<ContactTracingReceiveModel> Process(PatientListParameter parameter)
        {

            List<ContactTracingReceiveModel> ctList = _restData.AQLQueryAsync<ContactTracingReceiveModel>(EmployeeContactTracing(parameter)).GetAwaiter().GetResult();

            if (ctList is null)
            {
                return new List<ContactTracingReceiveModel>();
            }

            return ctList;
        }

        private static AQLQuery EmployeeContactTracing(PatientListParameter patientList)
        {
            return new AQLQuery
            {
                Name = "EmployeeContactTracing",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                        AND e/ehr_status/subject/external_ref/id/value matches {patientList.ToAQLMatchString()}"
            };
        }
    }
}
