using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-09
    /// </summary>
    public class CswUpdateSchemaTo01H09 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 09 ); } }
        public CswUpdateSchemaTo01H09( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            // BZ 20081 - Set setup tab to be last.
            // This implementation updates the locked ones.
            string InspectionNTIds = string.Empty;
            foreach( CswNbtMetaDataNodeType InspectionDesignNT in InspectionDesignOC.NodeTypes )
            {
                if( InspectionNTIds != string.Empty )
                    InspectionNTIds += ",";
                InspectionNTIds += InspectionDesignNT.NodeTypeId;
            }

            CswTableUpdate TabsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-09_Tabs_Update", "nodetype_tabset" );
            DataTable TabsTable = TabsUpdate.getTable( "where tabname = 'Setup' and nodetypeid in (" + InspectionNTIds + ")" );
            foreach( DataRow TabRow in TabsTable.Rows )
            {
                TabRow["taborder"] = CswConvert.ToDbVal( 10 );
            }
            TabsUpdate.update( TabsTable );

            // BZ 20081 - Make Finished and Cancelled 'required' to remove the blank option
            CswNbtMetaDataObjectClassProp FinishedOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.FinishedPropertyName );
            CswNbtMetaDataObjectClassProp CancelledOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.CancelledPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FinishedOCP, "isrequired", CswConvert.ToDbVal( true ) );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CancelledOCP, "isrequired", CswConvert.ToDbVal( true ) );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FinishedOCP, CswNbtSubField.SubFieldName.Checked, false );
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
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

        } // update()

    }//class CswUpdateSchemaTo01H09

}//namespace ChemSW.Nbt.Schema


