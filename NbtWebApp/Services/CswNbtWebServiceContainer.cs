using System.Globalization;
using System.Threading;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Security;
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

        #endregion Public

    }
}