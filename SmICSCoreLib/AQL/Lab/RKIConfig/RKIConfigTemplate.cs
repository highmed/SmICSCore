
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SmICSCoreLib.AQL.Lab.RKIConfig
{
    public class RKIConfigTemplate
    {
        [BsonId]
        public Guid ID { get; set; }
        [Required]
        public string Station { get; set; }
        public List<LabDataKeimReceiveModel> ErregerID { get; set; }
        [Required]
        public string Erreger { get; set; }
        [Required]
        public string Zeitraum { get; set; }
        public bool Retro { get; set; } = false;
        [Required]
        public string Erstellungsdatum { get; set; } = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");

    }
}
