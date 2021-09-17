using SmICSCoreLib.JSONFileStream;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;


namespace SmICSCoreLib.Factories.NEC
{
    public class NECResultFileFactory : INECResultFileFactory
    {
        public void Process(List<NECResultDataModel> data)
        {
            JSONWriter.Write(data, @"../SmICSWebApp/Resources/nec/json", DateTime.Now.ToString("yyyy-MM-dd"));
        }
    }
}
