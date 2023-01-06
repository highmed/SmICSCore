using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.DB.MenuItems;
using SmICSCoreLib.Factories.EpiCurve;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.OutbreakDetection;
using SmICSWebApp.Data.ContactNetwork;
using SmICSWebApp.Data.MedicalFinding;
using SmICSWebApp.Data.OutbreakDetection;
using SmICSWebApp.Data.PatientMovement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSWebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoredProceduresController : ControllerBase
    {
        private readonly ILogger<StoredProceduresController> _logger;

        private readonly IEpiCurveFactory _epiCurveFac;
        private readonly OutbreakDetectionService _outbreakService;
        private readonly MedicalFindingService _medicalFindingService;
        private readonly PatientMovementService _movementService;
        private readonly ContactNetworkService _contactService;
        private readonly IMenuItemDataAccess _menuItemDataAccess;
        public StoredProceduresController(ILogger<StoredProceduresController> logger, IEpiCurveFactory epiCurveFac, OutbreakDetectionService outbreakService, MedicalFindingService medicalFindingService, PatientMovementService movementService, ContactNetworkService contactService, IMenuItemDataAccess menuItemDataAccess)
        {
            _logger = logger;
            _outbreakService = outbreakService;
            _medicalFindingService = medicalFindingService;
            _movementService = movementService;
            _contactService = contactService;
            _menuItemDataAccess = menuItemDataAccess;
        }

        /// <summary></summary>
        /// <remarks>
        /// Gibt zu dem angegebenen Patienten und dem angegebenen Zeitraum alle weiteren Patienten zurück mit welchem der Patient kontakt hatte. Hierzu werden Informationen bzgl. des Zeitraums und der Grad des Kontakts geliefert. 
        /// </remarks>
        /// <param name="parameter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("Contact_NthDegree_TTKP_Degree")]
        [HttpPost]
        public ActionResult<Data.ContactNetwork.ContactModel> ContactNetwork([FromBody] ContactNetworkParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            try
            {
                Data.ContactNetwork.ContactModel contacts = _contactService.GetContactNetwork(parameter);
                return contacts;
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
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("Patient_Labordaten_Ps")]
        [HttpPost]
        public ActionResult<List<VisuLabResult>> PatientLabData([FromBody] PatientLabDataParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            try
            {
                List<VisuLabResult> visuLabResults = new List<VisuLabResult>();
                List<string> codes = _menuItemDataAccess.GetPathogendByName(parameter.Pathogen).Result.Select(p => p.Code).ToList();
                PathogenParameter pathogen = new PathogenParameter() { PathogenCodes = codes };
                foreach (string pat in parameter.patientList)
                {
                    Patient patient = new Patient() { PatientID = pat };
                    List<VisuLabResult> labs = _medicalFindingService.GetMedicalFinding(patient, pathogen).GetAwaiter().GetResult();
                    visuLabResults.AddRange(labs);
                }
                return visuLabResults;
            }
            catch (Exception e)
            {
                _logger.LogWarning("CALLED PatientLabData:" + e.Message);
                return ErrorHandling(e);
            }
        }

        [Route("Patient_Bewegung_Ps")]
        [HttpPost]
        public ActionResult<List<VisuPatientMovement>> PatientMovements([FromBody] PatientListParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            _logger.LogInformation("CALLED Patient_Bewegung_Ps with parameters: \n\r PatientIDs: {patList}", parameter.ToAQLMatchString());

            try
            {
                List<VisuPatientMovement> visuMovements = new List<VisuPatientMovement>();
                foreach (string pat in parameter.patientList)
                {
                    Patient patient = new Patient() { PatientID = pat };
                    List<VisuPatientMovement> movements = _movementService.GetPatientMovements(patient).GetAwaiter().GetResult();
                    visuMovements.AddRange(movements);
                }
                return visuMovements;
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
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("Labor_ErregerProTag_TTEsKSs")]
        [HttpPost]
        public ActionResult<List<EpiCurveModel>> Labor_ErregerProTag_TTEsKSs([FromBody] EpiCurveParameter parameter, [FromHeader(Name = "Authorization")] string token = "NoToken")
        {
            _logger.LogInformation("CALLED Labor_ErregerProTag_TTEsKSs with parameters: \n\r Starttime: {start} \n\r Endtime: {end} \n\r internal PathogenList: 94500-6, 94745-7, 94558-4", parameter.Starttime, parameter.Endtime);

            try
            {
                _epiCurveFac.RestDataAccess.SetAuthenticationHeader(token);
                return _epiCurveFac.Process(parameter);
            }
            catch (Exception e)
            {
                _logger.LogWarning("CALLED Labor_ErregerProTag_TTEsKSs:" + e.Message);
                return ErrorHandling(e);
            }
        }





        [Route("OutbreakDetectionConfigurations")]
        [HttpPost]
        public ActionResult<List<OutbreakDetectionConfig>> OutbreakDetectionConfigurations()
        {
            try
            {
                return _outbreakService.GetConfigurations();
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        [Route("LatestOutbreakDetectionResult")]
        [HttpPost]
        public ActionResult<OutbreakDetectionStoringModel> LatestOutbreakDetectionResult([FromBody] OutbreakSaving outbreak)
        {
            try
            {
                return _outbreakService.GetLatestResult(outbreak);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        [Route("OutbreakDetectionResultSet")]
        [HttpPost]
        public ActionResult<List<OutbreakDetectionStoringModel>> OutbreakDetectionResultSet([FromBody] OutbreakSavingInTimespan outbreak)
        {
            try
            {
                return _outbreakService.GetsResultsInTimespan(outbreak);
            }
            catch (Exception e)
            {
                return ErrorHandling(e);
            }
        }

        private ActionResult ErrorHandling(Exception e)
        {
            if (e is ArgumentNullException) { return new StatusCodeResult(412); }
            else if (e is ArgumentException) { return new StatusCodeResult(412); }
            else { return new StatusCodeResult(500); }
        }
    }
}
