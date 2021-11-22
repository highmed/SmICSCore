namespace SmICSCoreLib.Factories.PatientInformation.PatientData
{
    public interface IPatientDataFactory
    {
        PatientData Process(string PatientID);
    }
}