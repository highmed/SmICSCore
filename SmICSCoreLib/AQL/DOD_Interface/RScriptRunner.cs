using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using RDotNet;

namespace ConsoleApp_00004
{
    /// <summary>
    /// This calss runs R code from a file using the console.
    /// </summary>
    public class RScriptRunner
    {
        /// <summary>
        /// Runs an R script from a file using Rscript.exe.
        /// Example:
        ///   RScriptRunner.RunFromCmd(curDirectory + @"\ImageClustering.r", "rscript.exe", curDirectory.Replace('\\', '/'));
        /// Getting args passed from C# using R:
        ///   args = commandArgs(trailingOnly = TRUE)
        ///   print(args[1]);
        /// </summary>
        /// <param name="rCodeFilePath">File where your R code is located</param>
        /// <param name="rScriptExecutablePath">Usually only requires "rscript.exe"</param>
        /// <param name="args">Multiple R args can be separated by spaces.</param>
        /// <returns>Returns a string with the R responses.</returns>
        public static string RunFromCmd(string rCodeFilePath, string rScriptExecutablePath, string args)
        {
            string file = rCodeFilePath;
            string result = string.Empty;

            try
            {
                var info = new ProcessStartInfo();
                info.FileName = rScriptExecutablePath;
                info.WorkingDirectory = Path.GetDirectoryName(rCodeFilePath);
                info.Arguments = Path.GetFileName(rCodeFilePath) + " " + args;

                info.RedirectStandardInput = false;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                System.Diagnostics.Debug.WriteLine(info.FileName);
                System.Diagnostics.Debug.WriteLine(info.WorkingDirectory);
                System.Diagnostics.Debug.WriteLine(info.Arguments);

                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    result = proc.StandardOutput.ReadToEnd();
                }
                System.Diagnostics.Debug.WriteLine("Stelle 00099.");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("R script failed: " + result, ex);
            }
        }
    }

    /// <summary>
    /// This class runs R procedures using an RDotNet engine.
    /// </summary>
    public class REngineRunner
    {
        //Hint: Use ConsoleApp_00007.

        /// <summary>
        /// Runs R commands and procedures.
        /// Calls R library DOD with RKI Algorithm.
        /// As a first approach: 2 different procedures for
        /// Mibi algorithm and for viro algorithm.
        /// Mibi algorithm at the beginning commented. (11.03.2020)
        /// Example:
        ///   //TODO:REngineRunner.RunProceduresEngine();
        /// /*Getting args passed from C# using R:
        ///   args = commandArgs(trailingOnly = TRUE)
        ///   print(args[1]);*/
        /// </summary>
        /// <param name="observed">Daily sum of positive findings.</param>
        /// <param name="epoch">Date associated with ''observed'', in days since 01.01.1970</param>
        /// <param name="frequency">Frequency of averaging the time series.</param>
        /// <param name="kindOfFinding">Microbiological/virological. For applying appropriate algorithm.</param>
        /// <returns>Returns a string with the R responses.</returns>
        /// //TODO:Later: Returns a JSON object with variables of interest.
        public static string RunProceduresEngine(int[] observed, int[] epoch, int frequency, string kindOfFinding)
        {
            try
            {
                if (kindOfFinding == "virological")
                {
                    int num_of_epochs = epoch.Length;

                    REngine.SetEnvironmentVariables();
                    REngine engine = REngine.GetInstance();

                    /*
                    engine.Evaluate("library(dod);");

                    int[, ] observedMatrixCSharp = new int[num_of_epochs, 1];

                    for (int o = 0; o < num_of_epochs; o++)
                    {
                        observedMatrixCSharp[o, 0] = observed[o];
                    }

                    IntegerVector rEpochs = engine.CreateIntegerVector(epoch);
                    IntegerMatrix observedMatrix = engine.CreateIntegerMatrix(observedMatrixCSharp);

                    engine.SetSymbol("r_epochs", rEpochs);
                    //engine.SetSymbol("r_freq", freq_00);
                    engine.SetSymbol("observed_matrix", observedMatrix);

                    string engineEvaluateString  = "";
                    engineEvaluateString += "cur_sts";
                    engineEvaluateString += " <- surveillance::sts(";
                    engineEvaluateString += "observed = observedMatrix,";
                    engineEvaluateString += " frequency = 7,";
                    engineEvaluateString += " epoch = r_epochs";
                    engineEvaluateString += ")";

                    SymbolicExpression curStsStructure = engine.Evaluate(engineEvaluateString);

                    engineEvaluateString  = "";
                    engineEvaluateString += "dod_res";
                    engineEvaluateString += " <- dod_covid19(";
                    engineEvaluateString += "cur_sts,";
                    engineEvaluateString += " fit_range = 32:326";
                    engineEvaluateString += ")";

                    SymbolicExpression dodRes = engine.Evaluate(engineEvaluateString);
                    */

                    engine.Dispose();
                }
                else if (kindOfFinding == "microbiological")
                {
                    System.Diagnostics.Debug.WriteLine("Using RDotNet for microbiological algorithm is not implemented yet.");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return "";
        }
    }
}
