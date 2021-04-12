using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.AQL.Contact_Nth_Network;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.REST;
using SmICSFactory.Tests;
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
				List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");
				yield return new object[] { 1, patient[0].EHR_ID, 2021, 1, 1, 2021, 1, 10, 0 };
				yield return new object[] { 1, patient[1].EHR_ID, 2021, 1, 1, 2021, 1, 10, 1 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

		private ContactModel getExpectedContactModels(int ResultSetID)
		{
			Dictionary<int, List<int>> ResultSet = new Dictionary<int, List<int>>
			{
				{
                    0,
					new List<int> { 0, 1, 3, 2, 5, 4, 6 }
				},
                {
					1,
					new List<int> { 0, 1, 3, 2, 5, 4, 6, 8, 7, 9, 11, 12, 13 }
                }
			};

			return CreateExpactedContactModel(ResultSet[ResultSetID]);
		}
		
		private PatientInformation CreatePatientInformation(IRestDataAccess rest)
        {

			IPatientMovementFactory patMoveFac = new PatientMovementFactory(rest, NullLogger<PatientMovementFactory>.Instance);
			IPatientLabordataFactory patLabFac = new PatientLabordataFactory(rest, NullLogger<PatientLabordataFactory>.Instance);
			ISymptomFactory symptomFac = new SymptomFactory(rest);
			IMibiPatientLaborDataFactory mibiLabFac = new MibiPatientLaborDataFactory(rest);

			return new PatientInformation(patMoveFac, patLabFac, symptomFac, mibiLabFac);

		}

		private ContactModel CreateExpactedContactModel(List<int> contactOrder)
        {
			string movementPath = "../../../../TestData/PatientMovementTestResults.json";
			string labPath = "../../../../TestData/LabDataTestResults.json";

			List<PatientMovementModel> moveResults = new List<PatientMovementModel>();
			List<LabDataModel> labDataResults = new List<LabDataModel>();
			foreach (int i in contactOrder)
			{
				List<PatientMovementModel> movementResult = ExpectedResultJsonReader.ReadResults<PatientMovementModel>(movementPath, i, ExpectedType.PATIENT_MOVEMENT);
				List<LabDataModel> labResult = ExpectedResultJsonReader.ReadResults<LabDataModel>(labPath, i, ExpectedType.LAB_DATA);

				moveResults.AddRange(movementResult);
				labDataResults.AddRange(labResult);
			}
			ContactModel contacts = new ContactModel { LaborData = labDataResults, PatientMovements = moveResults };
			return contacts;
		}
    }
}
