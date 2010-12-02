using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Collections;
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
        public static CswNbtResources makeCswNbtResources( CswSetupVblsNbt CswSetupVblsNbt, CswDbCfgInfoNbt CswDbCfgInfoNbt , string FilesPath  )
        {
                CswNbtResources ReturnVal = new CswNbtResources( AppType.Nbt, CswSetupVblsNbt, CswDbCfgInfoNbt,  true, false );
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
