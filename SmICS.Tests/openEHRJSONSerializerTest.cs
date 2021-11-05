using SmICSCoreLib.Factories.PatientMovement.ReceiveModels;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace SmICSDataGenerator.Tests
{
    
    public class OpenEHRJSONSerializerTest
    {
        [Fact]
        public void Serialize_TransformationTest()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("{\"meta\":{\"_type\":\"RESULTSET\",\"_created\":\"2020-09-01T11:28:43.095Z\",\"_executed_aql\":\"SELECT u/data[at0001]/items[at0004]/value/value as Beginn, u/data[at0001]/items[at0005]/value/value as Ende, u/data[at0001]/items[at0006]/value/value as Bewegungsart_l, a/items[at0048]/value/defining_code/code_string as Fachabteilung, e/ehr_id/value as PatientID, n/items[at0001,'Zugehörige Versorgungsfall-Kennung']/value/value as FallID, a/items[at0029,'Zimmerkennung']/value/value as Raum, a/items[at0046]/value/value as StationID FROM EHR e CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] CONTAINS (ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] CONTAINS CLUSTER a[openEHR-EHR-CLUSTER.location.v1] and CLUSTER n[openEHR-EHR-CLUSTER.case_identification.v0]) WHERE c/name/value='Patientenaufenthalt' and e/ehr_id/value MATCHES {'6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8'} and EXISTS n/items[at0001,'Zugehörige Versorgungsfall-Kennung']/value/value ORDER BY e/ehr_id/value ASC, u/data[at0001]/items[at0004]/value/value ASC\"},\"q\":\"SELECT u/data[at0001]/items[at0004]/value/value as Beginn, u/data[at0001]/items[at0005]/value/value as Ende, u/data[at0001]/items[at0006]/value/value as Bewegungsart_l, a/items[at0048]/value/defining_code/code_string as Fachabteilung, e/ehr_id/value as PatientID, n/items[at0001,'Zugehörige Versorgungsfall-Kennung']/value/value as FallID, a/items[at0029,'Zimmerkennung']/value/value as Raum, a/items[at0046]/value/value as StationID FROM EHR e CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] CONTAINS (ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] CONTAINS CLUSTER a[openEHR-EHR-CLUSTER.location.v1] and CLUSTER n[openEHR-EHR-CLUSTER.case_identification.v0]) WHERE c/name/value='Patientenaufenthalt' and e/ehr_id/value MATCHES {'6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8'} and EXISTS n/items[at0001,'Zugehörige Versorgungsfall-Kennung']/value/value ORDER BY e/ehr_id/value ASC, u/data[at0001]/items[at0004]/value/value ASC\",\"columns\":[{\"name\":\"Beginn\",\"path\":\"/data[at0001]/items[at0004]/value/value\"},{\"name\":\"Ende\",\"path\":\"/data[at0001]/items[at0005]/value/value\"},{\"name\":\"Bewegungsart_l\",\"path\":\"/data[at0001]/items[at0006]/value/value\"},{\"name\":\"Fachabteilung\",\"path\":\"/items[at0048]/value/defining_code/code_string\"},{\"name\":\"PatientID\",\"path\":\"/ehr_id/value\"},{\"name\":\"FallID\",\"path\":\"/items[at0001]/value/value\"},{\"name\":\"Raum\",\"path\":\"/items[at0029]/value/value\"},{\"name\":\"StationID\",\"path\":\"/items[at0046]/value/value\"}],\"rows\":[[\"2020-03-19T20:48:23+01:00\",\"2020-03-23T06:44:16+01:00\",\"AK Aufn and.KH\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20119981\",null,\"Station 14\"],[\"2020-03-20T12:00:00+01:00\",\"2020-03-20T12:00:00+01:00\",\"LA Klin.Auftrag\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20119981\",null,\"PNE Bronchoskopie\"]]}");

            PatientStayModel patStay1 = new PatientStayModel();
            patStay1.Beginn = DateTime.Parse("2020-03-19T20:48:23+01:00");
            patStay1.Ende = DateTime.Parse("2020-03-23T06:44:16+01:00");
            patStay1.Bewegungsart_l = "AK Aufn and.KH";
            patStay1.Fachabteilung = "0800";
            patStay1.PatientID = "6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8";
            patStay1.FallID = "20119981";
            patStay1.Raum = null;
            patStay1.StationID = "Station 14";

            PatientStayModel patStay2 = new PatientStayModel();
            patStay2.Beginn = DateTime.Parse("2020-03-20T12:00:00+01:00");
            patStay2.Ende = DateTime.Parse("2020-03-20T12:00:00+01:00");
            patStay2.Bewegungsart_l = "LA Klin.Auftrag";
            patStay2.Fachabteilung = "0800";
            patStay2.PatientID = "6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8";
            patStay2.FallID = "20119981";
            patStay2.Raum = null;
            patStay2.StationID = "PNE Bronchoskopie";

            List<PatientStayModel> actualPatientStays = new List<PatientStayModel>() { patStay1, patStay2 };
            List<PatientStayModel> expectedPatientStays = openEHRJSONSerializer<PatientStayModel>.ReceiveModelConstructor(response);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(actualPatientStays != null);
            Assert.Equal(expectedPatientStays.Count, actualPatientStays.Count);
            for(int i=0; i < expectedPatientStays.Count; i++)
            {
                Assert.Equal(expectedPatientStays[i].Beginn, actualPatientStays[i].Beginn);
                Assert.Equal(expectedPatientStays[i].Bewegungsart_l, actualPatientStays[i].Bewegungsart_l);
                Assert.Equal(expectedPatientStays[i].Ende, actualPatientStays[i].Ende);
                Assert.Equal(expectedPatientStays[i].Fachabteilung, actualPatientStays[i].Fachabteilung);
                Assert.Equal(expectedPatientStays[i].FallID, actualPatientStays[i].FallID);
                Assert.Equal(expectedPatientStays[i].PatientID, actualPatientStays[i].PatientID);
                Assert.Equal(expectedPatientStays[i].Raum, actualPatientStays[i].Raum);
                Assert.Equal(expectedPatientStays[i].StationID, actualPatientStays[i].StationID);
            }
        }
    }
}


