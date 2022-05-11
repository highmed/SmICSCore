using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SmICSCoreLib.Factories.RKIConfig
{
    public class RKIConfigTemplate
    {
        [Required]
        public string Ward { get; set; }
        public List<string> PathogenCodes { get; set; }
        [Required]
        public string Pathogen { get; set; }
        [Required]
        public string Timespan { get; set; }
        public bool Retro { get; set; } = false;
        [Required]
        public string Entrydate { get; set; } = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
        public string Pathogenstatus { get; set; }
        public string Resistance { get; set; }
    }
}
