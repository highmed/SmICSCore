using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.General;
using System.Collections.Generic;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using System.Linq;
using System;
using SmICSWebApp.Data.PatientView.Models;

namespace SmICSWebApp.Data.PatientView
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
        public PatientViewData LoadData(string PatientID)
        {
            PatientListParameter param = new PatientListParameter { patientList = new List<string> { PatientID } };
            List<PatientMovementModel> _movements = GetOrderedMovements(param);
            PatientViewData data = GetCasesDictionary(_movements);
            List<MibiLabDataModel> _labData = GetOrderedLabData(param);
            GetLabDataToCaseID(_labData, _movements, data);

            return data;
        }

        #region LoadData Functions
        private PatientViewData GetCasesDictionary(List<PatientMovementModel> _movements)
        {
            List<string> cases = _movements.Where(m => m.BewegungstypID == 1).Select(m => m.FallID).ToList();
            PatientViewData casesDict = new PatientViewData();
            foreach(string _case in cases)
            {
                casesDict.Add(new Models.Case { ID = _case }, new LabDataCollection());
            }
            return casesDict;
        }
        private void GetLabDataToCaseID(List<MibiLabDataModel> _labData, List<PatientMovementModel> _movements, PatientViewData data)
        { 
            foreach(MibiLabDataModel ld in _labData)
            {
                PatientMovementModel associatedMovement = _movements.Where(m => m.Beginn <= ld.ZeitpunktProbenentnahme && m.Ende >= ld.ZeitpunktProbenentnahme).FirstOrDefault();
                
                Models.LabData patView = new Models.LabData(ld, associatedMovement);
                data[patView.CaseID].Add(new LabMetaData { ReportDate = patView.ZeitpunktProbenentnahme }, patView);

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
