﻿using Newtonsoft.Json.Linq;
using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.ScriptService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmICSCoreLib.OutbreakDetection
{
    public class OutbreakDetectionProxy
    {
        private readonly string RExecPath = "C:\\Programme\\R\\R-4.1.1\\bin\\Rscript.exe"; //EnviromentVariable 
        private readonly int FREQUENCY = 7;

        public void Covid19Extension(ProxyParameterModel parameter)
        {
            string RScript = Directory.GetCurrentDirectory() + "\\Resources\\RRuntime\\R_Script_00010.R";
            string RArgPath = Directory.GetCurrentDirectory().Replace(@"\", @"/") + "/Resources/RRuntime/";
            string RResultFileName = @"./Resources/RRuntime/Variables_for_Visualization.json";

            //Rscript.exe rscript00010.r pfad fitrange fitrange lookbackweek
            string argumentString = RArgPath + " " + parameter.FitRange[0] + " " + parameter.FitRange[1] + " " + parameter.LookbackWeeks;

            //GenerateTransferScript(parameter.EpochsObserved);
            ExternalProcess.Execute(RScript, RExecPath, argumentString);
            List<OutbreakDetectionStoringModel> results = JSONReader<OutbreakDetectionStoringModel>.ReadOutbreakDetectionResult(RResultFileName);
            SaveResults(results, parameter.SavingFolder);
        }

        private void GenerateTransferScript(int[][] epochs_and_observed)
        {
            TransfersScriptModel transfersScript = new TransfersScriptModel() { Epoch = epochs_and_observed[0].ToList(), Frequency = FREQUENCY, Observed = epochs_and_observed[1].ToList()};
            JSONWriter.Write(transfersScript, @"./Resources/RRuntime", "variables_for_r_transfer_script");
        }

        private void SaveResults(List<OutbreakDetectionStoringModel> results, string savingFolder)
        {
           foreach(OutbreakDetectionStoringModel result in results)
           {
                //Speicherordner aus Parameter bestimmen
                JSONWriter.Write(result, @"../Resources/OutbreakDetection/" + savingFolder, result.Date.ToString("yyyy-MM-dd") + ".json");
           }
        }
    }
}