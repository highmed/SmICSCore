using Autofac.Extras.Moq;
using SmICSCoreLib.AQL;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;


namespace SmICSDataGenerator.Tests.PatientInformationTests
{
    public class PatientSymptomTest
    {
        [Theory]
        [ClassData(typeof(PatientSymptomTestData))]
        public void ProcessorTest(string ehrID, int ResultSetID) 
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>() { ehrID }
            };

            SymptomFactory factory = new SymptomFactory(_data);
            List<SymptomModel> actual = factory.Process(patientParams);
            List<SymptomModel> expected = GetExpectedSymptomModels(ResultSetID);

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientenID, actual[i].PatientenID);
                Assert.Equal(expected[i].BefundDatum.ToUniversalTime(), actual[i].BefundDatum.ToUniversalTime());
                Assert.Equal(expected[i].NameDesSymptoms, actual[i].NameDesSymptoms);
                Assert.Equal(expected[i].Lokalisation, actual[i].Lokalisation);
                Assert.Equal(expected[i].Beginn.ToUniversalTime(), actual[i].Beginn.ToUniversalTime());
                Assert.Equal(expected[i].Schweregrad, actual[i].Schweregrad);
                Assert.Equal(expected[i].Rueckgang.ToUniversalTime(), actual[i].Rueckgang.ToUniversalTime());
                Assert.Equal(expected[i].AusschlussAussage, actual[i].AusschlussAussage);
                Assert.Equal(expected[i].Diagnose, actual[i].Diagnose);
                Assert.Equal(expected[i].UnbekanntesSymptom, actual[i].UnbekanntesSymptom);
                Assert.Equal(expected[i].AussageFehlendeInfo, actual[i].AussageFehlendeInfo);
            }
        }

        private class PatientSymptomTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                List<PatientIDs> patient = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
                yield return new object[] { patient[0].EHR_ID, 0 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private List<SymptomModel> GetExpectedSymptomModels(int ResultSetID)
        {
            List<PatientIDs> patients = SmICSCoreLib.JSONFileStream.JSONReader<PatientIDs>.Read(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
            Dictionary<int, List<SymptomModel>> ResultSet = new Dictionary<int, List<SymptomModel>>
            {
                { 
                    0, 
                    new List<SymptomModel>
                    {
                        new SymptomModel
                        {
                            PatientenID = patients[0].EHR_ID,
                            BefundDatum = new DateTime(2020, 12, 2, 12, 45, 0),
                            NameDesSymptoms = "Diarrhea (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 12, 2, 12, 45, 0),
                            Schweregrad = "Mild (qualifier value)",
                            Rueckgang = new DateTime(2020, 12, 6, 9, 30, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo = null

                        },
                        new SymptomModel
                        {
                            PatientenID = patients[0].EHR_ID,
                            BefundDatum = new DateTime(2020, 12, 2, 12, 45, 0),
                            NameDesSymptoms = "Vomiting (disorder)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 12, 2, 9, 47, 45),
                            Schweregrad = "Mild (qualifier value)",
                            Rueckgang = new DateTime(2020, 12, 4, 12, 34, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo = null
                        }
                    }
                },
                {
                    1,
                    new List<SymptomModel>
                    {
                        new SymptomModel
                        {
                            PatientenID = patients[1].EHR_ID,
                            BefundDatum = new DateTime(2020, 12, 3, 10, 47, 45),
                            NameDesSymptoms = "Diarrhea (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 12, 3, 9, 47, 45),
                            Schweregrad = "Mild (qualifier value)",
                            Rueckgang = new DateTime(2020, 12, 5, 9, 34, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo = null

                        },
                        new SymptomModel
                        {
                            PatientenID = patients[1].EHR_ID,
                            BefundDatum = new DateTime(2020, 12, 3, 22, 47, 45),
                            NameDesSymptoms = "Vomiting (disorder)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 12, 3, 22, 47, 45),
                            Schweregrad = "Moderate (severity modifier) (qualifier value)",
                            Rueckgang = new DateTime(2020, 12, 5, 1, 34, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo = null
                        }
                    }
                },
                {
                    2,
                    new List<SymptomModel>
                    {
                        new SymptomModel
                        {
                            PatientenID = patients[2].EHR_ID,
                            BefundDatum = new DateTime(2020, 12, 3, 9, 47, 45),
                            NameDesSymptoms = "Diarrhea (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 12, 3, 9, 47, 45),
                            Schweregrad = "Mild (qualifier value)",
                            Rueckgang = new DateTime(2020, 12, 5, 12, 34, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo = null

                        },
                        new SymptomModel
                        {
                            PatientenID = patients[2].EHR_ID,
                            BefundDatum = new DateTime(2020, 12, 3, 20, 47, 45),
                            NameDesSymptoms = "Vomiting (disorder)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 12, 3, 20, 47, 45),
                            Schweregrad = "Moderate (severity modifier) (qualifier value)",
                            Rueckgang = new DateTime(2020, 12, 5, 0, 34, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo = null
                        }
                    }
                },
                {
                    3,
                    new List<SymptomModel>
                    {
                        new SymptomModel
                        {
                            PatientenID = patients[3].EHR_ID,
                            BefundDatum = new DateTime(2020, 12, 5, 20, 47, 45),
                            NameDesSymptoms = "Vomiting (disorder)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 12, 5, 20, 47, 45),
                            Schweregrad = "Moderate (severity modifier) (qualifier value)",
                            Rueckgang = new DateTime(2020, 12, 6, 0, 34, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo = null
                        }
                    }
                },
                {
                    4,
                    new List<SymptomModel>
                    {
                        new SymptomModel
                        {
                            PatientenID = patients[4].EHR_ID,
                            BefundDatum = new DateTime(2020, 2, 10, 20, 47, 45),
                            NameDesSymptoms = "Cough (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 2, 10, 20, 47, 45),
                            Schweregrad = "Moderate (severity modifier) (qualifier value)",
                            Rueckgang = new DateTime(2020, 2, 17, 0, 34, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo = null
                        },
                        new SymptomModel
                        {
                            PatientenID = patients[4].EHR_ID,
                            BefundDatum = new DateTime(2020, 2, 10, 16, 29, 49),
                            NameDesSymptoms = "Fever (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 2, 10, 16, 29, 49),
                            Schweregrad = "Moderate (severity modifier) (qualifier value)",
                            Rueckgang = new DateTime(2020, 3, 14, 10, 35, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo =null
                        }
                    }
                },
                {
                    5,
                    new List<SymptomModel>
                    {
                        new SymptomModel
                        {
                            PatientenID = patients[5].EHR_ID,
                            BefundDatum = new DateTime(2020, 2, 13, 21, 47, 45),
                            NameDesSymptoms = "Cough (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 2, 13, 21, 47, 45),
                            Schweregrad = "Moderate (severity modifier) (qualifier value)",
                            Rueckgang = new DateTime(2020, 2, 22, 9, 35, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo =null
                        },
                        new SymptomModel
                        {
                            PatientenID = patients[5].EHR_ID,
                            BefundDatum = new DateTime(2020, 2, 13, 23, 59, 59),
                            NameDesSymptoms = "Feeling feverish (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 2, 13, 23, 59, 59),
                            Schweregrad = "Mild (qualifier value)",
                            Rueckgang = new DateTime(2020, 3, 17, 2, 35, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo =null
                        },
                        new SymptomModel
                        {
                            PatientenID = patients[5].EHR_ID,
                            BefundDatum = new DateTime(2020, 2, 13, 14, 59, 59),
                            NameDesSymptoms = "Fatigue (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 2, 13, 14, 59, 59),
                            Schweregrad = "Mild (qualifier value)",
                            Rueckgang = new DateTime(2020, 3, 21, 2, 35, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo =null
                        }
                    }
                },
                {
                    6,
                    new List<SymptomModel>
                    {
                        new SymptomModel
                        {
                            PatientenID = patients[6].EHR_ID,
                            BefundDatum = new DateTime(2020, 2, 13, 23, 49, 49),
                            NameDesSymptoms = "Cough (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 2, 13, 23, 49, 49),
                            Schweregrad = "Mild (qualifier value)",
                            Rueckgang = new DateTime(2020, 2, 25, 9, 35, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo =null
                        },
                        new SymptomModel
                        {
                            PatientenID = patients[6].EHR_ID,
                            BefundDatum = new DateTime(2020, 2, 13, 20, 59, 59),
                            NameDesSymptoms = "Feeling feverish (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 2, 13, 20, 59, 59),
                            Schweregrad = "Mild (qualifier value)",
                            Rueckgang = new DateTime(2020, 3, 19, 12, 35, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo =null
                        },
                        new SymptomModel
                        {
                            PatientenID = patients[6].EHR_ID,
                            BefundDatum = new DateTime(2020, 2, 13, 20, 59, 59),
                            NameDesSymptoms = "Fatigue (finding)",
                            Lokalisation = "Anatomische Lokalisation 51",
                            Beginn = new DateTime(2020, 2, 13, 20, 59, 59),
                            Schweregrad = "Mild (qualifier value)",
                            Rueckgang = new DateTime(2020, 3, 22, 12, 35, 20),
                            AusschlussAussage = null,
                            Diagnose = null,
                            UnbekanntesSymptom = null,
                            AussageFehlendeInfo =null
                        }
                    }
                },
                {
                    7,
                    new List<SymptomModel>
                    {
                        new SymptomModel
                        {

                        },
                        new SymptomModel
                        {

                        }
                    }
                }
            };
            return ResultSet[ResultSetID];
        }

    }
}
