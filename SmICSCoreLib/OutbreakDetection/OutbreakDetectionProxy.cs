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

        public void Covid19Extension(int[][] epochs_and_observed)
        {
            string RScriptPath = "./R_Script_00010.R";
            string RResultPath = ""; //Speicherpfad für das Ergebnis
            string RResultFileName = "./Variables_for_Visualization.json";

            GenerateTransferScript(epochs_and_observed);
            ExternalProcess.Execute(RResultFileName, RExecPath, RResultPath);
            List<OutbreakDetectionResultModel> results = JSONReader<OutbreakDetectionResultModel>.Read(RResultFileName);
            SaveResults(results);
        }

        private void GenerateTransferScript(int[][] epochs_and_observed)
        {
            TransfersScriptModel transfersScript = new TransfersScriptModel() { Epoch = epochs_and_observed[0].ToList(), Frequency = FREQUENCY, Observed = epochs_and_observed[1].ToList()};
            JSONWriter.Write(transfersScript, @"./", "variables_for_r_transfer_script.json");
        }

        private void SaveResults(List<OutbreakDetectionResultModel> results)
        {
           foreach(OutbreakDetectionResultModel result in results)
           {
                //Casten
                JSONWriter.Write(results, @"", "filename");
           }
        }
    }
}
