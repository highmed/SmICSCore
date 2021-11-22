using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.ViroLabData.ReceiveModel;

namespace SmICSCoreLib.Factories.Lab.ViroLabData
{
    public class LabDataModel : LabData
    {
        public LabDataModel() { }
        public LabDataModel(LabDataReceiveModel labDataReceive)
        {
            LabordatenID = labDataReceive.LabordatenID;
            PatientID = labDataReceive.PatientID;
            CaseID = labDataReceive.FallID;
            ProbeID = labDataReceive.LabordatenID;
            ZeitpunktProbenentnahme = labDataReceive.ZeitpunktProbenentnahme;
            ZeitpunktProbeneingang = labDataReceive.ZeitpunktProbeneingang.Value;
            MaterialID = labDataReceive.MaterialID == null ? "MissingID" : labDataReceive.MaterialID;
            Material_l = labDataReceive.Material_l == null ? " " : labDataReceive.Material_l;
            Befund = (labDataReceive.BefundCode == "260373001") ? true : false;
            KeimID = labDataReceive.KeimID;
            Befunddatum = labDataReceive.Befunddatum;
            Befundkommentar = labDataReceive.Befundkommentar;
            Viruslast = labDataReceive.Viruslast;
        }

        public string Viruslast { get; set; }
    }


}
