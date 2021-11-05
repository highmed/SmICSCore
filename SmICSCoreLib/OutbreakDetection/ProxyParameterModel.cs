namespace SmICSCoreLib.OutbreakDetection
{
    public class ProxyParameterModel
    {
        public int[][] EpochsObserved { get; set; }
        public string SavingFolder { get; set; }
        public int[] FitRange { get; set; }
        public int LookbackWeeks { get; set; }
        public string SavingDirectory { get; set; }
    }
}
