using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-03
    /// </summary>
    public class CswUpdateSchemaTo01I03 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 03 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
        {
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "jct_nodes_props", "field2_numeric" ) )
			{
				_CswNbtSchemaModTrnsctn.addDoubleColumn( "jct_nodes_props", "field2_numeric", "A second numeric value", false, false, 6 );
			}
	
			// case 21877
			
			CswTableUpdate WelcomeUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("01I-03_Welcome_Update", "welcome");
			DataTable WelcomeTable = WelcomeUpdate.getTable();
			foreach( DataRow WelcomeRow in WelcomeTable.Rows )
			{
				// Remove 'Find Mount Point' from welcome page
				if( WelcomeRow["displaytext"].ToString() == "Find Mount Point" )
				{
					WelcomeRow.Delete();
				}

				// Fix typo: 'Find Inpsection Point' to 'Find Inspection Point'
				else if( WelcomeRow["displaytext"].ToString() == "Find Inpsection Point" )
				{
					WelcomeRow["displaytext"] = "Find Inspection Point";
				}
			} // foreach( DataRow WelcomeRow in WelcomeTable.Rows )

			WelcomeUpdate.update( WelcomeTable );


			// case 21064
			CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
			CswNbtMetaDataObjectClassProp GeneratorWarningDaysOCP = GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.WarningDaysPropertyName );
			_CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GeneratorWarningDaysOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberminvalue, 0 );

			CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
			CswNbtMetaDataObjectClassProp MailReportWarningDaysOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.WarningDaysPropertyName );
			_CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MailReportWarningDaysOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberminvalue, 0 );


			// case 8635

			CswTableUpdate FieldTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I03_FieldTypes_Update", "field_types" );
			DataTable FieldTypeTable = FieldTypesUpdate.getEmptyTable();
			DataRow NewFTRow = FieldTypeTable.NewRow();
			NewFTRow["auditflag"] = "0";
			NewFTRow["datatype"] = "double";
			NewFTRow["deleted"] = CswConvert.ToDbVal(false);
			NewFTRow["fieldtype"] = CswNbtMetaDataFieldType.NbtFieldType.Scientific.ToString();
			FieldTypeTable.Rows.Add( NewFTRow );
			FieldTypesUpdate.update( FieldTypeTable );

			
			// case 8179
			// create 'Version' property
			CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
			
			CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("01I-03_OCP_Update", "object_class_props");
			DataTable OCPTable = OCPUpdate.getEmptyTable();
			_CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, InspectionDesignOC, CswNbtObjClassInspectionDesign.VersionPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text, false, true, false, string.Empty, Int32.MinValue, false, false, false, true, string.Empty, Int32.MinValue, Int32.MinValue );
			OCPUpdate.update( OCPTable );

			_CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

			// set value of Version property
			Collection<CswNbtMetaDataNodeType> InspectionNodeTypes = new Collection<CswNbtMetaDataNodeType>();
			foreach( CswNbtMetaDataNodeType InspectionDesignNT in InspectionDesignOC.NodeTypes )
			{
				if( InspectionDesignNT.IsLatestVersion )
				{
					InspectionNodeTypes.Add( InspectionDesignNT );
				}
			}

			foreach( CswNbtMetaDataNodeType InspectionDesignNT in InspectionNodeTypes )
			{
				CswNbtMetaDataNodeTypeProp VersionNTP = InspectionDesignNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.VersionPropertyName );
				VersionNTP.UseNumbering = false;

				CswNbtMetaDataNodeTypeTab DetailTab = InspectionDesignNT.getNodeTypeTab( "Detail" );
				if( DetailTab != null )
				{
					// case 23047
					// Prevent versioning by editing directly
					//VersionNTP.NodeTypeTab = DetailTab;
					//VersionNTP.DisplayRow = DetailTab.getCurrentMaxDisplayRow() + 1;
					//VersionNTP.DisplayColumn = 1;
					
					//VersionNTP._DataRow["nodetypetabsetid"] = CswConvert.ToDbVal( DetailTab.TabId );
					//VersionNTP._DataRow["display_row"] = CswConvert.ToDbVal( DetailTab.getCurrentMaxDisplayRow() + 1 );
					//VersionNTP._DataRow["display_col"] = CswConvert.ToDbVal( 1 );
					VersionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DetailTab, Int32.MinValue, Int32.MinValue );
				}

				foreach( CswNbtNode Node in InspectionDesignNT.getNodes( false, true ) )
				{
					CswNbtObjClassInspectionDesign InspectionNode = CswNbtNodeCaster.AsInspectionDesign( Node );
					InspectionNode.Version.Text = InspectionDesignNT.NodeTypeName + " v" + InspectionDesignNT.VersionNo.ToString();
					InspectionNode.postChanges( false );
				}
			}


        } // Update()

    }//class CswUpdateSchemaTo01I03

}//namespace ChemSW.Nbt.Schema


