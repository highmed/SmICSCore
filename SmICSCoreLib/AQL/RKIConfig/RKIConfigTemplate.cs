using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SmICSCoreLib.AQL.RKIConfig
{
    public class RKIConfigTemplate
    {
        //[Required]
        //public string Station { get; set; }
        //[Required]
        //public string Erreger { get; set; }
        //[Required]
        //public string Zeitraum { get; set; }
        //public bool Retro { get; set; } = false;
        [Required]
        public string Zeitpunkt { get; set; }

        public List<RKIConfigRulesTemplate> RKIConfigFormAdd { get; set; }

        public RKIConfigTemplate()
        {
            RKIConfigFormAdd = new List<RKIConfigRulesTemplate>();
        }

    }
}
