using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.AQL.Contact_Nth_Network;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SmICSDataGenerator.Tests.Contact_Network
{
	public class ContactNetworkTest
	{
		[Theory]
		[ClassData(typeof(ContactNetworkTestData))]
		public void ProcessorTest(int degree, string ehr_id, int start_year, int start_month, int start_day, int end_year, int end_month, int end_day, int expectedResultSet)
		{
			IRestDataAccess _data = TestConnection.Initialize();

			ContactParameter contactParams = new ContactParameter()
			{
				Degree = degree,
				PatientID = ehr_id,
				Starttime = new DateTime(start_year, start_month, start_day),
				Endtime = new DateTime(end_year, end_month, end_day)
			};

			IPatientInformation patientInformation = CreatePatientInformation(_data);
			ContactNetworkFactory factory = new ContactNetworkFactory(_data, NullLogger<ContactNetworkFactory>.Instance, patientInformation);
			ContactModel actual = factory.Process(contactParams);
			ContactModel expected = getExpectedContactModels(expectedResultSet);

			Assert.Equal(expected.PatientMovements.Count, actual.PatientMovements.Count);
			Assert.Equal(expected.LaborData.Count, actual.LaborData.Count);

			for (int i = 0; i < actual.PatientMovements.Count; i++)
			{
				Assert.Equal(expected.PatientMovements[i].PatientID, actual.PatientMovements[i].PatientID);
				Assert.Equal(expected.PatientMovements[i].FallID, actual.PatientMovements[i].FallID);
				Assert.Equal(expected.PatientMovements[i].Bewegungsart_l, actual.PatientMovements[i].Bewegungsart_l);
				Assert.Equal(expected.PatientMovements[i].Bewegungstyp, actual.PatientMovements[i].Bewegungstyp);
				Assert.Equal(expected.PatientMovements[i].BewegungstypID, actual.PatientMovements[i].BewegungstypID);
				Assert.Equal(expected.PatientMovements[i].Beginn.ToString("s"), actual.PatientMovements[i].Beginn.ToString("s"));
				Assert.Equal(expected.PatientMovements[i].Ende.ToString("g"), actual.PatientMovements[i].Ende.ToString("g"));
				Assert.Equal(expected.PatientMovements[i].StationID, actual.PatientMovements[i].StationID);
				Assert.Equal(expected.PatientMovements[i].Raum, actual.PatientMovements[i].Raum);
			}

			for (int i = 0; i < actual.LaborData.Count; i++)
			{
				Assert.Equal(expected.LaborData[i].PatientID, actual.LaborData[i].PatientID);
				Assert.Equal(expected.LaborData[i].FallID, actual.LaborData[i].FallID);
				Assert.Equal(expected.LaborData[i].Befund, actual.LaborData[i].Befund);
				Assert.Equal(expected.LaborData[i].Befunddatum.ToString("s"), actual.LaborData[i].Befunddatum.ToUniversalTime().ToString("s"));
				Assert.Equal(expected.LaborData[i].Befundkommentar, actual.LaborData[i].Befundkommentar);
				Assert.Equal(expected.LaborData[i].KeimID, actual.LaborData[i].KeimID);
				Assert.Equal(expected.LaborData[i].LabordatenID, actual.LaborData[i].LabordatenID);
				Assert.Equal(expected.LaborData[i].MaterialID, actual.LaborData[i].MaterialID);
				Assert.Equal(expected.LaborData[i].Material_l, actual.LaborData[i].Material_l);
				Assert.Equal(expected.LaborData[i].ProbeID, actual.LaborData[i].ProbeID);
				Assert.Equal(expected.LaborData[i].ZeitpunktProbeneingang.ToString("s"), actual.LaborData[i].ZeitpunktProbeneingang.ToUniversalTime().ToString("s"));
				Assert.Equal(expected.LaborData[i].ZeitpunktProbenentnahme.ToString("s"), actual.LaborData[i].ZeitpunktProbenentnahme.ToUniversalTime().ToString("s"));
			}
		}

		private class ContactNetworkTestData : IEnumerable<object[]>
		{
			public IEnumerator<object[]> GetEnumerator()
            {
				List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
				yield return new object[] { 1, patient[0].EHR_ID, 2021, 1, 1, 2021, 1, 10, 0 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

		private ContactModel getExpectedContactModels(int ResultSetID)
		{
			List<PatientIDs> patients = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
			Dictionary<int, ContactModel> ResultSet = new Dictionary<int, ContactModel>
			{
				{ 
					0, 
					new ContactModel()
					{
						PatientMovements = new List<PatientMovementModel>
						{
							new PatientMovementModel
							{
								PatientID = patients[0].EHR_ID,
								FallID = "00000001",
								BewegungstypID = 1,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Aufnahme",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 1, 9, 0, 0),
								Ende = new DateTime(2021, 1, 1, 9, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[0].EHR_ID,
								FallID = "00000001",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 1, 9, 0, 0),
								Ende = new DateTime(2021, 1, 5, 15, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[0].EHR_ID,
								FallID = "00000001",
								BewegungstypID = 2,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Entlassung",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 5, 15, 0, 0),
								Ende = new DateTime(2021, 1, 5, 15, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[1].EHR_ID,
								FallID = "00000002",
								BewegungstypID = 1,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Aufnahme",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
								Ende = new DateTime(2021, 1, 2, 9, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[1].EHR_ID,
								FallID = "00000002",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
								Ende = new DateTime(2021, 1, 7, 15, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[1].EHR_ID,
								FallID = "00000002",
								BewegungstypID = 2,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Entlassung",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 7, 15, 0, 0),
								Ende = new DateTime(2021, 1, 7, 15, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[3].EHR_ID,
								FallID = "00000004",
								BewegungstypID = 1,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Aufnahme",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 3, 9, 0, 0),
								Ende = new DateTime(2021, 1, 3, 9, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[3].EHR_ID,
								FallID = "00000004",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 3, 9, 0, 0),
								Ende = new DateTime(2021, 1, 9, 15, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[3].EHR_ID,
								FallID = "00000004",
								BewegungstypID = 2,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Entlassung",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 9, 15, 0, 0),
								Ende = new DateTime(2021, 1, 9, 15, 0, 0)
							},
							 new PatientMovementModel
							{
								PatientID = patients[2].EHR_ID,
								FallID = "00000003",
								BewegungstypID = 1,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Aufnahme",
								Raum = "Zimmerkennung 101",
								StationID = "Stationskennung X",
								Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
								Ende = new DateTime(2021, 1, 2, 9, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[2].EHR_ID,
								FallID = "00000003",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Stationskennung X",
								Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
								Ende = new DateTime(2021, 1, 3, 11, 0, 0)
							}
							,new PatientMovementModel
							{
								PatientID = patients[2].EHR_ID,
								FallID = "00000003",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 3, 11, 0, 0),
								Ende = new DateTime(2021, 1, 9, 15, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[2].EHR_ID,
								FallID = "00000003",
								BewegungstypID = 2,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Entlassung",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 9, 15, 0, 0),
								Ende = new DateTime(2021, 1, 9, 15, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[5].EHR_ID,
								FallID = "00000006",
								BewegungstypID = 1,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Aufnahme",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 4, 9, 0, 0),
								Ende = new DateTime(2021, 1, 4, 9, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[5].EHR_ID,
								FallID = "00000006",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 4, 9, 0, 0),
								Ende = DateTime.Now
							},
							new PatientMovementModel
							{
								PatientID = patients[4].EHR_ID,
								FallID = "00000005",
								BewegungstypID = 1,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Aufnahme",
								Raum = "Zimmerkennung 101",
								StationID = "Stationskennung X",
								Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
								Ende = new DateTime(2021, 1, 2, 9, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[4].EHR_ID,
								FallID = "00000005",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Stationskennung X",
								Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
								Ende = new DateTime(2021, 1, 4, 15, 30, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[4].EHR_ID,
								FallID = "00000005",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 4, 15, 30, 0),
								Ende = new DateTime(2021, 1, 6, 16, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[4].EHR_ID,
								FallID = "00000005",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Stationskennung Y",
								Beginn = new DateTime(2021, 1, 6, 16, 0, 0),
								Ende = new DateTime(2021, 1, 8, 14, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[4].EHR_ID,
								FallID = "00000005",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 8, 14, 0, 0),
								Ende = DateTime.Now
							},
							new PatientMovementModel
							{
								PatientID = patients[6].EHR_ID,
								FallID = "00000007",
								BewegungstypID = 1,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Aufnahme",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 5, 9, 0, 0),
								Ende = new DateTime(2021, 1, 5, 9, 0, 0)
							},
							new PatientMovementModel
							{
								PatientID = patients[6].EHR_ID,
								FallID = "00000007",
								BewegungstypID = 3,
								Bewegungsart_l = "Diagn./Therap.",
								Bewegungstyp = "Wechsel",
								Raum = "Zimmerkennung 101",
								StationID = "Coronastation",
								Beginn = new DateTime(2021, 1, 5, 9, 0, 0),
								Ende = DateTime.Now
							}
						},	
						LaborData = new List<LabDataModel>
                        {
							new LabDataModel
							{
								PatientID = patients[0].EHR_ID,
								FallID = "00000001",
								Befund = true,
								Befunddatum = new DateTime(2021, 1, 1, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "01",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "01",
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 1, 9, 30, 0),
								ZeitpunktProbeneingang = new DateTime(2021, 1, 1, 10, 0, 0)
							},
							new LabDataModel
							{
								PatientID = patients[0].EHR_ID,
								FallID = "00000001",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 3, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "02",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "02",
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 3, 9, 30, 0),
								ZeitpunktProbeneingang = new DateTime(2021, 1, 3, 10, 0, 0)
							},new LabDataModel
							{
								PatientID = patients[0].EHR_ID,
								FallID = "00000001",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 5, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "03",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "03",
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 5, 9, 30, 0),
								ZeitpunktProbeneingang = new DateTime(2021, 1, 5, 10, 0, 0)
							},
							new LabDataModel
							{
								PatientID = patients[1].EHR_ID,
								FallID = "00000002",
								Befund = true,
								Befunddatum = new DateTime(2021, 1, 2, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "01",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "01",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 2, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 2, 9, 30, 0)
							},new LabDataModel
							{
								PatientID = patients[1].EHR_ID,
								FallID = "00000002",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 4, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "02",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "02",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 4, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 4, 9, 30, 0)
							},
							new LabDataModel
							{
								PatientID = patients[1].EHR_ID,
								FallID = "00000002",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 7, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "03",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "03",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 7, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 7, 9, 30, 0)
							},
							new LabDataModel
							{
								PatientID = patients[3].EHR_ID,
								FallID = "00000004",
								Befund = true,
								Befunddatum = new DateTime(2021, 1, 3, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "01",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "01",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 3, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 3, 9, 30, 0)
							},new LabDataModel
							{
								PatientID = patients[3].EHR_ID,
								FallID = "00000004",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 6, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "02",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "02",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 6, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 6, 9, 30, 0)
							},
							new LabDataModel
							{
								PatientID = patients[3].EHR_ID,
								FallID = "00000004",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 9, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "03",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "03",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 9, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 9, 9, 30, 0)
							},
							 new LabDataModel
							{
								PatientID = patients[2].EHR_ID,
								FallID = "00000003",
								Befund = true,
								Befunddatum = new DateTime(2021, 1, 3, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "01",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "01",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 3, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 3, 9, 30, 0)
							},
							new LabDataModel
							{
								PatientID = patients[2].EHR_ID,
								FallID = "00000003",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 7, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "02",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "02",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 7, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 7, 9, 30, 0)
							},new LabDataModel
							{
								PatientID = patients[2].EHR_ID,
								FallID = "00000003",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 9, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "03",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "03",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 9, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 9, 9, 30, 0)
							},
							new LabDataModel
							{
								PatientID = patients[5].EHR_ID,
								FallID = "00000006",
								Befund = true,
								Befunddatum = new DateTime(2021, 1, 4, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "01",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "01",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 4, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 4, 9, 30, 0)
							},
							new LabDataModel
							{
								PatientID = patients[5].EHR_ID,
								FallID = "00000006",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 9, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "02",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "02",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 9, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 9, 9, 30, 0)
							},
							 new LabDataModel
							{
								PatientID = patients[4].EHR_ID,
								FallID = "00000005",
								Befund = true,
								Befunddatum = new DateTime(2021, 1, 4, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "01",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "01",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 4, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 4, 9, 30, 0)
							},
							new LabDataModel
							{
								PatientID = patients[4].EHR_ID,
								FallID = "00000005",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 8, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "02",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "02",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 8, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 8, 9, 30, 0)
							},
							new LabDataModel
							{
								PatientID = patients[6].EHR_ID,
								FallID = "00000007",
								Befund = true,
								Befunddatum = new DateTime(2021, 1, 5, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "01",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "01",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 5, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 5, 9, 30, 0)
							},
							new LabDataModel
							{
								PatientID = patients[6].EHR_ID,
								FallID = "00000007",
								Befund = false,
								Befunddatum = new DateTime(2021, 1, 9, 9, 30, 0),
								Befundkommentar = "Kommentar 1",
								KeimID = "94500-6",
								LabordatenID = "02",
								MaterialID = "119342007",
								Material_l = "Salvia specimen (specimen)",
								ProbeID = "02",
								ZeitpunktProbeneingang = new DateTime(2021, 1, 9, 10, 0, 0),
								ZeitpunktProbenentnahme = new DateTime(2021, 1, 9, 9, 30, 0)
							}
						}
					}
				}	
			};

			return ResultSet[ResultSetID];
		}
		
		private PatientInformation CreatePatientInformation(IRestDataAccess rest)
        {

			IPatientMovementFactory patMoveFac = new PatientMovementFactory(rest);
			IPatientLabordataFactory patLabFac = new PatientLabordataFactory(rest);
			ISymptomFactory symptomFac = new SymptomFactory(rest);
			IMibiPatientLaborDataFactory mibiLabFac = new MibiPatientLaborDataFactory(rest);

			return new PatientInformation(patMoveFac, patLabFac, symptomFac, mibiLabFac);

		}
    }
}
