using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.General;
using System.Collections.Generic;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using System.Linq;

namespace SmICSWebApp.Data
{
    public class PatientViewService
    {
        private readonly IPatientMovementFactory _move;

        private readonly IMibiPatientLaborDataFactory _mibi;

        public PatientViewService(IPatientMovementFactory move, IMibiPatientLaborDataFactory mibi)
        {
            _move = move;
            _mibi = mibi;
        }
        public PatientViewModel LoadData(string PatientID)
        {
            PatientListParameter param = new PatientListParameter { patientList = new List<string> { PatientID } };
            List<PatientMovementModel> _movements = _move.Process(param);
            SortedDictionary<string, dynamic> data = GetCasesDictionary(_movements);
            PatientViewModel patView = new PatientViewModel();
            patView.Movements = _move.Process(param);
            patView.LabData = _mibi.Process(param);


            return patView;
        }

        private SortedDictionary<string, dynamic> GetCasesDictionary(List<PatientMovementModel> _movements)
        {
            List<string> cases = _movements.Where(m => m.BewegungstypID == 1).OrderBy(m => m.Beginn).Select(m => m.FallID).ToList();
            SortedDictionary<string, dynamic> casesDict = new SortedDictionary<string, dynamic>();
            foreach(string _case in cases)
            {
                casesDict.Add(_case, new { });
            }
            return casesDict;
        }
    }
}
