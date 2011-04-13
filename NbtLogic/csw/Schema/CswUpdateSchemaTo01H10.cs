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
	/// Updates the schema to version 01H-10
	/// </summary>
	public class CswUpdateSchemaTo01H10 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 10 ); } }
		public CswUpdateSchemaTo01H10( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
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
	   
			// case 20104
			CswNbtMetaDataNodeType FireExtinguisherNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Fire_Extinguisher ) );
			CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point ) );
			CswNbtView FireExtinguisherView = _CswNbtSchemaModTrnsctn.makeView();
			FireExtinguisherView.makeNew( "All Fire Extinguishers", NbtViewVisibility.Global, null, null, null );
			FireExtinguisherView.Category = "Search";
			FireExtinguisherView.SetViewMode(NbtViewRenderingMode.Grid);
			
			CswNbtViewRelationship FExtinguisherRelationship = FireExtinguisherView.AddViewRelationship( FireExtinguisherNT, false );
			CswNbtMetaDataNodeTypeProp FEBarcodeNTP = FireExtinguisherNT.getNodeTypeProp( "Barcode" );
			CswNbtMetaDataNodeTypeProp FEDescriptionNTP = FireExtinguisherNT.getNodeTypeProp( CswNbtObjClassFireExtinguisher.DescriptionPropertyName );
			CswNbtMetaDataNodeTypeProp FEMountPointNTP = FireExtinguisherNT.getNodeTypeProp( CswNbtObjClassFireExtinguisher.InspectionTargetPropertyName );
			CswNbtMetaDataNodeTypeProp FEStatusNTP = FireExtinguisherNT.getNodeTypeProp( CswNbtObjClassFireExtinguisher.StatusPropertyName );
			CswNbtViewProperty FEBarcodeVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, FEBarcodeNTP );
			FEBarcodeVP.Order = 1;
			CswNbtViewPropertyFilter FEBarcodeVPF = FireExtinguisherView.AddViewPropertyFilter( FEBarcodeVP, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

			CswNbtViewProperty FEStatusVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, FEStatusNTP );
			FEStatusVP.Order = 2;
			CswNbtViewPropertyFilter FEStatusVPF = FireExtinguisherView.AddViewPropertyFilter( FEStatusVP, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
			
			CswNbtViewProperty FEMountPointVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, FEMountPointNTP );
			FEMountPointVP.Order = 3;
			CswNbtViewPropertyFilter FEMountPointVPF = FireExtinguisherView.AddViewPropertyFilter( FEMountPointVP, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

			CswNbtViewProperty FEDescriptionVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, FEDescriptionNTP );
			FEDescriptionVP.Order = 4;
			CswNbtViewPropertyFilter FEDescriptionVPF = FireExtinguisherView.AddViewPropertyFilter( FEDescriptionVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

			CswNbtViewRelationship MPointRelationship = FireExtinguisherView.AddViewRelationship( FExtinguisherRelationship, CswNbtViewRelationship.PropOwnerType.First, FEMountPointNTP, false );
            CswNbtMetaDataNodeTypeProp MPLocationNTP = MountPointNT.getNodeTypeProp( CswNbtObjClassInspectionTarget.LocationPropertyName );
            CswNbtMetaDataNodeTypeProp MPDescriptionNTP = MountPointNT.getNodeTypeProp( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
			CswNbtViewProperty MPLocationVP = FireExtinguisherView.AddViewProperty( MPointRelationship, MPLocationNTP );
			FEBarcodeVP.Order = 5;
			CswNbtViewPropertyFilter MPLocationVPF = FireExtinguisherView.AddViewPropertyFilter( MPLocationVP, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

			CswNbtViewProperty MPDescriptionVP = FireExtinguisherView.AddViewProperty( FExtinguisherRelationship, MPDescriptionNTP );
			FEBarcodeVP.Order = 6;
			CswNbtViewPropertyFilter MPDescriptionVPF = FireExtinguisherView.AddViewPropertyFilter( MPDescriptionVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );
			
			FireExtinguisherView.save();

		} // update()

	}//class CswUpdateSchemaTo01H10

}//namespace ChemSW.Nbt.Schema


