using System.Runtime.Serialization;

namespace SmICSCoreLib.Factories.General
{
    public enum MedicalField
    {
        [EnumMember(Value = "Mikrobiologischer Befund")]
        MICROBIOLOGY,
        [EnumMember(Value = "Virologischer Befund")] 
        VIROLOGY
    }
}
