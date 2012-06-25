using System.Globalization;
using System.Threading;
using ChemSW.Nbt.csw.Actions;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceDispenseContainer
    {
        #region ctor

        private CswNbtResources _CswNbtResources;
        private readonly ICswNbtUser _CurrentUser;
        private readonly TextInfo _TextInfo;
        public CswNbtWebServiceDispenseContainer( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            _CurrentUser = _CswNbtResources.CurrentNbtUser;
            CultureInfo Culture = Thread.CurrentThread.CurrentCulture;
            _TextInfo = Culture.TextInfo;
        }

        #endregion ctor


        #region Public

        public JObject upsertDispenseContainers( string SourceContainerNodeId, string ContainerNodeTypeId, string DesignGrid )
        {
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( _CswNbtResources );
            return ( wiz.upsertDispenseContainers( SourceContainerNodeId, ContainerNodeTypeId, DesignGrid ) );
        }

        public JObject updateDispensedContainer( string SourceContainerNodeId, string DispenseType, string Quantity )
        {
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( _CswNbtResources );
            return ( wiz.updateDispensedContainer( SourceContainerNodeId, DispenseType, Quantity ) );
        }

        #endregion Public

    }
}