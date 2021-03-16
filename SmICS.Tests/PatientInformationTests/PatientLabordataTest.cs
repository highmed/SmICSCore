using Autofac.Extras.Moq;
using SmICSCoreLib.AQL;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using Xunit;

namespace SmICSDataGenerator.Tests
{
    public class PatientLabordataTest
    {

        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();

            PatientListParameter patientParams = new PatientListParameter()
            {
                patientList = new List<string>()
            };

            PatientLabordataFactory factory = new PatientLabordataFactory(_data);
            List<LabDataModel> actual = factory.Process(patientParams);
            List<LabDataModel> expected = GetExpectedLabDataModels();

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                Assert.Equal(expected[i].FallID, actual[i].FallID);
                Assert.Equal(expected[i].Befund, actual[i].Befund);
                Assert.Equal(expected[i].Befunddatum, actual[i].Befunddatum);
                Assert.Equal(expected[i].Befundkommentar, actual[i].Befundkommentar);
                Assert.Equal(expected[i].KeimID, actual[i].KeimID);
                Assert.Equal(expected[i].LabordatenID, actual[i].LabordatenID);
                Assert.Equal(expected[i].MaterialID, actual[i].MaterialID);
                Assert.Equal(expected[i].Material_l, actual[i].Material_l);
                Assert.Equal(expected[i].ProbeID, actual[i].ProbeID);
                Assert.Equal(expected[i].ZeitpunktProbeneingang, actual[i].ZeitpunktProbeneingang);
                Assert.Equal(expected[i].ZeitpunktProbenentnahme, actual[i].ZeitpunktProbenentnahme);
                //Assert.Equal(expected[i].Fachabteilung, actual[i].Fachabteilung); --> Exisitiert noch nicht, muss aber eingebunden werden
            }
        }

        private List<LabDataModel> GetExpectedLabDataModels()
        {
            Patient ehrID = SmICSCoreLib.JSONFileStream.JSONReader<Patient>.ReadSingle(@"../../../../SmICSDataGenerator.Test/Resources/GeneratedEHRIDs.json");
            return new List<LabDataModel>()
            {
                new LabDataModel
                {
                    PatientID = ehrID.Patient17,
                    FallID = "00000001",
                    Befund = true,
                    Befunddatum = new DateTime(2021, 1, 1, 9, 30, 0),
                    Befundkommentar = "Kommentar 1",
                    KeimID = "94500-6",
                    LabordatenID = "01",
                    MaterialID = "119342007",
                    Material_l = "Salvia specimen (specimen)",
                    ProbeID = "01",
                    ZeitpunktProbeneingang = new DateTime(2021, 1, 1, 10, 0, 0),
                    ZeitpunktProbenentnahme = new DateTime(2021, 1, 1, 9, 30, 0)
                },
                new LabDataModel
                {
                    PatientID = ehrID.Patient17,
                    FallID = "00000001",
                    Befund = false,
                    Befunddatum = new DateTime(2021, 1, 3, 9, 30, 0),
                    Befundkommentar = "Kommentar 1",
                    KeimID = "94500-6",
                    LabordatenID = "02",
                    MaterialID = "119342007",
                    Material_l = "Salvia specimen (specimen)",
                    ProbeID = "02",
                    ZeitpunktProbeneingang = new DateTime(2021, 1, 3, 10, 0, 0),
                    ZeitpunktProbenentnahme = new DateTime(2021, 1, 3, 9, 30, 0)
                },new LabDataModel
                {
                    PatientID = ehrID.Patient17,
                    FallID = "00000001",
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
                    PatientID = ehrID.Patient18,
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
                    PatientID = ehrID.Patient18,
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
                    PatientID = ehrID.Patient18,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient19,
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
                    PatientID = ehrID.Patient19,
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
                    PatientID = ehrID.Patient19,
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
                    PatientID = ehrID.Patient20,
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
                    PatientID = ehrID.Patient20,
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
                    PatientID = ehrID.Patient20,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient21,
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
                    PatientID = ehrID.Patient21,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient22,
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
                    PatientID = ehrID.Patient22,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient23,
                    FallID = "00000007",
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
                    PatientID = ehrID.Patient23,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient24,
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
                    PatientID = ehrID.Patient24,
                    FallID = "00000008",
                    Befund = false,
                    Befunddatum = new DateTime(2021, 1, 1, 9, 30, 0),
                    Befundkommentar = "Kommentar 1",
                    KeimID = "94500-6",
                    LabordatenID = "02",
                    MaterialID = "119342007",
                    Material_l = "Salvia specimen (specimen)",
                    ProbeID = "02",
                    ZeitpunktProbeneingang = new DateTime(2021, 1, 10, 10, 0, 0),
                    ZeitpunktProbenentnahme = new DateTime(2021, 1, 10, 9, 30, 0)
                },new LabDataModel
                {
                    PatientID = ehrID.Patient25,
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
                    PatientID = ehrID.Patient25,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient26,
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
                    PatientID = ehrID.Patient26,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient27,
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
                    PatientID = ehrID.Patient27,
                    FallID = "00000011",
                    Befund = false,
                    Befunddatum = new DateTime(2021, 1, 1, 9, 30, 0),
                    Befundkommentar = "Kommentar 1",
                    KeimID = "94500-6",
                    LabordatenID = "02",
                    MaterialID = "119342007",
                    Material_l = "Salvia specimen (specimen)",
                    ProbeID = "02",
                    ZeitpunktProbeneingang = new DateTime(2021, 1, 11, 10, 0, 0),
                    ZeitpunktProbenentnahme = new DateTime(2021, 1, 11, 9, 30, 0)
                },new LabDataModel
                {
                    PatientID = ehrID.Patient28,
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
                    PatientID = ehrID.Patient28,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient29,
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
                    PatientID = ehrID.Patient29,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient30,
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
                    PatientID = ehrID.Patient30,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient31,
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
                    PatientID = ehrID.Patient31,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient32,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient33,
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
                },new LabDataModel
                {
                    PatientID = ehrID.Patient34,
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
            };
        }

        #region Mock
        [Fact]
        public void ProcessorMockTest()
        {
            using (var mock = AutoMock.GetLoose())
            {
                PatientListParameter patientList = new PatientListParameter() { patientList = new List<string> { "1000000001" } };

                mock.Mock<IRestDataAccess>().Setup(x => x.AQLQuery<LabDataReceiveModel>(AQLCatalog.PatientLaborData(patientList).Query)).Returns(getMockLabDataReceiveSamples());

                var cls = mock.Create<PatientLabordataFactory>();

                var expected = GetMockExpectedSamples();

                var actual = cls.Process(patientList);

                Assert.True(actual != null);
                Assert.Equal(expected.Count, actual.Count);

                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.Equal(expected[i].PatientID, actual[i].PatientID);
                    Assert.Equal(expected[i].FallID, actual[i].FallID);
                    Assert.Equal(expected[i].Befund, actual[i].Befund);
                    Assert.Equal(expected[i].Befunddatum, actual[i].Befunddatum);
                    Assert.Equal(expected[i].Befundkommentar, actual[i].Befundkommentar);
                    Assert.Equal(expected[i].ZeitpunktProbeneingang, actual[i].ZeitpunktProbeneingang);
                    Assert.Equal(expected[i].ZeitpunktProbenentnahme, actual[i].ZeitpunktProbenentnahme);
                    Assert.Equal(expected[i].LabordatenID, actual[i].LabordatenID);
                    Assert.Equal(expected[i].ProbeID, actual[i].ProbeID);
                    Assert.Equal(expected[i].KeimID, actual[i].KeimID);
                    Assert.Equal(expected[i].MaterialID, actual[i].MaterialID);
                    Assert.Equal(expected[i].Material_l, actual[i].Material_l);
                }
            }
        }

        private List<LabDataModel> GetMockExpectedSamples()
        {
            return new List<LabDataModel>() {
                new LabDataModel
                {
                    FallID = "12345678",
                    PatientID = "1000000001",
                    Befund = true,
                    KeimID = "COV",
                    MaterialID = "Abstrich",
                    Material_l = "Abstrich",
                    Befunddatum = new DateTime(2020, 12, 17, 9, 0, 0),
                    ZeitpunktProbenentnahme = new DateTime(2020, 12, 16, 12, 31, 0),
                    ProbeID = "111",
                    LabordatenID = "1111",
                    Befundkommentar = null,
                    ZeitpunktProbeneingang = DateTime.MinValue
                }
            };
        }

        private List<LabDataReceiveModel> getMockLabDataReceiveSamples()
        {
            return new List<LabDataReceiveModel>() {
                new LabDataReceiveModel
                {
                    FallID = "12345678",
                    PatientID = "1000000001",
                    Befund = "positiv",
                    KeimID = "COV",
                    MaterialID = "Abstrich",
                    Material_l = "Abstrich",
                    Befunddatum = new DateTime(2020, 12, 17, 9, 0, 0),
                    ZeitpunktProbenentnahme = new DateTime(2020, 12, 16, 12, 31, 0),
                    ProbeID = "111",
                    LabordatenID = "1111",
                    Befundkommentar = null,
                    ZeitpunktProbeneingang = DateTime.MinValue
                }
            };
        }
        #endregion
    }
}
