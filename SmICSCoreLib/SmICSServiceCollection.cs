using Microsoft.Extensions.DependencyInjection;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.PatientInformation.Vaccination;
using SmICSCoreLib.AQL.Contact_Nth_Network;
using SmICSCoreLib.AQL.Lab;
using SmICSCoreLib.AQL.Lab.EpiKurve;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.Algorithm;
using SmICSCoreLib.AQL.Algorithm.NEC;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.Patient_Stay;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.Patient_Stay.Cases;
using SmICSCoreLib.AQL.Patient_Stay.WeekCase;
using SmICSCoreLib.AQL.Employees;
using SmICSCoreLib.AQL.Employees.PersInfoInfecCtrl;
using SmICSCoreLib.AQL.Employees.ContactTracing;
using SmICSCoreLib.AQL.Employees.PersonData;

namespace SmICS
{
    public static class SmICSServiceCollection
    {
        public static IServiceCollection AddSmICSLibrary(this IServiceCollection services)
        {
            services.AddSingleton<RestClientConnector>();
            services.AddSingleton<IRestDataAccess, RestDataAccess>();
            
            services.AddTransient<IPatientMovementFactory, PatientMovementFactory>();
            services.AddTransient<IPatientLabordataFactory, PatientLabordataFactory>();
            services.AddTransient<IMibiPatientLaborDataFactory, MibiPatientLaborDataFactory>();
            services.AddTransient<ISymptomFactory, SymptomFactory>();
            services.AddTransient<IVaccinationFactory, VaccinationFactory>();

            services.AddTransient<IPatientInformation, PatientInformation>();

            services.AddTransient<IContactNetworkFactory, ContactNetworkFactory>();
            services.AddTransient<IEpiCurveFactory, EpiCurveFactory>();
            services.AddTransient<ISymptomFactory, SymptomFactory>();
            services.AddTransient<IMibiPatientLaborDataFactory, MibiPatientLaborDataFactory>();
            
            services.AddTransient<INECCombinedFactory, NECCombinedFactory>();

            services.AddTransient<INECResultDataFactory, NECResultDataFactory>();
            services.AddTransient<INECResultFileFactory, NECResultFileFactory>();

            services.AddTransient<ILabData, LabData>();
            services.AddSingleton<IAlgorithmData, AlgortihmData>();

            services.AddTransient<IStationaryFactory, StationaryFactory>();
            services.AddTransient<IPatinet_Stay, Patinet_Stay>();
            services.AddTransient<ICountFactory, CountFactory>();
            services.AddTransient<ICaseFactory, CaseFactory>();
            services.AddTransient<IWeekCaseFactory, WeekCaseFactory>();

            services.AddTransient<IEmployeeInformation, EmployeeInformation>();
            services.AddTransient<IContactTracingFactory, ContactTracingFactory>();
            services.AddTransient<IPersInfoInfecCtrlFactory, PersInfoInfecCtrlFactory>();
            services.AddTransient<IPersonDataFactory, PersonDataFactory>();

            return services;
        }
    }
}
