using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MenuList
{
    public interface IMenuListFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<PathogenMenuEntry> Pathogens(JobType type, DateTime StartDate);
        List<WardMenuEntry> Wards(JobType type, DateTime StartDate);
    }
}