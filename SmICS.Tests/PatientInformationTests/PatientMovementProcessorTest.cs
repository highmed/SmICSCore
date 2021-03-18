using Autofac.Extras.Moq;
using SmICSCoreLib.AQL;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement.ReceiveModels;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;


namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientMovementProcessorTest
    {
        [Theory]
        [ClassData(typeof(PatientMovementTestData))]
        public void ProcessorTest(string ehrID, int ResultSetID) 
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { ehrID }
            };

            PatientMovementFactory factory = new PatientMovementFactory(_data);
            List<PatientMovementModel> actual = factory.Process(patientParams);
            List<PatientMovementModel> expected = GetExpectedPatientMovementModels(ResultSetID);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].FallID, actual[i].FallID);
                Assert.Equal(expected[i].Bewegungsart_l, actual[i].Bewegungsart_l);
                Assert.Equal(expected[i].Bewegungstyp, actual[i].Bewegungstyp);
                Assert.Equal(expected[i].BewegungstypID, actual[i].BewegungstypID);
                Assert.Equal(expected[i].Beginn.ToUniversalTime(), actual[i].Beginn.ToUniversalTime());
                Assert.Equal(expected[i].Ende.ToUniversalTime(), actual[i].Ende.ToUniversalTime());
                Assert.Equal(expected[i].StationID, actual[i].StationID);
                Assert.Equal(expected[i].Raum, actual[i].Raum);
                //Assert.Equal(expected[i].Fachabteilung, actual[i].Fachabteilung); 
            }
        }

        private class PatientMovementTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
                yield return new object[] { patient[0].EHR_ID, 0 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<PatientMovementModel> GetExpectedPatientMovementModels(int ResultSetID)
        {
            List<PatientIDs> patients = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
            Dictionary<int, List<PatientMovementModel>> ResultSet = new Dictionary<int, List<PatientMovementModel>>
            {
                { 
                    0, 
                    new List<PatientMovementModel>
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
                        }
                    }
                }
            };
            return ResultSet[ResultSetID];
        }


        /*new PatientMovementModel
               {
                   PatientID = ehrID.Patient18,
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
                   PatientID = ehrID.Patient18,
                   FallID = "00000002",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 2, 15, 0, 0)
               },new PatientMovementModel
               {
                   PatientID = ehrID.Patient18,
                   FallID = "00000002",
                   BewegungstypID = 2,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Entlassung",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 2, 15, 0, 0),
                   Ende = new DateTime(2021, 1, 2, 15, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient19,
                   FallID = "00000003",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 2, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient19,
                   FallID = "00000003",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 3, 11, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient19,
                   FallID = "00000003",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 3, 11, 0, 0),
                   Ende = new DateTime(2021, 1, 7, 15, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient19,
                   FallID = "00000003",
                   BewegungstypID = 2,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 7, 15, 0, 0),
                   Ende = new DateTime(2021, 1, 7, 15, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient20,
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
                   PatientID = ehrID.Patient20,
                   FallID = "00000004",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 3, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 3, 15, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient20,
                   FallID = "00000004",
                   BewegungstypID = 2,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Entlassung",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 3, 15, 0, 0),
                   Ende = new DateTime(2021, 1, 3, 15, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient21,
                   FallID = "00000005",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 2, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient21,
                   FallID = "00000005",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 2, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 4, 15, 30, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient21,
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
                   PatientID = ehrID.Patient21,
                   FallID = "00000005",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Intensivstation",
                   Beginn = new DateTime(2021, 1, 6, 16, 0, 0),
                   Ende = new DateTime(2021, 1, 8, 14, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient21,
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
                   PatientID = ehrID.Patient22,
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
                   PatientID = ehrID.Patient22,
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
                   PatientID = ehrID.Patient23,
                   FallID = "00000007",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 6, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 6, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient23,
                   FallID = "00000007",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 6, 9, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient24,
                   FallID = "00000008",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 6, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 6, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient24,
                   FallID = "00000008",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 6, 9, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient25,
                   FallID = "00000009",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 6, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 6, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient25,
                   FallID = "00000009",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 6, 9, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient26,
                   FallID = "00000010",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 6, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 6, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient26,
                   FallID = "00000010",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 6, 9, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient27,
                   FallID = "00000011",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 4, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 4, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient27,
                   FallID = "00000011",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 4, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 6, 8, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient27,
                   FallID = "00000011",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Radiologie",
                   Beginn = new DateTime(2021, 1, 6, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 7, 16, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient27,
                   FallID = "00000011",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 7, 16, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient28,
                   FallID = "00000012",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 7, 16, 0, 0),
                   Ende = new DateTime(2021, 1, 7, 16, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient28,
                   FallID = "00000012",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 7, 16, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient29,
                   FallID = "00000013",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 7, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 7, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient29,
                   FallID = "00000013",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 7, 9, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient30,
                   FallID = "00000014",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 1, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 1, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient30,
                   FallID = "00000014",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 1, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 7, 14, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient30,
                   FallID = "00000014",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 7, 14, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient31,
                   FallID = "00000015",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Radiologie",
                   Beginn = new DateTime(2021, 1, 7, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 7, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient31,
                   FallID = "00000015",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Radiologie",
                   Beginn = new DateTime(2021, 1, 7, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 8, 13, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient31,
                   FallID = "00000015",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 8, 13, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient32,
                   FallID = "00000016",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 1, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 1, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient32,
                   FallID = "00000016",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Kardiologie",
                   Beginn = new DateTime(2021, 1, 1, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 3, 12, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient32,
                   FallID = "00000016",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechse",
                   Raum = "Zimmerkennung 101",
                   StationID = "Radiologie",
                   Beginn = new DateTime(2021, 1, 3, 12, 0, 0),
                   Ende = new DateTime(2021, 1, 9, 13, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient32,
                   FallID = "00000016",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 9, 13, 0, 0),
                   Ende = new DateTime(2021, 1, 10, 20, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient32,
                   FallID = "00000016",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Intensivstation",
                   Beginn = new DateTime(2021, 1, 10, 20, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient33,
                   FallID = "00000017",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 9, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 9, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient33,
                   FallID = "00000017",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 9, 9, 0, 0),
                   Ende = DateTime.Now
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient34,
                   FallID = "00000018",
                   BewegungstypID = 1,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Aufnahme",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 10, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 10, 9, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient34,
                   FallID = "00000018",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Coronastation",
                   Beginn = new DateTime(2021, 1, 10, 9, 0, 0),
                   Ende = new DateTime(2021, 1, 10, 18, 0, 0)
               },
               new PatientMovementModel
               {
                   PatientID = ehrID.Patient34,
                   FallID = "00000018",
                   BewegungstypID = 3,
                   Bewegungsart_l = "Diagn./Therap.",
                   Bewegungstyp = "Wechsel",
                   Raum = "Zimmerkennung 101",
                   StationID = "Intensivstation",
                   Beginn = new DateTime(2021, 1, 10, 18, 0, 0),
                   Ende = DateTime.Now
               }             

       private List<EpisodeOfCareModel> GetMockingSampleEpisodeOfCare1000000001()
       {
           return new List<EpisodeOfCareModel>() {
               new EpisodeOfCareModel {
                   Beginn = new DateTime(2020, 12, 16, 8, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 16, 0, 0)
               }
           };
       }
       private List<EpisodeOfCareModel> GetMockingSampleEpisodeOfCare1000000002()
       {
           return new List<EpisodeOfCareModel>() {
               new EpisodeOfCareModel {
                   Beginn = new DateTime(2020, 12, 15, 8, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 16, 0, 0)
               }
           };
       }
       private List<PatientMovementModel> GetMockingExpectedSamples()
       {
           return new List<PatientMovementModel>()
           {
               new PatientMovementModel {
                   PatientID = "1000000001",
                   Beginn = new DateTime(2020, 12, 16, 8, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 8, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Station Corona",
                   Raum = "17",
                   FallID = "19784321",
                   BewegungstypID = 1,
                   Bewegungstyp = "Aufnahme"
               },
               new PatientMovementModel {
                   PatientID = "1000000001",
                   Beginn = new DateTime(2020, 12, 16, 8, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 12, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Station Corona",
                   Raum = "17",
                   FallID = "19784321",
                   BewegungstypID = 3,
                   Bewegungstyp = "Wechsel"
               },
               new PatientMovementModel {
                   PatientID = "1000000001",
                   Beginn = new DateTime(2020, 12, 16, 12, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 12, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Station Behandlung",
                   Raum = "Behandlungszimmer 01",
                   FallID = "19784321",
                   BewegungstypID = 4,
                   Bewegungstyp = "Behandlung"
               },
               new PatientMovementModel {
                   PatientID = "1000000001",
                   Beginn = new DateTime(2020, 12, 16, 12, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 16, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Corona Intensivstation",
                   Raum = "1",
                   FallID = "19784321",
                   BewegungstypID = 3,
                   Bewegungstyp = "Wechsel"
               },
               new PatientMovementModel {
                   PatientID = "1000000001",
                   Beginn = new DateTime(2020, 12, 16, 16, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 16, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Corona Intensivstation",
                   Raum = "1",
                   FallID = "19784321",
                   BewegungstypID = 2,
                   Bewegungstyp = "Entlassung"
               },
               new PatientMovementModel {
                   PatientID = "1000000002",
                   Beginn = new DateTime(2020, 12, 15, 8, 0, 0),
                   Ende = new DateTime(2020, 12, 15, 8, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Station Corona",
                   Raum = "13",
                   FallID = "19784327",
                   BewegungstypID = 1,
                   Bewegungstyp = "Aufnahme"
               },
               new PatientMovementModel {
                   PatientID = "1000000002",
                   Beginn = new DateTime(2020, 12, 15, 8, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 11, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Station Corona",
                   Raum = "13",
                   FallID = "19784327",
                   BewegungstypID = 3,
                   Bewegungstyp = "Wechsel"
               },
               new PatientMovementModel {
                   PatientID = "1000000002",
                   Beginn = new DateTime(2020, 12, 16, 11, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 11, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Station Behandlung",
                   Raum = "Behandlungszimmer 01",
                   FallID = "19784327",
                   BewegungstypID = 4,
                   Bewegungstyp = "Behandlung"
               },
               new PatientMovementModel {
                   PatientID = "1000000002",
                   Beginn = new DateTime(2020, 12, 16, 11, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 16, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Station Corona",
                   Raum = "13",
                   FallID = "19784327",
                   BewegungstypID = 3,
                   Bewegungstyp = "Wechsel"
               },
               new PatientMovementModel {
                   PatientID = "1000000002",
                   Beginn = new DateTime(2020, 12, 16, 16, 0, 0),
                   Ende = new DateTime(2020, 12, 16, 16, 0, 0),
                   Bewegungsart_l = "",
                   StationID = "Station Corona",
                   Raum = "13",
                   FallID = "19784327",
                   BewegungstypID = 2,
                   Bewegungstyp = "Entlassung"
               }*/      

    }
}
