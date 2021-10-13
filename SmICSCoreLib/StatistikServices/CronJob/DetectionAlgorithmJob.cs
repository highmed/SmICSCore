
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Quartz;
using Quartz.Logging;
using Quartz.Impl;
using SmICSCoreLib.Factories.RKIConfig;
using SmICSCoreLib.Factories.General;

namespace SmICSCoreLib.Factories.DetectionAlgorithmInterface
{
    public class DetectionAlgorithmJob
    {
        // simple log provider to get something to the console
        private class ConsoleLogProvider : ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                    }
                    return true;
                };
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
            {
                throw new NotImplementedException();
            }
        }

        [DisallowConcurrentExecution]
        public class myJob_00001_DetectionAlgorithmJob : IJob
        {
            private readonly BuildDetectionInput _buildDetectionInput;

            public myJob_00001_DetectionAlgorithmJob(BuildDetectionInput buildDetectionInput)
            {
                _buildDetectionInput = buildDetectionInput;
            }

            //public async Task Execute(IJobExecutionContext context)
            //public async Task<Newtonsoft.Json.Linq.JObject> Execute(IJobExecutionContext context)
            public async Task Execute(IJobExecutionContext context)
            {
                try
                {
                    DateTime anfang_00 = new DateTime(2020, 3, 1);
                    DateTime ende_00 = new DateTime(2021, 3, 1);
                    if (!true)
                    {
                        anfang_00 = new DateTime(2020, 3, 1);
                        ende_00 = new DateTime(2021, 3, 1);
                    }
                    else if (true)
                    {
                        anfang_00 = new DateTime(2020, 1, 1);
                        ende_00 = new DateTime(2021, 5, 1);
                    }

                    string pathForCSharp = Directory.GetCurrentDirectory().Replace(@"\", @"\\");
                    string pathForR = Directory.GetCurrentDirectory().Replace(@"\", @"/");
                    pathForCSharp = pathForCSharp.Insert(pathForCSharp.Length, @"\\");
                    pathForR = pathForR.Insert(pathForR.Length, @"/");

                    string cur_path_of_r_script = pathForCSharp + "..\\SmICSCoreLib\\AQL\\DetectionAlgorithmInterface\\";
                    string cur_json_inp_name = "./variables_for_r_transfer_script.json"; //CAUTION: This parameter must be the same as in cur_r_transfer_file_name
                    string cur_r_transfer_file_name = "R_Script_00010.R";
                    string cur_path_of_r_exec = "C:\\Programme\\R\\R-4.0.3\\bin\\";
                    string cur_r_out_file_name = "./Variables_for_Visualization.json"; //CAUTION: This parameter must be the same as in cur_r_transfer_file_name

                    // Konfig-Parameter aus einer Json-Datei einlesen
                    string path_for_example_parameters_from_config_site = pathForCSharp + ".\\Resources\\detection\\json\\";
                    List<ConfigSiteParametersReceiveModel> parametersItems = new();

                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine("myJob_00001_DetectionAlgorithmJob");
                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine("");

                    using (StreamReader sr_00 = new StreamReader(  path_for_example_parameters_from_config_site
                                                                 + "ExampleParametersFromConfigSite_00000.json"))
                    {
                        string readJson = sr_00.ReadToEnd();
                        parametersItems = JsonConvert.DeserializeObject<List<ConfigSiteParametersReceiveModel>>(readJson);
                    }

                    // Schleife ueber Parametersaetze
                    List<DetectionAlgorithmResultModel> outputListWithResultElements = new List<DetectionAlgorithmResultModel>();

                    //int[] a_00, b_00;
                    //int[][] epochs_and_observed_test = new [] {a_00, b_00};
                    //int[][] epochs_and_observed;

                    // Parameters for Disease Detection algorithm can be defined using this variable.
                    // For details compare previous versions.
                    string cur_rParameter  = "";
                    // Here this parameter is used for passing the DODInterface directory.
                    cur_rParameter += pathForR + "../SmICSCoreLib/AQL/DetectionAlgorithmInterface/";

                    int lookBack, fitRangeStartInt, fitRangeEndInt, beginRestore, curDiffOfDays;
                    DateTime fitRangeStart, fitRangeEnd;

                    string currentStationID  = "";
                    currentStationID  = "Coronastation"; //PW20211005__curParametersItem.Ward;

                    //int[][] epochs_and_observed = _buildDetectionInput.GetTimeSeriesForDetectionAlgorithm(new TimespanParameter
                    int[][] epochs_and_observed = _buildDetectionInput.GetTimeSeriesForDetectionAlgorithm(new TimespanParameter {Starttime = anfang_00, Endtime = ende_00},
                                                                                                          "virological",
                                                                                                          currentStationID);

                    lookBack = 4; //PW20211005__curParametersItem.LookbackWeeks;
                    fitRangeStart = new DateTime(2021, 1, 11); //PW20211005__curParametersItem.Datum;
                    fitRangeEnd = new DateTime(2021, 1, 11); //PW20211005__scurParametersItem.Datum;
                    fitRangeStartInt = (int) (fitRangeStart - new DateTime(1970, 1, 1)).TotalDays;
                    fitRangeEndInt = (int) (fitRangeEnd - new DateTime(1970, 1, 1)).TotalDays;
                    curDiffOfDays = fitRangeStartInt - lookBack*7;
                    beginRestore = Array.FindIndex(epochs_and_observed[0], x => x.Equals(curDiffOfDays));

                    cur_rParameter += " " + Convert.ToString(lookBack*7 + (fitRangeStartInt-fitRangeStartInt) + 1); // fit_range
                    cur_rParameter += " " + Convert.ToString(lookBack*7 + (fitRangeEndInt-fitRangeStartInt) + 1); // ''
                    cur_rParameter += " " + Convert.ToString(lookBack); // weeks_back

                    int[] epochs_01 = new int[lookBack*7 + 1];
                    int[] observed_01 = new int[lookBack*7 + 1];
                    for (int o = 0; o < epochs_01.Length; o++)
                    {
                         epochs_01[o] = epochs_and_observed[0][beginRestore + o];
                        observed_01[o] = epochs_and_observed[1][beginRestore + o];

                        TimeSpan timeSpan = new TimeSpan(epochs_01[o], 0, 0, 0);
                        DateTime curDateTime = new DateTime(1970, 1, 1);
                        System.Diagnostics.Debug.WriteLine("    {0}    {1}    {2}", new DateTime(1970, 1, 1)+timeSpan, epochs_01[o], observed_01[o]);
                    }

                    int[][] epochs_and_observed_01 = new [] {epochs_01, observed_01};

                    JObject dODAlgorithmResultJson = SmICSCoreLib.Factories.DetectionAlgorithmInterface.DetectionAlgorithmInterface.RunDetectionAlgorithmCovid19Extension(cur_path_of_r_script,
                                                                                                                   epochs_and_observed_01,
                                                                                                                   epochs_and_observed_01[0].Length,
                                                                                                                   epochs_and_observed_01[0].Length,
                                                                                                                   cur_json_inp_name,
                                                                                                                   cur_r_transfer_file_name,
                                                                                                                   cur_path_of_r_exec,
                                                                                                                   cur_r_out_file_name,
                                                                                                                   cur_rParameter);

                    for (int o = 0; o < (fitRangeEndInt-fitRangeStartInt)+1; o++)
                    {
                        outputListWithResultElements.Add(
                        new DetectionAlgorithmResultModel
                        {
                            Datum                       = (DateTime) dODAlgorithmResultJson["Zeitstempel"][o],
                            Ausbruchswahrscheinlichkeit = (double?) dODAlgorithmResultJson["Ausbruchswahrscheinlichkeit"][o],
                            PValue                      = (double?) dODAlgorithmResultJson["p-Value"][o],
                            Erregeranzahl               = (int?) dODAlgorithmResultJson["Erregeranzahl"][o],
                            EndemischesNiveau           = (double?) dODAlgorithmResultJson["Endemisches Niveau"][o],
                            EpidemischesNiveau          = (double?) dODAlgorithmResultJson["Epidemisches Niveau"][o],
                            Obergrenze                  = (int?) dODAlgorithmResultJson["Obergrenze"][o],
                            FaelleUnterObergrenze       = (int?) dODAlgorithmResultJson["Faelle unter der Obergrenze"][o],
                            FaelleUeberObergrenze       = (int?) dODAlgorithmResultJson["Faelle ueber der Obergrenze"][o],
                            KlassifikationAlarmfaelle   = (string?) dODAlgorithmResultJson["Klassifikation der Alarmfaelle"][o],
                            KeineNullWerte              = (bool) dODAlgorithmResultJson["Algorithmusergebnis enthaelt keine null-Werte"][0]
                        });
                    }

                    string resultsJsonString = JsonConvert.SerializeObject(outputListWithResultElements, Formatting.Indented);
                    JsonTextReader cur_reader_01 = new JsonTextReader(new StringReader(resultsJsonString));
                    JObject jObjectWithResultsOfDetectionAlgorithm = new JObject();
                    jObjectWithResultsOfDetectionAlgorithm.Add("Results from Detection Algorithm", JToken.ReadFrom(new JsonTextReader(new StringReader(resultsJsonString))));

                    //BEGIN Write the results of detection algorithm into file
                    if (true)
                    {
                        System.IO.File.WriteAllText(  pathForCSharp
                                                    + ".\\Resources\\detection\\json\\"
                                                    + "ResultsFromDetectionAlgorithm.json",

                                                    jObjectWithResultsOfDetectionAlgorithm.ToString());
                    }
                    //END Write the results of detection algorithm into file

                    //await new JObject();
                    //return jObjectWithResultsOfDetectionAlgorithm;
                }
                catch (Exception e)
                {
                    //ErrorHandling(e);
                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine("");

                    //await new JObject();
                    //return new JObject();
                }
            }
        }
    }
}
