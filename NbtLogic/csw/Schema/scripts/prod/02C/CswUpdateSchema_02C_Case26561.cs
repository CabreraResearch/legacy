using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02C_Case26561 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 26561; }
        }

        public override void update()
        {
            CswTableUpdate CswTableUpdateAction = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "update_for_" + CaseNo.ToString(), "actions" );
            DataTable ActionsTable = CswTableUpdateAction.getTable( "where actionname='Assign Inventory Groups'" );
            if( 1 == ActionsTable.Rows.Count )
            {
                ActionsTable.Rows[0]["actionname"] = "Manage Locations";
                CswTableUpdateAction.update( ActionsTable );
            }


            

            CswNbtMetaDataObjectClass CswNbtMetaDataObjClassLocation = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            if( null != CswNbtMetaDataObjClassLocation )
            {
                foreach( CswNbtMetaDataNodeType CurrentLocationNodeType in CswNbtMetaDataObjClassLocation.getNodeTypes() )
                {


                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Preview, CurrentLocationNodeType.NodeTypeId, CurrentLocationNodeType.getNodeTypeProp( CswNbtObjClassLocation.PropertyName.AllowInventory ), false );
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Preview, CurrentLocationNodeType.NodeTypeId, CurrentLocationNodeType.getNodeTypeProp( CswNbtObjClassLocation.PropertyName.InventoryGroup ), false );
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Preview, CurrentLocationNodeType.NodeTypeId, CurrentLocationNodeType.getNodeTypeProp( CswNbtObjClassLocation.PropertyName.ControlZone ), false );
                }
            }
            //_CswNbtSchemaModTrnsctn


        } // update()

    }//class CswUpdateSchema_02B_CaseXXXXX

}//namespace ChemSW.Nbt.Schema