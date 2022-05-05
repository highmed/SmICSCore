using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Quartz;
using System;
using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.DB.MenuItems;
using SmICSCoreLib.DB.Models;
using SmICSCoreLib.Factories.MenuList;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.CronJobs
{
    [DisallowConcurrentExecution]
    public class MenuItemsJob : IJob
    {
        private IMenuItemDataAccess _dataAccess;
        private IMenuListFactory _menuListFac;
        public MenuItemsJob(IMenuItemDataAccess dataAccess, IMenuListFactory menuListFac)
        {
            _dataAccess = dataAccess;
            _menuListFac = menuListFac;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Task patho = Task.Run(UpdatePathogens);
            Task wards = Task.Run(UpdateWards);
            await Task.WhenAll(wards);
        }

        public async Task UpdateWards()
        {
            List<Ward> wards = await _dataAccess.GetWards();
            List<WardMenuEntry> wardMenuEntries = null;
            if (wards.Count == 0)
            {
                DateTime date = new DateTime((DateTime.Now.Year - 10), 1, 1);
                try {
                    string year = Environment.GetEnvironmentVariable("FIRST_DATA_ENTRY_YEAR");
                    date = new DateTime(Convert.ToInt32(year), 1, 1);
                }
                catch
                { 
                    Console.WriteLine(string.Format("ENV FIRST_DATA_ENRTY_YEAR couldn't be read. Instead {0} was set as FIRST_DATA_ENRTY_YEAR!", date.Year));
                }
                wardMenuEntries = _menuListFac.Wards(JobType.FIRST_STARTUP, date);
            }
            else
            {
                wardMenuEntries = _menuListFac.Wards(JobType.DAILY, DateTime.Now.Date.AddDays(-1.0));
            }
            if (wardMenuEntries is not null)
            {
                foreach (WardMenuEntry ward in wardMenuEntries)
                {
                    if (!string.IsNullOrEmpty(ward.ID))
                    {
                        if (wards is not null && wards.Where(w => w.Name == ward.ID).Count() == 0)
                        {
                            _dataAccess.SetWard(new Ward() { Name = ward.ID });
                        }
                        else if (wards is null)
                        {
                            _dataAccess.SetWard(new Ward() { Name = ward.ID });
                        }
                    }
                }
                foreach (Ward ward in wards)
                {
                    if (wardMenuEntries.Where(w => w.ID == ward.Name).Count() == 0)
                    {
                        _dataAccess.DeleteWard(ward);
                    }
                }
                wardMenuEntries = null;
                wards = null;
            }
        }

        public async Task UpdatePathogens()
        {
            List<PathogenMenuEntry> pathoMenu = null;
            List<Pathogen> pathogens = await _dataAccess.GetPathogens();
            if(pathogens.Count == 0)
            {
                DateTime date = new DateTime((DateTime.Now.Year - 10), 1, 1);
                try
                {
                    string year = Environment.GetEnvironmentVariable("FIRST_DATA_ENTRY_YEAR");
                    date = new DateTime(Convert.ToInt32(year), 1, 1);
                }
                catch
                {
                    Console.WriteLine(string.Format("ENV FIRST_DATA_ENRTY_YEAR couldn't be read. Instead {0} was set as FIRST_DATA_ENRTY_YEAR!", date.Year));
                }
                pathoMenu = _menuListFac.Pathogens(JobType.FIRST_STARTUP, date);
            }
            else
            {
                pathoMenu = _menuListFac.Pathogens(JobType.DAILY, DateTime.Now.Date.AddDays(-1.0));
            }
            if (pathoMenu is not null)
            {
                foreach (PathogenMenuEntry pat in pathoMenu)
                {
                    if (!string.IsNullOrEmpty(pat.ID))
                    {
                        if (pathogens.Where(p => p.Code == pat.ID).Count() == 0)
                        {
                            _dataAccess.SetPathogen(new Pathogen() { Code = pat.ID, Name = pat.Name });
                        }
                        else if (pathogens.Where(p => p.Code == pat.ID).Count() == 1)
                        {
                            _dataAccess.UpdatePathogen(new Pathogen() { Code = pat.ID, Name = pat.Name });
                        }
                    }
                }
                foreach (Pathogen pat in pathogens)
                {
                    if (pathoMenu.Where(p => p.ID == pat.Code).Count() == 0)
                    {
                        _dataAccess.DeletePathogen(pat);
                    }
                }
                pathoMenu = null;
                pathogens = null;
            }
        }
    }
}
