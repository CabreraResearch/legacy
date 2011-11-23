using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Collections;
using ChemSW;
using ChemSW.RscAdo;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Mail;
using ChemSW.Log;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Config;
using ChemSW.Security;
using ChemSW.TblDn;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Config;
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
                                        OtherResources.IsDeleteModeLogical );
        }

        /// <summary>
        /// Create a new CswNbtResources
        /// </summary>
        public static CswNbtResources makeCswNbtResources( AppType AppType, SetupMode SetupMode, bool ExcludeDisabledModules, bool IsDeleteModeLogical, ICswSuperCycleCache CswSuperCycleCache = null )
        {


            if( SetupMode.NbtWeb == SetupMode )
            {
                if( null == CswSuperCycleCache )
                {
                    throw ( new CswDniException( "The web consumer must provide a super cycle cache!" ) );
                }
            }
            else
            {
                CswSuperCycleCache = new CswSuperCycleCacheDefault();
            }

            CswSetupVblsNbt SetupVbls = new CswSetupVblsNbt( SetupMode );
            CswDbCfgInfoNbt ConfigInfo = new CswDbCfgInfoNbt( SetupMode );


            string FilesPath = CswTools.getConfigurationFilePath( SetupMode );

            CswNbtResources ReturnVal = new CswNbtResources( AppType, SetupVbls, ConfigInfo, ExcludeDisabledModules, IsDeleteModeLogical, CswSuperCycleCache );
            ReturnVal.SetDbResources( new CswNbtTreeFactory( FilesPath ) );

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
