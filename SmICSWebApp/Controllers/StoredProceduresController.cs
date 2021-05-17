using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.Contact_Nth_Network;
using SmICSCoreLib.AQL.Lab;
using SmICSCoreLib.AQL.Algorithm;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Lab.EpiKurve;
using SmICSCoreLib.AQL.Algorithm.NEC;
using SmICSCoreLib.AQL.Patient_Stay;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.Patient_Stay.Cases;
using SmICSCoreLib.AQL.Patient_Stay.WeekCase;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.AQL.DOD_Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmICSWebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoredProceduresController : ControllerBase
    {
        private readonly ILogger<StoredProceduresController> _logger;
        
        private readonly ILabData _labData;
        private readonly IPatientInformation _patientInformation;
        private readonly IContactNetworkFactory _contact;
        private readonly IAlgorithmData _algorithm;
        private readonly IPatinet_Stay _patinet_Stay;
        private readonly BuildBasicInput _buildBasicInput;

        public StoredProceduresController(ILogger<StoredProceduresController> logger, ILabData labData, IPatientInformation patientInformation, IContactNetworkFactory contact, IAlgorithmData algorithm, IPatinet_Stay patinet_Stay, BuildBasicInput buildBasicInput)
        {
            _logger = logger;
            _labData = labData;
            _patientInformation = patientInformation;
            _contact = contact;
            _algorithm = algorithm;
            _patinet_Stay = patinet_Stay;
            _buildBasicInput = buildBasicInput;
        }

        /// <summary></summary>
        /// <remarks>
        /// Gibt zu dem angegebenen Patienten und dem angegebenen Zeitraum alle weiteren Patienten zurück mit welchem der Patient kontakt hatte. Hierzu werden Informationen bzgl. des Zeitraums und der Grad des Kontakts geliefert. 
        /// </remarks>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Route("Contact_NthDegree_TTKP_Degree")]
        [HttpPost]
        public ActionResult<ContactModel> Contact_NthDegree_TTP_Degree([FromBody] ContactParameter parameter)
        {
            _logger.LogInformation("CALLED Contact_NthDegree_TTP_Degree with parameters: \n\r PatientID: {patID}\n\r Starttime: {start} \n\r Endtime: {end} \n\r Degree: {d} ", parameter.PatientID, parameter.Starttime, parameter.Endtime, parameter.Degree);
            try
            {
                System.Diagnostics.Debug.WriteLine("CALLED Contact_NthDegree_TTKP_Degree " + parameter.PatientID + " - " + parameter.Starttime + " - " + parameter.Endtime + " - " + parameter.Degree);
                return _contact.Process(parameter);
            }
            catch (Exception e)
            {
                _logger.LogWarning("CALLED Contact_NthDegree_TTP_Degree:" + e.Message);
                return ErrorHandling(e);
            }
        }

        /// <summary></summary>
        /// <remarks>
        /// Gibt alle virologischen Befunde der angegeben Patienten wieder. 
        /// Momentan sind die virologischen Befunde auf das SARS-CoV-2 Wirus beschränkt.
        /// </remarks>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Route("Patient_Labordaten_Ps")]
        [HttpPost]
        public ActionResult<List<LabDataModel>> Patient_Labordaten_Ps([FromBody] PatientListParameter parameter)
        {
            _logger.LogInformation("CALLED Patient_Labordaten_Ps with parameters: PatientIDs: {patList}", parameter.ToAQLMatchString());
            try
            {
                return _patientInformation.Patient_Labordaten_Ps(parameter);
            }
            catch (Exception e)
            {
                _logger.LogWarning("CALLED Patient_Labordaten_Ps:" + e.Message);
                return ErrorHandling(e);
            }
        }


        /// <summary></summary>
        /// <remarks>
        /// Gibt alle stationären Aufnahmen, Entlassungen, Stationswechsel und Prozeduren der angegeben Patienten wieder. 
        /// Eine Prozedur wird immer nur als ein Zeitpunkt wiedergegeben, da in den meisten Fällen die genaue Dauer einer Prozedur nicht dokumentiert wird.
        /// </remarks>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Route("Patient_Bewegung_Ps")]
        [HttpPost]
        public ActionResult<List<PatientMovementModel>> Patient_Bewegung_Ps([FromBody] PatientListParameter parameter)
        {
            _logger.LogInformation("CALLED Patient_Bewegung_Ps with parameters: \n\r PatientIDs: {patList}", parameter.ToAQLMatchString());

            try
            {
                return _patientInformation.Patient_Bewegung_Ps(parameter);
            }
            catch (Exception e)
            {
                _logger.LogWarning("CALLED Patient_Bewegung_Ps:" + e.Message);
                return ErrorHandling(e);
            }
        }

        /// <summary></summary>
        /// <remarks>
        /// Gibt die Anzahl der aggregierten virologischen Befundee für das gesamte Krankenhaus pro Tag in dem angegebenen Zeitraum zurück. Hierbei werden die für Aggregierung die Stationen berücksichtigt auf welcher die entsprechende Probe für den Nachweis entnommen wurde. Außerdem wirden für jeden Tag der gleitende Mittelwert für 7 und 28 Tage ermittelt.  
        /// Momentan sind die virologischen Befunde auf das SARS-CoV-2 Wirus beschränkt.
        /// Alle mit "_cs" markierten Werte sind für die virologische Auswertung irrelevant.
        /// </remarks>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Route("Labor_ErregerProTag_TTEsKSs")]
        [HttpPost]
        public ActionResult<List<EpiCurveModel>> Labor_ErregerProTag_TTEsKSs([FromBody] TimespanParameter parameter)
        {
            _logger.LogInformation("CALLED Labor_ErregerProTag_TTEsKSs with parameters: \n\r Starttime: {start} \n\r Endtime: {end} \n\r internal PathogenList: 94500-6, 94745-7, 94558-4", parameter.Starttime, parameter.Endtime);

            try
            {
                EpiCurveParameter epiParams = new EpiCurveParameter() { Endtime = parameter.Endtime, Starttime = parameter.Starttime, PathogenCodes = new List<string>() { "94500-6", "94745-7", "94558-4" } };
                return _labData.Labor_Epikurve(epiParams);
            }
            catch (Exception e)
            {
                _logger.LogWarning("CALLED Labor_ErregerProTag_TTEsKSs:" + e.Message);
                return ErrorHandling(e);
            }
        }

        //[Route("Patient_Stay_Stationary")]
        //[HttpPost]
        //public ActionResult<List<StationaryDataModel>> Patient_Stay_Stationary(string patientId)
        //{
        //    try
        //    {
        //        return _patinet_Stay.Stationary_Stay(patientId);
        //    }
        //    catch (Exception e)
        //    {
        //        return ErrorHandling(e);
        //    }
        //}

        /*
        [Route("Patient_Count")]
        [HttpPost]
        public ActionResult<List<CountDataModel>> Patient_Count(string nachweis)
        {
            try
            {
                return _patinet_Stay.CovidPat(nachweis);
            }
            catch (Exception e)
            {
                
                return ErrorHandling(e);
            }
        }

        [Route("Patient_Case")]
        [HttpPost]
        public ActionResult<List<CaseDataModel>> Patient_Case(DateTime date)
        {
            try
            {
                return _patinet_Stay.Case(date);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        } 
        
        [Route("Patient_WeekCase")]
        [HttpPost]
        public ActionResult<List<WeekCaseDataModel>> Patient_WeekCase(DateTime startDate, DateTime endDate)
        {
            try
            {
                return _patinet_Stay.WeekCase(startDate, endDate);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        [Route("NECAlgorithm")]
        [HttpPost]
        public ActionResult NECAlgorithm([FromBody] List<NECResultDataModel> parameter)
        {
            try
            {
                _algorithm.NECResultFile(parameter);
                return NoContent();  
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        [Route("NECAlgorithmResult")]
        [HttpPost]
        public ActionResult<List<NECResultDataModel>> NECAlgorithmResult([FromBody] TimespanParameter parameter)
        {
            try
            {
                return _algorithm.NECAlgorithmResult(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        [Route("NEC_Dataset")]
        [HttpGet]
        public ActionResult<NECCombinedDataModel> NEC_Dataset([FromQuery] DateTime parameter)
        {
            try
            {
                return _algorithm.NEC_Dataset(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }*/

        /*[Route("RKI_Dataset")]
        [HttpPost]
        public ActionResult<JArray> RKI_Dataset([FromBody] JObject parameter)
        {
            try
            {
                Dictionary<string, object> param = JsonConvert.DeserializeObject<Dictionary<string, object>>(parameter.ToString());
                return _algorithm.RKI_Dataset(param);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }*/

        private ActionResult ErrorHandling(Exception e) 
        {
            if (e is ArgumentNullException) { return new StatusCodeResult(412); }
            else if (e is ArgumentException) { return new StatusCodeResult(412); }
            else { return new StatusCodeResult(500); }
        }

        /*
        [Route("Patient_Symptom_TTPs")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [HttpPost]
        public ActionResult<List<SymptomModel>> Patient_Symptom_TTPs([FromBody] PatientListParameter parameter)
        {
            try
            {
                return _patientInformation.Patient_Symptom_TTPs(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }
        */

        /// <summary></summary>
        /// <remarks>
        /// Erstellung einer Infektionsentwicklungskurve und Aufruf des RKI-Algorithmus mit der.
        /// Infektionsentwicklungskurve als Parameter.
        /// </remarks>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Route("DODInterface")]
        [HttpGet]
        public JObject DodInterface()
        {
            try
            {
                DateTime anfang_00 = new DateTime(2020, 3, 1);
                DateTime ende_00 = new DateTime(2021, 3, 1);
                anfang_00 = new DateTime(2020, 3, 1);
                ende_00 = new DateTime(2021, 5, 1);
                string pathForCSharp = Directory.GetCurrentDirectory().Replace(@"\", @"\\");
                string pathForR = Directory.GetCurrentDirectory().Replace(@"\", @"/");
                pathForCSharp = pathForCSharp.Insert(pathForCSharp.Length, @"\\");
                pathForR = pathForR.Insert(pathForR.Length, @"/");
                //C:\\Users\\waldstein1\\Source\\Repos\\SmICSCore\\SmICSWebApp\\
                //C:/Users/waldstein1/Source/Repos/SmICSCore/SmICSWebApp/

                // Build a time series from data received using AQL and call DOD Algorithm
                // version for Covid 19
                if (!true)
                {
                    // The following method call requires:
                    // - TimespanParameter, constructed using the DateTimes anfang_00 and ende_00,
                    // - string "microbiological"/"virological" for pathogen type,
                    int[][] epochs_and_observed = _buildBasicInput.getTimeSeriesForDiseaseOutbreakDetectionAlgorithm(new TimespanParameter{Starttime = anfang_00, Endtime = ende_00}, "virological");
                }
                else if (true)
                {
                    int[] epochs_extension = new int[336];
                    int[] observed_extension = new int[336];

                    //string path_for_extension_test_data = "C:\\Users\\waldstein1\\Ordner\\BFAST\\Dateien\\DOD_Interface\\";
                    string path_for_extension_test_data = pathForCSharp + "..\\SmICSCoreLib\\AQL\\DOD_Interface\\";
                    string inp_filename_for_extension_test_data = "DOD_Covid_19_Input_00000.dat";
                    string current_line;

                    try
                    {
                        System.IO.StreamReader inp_file_for_extension_test_data = new System.IO.StreamReader(path_for_extension_test_data + inp_filename_for_extension_test_data);

                        for(int o = 0; o < observed_extension.Length; o++)
                        {
                            current_line = inp_file_for_extension_test_data.ReadLine();

                            observed_extension[o] = (int) Convert.ToInt64(current_line.Substring(0, 8));
                        }

                        for(int o = 0; o < epochs_extension.Length; o++)
                        {
                            current_line = inp_file_for_extension_test_data.ReadLine();

                            epochs_extension[o] = (int) Convert.ToInt64(current_line.Substring(0, 8));
                        }

                        inp_file_for_extension_test_data.Close();
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("Cannot read surveillance time series data from file.");
                    }

                    int[][] epochs_and_observed_extension = new [] {epochs_extension, observed_extension};

                    //string cur_path_of_r_script = "C:\\Users\\waldstein1\\source\\repos\\SmICSCore\\SmICSCoreLib\\AQL\\DOD_Interface\\";
                    string cur_path_of_r_script = pathForCSharp + "..\\SmICSCoreLib\\AQL\\DOD_Interface\\";
                    string cur_json_inp_name = "./variables_for_r_transfer_script.json"; //CAUTION: This parameter must be the same as in cur_r_transfer_file_name
                    string cur_r_transfer_file_name = "R_Script_00010.R";
                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine(cur_r_transfer_file_name);
                    System.Diagnostics.Debug.WriteLine("");
                    System.Diagnostics.Debug.WriteLine("");
                    string cur_path_of_r_exec = "C:\\Program Files\\R\\R-4.0.5\\bin\\";
                    string cur_r_out_file_name = "./Variables_for_Visualization.json"; //CAUTION: This parameter must be the same as in cur_r_transfer_file_name
                    string cur_path_of_r_script_for_r = cur_path_of_r_script.Replace(@"\\", @"/");

                    // Parameters for DOD algorithm can be defined using this variable.
                    // For details compare previous versions.
                    string cur_rParameter  = "";
                    // Here this parameter is used for passing the DODInterface directory.
                    cur_rParameter += pathForR + "../SmICSCoreLib/AQL/DOD_Interface/";

                    JObject dODAlgorithmResultJson = ConsoleApp_00004.DODInterface.RunDODAlgorithmCovid19Extension(cur_path_of_r_script,
                                                                                                                   epochs_and_observed_extension,
                                                                                                                   epochs_and_observed_extension[0].Length,
                                                                                                                   epochs_and_observed_extension[0].Length,
                                                                                                                   cur_json_inp_name,
                                                                                                                   cur_r_transfer_file_name,
                                                                                                                   cur_path_of_r_exec,
                                                                                                                   cur_r_out_file_name,
                                                                                                                   cur_rParameter);

                    return dODAlgorithmResultJson;
                }
                else
                {
                    List<string> stringForJson = new  List<string>{"This", "is", "a", "standard", "output"};

                    return null; //(JObject) JsonConvert.SerializeObject(stringForJson);
                }

                //
            }
            catch (Exception e)
            {
                ErrorHandling(e);
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("");

                List<string> stringForJson = new  List<string>{"This", "is", "an", "exception", "output"};

                return null; //(JObject) JsonConvert.SerializeObject(stringForJson);
            }
        }
    }
}
