using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using SmICSCoreLib.Factories.ContactNetwork;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Lab.ViroLabData;
using SmICSCoreLib.Factories.Symptome;
using SmICSCoreLib.Factories.Vaccination;
using SmICSCoreLib.Factories.Employees.ContactTracing;
using SmICSCoreLib.Factories.Employees.PersInfoInfecCtrl;
using SmICSCoreLib.Factories.Employees.PersonData;
using SmICSCoreLib.Factories.Employees;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.EpiCurve;
using SmICSCoreLib.Factories.PatientStay;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.Factories.InfectionSituation;
using System.Linq;
using SmICSWebApp.Helper;

namespace SmICSWebApp.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoredProceduresController : ControllerBase
    {
        private readonly ILogger<StoredProceduresController> _logger;
        
        private readonly IContactNetworkFactory _contact;
        private readonly IPatientStay _patientStay;
        private readonly IEmployeeInformation _employeeinformation;
        private readonly IViroLabDataFactory _viroLabDataFac;
        private readonly IPatientMovementFactory _patientMoveFac;
        private readonly IEpiCurveFactory _epiCurveFac;
        private readonly IInfectionSituationFactory _infectionSituationFac;
        private readonly ISymptomFactory _symptomFac;
        private readonly IVaccinationFactory _vaccinationFac;

        public StoredProceduresController(ILogger<StoredProceduresController> logger, IContactNetworkFactory contact, IPatientStay patientStay, IEmployeeInformation employeeInfo, IViroLabDataFactory viroLabDataFac, IPatientMovementFactory patientMoveFac, IEpiCurveFactory epiCurveFac, IInfectionSituationFactory infectionSituationFac, ISymptomFactory symptomFac, IVaccinationFactory vaccinationFac)
        {
            _logger = logger;
            _contact = contact;
            _patientStay = patientStay;
            _employeeinformation = employeeInfo;
            _viroLabDataFac = viroLabDataFac;
            _patientMoveFac = patientMoveFac;
            _epiCurveFac = epiCurveFac;
            _infectionSituationFac = infectionSituationFac;
            _symptomFac = symptomFac;
            _vaccinationFac = vaccinationFac;
        }

        /// <summary></summary>
        /// <remarks>
        /// Gibt zu dem angegebenen Patienten und dem angegebenen Zeitraum alle weiteren Patienten zurück mit welchem der Patient kontakt hatte. Hierzu werden Informationen bzgl. des Zeitraums und der Grad des Kontakts geliefert. 
        /// </remarks>
        /// <param name="parameter"></param>
        /// <returns></returns>
        
        [Route("Contact_NthDegree_TTKP_Degree")]
        [HttpPost]
        public ActionResult<ContactModel> Contact_NthDegree_TTP_Degree([FromBody] ContactParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            _logger.LogInformation("CALLED Contact_NthDegree_TTP_Degree with parameters: \n\r PatientID: {patID}\n\r Starttime: {start} \n\r Endtime: {end} \n\r Degree: {d} ", parameter.PatientID, parameter.Starttime, parameter.Endtime, parameter.Degree);
            try
            {
                _contact.RestDataAccess.SetAuthenticationHeader(token);
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
        public ActionResult<List<LabDataModel>> Patient_Labordaten_Ps([FromBody] PatientListParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            _logger.LogInformation("CALLED Patient_Labordaten_Ps with parameters: PatientIDs: {patList}", parameter.ToAQLMatchString());
            try
            {
                _viroLabDataFac.RestDataAccess.SetAuthenticationHeader(token);

                //Console.WriteLine("Name:" + TokenTranslator.getPrincipal(token.Split(" ")[1]).Identity.Name);

                
                return _viroLabDataFac.Process(parameter);
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
        public ActionResult<List<PatientMovementModel>> Patient_Bewegung_Ps([FromBody] PatientListParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            _logger.LogInformation("CALLED Patient_Bewegung_Ps with parameters: \n\r PatientIDs: {patList}", parameter.ToAQLMatchString());

            try
            {
                _patientMoveFac.RestDataAccess.SetAuthenticationHeader(token);
                return _patientMoveFac.Process(parameter);
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
        public ActionResult<List<EpiCurveModel>> Labor_ErregerProTag_TTEsKSs([FromBody] TimespanParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            _logger.LogInformation("CALLED Labor_ErregerProTag_TTEsKSs with parameters: \n\r Starttime: {start} \n\r Endtime: {end} \n\r internal PathogenList: 94500-6, 94745-7, 94558-4", parameter.Starttime, parameter.Endtime);

            try
            {
                EpiCurveParameter epiParams = new EpiCurveParameter() { Endtime = parameter.Endtime, Starttime = parameter.Starttime, PathogenCodes = new List<string>() { "94500-6", "94745-7", "94558-4" } };
                _epiCurveFac.RestDataAccess.SetAuthenticationHeader(token);
                return _epiCurveFac.Process(epiParams);
            }
            catch (Exception e)
            {
                _logger.LogWarning("CALLED Labor_ErregerProTag_TTEsKSs:" + e.Message);
                return ErrorHandling(e);
            }
        }

        /// <summary></summary>
        /// <remarks>
        /// Gibt alle mögliche Nosokomiale Infektion. 
        /// Regeln für eine mögliche Nosokomiale Infektion sind: SARS-CoV-2 negative Test und keine SARS-CoV-2 Symptome bei Aufnahme. 
        /// Positive PCR von SARS-CoV-2 ab Tag 4 nach stationärer Aufnahme.
        /// </remarks>
        /// <returns></returns>
        
        [Route("Infection_Situation")]
        [HttpPost]
        public ActionResult<List<Patient>> Infection_Situation([FromBody] PatientListParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            _logger.LogInformation("CALLED Infection_Situation without any parameters");

            try
            {
                return _infectionSituationFac.Process(parameter);
            }
            catch (Exception e)
            {
                _logger.LogWarning("CALLED Infection_Situation:" + e.Message);
                return ErrorHandling(e);
            }
        }

        private ActionResult ErrorHandling(Exception e) 
        {
            if (e is ArgumentNullException) { return new StatusCodeResult(412); }
            else if (e is ArgumentException) { return new StatusCodeResult(412); }
            else { return new StatusCodeResult(500); }
        }

        
        [Route("Patient_Symptom")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [HttpPost]
        public ActionResult<List<SymptomModel>> Patient_Symptom([FromBody] PatientListParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            try
            {
                _symptomFac.RestDataAccess.SetAuthenticationHeader(token);
                return _symptomFac.Process(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        
        [Route("Patient_Vaccination")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [HttpPost]
        public ActionResult<List<VaccinationModel>> Patient_Vaccination([FromBody] PatientListParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            try
            {
                _vaccinationFac.RestDataAccess.SetAuthenticationHeader(token);
                return _vaccinationFac.Process(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        
        [Route("Employee_ContactTracing")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [HttpPost]
        public ActionResult<List<ContactTracingReceiveModel>> Employee_ContactTracing([FromBody] PatientListParameter parameter)
        {
            try
            {
                return _employeeinformation.Employee_ContactTracing(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        
        [Route("Employee_PersonData")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [HttpPost]
        public ActionResult<List<PersonDataModel>> Employee_PersonData([FromBody] PatientListParameter parameter)
        {
            try
            {
                return _employeeinformation.Employee_PersonData(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        
        [Route("Employee_PersInfoInfecCtrl")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [HttpPost]
        public ActionResult<List<PersInfoInfecCtrlModel>> Employee_PersInfoInfecCtrl([FromBody] PatientListParameter parameter)
        {
            try
            {
                return _employeeinformation.Employee_PersInfoInfecCtrl(parameter);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        } 
    }
}
