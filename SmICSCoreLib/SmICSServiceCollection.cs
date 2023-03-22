using Microsoft.Extensions.DependencyInjection;
using SmICSCoreLib.Factories.EpiCurve;
using SmICSCoreLib.REST;
using SmICSCoreLib.Factories.RKIConfig;
using SmICSCoreLib.Factories.OutbreakDetection;
using SmICSCoreLib.OutbreakDetection;
using SmICSCoreLib.Factories.PatientInformation.PatientData;
using SmICSCoreLib.Factories.MiBi;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.Contact;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.MenuList;
using SmICSCoreLib.Factories.Feasability;
using SmICSCoreLib.DB;
using SmICSCoreLib.DB.MenuItems;
using SmICSCoreLib.Factories.Helpers;

namespace SmICS
{
    public static class SmICSServiceCollection
    {
        public static IServiceCollection AddSmICSLibrary(this IServiceCollection services)
        {
            services.AddSingleton<RestClientConnector>();
            services.AddSingleton<SmICSCoreLib.REST.IRestDataAccess, RestDataAccess>();
            services.AddSingleton<DapperContext>();
            services.AddSingleton<IDataAccess, DataAccess>();

            services.AddSingleton<IMenuItemDataAccess, MenuItemDataAccess>();
            services.AddSingleton<IMenuListFactory, MenuListFactory>();

            services.AddTransient<IPatientDataFactory, PatientDataFactory>();
            #region New MiBi Services
            //Could replace some old factories
            services.AddScoped<IHospitalizationFactory, HospitalizationFactory>();
            services.AddScoped<IPatientStayFactory, PatientStayFactory>();
            services.AddScoped<IAntibiogramFactory, AntibiogramFactory>();
            services.AddScoped<IPathogenFactory, PathogenFactory>();
            services.AddScoped<ISpecimenFactory, SpecimenFactory>();
            services.AddScoped<ILabResultFactory, LabResultFactory>();
            services.AddScoped<IContactFactory, ContactFactory>();
            services.AddScoped<IInfectionStatusFactory, InfectionStatusFactory>();
            services.AddScoped<IHelperFactory, HelperFactory>();
            #endregion
            services.AddTransient<IEpiCurveFactory, EpiCurveFactory>();


            services.AddTransient<IOutbreakDetectionParameterFactory, OutbreakDetectionParameterFactory>();
            services.AddSingleton<OutbreakDetectionProxy>();

            services.AddTransient<IRKILabDataFactory, RKILabDataFactory>();
            services.AddSingleton<IFeasabilityFactory, FeasabilityFactory>();
            return services;
        }
    }
}
