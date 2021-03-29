using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientLabordataTest
    {

        [Theory]
        [ClassData(typeof(LabTestData))]
        public void ProcessorTest(string ehrID, int expectedResultSet)
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { ehrID }
            };

            PatientLabordataFactory factory = new PatientLabordataFactory(_data);
            List<LabDataModel> actual = factory.Process(patientParams);
            List<LabDataModel> expected = GetExpectedLabDataModels(expectedResultSet);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].FallID, actual[i].FallID);
                Assert.Equal(expected[i].Befund, actual[i].Befund);
                Assert.Equal(expected[i].Befunddatum.ToString("s"), actual[i].Befunddatum.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].Befundkommentar, actual[i].Befundkommentar);
                Assert.Equal(expected[i].KeimID, actual[i].KeimID);
                Assert.Equal(expected[i].LabordatenID, actual[i].LabordatenID);
                Assert.Equal(expected[i].MaterialID, actual[i].MaterialID);
                Assert.Equal(expected[i].Material_l, actual[i].Material_l);
                Assert.Equal(expected[i].ProbeID, actual[i].ProbeID);
                Assert.Equal(expected[i].ZeitpunktProbeneingang.ToString("s"), actual[i].ZeitpunktProbeneingang.ToUniversalTime().ToString("s"));
                Assert.Equal(expected[i].ZeitpunktProbenentnahme.ToString("s"), actual[i].ZeitpunktProbenentnahme.ToUniversalTime().ToString("s"));
                //Assert.Equal(expected[i].Fachabteilung, actual[i].Fachabteilung); --> Exisitiert noch nicht, muss aber eingebunden werden
            }
        }

        private class LabTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
                for (int i = 0; i <= 17; i++)
                {
                    yield return new object[] { patient[i].EHR_ID, i };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<LabDataModel> GetExpectedLabDataModels(int ResultSetID)
        {
            List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
            Dictionary<int, List<LabDataModel>> ResultSet = new Dictionary<int, List<LabDataModel>>
            {
                {
                    0,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[0].EHR_ID,
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
                            PatientID = patient[0].EHR_ID,
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
                            PatientID = patient[0].EHR_ID,
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
                        }
                    }
                },
                {
                    1,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[1].EHR_ID,
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
                            PatientID = patient[1].EHR_ID,
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
                            PatientID = patient[1].EHR_ID,
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
                        }
                    }
                },
                {
                    2,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[2].EHR_ID,
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
                            PatientID = patient[2].EHR_ID,
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
                            PatientID = patient[2].EHR_ID,
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
                        }
                    }
                },
                {
                    3,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[3].EHR_ID,
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
                            PatientID = patient[3].EHR_ID,
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
                            PatientID = patient[3].EHR_ID,
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
                        }
                    }
                },
                {
                    4,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[4].EHR_ID,
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
                            PatientID = patient[4].EHR_ID,
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
                        }
                    }
                },
                {
                    5,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[5].EHR_ID,
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
                            PatientID = patient[5].EHR_ID,
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
                        }
                    }
                },
                {
                    6,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[6].EHR_ID,
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
                            PatientID = patient[6].EHR_ID,
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
                },
                {
                    7,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[7].EHR_ID,
                            FallID = "00000008",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 6, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 6, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 6, 9, 30, 0)
                        },
                        new LabDataModel
                        {
                            PatientID = patient[7].EHR_ID,
                            FallID = "00000008",
                            Befund = false,
                            Befunddatum = new DateTime(2021, 1, 10, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "02",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "02",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 10, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 10, 9, 30, 0)
                        }
                    }
                },
                {
                    8,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[8].EHR_ID,
                            FallID = "00000009",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 6, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 6, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 6, 9, 30, 0)
                        },
                        new LabDataModel
                        {
                            PatientID = patient[8].EHR_ID,
                            FallID = "00000009",
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
                },
                {
                    9,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[9].EHR_ID,
                            FallID = "00000010",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 6, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 6, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 6, 9, 30, 0)
                        },
                        new LabDataModel
                        {
                            PatientID = patient[9].EHR_ID,
                            FallID = "00000010",
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
                        }
                    }
                },
                {
                    10,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[10].EHR_ID,
                            FallID = "00000011",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 7, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 7, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 7, 9, 30, 0)
                        },
                        new LabDataModel
                        {
                            PatientID = patient[10].EHR_ID,
                            FallID = "00000011",
                            Befund = false,
                            Befunddatum = new DateTime(2021, 1, 11, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "02",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "02",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 11, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 11, 9, 30, 0)
                        }
                    }
                },
                {
                    11,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[11].EHR_ID,
                            FallID = "00000012",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 7, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 7, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 7, 9, 30, 0)
                        },
                        new LabDataModel
                        {
                            PatientID = patient[11].EHR_ID,
                            FallID = "00000012",
                            Befund = false,
                            Befunddatum = new DateTime(2021, 1, 10, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "02",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "02",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 10, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 10, 9, 30, 0)
                        }
                    }
                },
                {
                    12,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[12].EHR_ID,
                            FallID = "00000013",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 7, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 7, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 7, 9, 30, 0)
                        },
                        new LabDataModel
                        {
                            PatientID = patient[12].EHR_ID,
                            FallID = "00000013",
                            Befund = false,
                            Befunddatum = new DateTime(2021, 1, 12, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "02",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "02",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 12, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 12, 9, 30, 0)
                        }
                    }
                },
                {
                    13,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[13].EHR_ID,
                            FallID = "00000014",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 7, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 7, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 7, 9, 30, 0)
                        },
                        new LabDataModel
                        {
                            PatientID = patient[13].EHR_ID,
                            FallID = "00000014",
                            Befund = false,
                            Befunddatum = new DateTime(2021, 1, 11, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "02",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "02",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 11, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 11, 9, 30, 0)
                        }
                    }
                },
                {
                    14,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[14].EHR_ID,
                            FallID = "00000015",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 8, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 8, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 8, 9, 30, 0)
                        },
                        new LabDataModel
                        {
                            PatientID = patient[14].EHR_ID,
                            FallID = "00000015",
                            Befund = false,
                            Befunddatum = new DateTime(2021, 1, 10, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "02",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "02",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 10, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 10, 9, 30, 0)
                        }
                    }
                },
                {
                    15,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[15].EHR_ID,
                            FallID = "00000016",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 9, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 9, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 9, 9, 30, 0)
                        }
                    }
                },
                {
                    16,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[16].EHR_ID,
                            FallID = "00000017",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 9, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 9, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 9, 9, 30, 0)
                        }
                    }
                },
                {
                    17,
                    new List<LabDataModel>()
                    {
                        new LabDataModel
                        {
                            PatientID = patient[17].EHR_ID,
                            FallID = "00000018",
                            Befund = true,
                            Befunddatum = new DateTime(2021, 1, 10, 9, 30, 0),
                            Befundkommentar = "Kommentar 1",
                            KeimID = "94500-6",
                            LabordatenID = "01",
                            MaterialID = "119342007",
                            Material_l = "Salvia specimen (specimen)",
                            ProbeID = "01",
                            ZeitpunktProbeneingang = new DateTime(2021, 1, 10, 10, 0, 0),
                            ZeitpunktProbenentnahme = new DateTime(2021, 1, 10, 9, 30, 0)
                        }
                    }
                }
            };
            return ResultSet[ResultSetID];
        }

       
    }
}
