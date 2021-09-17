using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.Factories.EpiCurve;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using Xunit;


namespace SmICSDataGenerator.Tests.LabTests
{
    public class EpiCurveTest
    {
        [Fact]
        public void ProcessorTest()
        {
            RestDataAccess _data = TestConnection.Initialize();

            EpiCurveParameter parameter = new EpiCurveParameter
            {
                Starttime = new DateTime(2021, 1, 1),
                Endtime = new DateTime(2021, 1, 10),
                PathogenCodes = new List<string>() { "94500-6", "94745-7", "94558-4" }
            };

            EpiCurveFactory epiCurveFactory = new EpiCurveFactory(_data, NullLogger<EpiCurveFactory>.Instance);
            List<EpiCurveModel> actual = epiCurveFactory.Process(parameter);
            List<EpiCurveModel> expected = GetExpectedEpiCurveModels();

            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Anzahl, actual[i].Anzahl);
                Assert.Equal(expected[i].Anzahl_cs, actual[i].Anzahl_cs);
                Assert.Equal(expected[i].anzahl_gesamt, actual[i].anzahl_gesamt);
                Assert.Equal(expected[i].anzahl_gesamt_av28, actual[i].anzahl_gesamt_av28);
                Assert.Equal(expected[i].anzahl_gesamt_av7, actual[i].anzahl_gesamt_av7);
                Assert.Equal(expected[i].Datum, actual[i].Datum);
                Assert.Equal(expected[i].ErregerBEZL, actual[i].ErregerBEZL);
                Assert.Equal(expected[i].ErregerBEZL, actual[i].ErregerBEZL);
                Assert.Equal(expected[i].ErregerID, actual[i].ErregerID);
                Assert.Equal(expected[i].MAVG28, actual[i].MAVG28);
                Assert.Equal(expected[i].MAVG28_cs, actual[i].MAVG28_cs);
                Assert.Equal(expected[i].MAVG7, actual[i].MAVG7);
                Assert.Equal(expected[i].MAVG7_cs, actual[i].MAVG7_cs);
                Assert.Equal(expected[i].StationID, actual[i].StationID);
            }
        }

        private List<EpiCurveModel> GetExpectedEpiCurveModels() 
        {
                return new List<EpiCurveModel>()
                {
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 1,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 1),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 1,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 1),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 1),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 1),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 1),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 2,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 2),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 2,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 2),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 2),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 2),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 2),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    },
                    
                    new EpiCurveModel
                    {
                        Anzahl = 3,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 5,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 3),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 3),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 1,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 3),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 1,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 3),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 3),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 3,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 8,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 4),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 4,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 4),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 2,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 4),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 2,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 4),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 4),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    },
                    
                    new EpiCurveModel
                    {
                        Anzahl = 2,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 9,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 5),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 4,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 5),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 2,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 5),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 5),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 5),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 3,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 12,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 6),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 3,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 7,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 6),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 2,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 6),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 6),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 0,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 6),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    },
                    
                    new EpiCurveModel
                    {
                        Anzahl = 4,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 15,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 3,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 7),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 2,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 8,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 2,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 7),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 7),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 7),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 1,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 7),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 16,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 3,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 8),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 8,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 2,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 8),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    }, 
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 8),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 8),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 2,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 8),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    },

                    new EpiCurveModel
                    {
                        Anzahl = 2,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 16,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 3,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 9),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 8,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 2,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 9),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 2,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 1,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 9),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 1,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 9),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 9),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 17,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 3,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 10),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "klinik"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 1,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 9,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 2,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 10),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Coronastation"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 2,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 1,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 10),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Stationskennung X"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 1,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 10),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "ohne Stationsangabe"
                    },
                    new EpiCurveModel
                    {
                        Anzahl = 0,
                        Anzahl_cs = 0,
                        anzahl_gesamt = 3,
                        anzahl_gesamt_av28 = 0,
                        anzahl_gesamt_av7 = 0,
                        MAVG28 = 0,
                        MAVG28_cs = 0,
                        MAVG7 = 0,
                        MAVG7_cs = 0,
                        Datum = new DateTime(2021, 1, 10),
                        ErregerBEZL = "SARS-CoV-2 (COVID-19) RNA [Presence] in Respiratory specimen by NAA with probe detection",
                        ErregerBEZK = null,
                        ErregerID = "94500-6",
                        StationID = "Radiologie"
                    }
                };
        }
    }
}
