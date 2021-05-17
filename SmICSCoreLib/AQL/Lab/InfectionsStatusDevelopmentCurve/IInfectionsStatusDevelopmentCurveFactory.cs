using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.AQL.General;

namespace SmICSCoreLib.AQL.Lab.InfectionsStatusDevelopmentCurve
{
    public interface IInfectionsStatusDevelopmentCurveFactory
    {
        List<InfectionsStatusDevelopmentCurveModel> Process(TimespanParameter parameter, string kindOfFinding);
    }
}
