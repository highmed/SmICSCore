using Newtonsoft.Json.Linq;
using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.ScriptService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.OutbreakDetection
{
    public class OutbreakDetectionProxy
    {
        private readonly string RExecPath = @""; //EnviromentVariable 
        private readonly int FREQUENCY = 7;

        public void Covid19Extension(ProxyParameterModel parameter)
        {
            string RScriptPath = "./R_Script_00010.R";
            string RArgPath = "./"; 
            string RResultFileName = "./Variables_for_Visualization.json";

            string argumentString = RArgPath + "" + parameter.FitRange[0] + " " + parameter.FitRange[1] + " " + parameter.LookbackWeeks;

            GenerateTransferScript(parameter.EpochsObserved);
            ExternalProcess.Execute(RResultFileName, RExecPath, argumentString);
            List<OutbreakDetectionResultModel> results = JSONReader<OutbreakDetectionResultModel>.Read(RResultFileName);
            SaveResults(results, parameter.SavingFolder);
        }

        private void GenerateTransferScript(int[][] epochs_and_observed)
        {
            TransfersScriptModel transfersScript = new TransfersScriptModel() { Epoch = epochs_and_observed[0].ToList(), Frequency = FREQUENCY, Observed = epochs_and_observed[1].ToList()};
            JSONWriter.Write(transfersScript, @"./", "variables_for_r_transfer_script.json");
        }

        private void SaveResults(List<OutbreakDetectionResultModel> results, string savingFolder)
        {
           foreach(OutbreakDetectionResultModel result in results)
           {
                //Speicherordner aus Parameter bestimmen
                JSONWriter.Write(result, @"../Resources/OutbreakDetection/" + savingFolder, result.Date.ToString("yyyy-MM-dd") + ".json");
           }
        }
    }
}
