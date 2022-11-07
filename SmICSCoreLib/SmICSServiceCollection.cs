using Microsoft.Extensions.DependencyInjection;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Lab.ViroLabData;
using SmICSCoreLib.Factories.Symptome;
using SmICSCoreLib.Factories.Vaccination;
using SmICSCoreLib.Factories.ContactNetwork;
using SmICSCoreLib.Factories.EpiCurve;
using SmICSCoreLib.REST;
using SmICSCoreLib.Factories.Lab.MibiLabData;
using SmICSCoreLib.Factories.PatientStay;
using SmICSCoreLib.Factories.PatientStay.Stationary;
using SmICSCoreLib.Factories.PatientStay.Count;
using SmICSCoreLib.Factories.Employees;
using SmICSCoreLib.Factories.Employees.PersInfoInfecCtrl;
using SmICSCoreLib.Factories.Employees.ContactTracing;
using SmICSCoreLib.Factories.Employees.PersonData;
using SmICSCoreLib.Factories.InfectionSituation;
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
using SmICSCoreLib.Factories.NUMNode;
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

            services.AddSingleton<INUMNodeFactory, NUMNodeFactory>();
            services.AddSingleton<DashboardRestClientConnector>();

            services.AddTransient<IPatientMovementFactory, PatientMovementFactory>();
            services.AddTransient<IViroLabDataFactory, ViroLabDataFactory>();
            services.AddTransient<IMibiPatientLaborDataFactory, MibiPatientLaborDataFactory>();
            services.AddTransient<ISymptomFactory, SymptomFactory>();
            services.AddTransient<IVaccinationFactory, VaccinationFactory>();
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
            services.AddScoped<InfectionStatusFactory>();
            services.AddScoped<IHelperFactory, HelperFactory>();
            #endregion
            services.AddTransient<IContactNetworkFactory, ContactNetworkFactory>();
            services.AddTransient<IEpiCurveFactory, EpiCurveFactory>();


            services.AddTransient<IOutbreakDetectionParameterFactory, OutbreakDetectionParameterFactory>();
            services.AddSingleton<OutbreakDetectionProxy>();

            services.AddTransient<IStationaryFactory, StationaryFactory>();
            services.AddTransient<SmICSCoreLib.Factories.PatientStay.IPatientStay, SmICSCoreLib.Factories.PatientStay.PatientStay>();
            services.AddTransient<ICountFactory, CountFactory>();

            services.AddTransient<IInfectionSituationFactory, InfectionSituationFactory>();

            services.AddTransient<IEmployeeInformation, EmployeeInformation>();
            services.AddTransient<IContactTracingFactory, ContactTracingFactory>();
            services.AddTransient<IPersInfoInfecCtrlFactory, PersInfoInfecCtrlFactory>();
            services.AddTransient<IPersonDataFactory, PersonDataFactory>();

            services.AddSingleton<IFeasabilityFactory, FeasabilityFactory>();
            return services;
        }
    }
}
