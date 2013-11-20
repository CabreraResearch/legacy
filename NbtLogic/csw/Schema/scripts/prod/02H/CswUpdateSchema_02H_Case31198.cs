using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    class CswUpdateSchema_02H_Case31198 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31198; }
        }

        public override string Title
        {
            get { return "Fix Inventory Levels View: NTPs"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp InventoryLevelsOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryLevels );

            string ViewXml = InventoryLevelsOCP.ViewXml;

            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp InventoryLvlsNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( LocationNT.NodeTypeId, InventoryLevelsOCP.ObjectClassPropId );
                CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "fixNTPViewXml_Case31198", "node_views" );
                DataTable NodeViewsDt = TableUpdate.getTable( "where nodeviewid = " + InventoryLvlsNTP.ViewId.get() );
                if( NodeViewsDt.Rows.Count > 0 )
                {
                    NodeViewsDt.Rows[0]["viewxml"] = ViewXml;
                }
                TableUpdate.update( NodeViewsDt );
            }

        } // update()

    }
}//namespace ChemSW.Nbt.Schema
