using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.General;
using System.Collections.Generic;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using System.Linq;
using System;

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
        public SortedDictionary<string, SortedDictionary<DateTime, PatientViewModel>> LoadData(string PatientID)
        {
            PatientListParameter param = new PatientListParameter { patientList = new List<string> { PatientID } };
            List<PatientMovementModel> _movements = GetOrderedMovements(param);
            SortedDictionary<string, SortedDictionary<DateTime, PatientViewModel>> data = GetCasesDictionary(_movements);
            List<MibiLabDataModel> _labData = GetOrderedLabData(param);
            GetLabDataToCaseID(_labData, data, _movements);

            return data;
        }

        #region LoadData Functions
        private SortedDictionary<string, SortedDictionary<DateTime, PatientViewModel>> GetCasesDictionary(List<PatientMovementModel> _movements)
        {
            List<string> cases = _movements.Where(m => m.BewegungstypID == 1).Select(m => m.FallID).ToList();
            SortedDictionary<string, SortedDictionary<DateTime, PatientViewModel>> casesDict = new SortedDictionary<string, SortedDictionary<DateTime, PatientViewModel>>();
            foreach(string _case in cases)
            {
                casesDict.Add(_case, new SortedDictionary<DateTime, PatientViewModel>());
            }
            return casesDict;
        }
        private void GetLabDataToCaseID(List<MibiLabDataModel> _labData, SortedDictionary<string, SortedDictionary<DateTime, PatientViewModel>> data, List<PatientMovementModel> _movements)
        { 
            foreach(MibiLabDataModel ld in _labData)
            {
                PatientMovementModel associatedMovement = _movements.Where(m => m.Beginn >= ld.ZeitpunktProbenentnahme && m.Ende <= ld.ZeitpunktProbenentnahme).FirstOrDefault();
                
                PatientViewModel patView = ld as PatientViewModel;
                patView.Ward = associatedMovement.StationID;
                patView.Room = associatedMovement.Raum;
                patView.Departement = associatedMovement.Fachabteilung;

                data[patView.CaseID].Add(patView.ZeitpunktProbenentnahme, patView);

            }
        }

        private List<PatientMovementModel> GetOrderedMovements(PatientListParameter param)
        {
            return _move.Process(param).OrderBy(m => m.FallID).ThenBy(m => m.BewegungstypID).ThenBy(m => m.Beginn).ToList();
        }

        private List<MibiLabDataModel> GetOrderedLabData(PatientListParameter param)
        {
            return _mibi.Process(param).OrderBy(l => l.CaseID).ThenBy(l => l.ZeitpunktProbenentnahme).ToList();
        }
        #endregion
    }
}
