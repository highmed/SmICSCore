﻿
namespace SmICSCoreLib.AQL.MiBi
{
    public class Antibiogram
    {
        public string Antibiotic { get; set; }
        public string Resistance { get; set; }
        public int MinInhibitorConcentration { get; set; }
        public string MICUnit { get; set; }
    }
}