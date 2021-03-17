using SmICSCoreLib.AQL.Contact_Nth_Network;
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

			ContactNetworkFactory factory = new ContactNetworkFactory(_data);
			List<ContactModel> actual = factory.Process(contactParams);
			List<ContactModel> expected = getExpectedContactModels(expectedResultSet);

			Assert.Equal(expected.Count, actual.Count);

			for (int i = 0; i < actual.Count; i++)
			{
				Assert.Equal(expected[i].paID, actual[i].paID);
				Assert.Equal(expected[i].pbID, actual[i].pbID);
				Assert.Equal(expected[i].Beginn, actual[i].Beginn);
				Assert.Equal(expected[i].Ende, actual[i].Beginn);
				Assert.Equal(expected[i].StationID, actual[i].StationID);
				Assert.Equal(expected[i].Grad, actual[i].Grad);
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

		private List<ContactModel> getExpectedContactModels(int ResultSetID)
		{
			List<PatientIDs> patients = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
			Dictionary<int, List<ContactModel>> ResultSet = new Dictionary<int, List<ContactModel>>
			{
				{ 
					0, 
					new List<ContactModel>()
					{
						new ContactModel
						{
							paID = patients[0].EHR_ID,
							pbID = patients[1].EHR_ID,
							Beginn = DateTime.Parse("2021-01-02 09:00:00"),
							Ende = DateTime.Parse("2021-01-05 15:00:00"),
							Grad = 1,
							StationID = "Coronastation"
						},
						new ContactModel
						{
							paID = patients[0].EHR_ID,
							pbID = patients[2].EHR_ID,
							Beginn = DateTime.Parse("2021-01-03 11:00:00"),
							Ende = DateTime.Parse("2021-01-05 15:00:00"),
							Grad = 1,
							StationID = "Coronastation"
						},
						new ContactModel
						{
							paID = patients[0].EHR_ID,
							pbID = patients[3].EHR_ID,
							Beginn = DateTime.Parse("2021-01-03 09:00:00"),
							Ende = DateTime.Parse("2021-01-05 15:00:00"),
							Grad = 1,
							StationID = "Coronastation"
						},
						new ContactModel
						{
							paID = patients[0].EHR_ID,
							pbID = patients[4].EHR_ID,
							Beginn = DateTime.Parse("2021-01-04 15:30:00"),
							Ende = DateTime.Parse("2021-01-05 15:00:00"),
							Grad = 1,
							StationID = "Coronastation"
						},
						new ContactModel
						{
							paID = patients[0].EHR_ID,
							pbID = patients[5].EHR_ID,
							Beginn = DateTime.Parse("2021-01-04 09:00:00"),
							Ende = DateTime.Parse("2021-01-05 15:00:00"),
							Grad = 1,
							StationID = "Coronastation"
						},new ContactModel
						{
							paID = patients[0].EHR_ID,
							pbID = patients[5].EHR_ID,
							Beginn = DateTime.Parse("2021-01-04 09:00:00"),
							Ende = DateTime.Parse("2021-01-05 15:00:00"),
							Grad = 1,
							StationID = "Coronastation"
						}
					} }	
			};

			return ResultSet[ResultSetID];
		}
            /*return new List<ContactModel>()
            {
				new ContactModel
				{
					paID = patients[0].EHR_ID,
					pbID = patients[1].EHR_ID,
					Beginn = DateTime.Parse("2021-01-02 09:00:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[0].EHR_ID,
					pbID = patients[2].EHR_ID,
					Beginn = DateTime.Parse("2021-01-03 11:00:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[0].EHR_ID,
					pbID = patients[3].EHR_ID,
					Beginn = DateTime.Parse("2021-01-03 09:00:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[0].EHR_ID,
					pbID = patients[4].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 15:30:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[0].EHR_ID,
					pbID = patients[5].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = patients[2].EHR_ID,
					Beginn = DateTime.Parse("2021-01-03 11:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = patients[3].EHR_ID,
					Beginn = DateTime.Parse("2021-01-03 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = patients[4].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 15:30:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = patients[5].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[1].EHR_ID,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = patients[4].EHR_ID,
					Beginn = DateTime.Parse("2021-01-02 09:00:00"),
					Ende = DateTime.Parse("2021-01-03 11:00:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-03 11:00:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-03 11:00:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = patients[3].EHR_ID,
					Beginn = DateTime.Parse("2021-01-03 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = patients[4].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 15:30:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = patients[5].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[2].EHR_ID,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = patients[4].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 15:30:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = patients[4].EHR_ID,
					Beginn = DateTime.Parse("2021-01-08 14:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = patients[5].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[3].EHR_ID,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-04 15:30:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-04 15:30:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-04 15:30:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = patients[5].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = patients[5].EHR_ID,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[4].EHR_ID,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = patients[5].EHR_ID,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient23,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient24,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient25,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient25,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient25,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient25,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient25,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient25,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient25,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient25,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient25,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient26,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient26,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient26,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient26,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient26,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient26,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient26,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient26,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 08:00:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 08:00:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 16:00:00"),
					Grad = 1,
					StationID = "Radiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-03 12:00:00"),
					Ende = DateTime.Parse("2021-01-07 16:00:00"),
					Grad = 1,
					StationID = "Radiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient27,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient28,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient28,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient28,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient28,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient28,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient28,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient29,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient29,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient29,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient29,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient29,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient30,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 14:00:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient30,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient30,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient30,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient30,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient31,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-03 12:00:00"),
					Ende = DateTime.Parse("2021-01-08 13:00:00"),
					Grad = 1,
					StationID = "Radiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient31,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient31,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient31,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient32,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-01-10 20:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient32,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-01-10 20:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient32,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 18:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Intensivstation"
				},
				new ContactModel
				{
					paID = ehrID.Patient33,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				}

			};*/
        
    }
}
