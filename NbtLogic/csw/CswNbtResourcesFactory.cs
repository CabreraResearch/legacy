using ChemSW.Config;
using ChemSW.Log;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.TreeEvents;

namespace ChemSW.Nbt
{
    /// <summary>
    /// A collection of useful resources for NBT business logic.
    /// </summary>
    public class CswNbtResourcesFactory
    {
        /// <summary>
        /// Create a new CswNbtResources using same initialization parameters as an existing one
        /// Does not copy data or access id
        /// </summary>
        public static CswNbtResources makeCswNbtResources( CswNbtResources OtherResources )
        {
            return makeCswNbtResources( OtherResources.AppType,
                                        OtherResources.SetupVbls.SetupMode,
                                        OtherResources.ExcludeDisabledModules,
                                        OtherResources.IsDeleteModeLogical,
                                        OtherResources.CswSuperCycleCache,
                                        OtherResources.PooledConnectionState,
                                        null, OtherResources.CswLogger );
        }

        /// <summary>
        /// Create a new CswNbtResources
        /// </summary>
        public static CswNbtResources makeCswNbtResources( AppType AppType, SetupMode SetupMode, bool ExcludeDisabledModules, bool IsDeleteModeLogical, ICswSuperCycleCache CswSuperCycleCache = null, ChemSW.RscAdo.PooledConnectionState PooledConnectionState = RscAdo.PooledConnectionState.Open, ICswResources CswResourcesMaster = null, ICswLogger CswLogger = null )
        {
            if( null == CswSuperCycleCache )
            {
                CswSuperCycleCache = new CswSuperCycleCacheDefault();
            }

            CswSetupVblsNbt SetupVbls = new CswSetupVblsNbt( SetupMode );
            CswDbCfgInfoNbt ConfigInfo = new CswDbCfgInfoNbt( SetupMode, IsMobile: false );
            
            CswNbtResources ReturnVal = new CswNbtResources( AppType, SetupVbls, ConfigInfo, ExcludeDisabledModules, IsDeleteModeLogical, CswSuperCycleCache, CswResourcesMaster, CswLogger );
            ReturnVal.SetDbResources( new CswNbtTreeFactory(), PooledConnectionState );

            //bz # 9896: This events must only be assigned when we first instance the class;
            //if we also assign them to cached resources, we get duplicate events occuring :-(
            CswNbtMetaDataEvents CswNbtMetaDataEvents = new CswNbtMetaDataEvents( ReturnVal );
            ReturnVal.OnMakeNewNodeType += new CswNbtResources.NewNodeTypeEventHandler( CswNbtMetaDataEvents.OnMakeNewNodeType );
            ReturnVal.OnCopyNodeType += new CswNbtResources.CopyNodeTypeEventHandler( CswNbtMetaDataEvents.OnCopyNodeType );
            ReturnVal.OnMakeNewNodeTypeProp += new CswNbtResources.NewNodeTypePropEventHandler( CswNbtMetaDataEvents.OnMakeNewNodeTypeProp );
            ReturnVal.OnEditNodeTypePropName += new CswNbtResources.EditPropNameEventHandler( CswNbtMetaDataEvents.OnEditNodeTypePropName );
            ReturnVal.OnDeleteNodeTypeProp += new CswNbtResources.DeletePropEventHandler( CswNbtMetaDataEvents.OnDeleteNodeTypeProp );
            ReturnVal.OnEditNodeTypeName += new CswNbtResources.EditNodeTypeNameEventHandler( CswNbtMetaDataEvents.OnEditNodeTypeName );

            return ( ReturnVal );
        }
    } // CswNbtResourcesFactory

}//ChemSW.NbtResources
