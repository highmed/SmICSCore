using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

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

        public List<Pathogen> Process(PathogenParameter pathogenParameter)
        {
            List<Pathogen> pathogens = RestDataAccess.AQLQuery<Pathogen>(PathogenQuery(pathogenParameter));
            if (pathogenParameter != null)
            {
                if (pathogenParameter.MedicalField == MedicalField.MICROBIOLOGY)
                {
                    foreach (Pathogen pathogen in pathogens)
                    {
                        AntibiogramParameter parameter = pathogenParameter as AntibiogramParameter;
                        parameter.IsolatNo = pathogen.IsolatNr;
                        parameter.Pathogen = pathogen.Name;
                        pathogen.Antibiograms = _antibiogramFac.Process(parameter);
                    }
                }
                return pathogens;
            }
            return null;
        }


        private AQLQuery PathogenQuery(PathogenParameter parameter)
        {
            AQLQuery query = null;
            switch (parameter.MedicalField)
            {
                case MedicalField.MICROBIOLOGY:
                    query = new AQLQuery()
                    {
                        Name = "Pathogen - Mikrobiologischer Befunde",
                        Query = @$"SELECT u/items[at0001,'Erregername']/value as Name,
                        u/items[at0024,'Nachweis?']/value as Result,
                        b/items[at0003]/value as Rate,
                        u/items[at0027,'Isolatnummer']/valu as IsolatNr,
                        {parameter.MedicalField} as MedicalField,
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER u[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                        CONTAINS (CLUSTER b[openEHR-EHR-CLUSTER.erregerdetails.v1]) 
                        WHERE c/name/value='{parameter.MedicalField}'
                        AND c/uid/value='{parameter.UID}'
                        AND u/items[at0026,'Zugehörige Laborprobe']/value = '{parameter.LabID}'"
                    };
                    if (!string.IsNullOrEmpty(parameter.Name))
                    {
                        query.Query += $" AND u/items[at0024,'Nachweis?']/value/value = '{ parameter.Name }'";
                    }
                    return query;
                case MedicalField.VIROLOGY:
                    query = new AQLQuery()
                    {
                        //Needs to be edited
                        Name = "Pathogen",
                        Query = @$"SELECT u/items[at0001,'Erregername']/value/value as Name,
                                u/items[at0024,'Nachweis?']/value/value as Result,
                                b/items[at0001,'Quantitatives Ergebnis']/value/magnitude as Rate,
                                b/items[at0001,'Quantitatives Ergebnis']/value/units as Unit,
                                {parameter.MedicalField} as MedicalField
                                FROM EHR e
                                CONTAINS COMPOSITION c
                                CONTAINS CLUSTER u[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                                CONTAINS (CLUSTER b[openEHR-EHR-CLUSTER.erregerdetails.v1]) 
                                WHERE c/name/value='{parameter.MedicalField}'
                                AND c/uid/value='{parameter.UID}'
                                AND u/items[at0026,'Zugehörige Laborprobe']/value/id = '{parameter.LabID}'"
                    };
                    if (!string.IsNullOrEmpty(parameter.Name))
                    {
                        query.Query += $" AND u/items[at0024,'Nachweis?']/value/value = '{ parameter.Name }'";
                    }
                    return query;
                default:
                    throw new NotImplementedException();
            }
            
        }
    }
}
