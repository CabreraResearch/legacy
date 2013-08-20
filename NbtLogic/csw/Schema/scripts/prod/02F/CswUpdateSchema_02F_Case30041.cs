using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30041_Vendors : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30041; }
        }

        private DataTable _importOrderTable;
        private DataTable _importBindingsTable;
        private DataTable _importRelationshipsTable;

        public override void update()
        {
            // CAF bindings definitions for Vendors

            _importOrderTable = CswNbtImportDefOrder.getDataTableForNewOrderEntries();
            _importBindingsTable = CswNbtImportDefBinding.getDataTableForNewBindingEntries();
            _importRelationshipsTable = CswNbtImportDefRelationship.getDataTableForNewRelationshipEntries();

            // Order
            _importOrder( 1, "vendors", "Vendor" );

            // Binding
            _importBinding( "vendors", "accountno",        "Vendor", "Account No"   , "" );
            _importBinding( "vendors", "city",             "Vendor", "City"         , "" );
            _importBinding( "vendors", "contactname",      "Vendor", "Contact Name" , "" );
            _importBinding( "vendors", "fax",              "Vendor", "Fax"          , "" );
            _importBinding( "vendors", "phone",            "Vendor", "Phone"        , "" );
            _importBinding( "vendors", "state",            "Vendor", "State"        , "" );
            _importBinding( "vendors", "street1",          "Vendor", "Street1"      , "" );
            _importBinding( "vendors", "street2",          "Vendor", "Street2"      , "" );
            _importBinding( "vendors", "vendorid",         "Vendor", "Legacy Id"    , "" );
            _importBinding( "vendors", "vendorname",       "Vendor", "Vendor Name"  , "" );
            _importBinding( "vendors", "zip",              "Vendor", "Zip"          , "" );

            // Relationship
            // none

            CswNbtImporter Importer = _CswNbtSchemaModTrnsctn.makeCswNbtImporter();
            Importer.storeDefinition( _importOrderTable, _importBindingsTable, _importRelationshipsTable, "CAF" );

        } // update()

        private void _importOrder( Int32 Order, string SheetName, string NodeTypeName, Int32 Instance = Int32.MinValue )
        {
            if( false == string.IsNullOrEmpty( NodeTypeName ) )
            {
                DataRow row = _importOrderTable.NewRow();
                row["sheet"] = SheetName;
                row["nodetype"] = NodeTypeName;
                row["order"] = Order;
                row["instance"] = Instance;
                _importOrderTable.Rows.Add( row );
            }
        } // _importOrder()

        private void _importBinding( string SheetName, string SourceColumnName, string DestNodeTypeName, string DestPropertyName, string DestSubFieldName, Int32 Instance = Int32.MinValue )
        {
            if( false == string.IsNullOrEmpty( DestNodeTypeName ) )
            {
                DataRow row = _importBindingsTable.NewRow();
                row["sheet"] = SheetName;
                row["destnodetype"] = DestNodeTypeName;
                row["destproperty"] = DestPropertyName;
                row["destsubfield"] = DestSubFieldName;
                row["sourcecolumnname"] = SourceColumnName;
                row["instance"] = Instance;
                _importBindingsTable.Rows.Add( row );
            }
        } // _importBinding()

        private void _importRelationship( string SheetName, string NodetypeName, string RelationshipPropName, Int32 Instance = Int32.MinValue )
        {
            DataRow row = _importRelationshipsTable.NewRow();
            row["sheet"] = SheetName;
            row["nodetype"] = NodetypeName;
            row["relationship"] = RelationshipPropName;
            row["instance"] = Instance;
            _importRelationshipsTable.Rows.Add( row );
        } // _importRelationship()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema