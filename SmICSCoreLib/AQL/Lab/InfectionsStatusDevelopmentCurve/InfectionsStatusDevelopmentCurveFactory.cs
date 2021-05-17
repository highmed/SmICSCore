using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Lab.InfectionsStatusDevelopmentCurve.ReceiveModel;
using SmICSCoreLib.AQL;
//
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using SmICSCoreLib.Util;

namespace SmICSCoreLib.AQL.Lab.InfectionsStatusDevelopmentCurve
{
    public class InfectionsStatusDevelopmentCurveFactory : IInfectionsStatusDevelopmentCurveFactory
    {
        private List<InfectionsStatusDevelopmentCurveModel> infectionsStatusDevelopmentCurveList;
        private Dictionary<string, SortedDictionary<DateTime, int>> dataAggregationStorage;
        private IRestDataAccess _restData;

        public InfectionsStatusDevelopmentCurveFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public List<InfectionsStatusDevelopmentCurveModel> Process(TimespanParameter parameter, string kindOfFinding)
        {
            List<TimeDataPointModel> specimenIdentifierList = new List<TimeDataPointModel>();
            //BEGIN Different treatment for 'virological' and 'microbiological' finding
            //      = Change the test result description
            if (kindOfFinding == "virological")
            {
                //PW20210510__specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurve(parameter).Query);
                specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurve(parameter));

                foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
                {
                    /*if (curTimeDataPoint.VirologicalTestResult.Length <= 7)
                    {
                        System.Diagnostics.Debug.WriteLine("    {0}    {1}    {2}    {3}",
                                                           curTimeDataPoint.SpecimenIdentifier,
                                                           curTimeDataPoint.VirologicalTestResult,
                                                           curTimeDataPoint.Zeitpunkt,
                                                           curTimeDataPoint.DiseaseName);
                    }
                    else if (curTimeDataPoint.VirologicalTestResult.Length > 7)
                    {
                        System.Diagnostics.Debug.WriteLine("    {0}    {1}    {2}    {3}    {4}",
                                                           curTimeDataPoint.SpecimenIdentifier,
                                                           curTimeDataPoint.VirologicalTestResult,
                                                           curTimeDataPoint.Zeitpunkt,
                                                           curTimeDataPoint.DiseaseName,
                                                           curTimeDataPoint.VirologicalTestResult.Substring(0, 6));
                    }*/

                    try
                    {
                        // Case for ''Not detected'', i.e. ''negativ''
                        if (   curTimeDataPoint.VirologicalTestResult.Length > 7
                            && curTimeDataPoint.VirologicalTestResult.Substring(0, 12) == "Not detected")
                        {
                            curTimeDataPoint.VirologicalTestResult = "negativ";
                        }
                        // Case for curTimeDataPoint.VirologicalTestResult different from positiv/negativ.
                        // Case for ''Not detected'' already treated above.
                        else if (   curTimeDataPoint.VirologicalTestResult != "negativ" 
                                 && curTimeDataPoint.VirologicalTestResult != "positiv")
                        {
                            System.Diagnostics.Debug.WriteLine("");
                            System.Diagnostics.Debug.WriteLine("");
                            System.Diagnostics.Debug.WriteLine("Warning in InfectionsStatusDevelopmentCurveFactory");
                            System.Diagnostics.Debug.WriteLine("");
                            System.Diagnostics.Debug.WriteLine("");
                            System.Diagnostics.Debug.WriteLine("");
                        }
                        // Future case for ''Detected''?
                        // Case for ''Nachweis'', i.e. ''positiv''
                        else if (!true && curTimeDataPoint.VirologicalTestResult.Length < "Kein Nachweis".Length-1)
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
            }
            else if (kindOfFinding == "microbiological")
            {
                //PW20210510__specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurveMicrobiologicalFinding(parameter).Query);
                specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurveMicrobiologicalFinding(parameter));
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

            TemporaryDataStorageConstructor(parameter);
            DataAggregation_00001_GroupedList(parameter, specimenIdentifierList);

            ReturnValueConstructor(parameter);

            return infectionsStatusDevelopmentCurveList;
        }

        private void TemporaryDataStorageConstructor(TimespanParameter parameter)
        {
            SortedDictionary<DateTime, int> active = new SortedDictionary<DateTime, int>();
            SortedDictionary<DateTime, int> sum = new SortedDictionary<DateTime, int>();

            //Prefills the dictionaries
            for (DateTime date = parameter.Starttime.Date; date <= parameter.Endtime.Date; date = date.AddDays(1))
            {
                active.Add(date, 0);
                sum.Add(date, 0);
            }

            dataAggregationStorage = new Dictionary<string, SortedDictionary<DateTime, int>>{{"active", active}, {"sum", sum}};
        }

        private void DataAggregation(List<TimeDataPointModel> specimenIdentifierList)
        {
            foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
            {
                try
                {
                    dataAggregationStorage["active"][curTimeDataPoint.Zeitpunkt.Date] += curTimeDataPoint.VirologicalTestResult == "positiv" ? 1 : 0;
                    dataAggregationStorage["sum"][curTimeDataPoint.Zeitpunkt.Date] += curTimeDataPoint.VirologicalTestResult == "positiv" ? 1 : 0;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }

                try
                {
                    dataAggregationStorage["sum"][curTimeDataPoint.Zeitpunkt.Date] -= curTimeDataPoint.VirologicalTestResult == "negativ" ? 1 : 0;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        private void DataAggregation_00001_GroupedList(TimespanParameter parameter, List<TimeDataPointModel> specimenIdentifierList)
        {
            //Die erste Sortierung kann entfallen, weil angenommen wird, dass unten bei FindAll nach Datum sortiert wird.
            List<TimeDataPointModel> specimenIdentifierList_sortedBySpecimenID = specimenIdentifierList.OrderBy(t => t.SpecimenIdentifier).ToList();
            //
            List<CountOfSpecimenIDAppearanceModel> countOfSpecimenIDAppearance = new List<CountOfSpecimenIDAppearanceModel>();
            foreach(var grp in specimenIdentifierList.GroupBy(t => t.SpecimenIdentifier))
            {
                countOfSpecimenIDAppearance.Add(new CountOfSpecimenIDAppearanceModel{SpecimenIdentifier = grp.Key, CountOfSpecimenIDAppearance = grp.Count()});
            }

            foreach(CountOfSpecimenIDAppearanceModel curCount in countOfSpecimenIDAppearance)
            {
                List<TimeDataPointModel> subListFoundCurSpecimenID = specimenIdentifierList.FindAll(t => t.SpecimenIdentifier == curCount.SpecimenIdentifier);

                if (subListFoundCurSpecimenID.Count > 1)
                {
                    for (int o = 0; o < subListFoundCurSpecimenID.Count; o++)
                    {
                        System.Diagnostics.Debug.WriteLine("    {0}    {1}    {2}", subListFoundCurSpecimenID[o].SpecimenIdentifier, subListFoundCurSpecimenID[o].VirologicalTestResult, subListFoundCurSpecimenID[o].Zeitpunkt);
                    }
                    System.Diagnostics.Debug.WriteLine("");

                    int firstPositiveIndex = subListFoundCurSpecimenID.FindIndex(t => t.VirologicalTestResult == "positiv");
                    for (int o = firstPositiveIndex; o < subListFoundCurSpecimenID.Count; o++)
                    {
                        // If first VirologicalTestResult is 'positiv', add 1 at the Zeitpunkt.
                        if (o == 0)
                        {
                            try
                            {
                                dataAggregationStorage["active"][subListFoundCurSpecimenID[o].Zeitpunkt.Date] += 1;

                                for (DateTime curDate = subListFoundCurSpecimenID[o].Zeitpunkt.Date; curDate <= parameter.Endtime.Date; curDate = curDate.AddDays(1))
                                {
                                    dataAggregationStorage["sum"][curDate] += 1;
                                }
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine(e.Message);
                            }
                        }
                        else if (o > 0)
                        {
                            // Check previous VirologicalTestResult,
                            //       --------
                            // because the change in comparison to last
                            // measurement/test have to be considered.
                            if (subListFoundCurSpecimenID[o-1].VirologicalTestResult == "positiv")
                            {
                                // If change in the test result is 'positiv' -> 'negativ' (equal 'false'): subtract 1 at the Zeitpunkt, otherwise do nothing.
                                try
                                {
                                    dataAggregationStorage["active"][subListFoundCurSpecimenID[o].Zeitpunkt.Date] -= !(subListFoundCurSpecimenID[o].VirologicalTestResult == "positiv") ? 1 : 0;

                                    for (DateTime curDate = subListFoundCurSpecimenID[o].Zeitpunkt.Date; curDate <= parameter.Endtime.Date; curDate = curDate.AddDays(1))
                                    {
                                        dataAggregationStorage["sum"][curDate] -= !(subListFoundCurSpecimenID[o].VirologicalTestResult == "positiv") ? 1 : 0;
                                    }
                                }
                                catch (Exception e)
                                {
                                    System.Diagnostics.Debug.WriteLine(e.Message);
                                }
                            }
                            else if (subListFoundCurSpecimenID[o-1].VirologicalTestResult == "negativ")
                            {
                                // If change in the test result is 'negativ' -> 'positiv': add 1 at the Zeitpunkt, otherwise do nothing.
                                try
                                {
                                    dataAggregationStorage["active"][subListFoundCurSpecimenID[o].Zeitpunkt.Date] += subListFoundCurSpecimenID[o].VirologicalTestResult == "positiv" ? 1 : 0;

                                    for (DateTime curDate = subListFoundCurSpecimenID[o].Zeitpunkt.Date; curDate <= parameter.Endtime.Date; curDate = curDate.AddDays(1))
                                    {
                                        dataAggregationStorage["sum"][curDate] += subListFoundCurSpecimenID[o].VirologicalTestResult == "positiv" ? 1 : 0;
                                    }
                                }
                                catch (Exception e)
                                {
                                    System.Diagnostics.Debug.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // The sub-list consists only of 1 member.
                    // If the result is 'positiv', add 1 at the Zeitpunkt.
                    try
                    {
                        dataAggregationStorage["active"][subListFoundCurSpecimenID[0].Zeitpunkt.Date] += subListFoundCurSpecimenID[0].VirologicalTestResult == "positiv" ? 1 : 0;

                        for (DateTime curDate = subListFoundCurSpecimenID[0].Zeitpunkt.Date; curDate <= parameter.Endtime.Date; curDate = curDate.AddDays(1))
                        {
                            dataAggregationStorage["sum"][curDate] += subListFoundCurSpecimenID[0].VirologicalTestResult == "positiv" ? 1 : 0;
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
            }
        }

        private void ReturnValueConstructor(TimespanParameter timespan)
        {
            infectionsStatusDevelopmentCurveList = new List<InfectionsStatusDevelopmentCurveModel>();
            List<int>last7days = new List<int>();

            for (DateTime curDate = timespan.Starttime.Date; curDate <= timespan.Endtime.Date; curDate = curDate.AddDays(1))
            {
                InfectionsStatusDevelopmentCurveModel infectionsStatusDevelopmentCurveModel = new InfectionsStatusDevelopmentCurveModel();
                infectionsStatusDevelopmentCurveModel.Datum = curDate;
                infectionsStatusDevelopmentCurveModel.ErregerID = "COV";
                infectionsStatusDevelopmentCurveModel.ErregerBEZK = "SARS-Cov-2";
                infectionsStatusDevelopmentCurveModel.Anzahl = dataAggregationStorage["active"][curDate];
                infectionsStatusDevelopmentCurveModel.anzahl_gesamt = dataAggregationStorage["sum"][curDate];
                infectionsStatusDevelopmentCurveModel.anzahl_gesamt_av7 = MovingAverage.Calculate(last7days, dataAggregationStorage["sum"][curDate], 7);

                infectionsStatusDevelopmentCurveList.Add(infectionsStatusDevelopmentCurveModel);
            }
        }
    }
}
