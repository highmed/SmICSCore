using Newtonsoft.Json.Linq;
using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.ScriptService;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.OutbreakDetection
{
    public class OutbreakDetectionProxy
    {
        private readonly string RExecPath = @""; //EnviromentVariable 
        private readonly int FREQUENCY = 7;

        public void Covid19Extension()
        {
            string RScriptPath = "./R_Script_00010.R";
            string RResultPath = "./Variables_for_Visualization.json";

            string args = "";
            GenerateTransferScript();
            ExternalProcess.Execute(RScriptPath, RExecPath, args);
            List<OutbreakDetectionResultModel> results = JSONReader<OutbreakDetectionResultModel>.Read(RResultPath);
            SaveResults(results);
        }

        private void GenerateTransferScript()
        {
            TransfersScriptModel transfersScript = new TransfersScriptModel() { Frequency = FREQUENCY};
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
