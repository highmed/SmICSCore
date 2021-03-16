using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_PathogenFlag
{
    public class PatientPathogenFlagFactory : IPatientPathogenFlagFactory
    {
        private IRestDataAccess _restData;
        public PatientPathogenFlagFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<PathogenFlagModel> Process(PatientListParameter parameter)
        {
            List<PathogenFlagModel> pathogenFlagList = _restData.AQLQuery<PathogenFlagModel>(AQLCatalog.PatientPathogenFlag(parameter).Query);

            if (pathogenFlagList is null)
            {
                return new List<PathogenFlagModel>();
            }

            return pathogenFlagList;
        }
    }
}
