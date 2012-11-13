using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceContainer
    {
        #region ctor

        private CswNbtResources _CswNbtResources;
        private readonly ICswNbtUser _CurrentUser;
        private readonly TextInfo _TextInfo;
        public CswNbtWebServiceContainer( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            _CurrentUser = _CswNbtResources.CurrentNbtUser;
            CultureInfo Culture = Thread.CurrentThread.CurrentCulture;
            _TextInfo = Culture.TextInfo;
        }

        #endregion ctor

        #region DataContract

        /// <summary>
        /// Return Object for Containers
        /// </summary>
        [DataContract]
        public class ContainerDataReturn : CswWebSvcReturn
        {
            public ContainerDataReturn()
            {
                Data = new ContainerData();
            }
            [DataMember]
            public ContainerData Data;
        }

        #endregion DataContract

        #region Public

        public JObject upsertDispenseContainers( string SourceContainerNodeId, string ContainerNodeTypeId, string DesignGrid, string RequestItemId )
        {
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( _CswNbtResources, SourceContainerNodeId );
            return ( wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DesignGrid, RequestItemId ) );
        }

        public JObject updateDispensedContainer( string SourceContainerNodeId, string DispenseType, string Quantity, string UnitId, string RequestItemId )
        {
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( _CswNbtResources, SourceContainerNodeId );
            return ( wiz.dispenseSourceContainer( DispenseType, Quantity, UnitId, RequestItemId ) );
        }

        public CswNbtView getDispensibleContainersView( CswPrimaryKey RequestItemId )
        {
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( _CswNbtResources );
            return wiz.getDispensibleContainersView( RequestItemId );
        }

        public JObject getContainerData( CswPrimaryKey ContainerId )
        {
            JObject Ret = new JObject();
            if( null != ContainerId && Int32.MinValue != ContainerId.PrimaryKey )
            {
                CswNbtObjClassContainer NodeAsContainer = _CswNbtResources.Nodes[ContainerId];
                if( null != NodeAsContainer )
                {
                    Ret["nodetypeid"] = NodeAsContainer.NodeTypeId;
                    Ret["location"] = NodeAsContainer.Location.CachedFullPath;
                    Ret["materialname"] = NodeAsContainer.Material.CachedNodeName;
                    Ret["barcode"] = NodeAsContainer.Barcode.Barcode;
                    Ret["quantity"] = NodeAsContainer.Quantity.Quantity;
                    Ret["unit"] = NodeAsContainer.Quantity.CachedUnitName;
                    Ret["unitid"] = ( NodeAsContainer.Quantity.UnitId ?? new CswPrimaryKey() ).ToString();
                    Ret["sizeid"] = ( NodeAsContainer.Size.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
                }
            }

            return Ret;
        }

        /// <summary>
        /// Gets both ContainerStatistics and ContainerStatuses data
        /// </summary>
        public static void getContainerData( ICswResources CswResources, ContainerDataReturn Return, ContainerData.ReconciliationRequest Request )
        {
            CswNbtActReconciliation _CswNbtActReconciliation = new CswNbtActReconciliation( (CswNbtResources) CswResources );
            Return.Data = _CswNbtActReconciliation.getContainerData( Request );
        }

        /// <summary>
        /// Gets all of the ContainerLocation Statuses along with their Container count and scan percentage for the given Location and timeframe
        /// </summary>
        public static void getContainerStatistics( ICswResources CswResources, ContainerDataReturn Return, ContainerData.ReconciliationRequest Request )
        {
            CswNbtActReconciliation _CswNbtActReconciliation = new CswNbtActReconciliation( (CswNbtResources) CswResources );
            Return.Data = _CswNbtActReconciliation.getContainerStatistics( Request );
        }

        /// <summary>
        /// Gets all Container barcodes and their most recent ContainerLocation Status for the given Location and timeframe
        /// </summary>
        public static void getContainerStatuses( ICswResources CswResources, ContainerDataReturn Return, ContainerData.ReconciliationRequest Request )
        {
            CswNbtActReconciliation _CswNbtActReconciliation = new CswNbtActReconciliation( (CswNbtResources) CswResources );
            Return.Data = _CswNbtActReconciliation.getContainerStatuses( Request );
        }

        #endregion Public

    }
}