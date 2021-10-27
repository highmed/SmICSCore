using Newtonsoft.Json.Linq;
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
            string RScript = parameter.SavingDirectory + "\\Resources\\RRuntime\\R_Script_00010.R";
            string RArgPath = parameter.SavingDirectory.Replace(@"\", @"/") + "/Resources/RRuntime/";
            string RResultFileName = parameter.SavingDirectory+"/Resources/RRuntime/Variables_for_Visualization.json";

            //Rscript.exe rscript00010.r pfad fitrange fitrange lookbackweek
            string argumentString = RArgPath + " " + parameter.FitRange[0] + " " + parameter.FitRange[1] + " " + parameter.LookbackWeeks;

            GenerateTransferScript(parameter.EpochsObserved, parameter.SavingDirectory);
            ExternalProcess.Execute(RScript, RExecPath, argumentString);
            List<OutbreakDetectionStoringModel> results = JSONReader<OutbreakDetectionStoringModel>.ReadOutbreakDetectionResult(RResultFileName);
            SaveResults(results, parameter.SavingDirectory, parameter.SavingFolder);
        }

        private void GenerateTransferScript(int[][] epochs_and_observed, string directory)
        {
            TransfersScriptModel transfersScript = new TransfersScriptModel() { Epoch = epochs_and_observed[0].ToList(), Frequency = FREQUENCY, Observed = epochs_and_observed[1].ToList()};
            JSONWriter.Write(transfersScript, directory+"/Resources/RRuntime", "variables_for_r_transfer_script");
        }

        private void SaveResults(List<OutbreakDetectionStoringModel> results, string savingDirectory, string savingFolder)
        {
           foreach(OutbreakDetectionStoringModel result in results)
           {
                //Speicherordner aus Parameter bestimmen
                JSONWriter.Write(result, savingDirectory + "/Resources/OutbreakDetection/" + savingFolder, result.Date.ToString("yyyy-MM-dd"));
           }
        }
    }
}
