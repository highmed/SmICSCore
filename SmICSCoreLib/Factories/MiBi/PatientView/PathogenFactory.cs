using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class PathogenFactory : IPathogenFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly IAntibiogramFactory _antibiogramFac;

        public PathogenFactory(IRestDataAccess restDataAccess, IAntibiogramFactory antibiogramFac)
        {
            RestDataAccess = restDataAccess;
            _antibiogramFac = antibiogramFac;
        }

        public async Task<List<Pathogen>> ProcessAsync(PathogenParameter pathogenParameter)
        {
            try { 
                List<Pathogen> pathogens = await RestDataAccess.AQLQueryAsync<Pathogen>(PathogenQuery(pathogenParameter));
                if (pathogens != null)
                {
                    if (pathogenParameter.MedicalField == MedicalField.MICROBIOLOGY)
                    {
                        foreach (Pathogen pathogen in pathogens)
                        {
                            AntibiogramParameter parameter = new AntibiogramParameter(pathogenParameter);
                            parameter.IsolatNo = pathogen.IsolatNr;
                            parameter.Pathogen = pathogen.Name;
                            pathogen.Antibiograms = await _antibiogramFac.ProcessAsync(parameter);
                        }
                    }
                    return pathogens;
                }
                return null;
            }
            catch
            {
                throw;
            }
}

        /*As prototypic organisms, methicillin-resistant S.aureus (MRSA) and carbapenem-resistant bacteria (CPB:
Klebsiella pneumoniae, E.coli, and Acinetobacter baumannii complex; for which mandatory reporting is
implemented) will be monitored first as a proof of principle, followed by methicillin-susceptible S.aureus
(MSSA).*/

        private AQLQuery PathogenQuery(PathogenParameter parameter)
        {
            AQLQuery query = null;
            switch (parameter.MedicalField)
            {
                case MedicalField.MICROBIOLOGY:
                    query = new AQLQuery()
                    {
                        Name = "Pathogen - Mikrobiologischer Befunde",
                        Query = @$"SELECT DISTINCT u/items[at0001,'Erregername']/value/value as Name,
                        u/items[at0001,'Erregername']/value/defining_code/code_string as ID,
                        u/items[at0024]/value/value as ResultString,
                        u/items[at0024]/value/value as ResultText,
                        b/items[at0003]/value/value as Rate,
                        u/items[at0027,'Isolatnummer']/value/magnitude as IsolatNr,
                        '{parameter.MedicalField}' as MedicalField
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER u[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                        CONTAINS (CLUSTER b[openEHR-EHR-CLUSTER.erregerdetails.v1]) 
                        WHERE c/name/value='{parameter.MedicalField}'
                        AND c/uid/value='{parameter.UID}'
                        AND u/items[at0026,'Zugehörige Laborprobe']/value/id = '{parameter.LabID}'"
                    };
                    if(parameter.PathogenCodes is not null)
                    {
                        query.Query += $" AND u/items[at0001]/value/defining_code/code_string MATCHES { parameter.PathogenCodesToAqlMatchString() }";
                    }
                    return query;
                case MedicalField.VIROLOGY:
                    query = new AQLQuery()
                    {
                        //Needs to be edited
                        Name = "Pathogen",
                        Query = @$"SELECT u/items[at0024]/value/value as Name,
                                u/items[at0024]/value/defining_code/code_string as ID,
                                u/items[at0001,'Nachweis']/value/defining_code/code_string as ResultString,
                                u/items[at0001,'Nachweis']/value/value as ResultText,
                                u/items[at0001,'Quantitatives Ergebnis']/value/magnitude as Rate,
                                u/items[at0001,'Quantitatives Ergebnis']/value/units as Unit,
                                '{parameter.MedicalField}' as MedicalField
                                FROM EHR e
                                CONTAINS COMPOSITION c
                                CONTAINS CLUSTER u[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                                WHERE c/name/value='{parameter.MedicalField}'
                                AND c/uid/value='{parameter.UID}'
                                AND u/items[at0026,'Zugehörige Laborprobe']/value/id = '{parameter.LabID}'"
                    };
                    if (parameter.PathogenCodes is not null)
                    {
                        query.Query += $" AND u/items[at0024]/value/defining_code/code_string MATCHES { parameter.PathogenCodesToAqlMatchString() }";
                    }
                    return query;
                default:
                    throw new NotImplementedException();
            }
            
        }
    }
}
