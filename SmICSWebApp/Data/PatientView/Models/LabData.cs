using SmICSCoreLib.Factories.Lab.MibiLabData;
using SmICSCoreLib.Factories.PatientMovement;

namespace SmICSWebApp.Data.PatientView.Models
{
    public class LabData : MibiLabDataModel
    {
        public string Ward { get; set; }
        public string Room { get; set; }
        public string Departement { get; set; }

        public LabData(MibiLabDataModel mLab, PatientMovementModel move)
        {
            Antibiogram = mLab.Antibiogram;
            Befund = mLab.Befund;
            Befunddatum = mLab.Befunddatum;
            Befundkommentar = mLab.Befundkommentar;
            CaseID = mLab.CaseID;
            KeimID = mLab.KeimID;
            LabordatenID = mLab.LabordatenID;
            MaterialID = mLab.MaterialID;
            Material_l = mLab.Material_l;
            MREKlasse = mLab.MREKlasse;
            PatientID = mLab.PatientID;
            ProbeID = mLab.ProbeID;
            screening = mLab.screening;
            ZeitpunktProbeneingang = mLab.ZeitpunktProbeneingang;
            ZeitpunktProbenentnahme = mLab.ZeitpunktProbenentnahme;

            Departement = move.Fachabteilung;
            Room = move.Raum;
            Ward = move.StationID;
        }
    }
}
