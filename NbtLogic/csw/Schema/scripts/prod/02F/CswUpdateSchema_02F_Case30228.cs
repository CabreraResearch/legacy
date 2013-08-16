using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30228: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30228; }
        }

        public override void update()
        {

            #region Update all NTPs "Hidden" column to "false"

            CswTableUpdate nodeTypePropsTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "ntp.setHidden", "nodetype_props" );
            DataTable nodetypePropsDT = nodeTypePropsTU.getTable();
            foreach( DataRow row in nodetypePropsDT.Rows )
            {
                row["hidden"] = CswConvert.ToDbVal( false );
            }
            nodeTypePropsTU.update( nodetypePropsDT );

            #endregion

            #region CISPro

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                if( null != UserJurisdictionNTP )
                {
                    UserJurisdictionNTP.updateLayout( CswEnumNbtLayoutType.Add, false );
                    UserJurisdictionNTP.updateLayout( CswEnumNbtLayoutType.Edit, DoMove: false, TabId: UserNT.getFirstNodeTypeTab().TabId );
                }
            }

            #endregion

            #region Containers

            CswNbtMetaDataObjectClass locationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int NodeTypeId in locationOC.getNodeTypeIds() )
            {
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.Containers, "Containers" );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( NodeTypeId, "Inventory Levels", "Inventory Levels", 2 );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );

                CswNbtMetaDataNodeTypeProp AllowInvNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, NodeTypeId, AllowInvNTP, false );

                CswNbtMetaDataNodeTypeProp InvGrpNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, NodeTypeId, InvGrpNTP, false );

                CswNbtMetaDataNodeTypeProp StorageCompatNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, NodeTypeId, StorageCompatNTP, false );
            }

            CswNbtMetaDataPropertySet MaterialSet = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass materialOC in MaterialSet.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType materialNT in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes( materialOC.ObjectClassId ) )
                {
                    string sizesNTPName = materialNT.NodeTypeName + " Sizes";
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, sizesNTPName, "Containers", 99 );

                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, "Inventory Levels", "Containers", 99 );

                    string containersNTPName = materialNT.NodeTypeName + " Containers";
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, containersNTPName, "Containers", 99 );

                    CswNbtMetaDataNodeTypeTab materialNTT = materialNT.getFirstNodeTypeTab();
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.ApprovedForReceiving, materialNTT );

                    CswNbtMetaDataNodeTypeTab materialIdentityNTT = materialNT.getIdentityTab();
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.Receive, materialIdentityNTT, 2, 2 );
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.Request, materialIdentityNTT, 1, 2 );

                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.StorageCompatibility, "Hazards" );
                }
            }

            #endregion

        } // update()

    }

}//namespace ChemSW.Nbt.Schema