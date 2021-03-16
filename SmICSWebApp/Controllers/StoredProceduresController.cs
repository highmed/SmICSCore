using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System;
using Newtonsoft.Json;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.Contact_Nth_Network;
using SmICSCoreLib.AQL.Lab;
using SmICSCoreLib.AQL.Algorithm;
using SmICSCoreLib.AQL.ConnectionTest;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Patient_PathogenFlag;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Patient_DiagnosticResult;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Lab.EpiKurve;
using SmICSCoreLib.AQL.Algorithm.NEC;
using SmICSCoreLib.AQL.Patient_Stay;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.Patient_Stay.Cases;
using SmICSCoreLib.AQL.Patient_Stay.WeekCase;

namespace SmICSWebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoredProceduresController : ControllerBase
    {
        private readonly IConnectionTest _connectionTest;
        private readonly ILabData _labData;
        private readonly IPatientInformation _patientInformation;
        private readonly IContactNetworkProcedures _contact;
        private readonly IAlgorithmData _algorithm;
        private readonly IPatinet_Stay _patinet_Stay;

        public StoredProceduresController(ILabData labData, IPatientInformation patientInformation, IContactNetworkProcedures contact, IAlgorithmData algorithm, IConnectionTest connectionTest, IPatinet_Stay patinet_Stay)
        {
            _connectionTest = connectionTest;
            _labData = labData;
            _patientInformation = patientInformation;
            _contact = contact;
            _algorithm = algorithm;
            _patinet_Stay = patinet_Stay;
        }

        [Route("ConnectionTest")]
        [HttpGet]
        public ActionResult<JArray> ConnectionTest()
        {
            try
            {
                return _connectionTest.Test();
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        [Route("Contact_1stDegree_TTPK")]
        [HttpPost]
        public ActionResult<List<ContactModel>> Contact_1stDegree_TTP([FromBody] ContactParameter parameter)
        {
            try
            { 
                return _contact.Contact_1stDegree_TTP(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }
        [Route("Contact_NthDegree_TTKP_Degree")]
        [HttpPost]
        public ActionResult<List<ContactModel>> Contact_NthDegree_TTP_Degree([FromBody] ContactParameter parameter)
        {
            try
            {
                return _contact.Contact_NthDegree_TTP_Degree(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }
       
        [Route("Patient_Labordaten_Ps")]
        [HttpPost]
        public ActionResult<List<LabDataModel>> Patient_Labordaten_Ps([FromBody] PatientListParameter parameter)
        {
            try
            {
                return _patientInformation.Patient_Labordaten_Ps(parameter);
            }
            catch (Exception e)
            {
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


        [Route("Patient_Count")]
        [HttpPost]
        public ActionResult<List<CountDataModel>> Patient_Count(string nachweis)
        {
            try
            {
                return _patinet_Stay.Count(nachweis);
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


        [Route("Patient_PathogenFlag_TTEPs")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [HttpPost]
        public ActionResult<List<PathogenFlagModel>> Patient_PathogenFlag_TTEPs([FromBody] PatientListParameter parameter)
        {
            try
            {
                return _patientInformation.Patient_PathogenFlag_TTEPs(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        [Route("Patient_DiagnosticResults_TTPs")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [HttpPost]
        public ActionResult<List<DiagnosticResultModel>> Patient_DiagnosticResults_TTPs([FromBody] PatientListParameter parameter)
        {
            try
            {
                return _patientInformation.Patient_DiagnosticResults_TTPs(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }
        [Route("Patient_Bewegung_Ps")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        public ActionResult<List<PatientMovementModel>> Patient_Bewegung_Ps([FromBody] PatientListParameter parameter)
        {
            try
            {
                /*{
                    "patientList": "4100936739"
                }*/
                /*{
                     "List": ["6abf1aec-27f9-463d-bdc4-8b08fdc5fdb8"]
                }*/
                return _patientInformation.Patient_Bewegung_Ps(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }
        
        [Route("Labor_ErregerProTag_TTEsKSs")]
        [HttpPost]
        public ActionResult<List<EpiCurveModel>> Labor_ErregerProTag_TTEsKSs([FromBody] TimespanParameter parameter)
        {
            try
            {
                EpiCurveParameter epiParams = new EpiCurveParameter() { Endtime = parameter.Endtime, Starttime = parameter.Starttime, PathogenName = "COV" };
                return _labData.Labor_Epikurve(epiParams);
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
        }

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
    }
}
