using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.InfectionsStatus.ReceiveModel;
using SmICSCoreLib.AQL;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using SmICSCoreLib.Util;
using System.IO;
using SmICSCoreLib.AQL.InfectionsStatus;

using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using Microsoft.Extensions.Logging;

namespace SmICSCoreLib.AQL.InfectionsStatus
{
    public class InfectionsStatusFactory : IInfectionsStatusFactory
    {
        private Dictionary<string, SortedDictionary<DateTime, int>> dataAggregationStorage;
        private IRestDataAccess _restData;
        private ILogger<PatientMovementFactory> _logger;

        public InfectionsStatusFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public Dictionary<string, SortedDictionary<DateTime, int>> Process(TimespanParameter parameter, string kindOfFinding)
        {
            List<TimeDataPointModel> specimenIdentifierList = new List<TimeDataPointModel>();
            Dictionary<string, SortedDictionary<DateTime, int>> sortedStations = new();
            bool useAQLs = true;

            //BEGIN Different treatment for 'virological' and 'microbiological' finding
            //      = Change the test result description
            if (kindOfFinding == "virological")
            {
                specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatus(parameter));

                foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
                {
                    try
                    {
                        // Case for ''Not detected'' or ''Inconclusive'', i.e. ''negativ''
                        if (   curTimeDataPoint.CodeForTestResult == 260415000
                            || curTimeDataPoint.CodeForTestResult == 419984006)
                        {
                            curTimeDataPoint.VirologicalTestResult = "negativ";
                        }
                        // Case for curTimeDataPoint.VirologicalTestResult different from positiv/negativ.
                        // Case for ''Not detected'' already treated above.
                        else if (   curTimeDataPoint.CodeForTestResult != 260415000
                                 && curTimeDataPoint.CodeForTestResult != 419984006
                                 && curTimeDataPoint.CodeForTestResult != 260373001)
                        {
                            System.Diagnostics.Debug.WriteLine("Warning in InfectionsStatusDevelopmentCurveFactory");
                            System.Diagnostics.Debug.WriteLine(curTimeDataPoint.CodeForTestResult);
                        }
                        // Case for ''Detected'', i.e. ''positiv''
                        else if (curTimeDataPoint.CodeForTestResult == 260373001)
                        {
                            curTimeDataPoint.VirologicalTestResult = "positiv";
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(curTimeDataPoint.VirologicalTestResult);
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }

                // Faelle nach Stationen sortieren
                Dictionary<string, SortedDictionary<DateTime, string>> patientenAufenthalte = new ();
                List<string> patientenIDs = new List<string>();

                // Liste mit PatientIDs erstellen. Gleichzeitig die PatientIDs zum Dictionary hinzufuegen
                foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
                {
                    if (!patientenIDs.Exists(t => t == curTimeDataPoint.PatientID))
                    {
                        patientenIDs.Add(curTimeDataPoint.PatientID);
                    }
                }

                PatientMovementFactory pmf_00001 = new PatientMovementFactory(_restData, _logger);

                foreach (string curPatientID in patientenIDs)
                {
                    SortedDictionary<DateTime, string> curValueOfDictionary = new();
                    List<TimeDataPointModel> curPatientIDFound = specimenIdentifierList.FindAll(t => t.PatientID == curPatientID);
                    List<PatientLocation> patientLocations = new();

                    List<PatientMovementModel> movementsCurPatient
                    = pmf_00001.Process(new PatientListParameter{patientList = new (){curPatientID}});

                    foreach (TimeDataPointModel curTimeDataPoint in curPatientIDFound)
                    {
                        foreach (PatientMovementModel curMovementModel in movementsCurPatient)
                        {
                            if (   curTimeDataPoint.VirologicalTestResult == "positiv"
                                && curTimeDataPoint.Zeitpunkt >= curMovementModel.Beginn
                                && curTimeDataPoint.Zeitpunkt < curMovementModel.Ende
                                && curMovementModel.BewegungstypID > 1)
                            {
                                curValueOfDictionary.Add(curTimeDataPoint.Zeitpunkt, curMovementModel.StationID);
                            }
                        }
                    }
                    patientenAufenthalte.Add(curPatientID, curValueOfDictionary);
                }

                // Nach Station sortieren
                List<string> stationsIDs = new List<string>();

                // Liste mit StationenIDs erstellen.
                foreach (var item0 in patientenAufenthalte)
                {
                    SortedDictionary<DateTime, string> curValueOfDictionary = item0.Value;
                    foreach (var item1 in curValueOfDictionary)
                    {
                        string curItem1Value = item1.Value.Trim();
                        if (!stationsIDs.Exists(t => t == curItem1Value))
                        {
                            stationsIDs.Add(curItem1Value);
                        }
                    }
                }

                foreach (string curStationID in stationsIDs)
                {
                    int[] curCounts = new int[(int)(parameter.Endtime-parameter.Starttime).TotalDays + 1];
                    SortedDictionary<DateTime, int> curSortedDictionary = new();
                    foreach (var item0 in patientenAufenthalte)
                    {
                        SortedDictionary<DateTime, string> curValueOfDictionary = new();
                        foreach (var item1 in item0.Value)
                        {
                            if ((item1.Value).Trim() == curStationID)
                            {
                                curValueOfDictionary.Add(item1.Key, item1.Value);
                            }
                        }

                        if (!(curValueOfDictionary.Count == 0))
                        {
                            DateTime curDate = new(curValueOfDictionary.Keys.Min().Year,
                                                   curValueOfDictionary.Keys.Min().Month,
                                                   curValueOfDictionary.Keys.Min().Day,
                                                   curValueOfDictionary.Keys.Min().Hour,
                                                   curValueOfDictionary.Keys.Min().Minute,
                                                   curValueOfDictionary.Keys.Min().Second);
                            int curIndex = (int)(curDate - parameter.Starttime).TotalDays;
                            curCounts[curIndex] += 1;
                        }
                    }

                    for (int o = 0; o < curCounts.Length; o++)
                    {
                        curSortedDictionary.Add(parameter.Starttime.AddDays(o), curCounts[o]);
                    }
                    sortedStations.Add(curStationID, curSortedDictionary);
                }

                foreach (string curPatientID in patientenIDs)
                {
                    SortedDictionary<DateTime, string> curValueOfDictionary = patientenAufenthalte[curPatientID];
                }
            }
            else if (kindOfFinding == "microbiological")
            {
                //PW20210510__specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurveMicrobiologicalFinding(parameter).Query);
                //PW20210705__specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurveMicrobiologicalFinding(parameter));
                //PW20210817__specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurveMicrobiologicalFinding(parameter));
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("Microbiological switch");
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("The length of the list is in this case {0}", specimenIdentifierList.Count);
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("");

                /*foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
                {
                    try
                    {
                        // Case for ''Kein Nachweis'', i.e. ''negativ''
                        if (curTimeDataPoint.VirologicalTestResult.Length > " Nachweis ".Length)
                        {
                            curTimeDataPoint.VirologicalTestResult = "negativ";
                        }
                        // Case for ''Nachweis'', i.e. ''positiv''
                        else if (curTimeDataPoint.VirologicalTestResult.Length < "Kein Nachweis".Length-1)
                        {
                            curTimeDataPoint.VirologicalTestResult = "positiv";
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }*/

                foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
                {
                    System.Diagnostics.Debug.WriteLine("    {0}    {1}", curTimeDataPoint.Zeitpunkt.Date, curTimeDataPoint.VirologicalTestResult);

                    try
                    {
                        curTimeDataPoint.VirologicalTestResult = "positiv";
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
            }
            //END Different treatment for 'virological' and 'microbiological' finding

            return sortedStations;
        }
    }
}
