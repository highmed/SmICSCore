using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using SmICSDataGenerator.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SmICSFactory.Tests
{
    public class InfectionStatusTest
    {
        [Theory]
        [ClassData(typeof(ÍnfectionStatusTestData))]
        public void ProcessTest(string patientID, string pathogenName)
        {
            RestDataAccess rest = TestConnection.Initialize();
            IAntibiogramFactory antiFac = new AntibiogramFactory(rest);
            IPathogenFactory pathoFac = new PathogenFactory(rest, antiFac);
            ISpecimenFactory specFac = new SpecimenFactory(rest, pathoFac);
            ILabResultFactory labFac = new LabResultFactory(rest, specFac);
            IHospitalizationFactory hosFac = new HospitalizationFactory(rest);
            IInfectionStatusFactory infectFac = new InfectionStatusFactory(rest, labFac, hosFac);

            Patient patient = new Patient() { PatientID = patientID };
            PathogenParameter pathogen = new PathogenParameter() { Name = pathogenName };
            SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> expected = Expected();
            SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> actual = infectFac.Process(patient, pathogen);

            Assert.Equal(expected.Keys.Count, actual.Keys.Count);

            for (int i = 0; i < expected.Keys.Count; i++)
            {
                Assert.Equal(expected.Keys[i].PatientID, actual.Keys[i].PatientID);
                Assert.Equal(expected.Keys[i].CaseID, actual.Keys[i].CaseID);
                Assert.Equal(expected.Keys[i].Admission.Date.ToUniversalTime().ToString("s"), actual.Keys[i].Admission.Date.ToUniversalTime().ToString("s"));
                Assert.Equal(expected.Keys[i].Discharge.Date.HasValue, actual.Keys[i].Discharge.Date.HasValue);
                if (expected.Keys[i].Discharge.Date.HasValue && actual.Keys[i].Discharge.Date.HasValue)
                {
                    Assert.Equal(expected.Keys[i].Discharge.Date.Value.ToUniversalTime().ToString("s"), actual.Keys[i].Discharge.Date.Value.ToUniversalTime().ToString("s"));
                }

                Assert.Equal(expected[expected.Keys[i]].Keys.Count, actual[actual.Keys[i]].Keys.Count);

                List<string> pathokeys = expected[expected.Keys[i]].Keys.ToList();
                foreach (string pat in pathokeys)
                {
                    Assert.True(actual[actual.Keys[i]].ContainsKey(pat));
                    foreach (string res in actual[actual.Keys[i]][pat].Keys)
                    {
                        Assert.True(expected[expected.Keys[i]][pat].ContainsKey(res));

                        Assert.Equal(expected[expected.Keys[i]][pat][res].ConsecutiveNegativeCounter, actual[actual.Keys[i]][pat][res].ConsecutiveNegativeCounter);
                        Assert.Equal(expected[expected.Keys[i]][pat][res].Infected, actual[actual.Keys[i]][pat][res].Infected);
                        Assert.Equal(expected[expected.Keys[i]][pat][res].Healed, actual[actual.Keys[i]][pat][res].Healed);
                        Assert.Equal(expected[expected.Keys[i]][pat][res].Known, actual[actual.Keys[i]][pat][res].Known);
                        Assert.Equal(expected[expected.Keys[i]][pat][res].Nosocomial, actual[actual.Keys[i]][pat][res].Nosocomial);
                        Assert.Equal(expected[expected.Keys[i]][pat][res].NosocomialDate.HasValue, actual[actual.Keys[i]][pat][res].NosocomialDate.HasValue);
                        if (expected[expected.Keys[i]][pat][res].NosocomialDate.HasValue && actual[actual.Keys[i]][pat][res].NosocomialDate.HasValue)
                        {
                            Assert.Equal(expected[expected.Keys[i]][pat][res].NosocomialDate.Value.ToUniversalTime().ToString("s"), actual[actual.Keys[i]][pat][res].NosocomialDate.Value.ToUniversalTime().ToString("s"));
                        }
                        Assert.Equal(expected[expected.Keys[i]][pat][res].Pathogen, actual[actual.Keys[i]][pat][res].Pathogen);
                        Assert.Equal(expected[expected.Keys[i]][pat][res].Resistance, actual[actual.Keys[i]][pat][res].Resistance);
                    }
                }
            }

        }

        private class ÍnfectionStatusTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "Patient100", "Staphylococcus aureus" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> Expected()
        {
            return new SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>>()
            {
                {
                    new Hospitalization() {
                        PatientID = "Patient100",
                        CaseID = "99999901",
                        Admission = new Admission() { Date = new DateTime(2021, 5, 28, 8, 0, 0, DateTimeKind.Utc), MovementTypeID = (int) MovementType.ADMISSION },
                        Discharge = new Discharge() { Date = new DateTime(2021, 6, 5, 16, 0, 0, DateTimeKind.Utc), MovementTypeID = (int) MovementType.DISCHARGE }
                        },
                    new Dictionary<string, Dictionary<string, InfectionStatus>>()
                    {
                        {
                            "Staphylococcus aureus",
                            new Dictionary<string, InfectionStatus>()
                            {
                                {
                                    "R",
                                    new InfectionStatus() {
                                        ConsecutiveNegativeCounter = 0,
                                        Healed = false,
                                        Infected = true,
                                        Known = false,
                                        Nosocomial = true, 
                                        NosocomialDate = new DateTime(2021, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                                        Pathogen = "Staphylococcus aureus",
                                        Resistance = "R"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
