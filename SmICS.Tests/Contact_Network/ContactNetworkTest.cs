using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.ContactNetwork;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Lab.ViroLabData;
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
		public void ProcessorTest(int degree, int ehrNo, int start_year, int start_month, int start_day, int end_year, int end_month, int end_day, int expectedResultSet)
		{
			IRestDataAccess _data = TestConnection.Initialize();
			List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../TestData/GeneratedEHRIDs.json");

			ContactParameter contactParams = new ContactParameter()
			{
				Degree = degree,
				PatientID = patient[ehrNo].EHR_ID,
				Starttime = new DateTime(start_year, start_month, start_day),
				Endtime = new DateTime(end_year, end_month, end_day)
			};

			IPatientMovementFactory patMoveFac = new PatientMovementFactory(_data, NullLogger<PatientMovementFactory>.Instance);
			IViroLabDataFactory patLabFac = new ViroLabDataFactory(_data, NullLogger<ViroLabDataFactory>.Instance);
			ContactNetworkFactory factory = new ContactNetworkFactory(_data, patMoveFac, patLabFac, NullLogger<ContactNetworkFactory>.Instance);
			ContactModel actual = factory.Process(contactParams);
			ContactModel expected = getExpectedContactModels(expectedResultSet, ehrNo);

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
                Assert.Equal(expected.LaborData[i].ZeitpunktProbeneingang.Value.ToString("s"), actual.LaborData[i].ZeitpunktProbeneingang.Value.ToUniversalTime().ToString("s"));
                Assert.Equal(expected.LaborData[i].ZeitpunktProbenentnahme.ToString("s"), actual.LaborData[i].ZeitpunktProbenentnahme.ToUniversalTime().ToString("s"));
            }
        }

		private class ContactNetworkTestData : IEnumerable<object[]>
		{
			public IEnumerator<object[]> GetEnumerator()
			{
				yield return new object[] { 1, 0, 2020, 11, 25, 2020, 12, 25, 0 };
				yield return new object[] { 1, 1, 2020, 11, 25, 2020, 12, 25, 1 };
			}
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		private ContactModel getExpectedContactModels(int ResultSetID, int ehrNo)
		{
			Dictionary<int, List<int>> ResultSet = new Dictionary<int, List<int>>
			{
				{
					0,
					new List<int> { 1, 2, 0, 3 }
				},
				{
					1,
					new List<int> { 1, 2, 0 , 3 }
				}
			};

			return CreateExpactedContactModel(ResultSet[ResultSetID], ehrNo);
		}

		private ContactModel CreateExpactedContactModel(List<int> contactOrder, int ehrNo)
		{
			string movementPath = "../../../../TestData/PatientMovementTestResults.json";
			string labPath = "../../../../TestData/LabDataTestResults.json";
			string parameterPath = "../../../../TestData/GeneratedEHRIDs.json";

			List<PatientMovementModel> moveResults = new List<PatientMovementModel>();
			List<LabDataModel> labDataResults = new List<LabDataModel>();
			foreach (int i in contactOrder)
			{
				ehrNo = i;
				List<PatientMovementModel> movementResult = ExpectedResultJsonReader.ReadResults<PatientMovementModel, PatientIDs>(movementPath, parameterPath, i, ehrNo, ExpectedType.PATIENT_MOVEMENT);
				List<LabDataModel> labResult = ExpectedResultJsonReader.ReadResults<LabDataModel, PatientIDs>(labPath, parameterPath, i, ehrNo, ExpectedType.LAB_DATA);

				moveResults.AddRange(movementResult);
				labDataResults.AddRange(labResult);
			}
			ContactModel contacts = new ContactModel { LaborData = labDataResults, PatientMovements = moveResults };
			return contacts;
		}
	}
}
