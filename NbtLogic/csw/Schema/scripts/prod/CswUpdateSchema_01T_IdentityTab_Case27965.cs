using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01T_IdentityTab_Case27965 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 27965; }
        }

        public override void update()
        {
            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "nodetype_tabset_servermanaged", "nodetype_tabset" );
            DataTable Table = TableUpdate.getTable();
            foreach( DataRow Row in Table.Rows )
            {
                Row["servermanaged"] = CswConvert.ToDbVal( false );
            }
            TableUpdate.update( Table );

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            Collection<CswNbtNode> Users = UserOc.getNodes( true, false );
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypesLatestVersion() )
            {
                Int32 TabCount = NodeType.getNodeTypeTabIds().Count;
                CswNbtMetaDataNodeTypeTab IdentityTab = NodeType.getIdentityTab();
                if( null == IdentityTab )
                {
                    IdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( NodeType, CswNbtMetaData.IdentityTabName, 0 );
                }
                if( null == IdentityTab )
                {
                    throw new CswDniException( ErrorType.Error, "Could not find or create an Identity tab for " + NodeType.NodeTypeName + ".", "Identity tab creation failed." );
                }
                IdentityTab.ServerManaged = true;
                if( TabCount > 1 )
                {
                    foreach( CswNbtMetaDataNodeTypeProp IdentityProp in CswNbtMetaData.TemplateValueToPropCollection( NodeType.getNodeTypeProps(), NodeType.NameTemplateValue ) )
                    {
                        if( null != IdentityProp )
                        {
                            IdentityProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, false, IdentityTab.TabId );
                        }
                    }
                }
                foreach( CswNbtObjClassUser User in Users )
                {
                    if( null != User )
                    {
                        if( null != User.RoleNode )
                        {
                            CswNbtObjClassRole Role = User.RoleNode;
                            if( false == _CswNbtSchemaModTrnsctn.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Edit, NodeType, User ) )
                            {
                                if( _CswNbtSchemaModTrnsctn.Permit.canAnyTab( CswNbtPermit.NodeTypePermission.Edit, NodeType, User ) )
                                {
                                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypeTabPermission.Edit, IdentityTab, Role, true );
                                }
                            }
                            else if( false == _CswNbtSchemaModTrnsctn.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, NodeType, User ) )
                            {
                                if( _CswNbtSchemaModTrnsctn.Permit.canAnyTab( CswNbtPermit.NodeTypePermission.View, NodeType, User ) )
                                {
                                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypeTabPermission.View, IdentityTab, Role, true );
                                }
                            }
                        }
                    }
                }
            }
        }

        //Update()

    }//class CswUpdateSchemaCaseXXXXX

}//namespace ChemSW.Nbt.Schema