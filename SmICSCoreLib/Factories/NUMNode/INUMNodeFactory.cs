using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NUMNode
{
    public interface INUMNodeFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        public List<NUMNodeModel> Process();
        public void FirstDataEntry();
    }
}
