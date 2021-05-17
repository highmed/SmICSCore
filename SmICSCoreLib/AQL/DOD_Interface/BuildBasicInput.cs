using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten.ReceiveModel;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Lab.InfectionsStatusDevelopmentCurve;
using System.Linq;

namespace SmICSCoreLib.AQL.DOD_Interface
{
    public class BuildBasicInput
    {
        private IRestDataAccess _restData;

        public BuildBasicInput(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public int[][] getTimeSeriesForDiseaseOutbreakDetectionAlgorithm(TimespanParameter timespanParameter, string kindOfInfection)
        {
            InfectionsStatusDevelopmentCurveFactory infectionsStatusDevelopmentCurveFactory = new InfectionsStatusDevelopmentCurveFactory(_restData);
            List<InfectionsStatusDevelopmentCurveModel> infectionsDevelopment_Process = infectionsStatusDevelopmentCurveFactory.Process(timespanParameter, kindOfInfection);

            /**/using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(@"O:\\Eigene Dateien\\Arbeitsverzeichnis\\Ausgaben_00001_210222.dat"))
            {
                foreach (InfectionsStatusDevelopmentCurveModel curInfectionsStatus in infectionsDevelopment_Process)
                {
                    outputFile.WriteLine("    {0}    {1}    {2}    {3}    {4}    {5}",
                                         curInfectionsStatus.Datum.Year,
                                         curInfectionsStatus.Datum.Month,
                                         curInfectionsStatus.Datum.Day,
                                         curInfectionsStatus.Anzahl,
                                         curInfectionsStatus.anzahl_gesamt,
                                         curInfectionsStatus.anzahl_gesamt_av7);
                }
            }

            int[] epochs = new int[infectionsDevelopment_Process.Count];
            int[] observed = new int[infectionsDevelopment_Process.Count];
            //PW20210217__List<int> epochs = new List<int>();
            //PW20210217__List<int> observed = new List<int>();

            int numberOfWeeks = (epochs.Length-(epochs.Length%7))/7;
            int startAtDay = 0;
            int[] epochs_av7 = new int[numberOfWeeks];
            int[] observed_av7 = new int[numberOfWeeks];

            for (int o = 0; o < infectionsDevelopment_Process.Count; o++)
            {
                epochs[o] = (int) (infectionsDevelopment_Process[o].Datum - new DateTime(1970, 1, 1)).TotalDays;
                observed[o] = infectionsDevelopment_Process[o].anzahl_gesamt;
                //PW20210217__epochs.Add((int) (infectionsDevelopment_Process[o].Datum - new DateTime(1970, 1, 1)).TotalDays);
                //PW20210217__observed.Add(infectionsDevelopment_Process[o].anzahl_gesamt);
            }

            for (int o = 0; o < numberOfWeeks; o++)
            {
                epochs_av7[o] = epochs[7*o + startAtDay];
                observed_av7[o] = observed[7*o + startAtDay];
            }

            //PW20210217__List<int> epochs_av7 = (List<int>)epochs.Where((c, index) => (index+1) % 7 == 0);
            //PW20210217__List<int> observed_av7 = (List<int>)observed.Where((c, index) => (index+1) % 7 == 0);

            /**/using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(@"O:\\Eigene Dateien\\Arbeitsverzeichnis\\Ausgaben_00001_210222.dat", true))
            {
                for (int o = 0; o < observed_av7.Length; o++)
                {
                    outputFile.WriteLine("    {0}    {1}",
                                         epochs_av7[o],
                                         observed_av7[o]);
                }
            }

            //PW20210217__return new [] {epochs.ToArray(), observed.ToArray(), epochs_av7.ToArray(), observed_av7.ToArray()};
            return new [] {epochs, observed, epochs_av7, observed_av7};
        }
    }
}
