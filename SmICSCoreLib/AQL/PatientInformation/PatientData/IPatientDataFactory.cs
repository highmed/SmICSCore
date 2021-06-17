namespace SmICSCoreLib.AQL.PatientInformation.PatientData
{
    public interface IPatientDataFactory
    {
        PatientData Process(string PatientID);
    }
}