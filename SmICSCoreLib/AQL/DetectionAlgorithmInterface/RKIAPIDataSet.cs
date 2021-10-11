using System;

namespace SmICSCoreLib.AQL.DetectionAlgorithmInterface
{
    // This is a class for receiving data from RKI API website.
    public class RKIAPIDataSet
    {
        public int Id { get; set; }
        public int IdBundesland { get; set; }
        public string Bundesland { get; set; }
        public string Landkreis { get; set; }
        public string Altersgruppe { get; set; }
        public string Geschlecht { get; set; }
        public int AnzahlFall { get; set; }
        public int AnzahlTodesfall { get; set; }
        public DateTime Meldedatum { get; set; }
        public int IdLandkreis { get; set; }
        public string Datenstand { get; set; }
        public int NeuerFall { get; set; }
        public int NeuerTodesfall { get; set; }
        public DateTime Refdatum { get; set; }
        public int NeuGenesen { get; set; }
        public int AnzahlGenesen { get; set; }
        public int IstErkrankungsbeginn { get; set; }
        public string Altersgruppe2 { get; set; }
    }
}
