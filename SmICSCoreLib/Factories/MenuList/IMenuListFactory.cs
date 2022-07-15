using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MenuList
{
    public interface IMenuListFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Task<List<PathogenMenuEntry>> PathogensAsync(JobType type, DateTime StartDate);
        Task<List<WardMenuEntry>> WardsAsync(JobType type, DateTime StartDate);
    }
}