/*The rest of the repsonse string if needed for test:
 
,[\"2020-03-20T12:00:00+01:00\",\"2020-03-20T12:00:00+01:00\",\"Diagn./Therap.\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20119981\",null,\"PNE Bronchoskopie\"],[\"2020-03-21T13:15:00+01:00\",\"2020-03-21T13:15:00+01:00\",\"LA Klin.Auftrag\",\"0900\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20119981\",null,\"PMR Krankengymnastik\"],[\"2020-03-23T06:44:16+01:00\",\"2020-04-01T15:00:00+02:00\",\"S/B-Wechsel\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20119981\",null,\"Station 14\"],[\"2020-03-23T11:00:00+01:00\",\"2020-03-23T11:00:00+01:00\",\"LA Klin.Auftrag\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20119981\",null,\"PNE Bronchoskopie\"],[\"2020-03-23T11:00:04+01:00\",\"2020-03-23T11:00:04+01:00\",\"Diagn./Therap.\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20119981\",null,\"PNE Bronchoskopie\"],[\"2020-04-01T23:04:07+02:00\",null,\"S/B-Wechsel\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20119981\",null,\"Station 14\"],[\"2020-04-13T11:45:17+02:00\",\"2020-04-23T19:00:07+02:00\",\"AK Aufn and.KH\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",\"4\",\"Station 48\"],[\"2020-04-14T08:50:00+02:00\",\"2020-04-14T08:50:00+02:00\",\"Diagn./Therap.\",\"3350\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"NER Magnet-Resonanz-Tomographi\"],[\"2020-04-14T10:52:00+02:00\",\"2020-04-14T10:52:00+02:00\",\"LA Klin.Auftrag\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"HNO Audiometrie\"],[\"2020-04-14T11:45:00+02:00\",\"2020-04-14T11:45:00+02:00\",\"LA Klin.Auftrag\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"HNO Allergologie\"],[\"2020-04-14T13:07:00+02:00\",\"2020-04-14T13:07:00+02:00\",\"LA Klin.Auftrag\",\"2800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"NEU Konsile\"],[\"2020-04-14T14:19:00+02:00\",\"2020-04-14T14:19:00+02:00\",\"LA Klin.Auftrag\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"HNO Vestibularis\"],[\"2020-04-14T14:19:00+02:00\",\"2020-04-14T14:19:00+02:00\",\"LA Klin.Auftrag\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"HNO Elekt. Reaktionsaudio\"],[\"2020-04-15T09:17:00+02:00\",\"2020-04-15T09:17:00+02:00\",\"LA Klin.Auftrag\",\"0100\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"IFK Konsile\"],[\"2020-04-15T12:51:00+02:00\",\"2020-04-15T12:51:00+02:00\",\"Diagn./Therap.\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"zentraler OP-03\"],[\"2020-04-15T15:32:00+02:00\",\"2020-04-15T15:32:00+02:00\",\"LA Klin.Auftrag\",\"3600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"ANE Prämedikationssprechstunde\"],[\"2020-04-16T08:57:00+02:00\",\"2020-04-16T08:57:00+02:00\",\"Diagn./Therap.\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"zentraler OP-01\"],[\"2020-04-17T09:23:00+02:00\",\"2020-04-17T09:23:00+02:00\",\"LA Klin.Auftrag\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"PNE Lungenfunktionslabor\"],[\"2020-04-17T10:03:00+02:00\",\"2020-04-17T10:03:00+02:00\",\"LA Klin.Auftrag\",\"0300\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"KAR Konsile\"],[\"2020-04-17T11:42:00+02:00\",\"2020-04-17T11:42:00+02:00\",\"Diagn./Therap.\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"zentraler OP-03\"],[\"2020-04-17T11:47:00+02:00\",\"2020-04-17T11:47:00+02:00\",\"Diagn./Therap.\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"HNO Digitaler Volumentomograph\"],[\"2020-04-17T13:15:15+02:00\",\"2020-04-17T13:15:15+02:00\",\"Diagn./Therap.\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"PNE Lungenfunktionslabor\"],[\"2020-04-20T10:13:00+02:00\",\"2020-04-20T10:13:00+02:00\",\"LA Klin.Auftrag\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"HNO Audiometrie\"],[\"2020-04-20T11:19:00+02:00\",\"2020-04-20T11:19:00+02:00\",\"LA Klin.Auftrag\",\"0800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"PNE Lungenfunktionslabor\"],[\"2020-04-20T14:59:00+02:00\",\"2020-04-20T14:59:00+02:00\",\"Diagn./Therap.\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"zentraler OP-03\"],[\"2020-04-21T10:49:00+02:00\",\"2020-04-21T10:49:00+02:00\",\"LA Klin.Auftrag\",\"0300\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"KAR Echokardiographie\"],[\"2020-04-21T11:58:00+02:00\",\"2020-04-21T11:58:00+02:00\",\"LA Klin.Auftrag\",\"2800\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"NEU Klinische Neurophysiologie\"],[\"2020-04-22T08:10:00+02:00\",\"2020-04-22T08:10:00+02:00\",\"LA Klin.Auftrag\",\"0300\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"KAR EKG\"],[\"2020-04-23T09:45:00+02:00\",\"2020-04-23T09:45:00+02:00\",\"LA Klin.Auftrag\",\"0300\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"KAR Konsile\"],[\"2020-04-23T11:54:00+02:00\",\"2020-04-23T11:54:00+02:00\",\"LA Klin.Auftrag\",\"9999\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"KLA Sozialdienst\"],[\"2020-04-23T12:42:00+02:00\",\"2020-04-23T12:42:00+02:00\",\"LA Klin.Auftrag\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"HNO Audiometrie\"],[\"2020-04-27T09:51:23+02:00\",\"2020-04-27T09:51:23+02:00\",\"NS Nachst. Beh.\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"Station 48\"],[\"2020-04-27T11:49:00+02:00\",\"2020-04-27T11:49:00+02:00\",\"LA Klin.Auftrag\",\"2600\",\"6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8\",\"20138482\",null,\"HNO Audiometrie\"]     
     
   */
