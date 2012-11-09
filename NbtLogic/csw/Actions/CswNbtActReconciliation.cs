using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActReconciliation
    {
        private CswNbtResources _CswNbtResources = null;

        public CswNbtActReconciliation( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public ContainerData getContainerStatistics( ContainerData.ReconciliationRequest Request )
        {
            ContainerData ContainerStatistics = new ContainerData();
            //TODO - for each status, grab the number of containers whose most recent ContainerLocation (within the given timeframe) matches that status
            return ContainerStatistics;
        }

        public ContainerData getContainerStatuses( ContainerData.ReconciliationRequest Request )
        {
            ContainerData ContainerStatuses = new ContainerData();
            //TODO - for each container, grab barcode and status/action/actionapplied of most recent ContainerLocation (within the given timeframe)
            return ContainerStatuses;
        }
    }
}
