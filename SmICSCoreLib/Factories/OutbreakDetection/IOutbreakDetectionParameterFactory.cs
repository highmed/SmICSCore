namespace SmICSCoreLib.Factories.OutbreakDetection
{
    public interface IOutbreakDetectionParameterFactory
    {
        int[][] Process(OutbreakDetectionParameter parameter, SmICSVersion version);
    }
}