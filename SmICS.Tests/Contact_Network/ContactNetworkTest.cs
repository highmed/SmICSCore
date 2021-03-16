using SmICSCoreLib.AQL.Contact_Nth_Network;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using Xunit;

namespace SmICSDataGenerator.Tests.Contact_Network
{
    public class ContactNetworkTest
    {
        [Fact]
        public void ProcessorTest()
        {
			IRestDataAccess _data = TestConnection.Initialize();

            ContactParameter contactParams = new ContactParameter()
            {
                Degree = 1,
                PatientID = "",
                Starttime = new DateTime(),
                Endtime = new DateTime()
            };

            ContactNetworkFactory factory = new ContactNetworkFactory(_data);
            List<ContactModel> actual = factory.Process(contactParams);
            List<ContactModel> expected = getExpectedContactModels();

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

        private List<ContactModel> getExpectedContactModels()
        {
			Patient ehrID = SmICSCoreLib.JSONFileStream.JSONReader<Patient>.ReadSingle(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
            return new List<ContactModel>()
            {
				new ContactModel
				{
					paID = ehrID.Patient17,
					pbID = ehrID.Patient18,
					Beginn = DateTime.Parse("2021-01-02 09:00:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient17,
					pbID = ehrID.Patient19,
					Beginn = DateTime.Parse("2021-01-03 11:00:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient17,
					pbID = ehrID.Patient20,
					Beginn = DateTime.Parse("2021-01-03 09:00:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient17,
					pbID = ehrID.Patient21,
					Beginn = DateTime.Parse("2021-01-04 15:30:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient17,
					pbID = ehrID.Patient22,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-05 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient19,
					Beginn = DateTime.Parse("2021-01-03 11:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient20,
					Beginn = DateTime.Parse("2021-01-03 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient21,
					Beginn = DateTime.Parse("2021-01-04 15:30:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient22,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient18,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient21,
					Beginn = DateTime.Parse("2021-01-02 09:00:00"),
					Ende = DateTime.Parse("2021-01-03 11:00:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-03 11:00:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-03 11:00:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient20,
					Beginn = DateTime.Parse("2021-01-03 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient21,
					Beginn = DateTime.Parse("2021-01-04 15:30:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient22,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient19,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-01-07 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient21,
					Beginn = DateTime.Parse("2021-01-04 15:30:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient21,
					Beginn = DateTime.Parse("2021-01-08 14:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient22,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient20,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-01-09 15:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-04 15:30:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-04 15:30:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-01 09:00:00"),
					Ende = DateTime.Parse("2021-01-04 15:30:00"),
					Grad = 1,
					StationID = "Kardiologie"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient22,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-01-06 16:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient22,
					Beginn = DateTime.Parse("2021-01-04 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient21,
					pbID = ehrID.Patient34,
					Beginn = DateTime.Parse("2021-01-10 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient23,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient24,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient25,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient26,
					Beginn = DateTime.Parse("2021-01-06 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient27,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient28,
					Beginn = DateTime.Parse("2021-01-07 16:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient29,
					Beginn = DateTime.Parse("2021-01-07 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient30,
					Beginn = DateTime.Parse("2021-01-07 14:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient31,
					Beginn = DateTime.Parse("2021-01-08 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient32,
					Beginn = DateTime.Parse("2021-01-09 13:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
					pbID = ehrID.Patient33,
					Beginn = DateTime.Parse("2021-01-09 09:00:00"),
					Ende = DateTime.Parse("2021-02-01 00:00:00"),
					Grad = 1,
					StationID = "Coronastation"
				},
				new ContactModel
				{
					paID = ehrID.Patient22,
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

			};
        }
    }
}
