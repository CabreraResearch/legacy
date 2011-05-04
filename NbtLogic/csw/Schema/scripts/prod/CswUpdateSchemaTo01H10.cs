using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-10
    /// </summary>
    public class CswUpdateSchemaTo01H10 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 10 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H10( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {

            // BZ 20081 - Set setup tab to be last.
            // This implementation updates the locked ones.
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            string InspectionNTIds = string.Empty;
            foreach( CswNbtMetaDataNodeType InspectionDesignNT in InspectionDesignOC.NodeTypes )
            {
                if( InspectionNTIds != string.Empty )
                    InspectionNTIds += ",";
                InspectionNTIds += InspectionDesignNT.NodeTypeId;
            }

            CswTableUpdate TabsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-10_Tabs_Update", "nodetype_tabset" );
            DataTable TabsTable = TabsUpdate.getTable( "where tabname = 'Setup' and nodetypeid in (" + InspectionNTIds + ")" );
            foreach( DataRow TabRow in TabsTable.Rows )
            {
                TabRow["taborder"] = CswConvert.ToDbVal( 10 );
            }
            TabsUpdate.update( TabsTable );

            // BZ 20081 - Make Finished and Cancelled 'required' to remove the blank option
            CswNbtMetaDataObjectClassProp FinishedOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.FinishedPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FinishedOCP, CswNbtSubField.SubFieldName.Checked, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FinishedOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, CswConvert.ToDbVal( true ) );

            CswNbtMetaDataObjectClassProp CancelledOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.CancelledPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CancelledOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, CswConvert.ToDbVal( true ) );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CancelledOCP, CswNbtSubField.SubFieldName.Checked, false );

            // Update existing values
            foreach( CswNbtMetaDataNodeType InspectionDesignNT in InspectionDesignOC.NodeTypes )
            {
                foreach( CswNbtNode IDNode in InspectionDesignNT.getNodes( false, true ) )
                {
                    CswNbtObjClassInspectionDesign IDNodeAsID = (CswNbtObjClassInspectionDesign) CswNbtNodeCaster.AsInspectionDesign( IDNode );
                    IDNodeAsID.Finished.Checked = Tristate.False;
                    IDNodeAsID.Cancelled.Checked = Tristate.False;
                    IDNode.postChanges( false );
                }
            }

            // Update existing values
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

        } // update()

    }//class CswUpdateSchemaTo01H10

}//namespace ChemSW.Nbt.Schema


