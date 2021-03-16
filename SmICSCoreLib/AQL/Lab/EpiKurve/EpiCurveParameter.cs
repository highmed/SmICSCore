using SmICSCoreLib.AQL.General;

namespace SmICSCoreLib.AQL.Lab.EpiKurve
{
    public class EpiCurveParameter : TimespanParameter
    {
        public string PathogenName { get; set; }

        public EpiCurveParameter() { }

        public EpiCurveParameter(TimespanParameter timespanParameter, string pathogenName)
        {
            Starttime = timespanParameter.Starttime;
            Endtime = timespanParameter.Endtime;
            PathogenName = pathogenName;
        }
    }
}