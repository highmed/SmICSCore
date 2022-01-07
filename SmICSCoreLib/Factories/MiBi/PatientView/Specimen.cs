using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class Specimen
    {
        public string Kind { get; set; }
        public string LabID { get; set; }
        public DateTime SpecimenCollectionDateTime { get; set; }
        public DateTime? SpecimenReceiptDate  { get; set; }
        public string Location { get; set; }
        public List<Pathogen> Pathogens { get; set; }

        public bool HasMRSA()
        {
            foreach(Pathogen p in Pathogens)
            {
                if (p.ID == "sau")
                {
                    if(p.Antibiograms.Count(a => a.Resistance == "R" && (a.AntibioticID == "OXA" || a.AntibioticID == "MET")) >= 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasMSSA()
        {
            foreach (Pathogen p in Pathogens)
            {
                if (p.ID == "sau")
                {
                    if (p.Antibiograms.Count(a => a.Resistance == "S" && (a.AntibioticID == "OXA" || a.AntibioticID == "MET")) >= 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasCarbapanemResistance()
        {
            foreach (Pathogen p in Pathogens)
            {
                if (p.ID == "kpn" || p.ID == "G_ac.bac" || p.ID == "eco")
                {
                    if (p.Antibiograms.Count(a => a.Resistance == "R" && (a.AntibioticID == "IPM" || a.AntibioticID == "MEM" || a.AntibioticID == "ETP" || a.AntibioticID == "DOR" || a.AntibioticID == "RZM")) >= 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}